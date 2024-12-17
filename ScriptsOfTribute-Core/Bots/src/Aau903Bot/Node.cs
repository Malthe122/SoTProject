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
    public Dictionary<MoveContainer, Node> MoveToChildNode;
    public int VisitCount = 0;
    public double TotalScore = 0;
    public int GameStateHash { get; private set; }
    public SeededGameState GameState { get; private set; }
    public List<MoveContainer> PossibleMoves;
    internal Aau903Bot Bot;

    public Node(SeededGameState gameState, Node parent, List<Move> possibleMoves, Aau903Bot bot)
    {
        GameState = gameState;
        Parent = parent;
        PossibleMoves = Utility.BuildUniqueMovesContainers(possibleMoves);
        MoveToChildNode = new Dictionary<MoveContainer, Node>();
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
                var message = "Possible moves:\n";
                PossibleMoves.ForEach(m => {message += "Move: " + m.Move.GetLog() + "\n";});
                message += "Keys in Move to childNode:\n";
                MoveToChildNode.ToList().ForEach(m => {message += "Move: " + m.Key.Move.GetLog() + "\n";});
                var expandedChild = Expand(message);
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


    internal Node Expand(string message)
    {
        foreach (var moveContainer in PossibleMoves)
        {
            if (!MoveToChildNode.Keys.Any(mc => mc.Move.IsIdentical(moveContainer.Move)))
            {
                if ((Bot.Params.INCLUDE_PLAY_MOVE_CHANCE_NODES && moveContainer.Move.IsNonDeterministic())
                    || Bot.Params.INCLUDE_END_TURN_CHANCE_NODES && moveContainer.Move.Command == CommandEnum.END_TURN)
                {
                    var newChild = new ChanceNode(GameState, this, moveContainer.Move, Bot);
                    MoveToChildNode.Add(new MoveContainer(moveContainer.Move), newChild);
                    return newChild;
                }
                else
                {
                    ulong randomSeed = (ulong)Utility.Rng.Next();
                    var (newGameState, newPossibleMoves) = GameState.ApplyMove(moveContainer.Move, randomSeed);
                    var newChild = Utility.FindOrBuildNode(newGameState, this, newPossibleMoves, Bot);
                    MoveToChildNode.Add(new MoveContainer(moveContainer.Move), newChild);
                    return newChild;
                }
            }
        }

        throw new Exception("Expand was unexpectedly called on a node that was fully expanded. Message:\n" + message);
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
                    rolloutPossibleMoves.RemoveAll(moveContainer => moveContainer.Move.Command == CommandEnum.END_TURN);
                }
            }

            var chosenIndex = Utility.Rng.Next(rolloutPossibleMoves.Count);
            var moveToMake = rolloutPossibleMoves[chosenIndex];

            var (newGameState, newPossibleMoves) = rolloutGameState.ApplyMove(moveToMake.Move);

            if (newGameState.CurrentPlayer != rolloutPlayer)
            {
                rolloutTurnsCompleted++;
                rolloutPlayer = newGameState.CurrentPlayer;
            }

            rolloutGameState = newGameState;
            rolloutPossibleMoves = Utility.BuildUniqueMovesContainers(newPossibleMoves);
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
        var rolloutPossibleMoves = new List<MoveContainer>(PossibleMoves);

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
                        rolloutPossibleMoves.RemoveAll(moveContainer => moveContainer.Move.Command == CommandEnum.END_TURN);
                    }
                }
                var chosenIndex = Utility.Rng.Next(rolloutPossibleMoves.Count);
                var moveToMake = rolloutPossibleMoves[chosenIndex];

                var (newGameState, newPossibleMoves) = rolloutGameState.ApplyMove(moveToMake.Move);
                rolloutGameState = newGameState;
                rolloutPossibleMoves = Utility.BuildUniqueMovesContainers(newPossibleMoves);
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
        foreach (var currMove in PossibleMoves)
        {
            if (currMove.Move.Command == CommandEnum.PLAY_CARD)
            {
                if (Utility.OBVIOUS_ACTION_PLAYS.Contains(((SimpleCardMove)currMove.Move).Card.CommonId))
                {
                    // TODO consider if some of the choice cards are also obvious moves, since the choice will be a new move
                    // or how to handle this issue
                    (GameState, var possibleMoves) = GameState.ApplyMove(currMove.Move, (ulong)Utility.Rng.Next());
                    PossibleMoves = Utility.BuildUniqueMovesContainers(possibleMoves);
                    ApplyAllDeterministicAndObviousMoves();
                    break;
                }
            }
            else if (currMove.Move.Command == CommandEnum.ACTIVATE_AGENT)
            {
                if (Utility.OBVIOUS_AGENT_EFFECTS.Contains(((SimpleCardMove)currMove.Move).Card.CommonId))
                {
                    (GameState, var possibleMoves) = GameState.ApplyMove(currMove.Move, (ulong)Utility.Rng.Next());
                    PossibleMoves = Utility.BuildUniqueMovesContainers(possibleMoves);
                    ApplyAllDeterministicAndObviousMoves();
                    break;
                }
            }
        }

        GameStateHash = GameState.GenerateHash();
    }
}
