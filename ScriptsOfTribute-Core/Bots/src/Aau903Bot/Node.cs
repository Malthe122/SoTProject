using ScriptsOfTribute;
using ScriptsOfTribute.Serializers;

namespace Aau903Bot;

public class Node
{
    /// <summary>
    /// Has to be stored like a seeded game state although its a bit non-intuitive, but this is the object type that applyMove method returns.
    /// However we never want to actually reuse the seed inside the object, so when we call apply move on the seeded state, we need to call it
    /// with a new random seed which is a possible argument for the applyMove method
    /// </summary>
    public Node? Parent = null;
    public Dictionary<Move, Node> MoveToChildNode;
    public int VisitCount = 0;
    public double TotalScore = 0;
    public int GameStateHash { get; private set; }
    public SeededGameState GameState { get; private set; }
    public List<Move> PossibleMoves;
    internal Aau903Bot Bot;

    public Node(SeededGameState gameState, Node parent, List<Move> possibleMoves, Aau903Bot bot)
    {
        GameState = gameState;
        Parent = parent;
        PossibleMoves = possibleMoves;
        MoveToChildNode = new Dictionary<Move, Node>();
        ApplyAllDeterministicAndObviousMoves();
        Bot = bot;
    }

    public virtual void Visit(out double score, int travelsDone)
    {
        travelsDone++;

        if (travelsDone > Bot.Params.MAX_TREE_TRAVELS) {
            score = Score();
            TotalScore += score;
            VisitCount++;
        }

        var playerId = GameState.CurrentPlayer.PlayerID;

        if (GameState.GameEndState == null)
        {
            if (VisitCount == 0)
            {
                ApplyAllDeterministicAndObviousMoves();
                score = Score();
            }
            else if (PossibleMoves.Count > MoveToChildNode.Count)
            {
                var expandedChild = Expand();
                expandedChild.Visit(out score, travelsDone++);
            }
            else
            {
                var selectedChild = Select();
                selectedChild.Visit(out score, travelsDone++);

                if (selectedChild.GameState.CurrentPlayer.PlayerID != playerId)
                {
                    score *= -1; //TODO check if this is also correct with the heuristic. The heurisitc evaluation might not be zero-sum
                }
            }
        }
        else
        {
            score = Score();
        }

        TotalScore += score;
        VisitCount++;
    }


    internal Node Expand()
    {
        foreach (var move in PossibleMoves)
        {
            if (!MoveToChildNode.ContainsKey(move))
            {
                if ((Bot.Params.INCLUDE_PLAY_MOVE_CHANCE_NODES && move.IsNonDeterministic())
                    || Bot.Params.INCLUDE_END_TURN_CHANCE_NODES && move.Command == CommandEnum.END_TURN)
                {
                    var newChild = new ChanceNode(GameState, this, move, Bot);
                    MoveToChildNode.Add(move, newChild);
                    return newChild;
                }
                else
                {
                    ulong randomSeed = (ulong)Utility.Rng.Next();
                    var (newGameState, newPossibleMoves) = GameState.ApplyMove(move, randomSeed);
                    var newChild = Utility.FindOrBuildNode(newGameState, this, newPossibleMoves, Bot);
                    MoveToChildNode.Add(move, newChild);
                    return newChild;
                }
            }
        }

        throw new Exception("Expand was unexpectedly called on a node that was fully expanded");
    }

    private double Score()
    {
        switch (Bot.Params.CHOSEN_SCORING_METHOD)
        {
            case ScoringMethod.Rollout:
                return Rollout();
            case ScoringMethod.Heuristic:
                return Utility.UseBestMCTS3Heuristic(GameState, false);
            case ScoringMethod.RolloutTurnsCompletionsThenHeuristic:
                return RolloutTillTurnsEndThenHeuristic(Bot.Params.ROLLOUT_TURNS_BEFORE_HEURSISTIC);
            default:
                throw new NotImplementedException("Tried to applied non-implemented scoring method: " + Bot.Params.CHOSEN_SCORING_METHOD);
        }
    }

    private double RolloutTillTurnsEndThenHeuristic(int turnsToComplete)
    {
        int rolloutTurnsCompleted = 0;
        var rolloutPlayer = GameState.CurrentPlayer;
        var rolloutGameState = GameState;
        var rolloutPossibleMoves = PossibleMoves;

        while (rolloutTurnsCompleted < turnsToComplete && rolloutGameState.GameEndState == null)
        {
            if (Bot.Params.FORCE_DELAY_TURN_END_IN_ROLLOUT)
            {
                if (rolloutPossibleMoves.Count > 1)
                {
                    rolloutPossibleMoves.RemoveAll(move => move.Command == CommandEnum.END_TURN);
                }
            }

            var chosenIndex = Utility.Rng.Next(rolloutPossibleMoves.Count);
            var moveToMake = rolloutPossibleMoves[chosenIndex];

            var (newGameState, newPossibleMoves) = rolloutGameState.ApplyMove(moveToMake);

            if (newGameState.CurrentPlayer != rolloutPlayer)
            {
                rolloutTurnsCompleted++;
                rolloutPlayer = newGameState.CurrentPlayer;
            }

            rolloutGameState = newGameState;
            rolloutPossibleMoves = newPossibleMoves;
        }

        var stateScore = Utility.UseBestMCTS3Heuristic(rolloutGameState, true);

        if (GameState.CurrentPlayer != rolloutGameState.CurrentPlayer)
        {
            stateScore *= -1;
        }

        return stateScore;
    }

    internal double Rollout()
    {
        double result = 0;
        var rolloutGameState = GameState;
        var rolloutPlayerId = rolloutGameState.CurrentPlayer.PlayerID;
        var rolloutPossibleMoves = new List<Move>(PossibleMoves);

        for (int i = 0; i < Bot.Params.NUMBER_OF_ROLLOUTS; i++)
        {
            // TODO also apply the playing obvious moves in here
            while (rolloutGameState.GameEndState == null)
            {
                // Choosing here to remove the "end turn" move before its the last move. This is done to make the random plays a bit more realistic
                if (Bot.Params.FORCE_DELAY_TURN_END_IN_ROLLOUT)
                {
                    if (rolloutPossibleMoves.Count > 1)
                    {
                        rolloutPossibleMoves.RemoveAll(move => move.Command == CommandEnum.END_TURN);
                    }
                }
                var chosenIndex = Utility.Rng.Next(rolloutPossibleMoves.Count);
                var moveToMake = rolloutPossibleMoves[chosenIndex];

                var (newGameState, newPossibleMoves) = rolloutGameState.ApplyMove(moveToMake);
                rolloutGameState = newGameState;
                rolloutPossibleMoves = newPossibleMoves;
            }

            if (rolloutGameState.GameEndState.Winner != PlayerEnum.NO_PLAYER_SELECTED)
            { //TODO here i assume that winner = NO_PLAYER_SELECTED is how they show a draw. Need to confirm this
                if (rolloutGameState.GameEndState.Winner == rolloutPlayerId)
                {
                    result += 1;
                }
                else
                {
                    result -= 1;
                }
            }
        }

        return result;
    }

    internal virtual Node Select()
    {
        double maxConfidence = -double.MaxValue;
        var highestConfidenceChild = MoveToChildNode.First().Value;

        foreach (var childNode in MoveToChildNode.Values)
        {
            double confidence = childNode.GetConfidenceScore();
            if (confidence > maxConfidence)
            {
                maxConfidence = confidence;
                highestConfidenceChild = childNode;
            }
        }

        return highestConfidenceChild;
    }

    public double GetConfidenceScore()
    {
        switch (Bot.Params.CHOSEN_EVALUATION_METHOD)
        {
            case EvaluationMethod.UCT:
                double exploitation = TotalScore / VisitCount;
                double exploration = Bot.Params.UCT_EXPLORATION_CONSTANT * Math.Sqrt(Math.Log(Parent.VisitCount) / VisitCount);
                return exploitation + exploration;
            case EvaluationMethod.Custom:
                return TotalScore - VisitCount;
            default:
                return 0;
        }
    }

    internal void ApplyAllDeterministicAndObviousMoves()
    {
        foreach (Move currMove in PossibleMoves)
        {
            if (currMove.Command == CommandEnum.PLAY_CARD)
            {
                if (Utility.OBVIOUS_ACTION_PLAYS.Contains(((SimpleCardMove)currMove).Card.CommonId))
                {
                    // TODO consider if some of the choice cards are also obvious moves, since the choice will be a new move
                    // or how to handle this issue
                    (GameState, PossibleMoves) = GameState.ApplyMove(currMove, (ulong)Utility.Rng.Next());
                    ApplyAllDeterministicAndObviousMoves();
                    break;
                }
            }
            else if (currMove.Command == CommandEnum.ACTIVATE_AGENT)
            {
                if (Utility.OBVIOUS_AGENT_EFFECTS.Contains(((SimpleCardMove)currMove).Card.CommonId))
                {
                    (GameState, PossibleMoves) = GameState.ApplyMove(currMove, (ulong)Utility.Rng.Next());
                    ApplyAllDeterministicAndObviousMoves();
                    break;
                }
            }
        }

        GameStateHash = GameState.GenerateHash();
    }
}
