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
    public List<Node> ChildNodes = new List<Node>();
    public int VisitCount = 0;
    public double TotalScore = 0;
    public int GameStateHash;
    public SeededGameState GameState;
    public Move? AppliedMove;
    public List<Move> PossibleMoves;
    public int Depth;

    public Node(SeededGameState gameState, Node parent, List<Move> possibleMoves, Move appliedMove, int depth)
    {
        GameState = gameState;
        Parent = parent;
        PossibleMoves = possibleMoves;
        AppliedMove = appliedMove;
        Depth = depth;
        /// <summary>
        /// TODO if this takes too much performance, look into only calling this method on children of chance nodes
        /// </summary>
        GameStateHash = GameState.GenerateHash();
    }

    public virtual void Visit(out double score)
    {
        // Node Metrics
        var playerId = GameState.CurrentPlayer.PlayerID;
        var coins = GameState.CurrentPlayer.Coins;
        var power = GameState.CurrentPlayer.Power;
        var prestige = GameState.CurrentPlayer.Prestige;
        var handCount = GameState.CurrentPlayer.Hand.Count;
        var cooldownCount = GameState.CurrentPlayer.CooldownPile.Count;
        var drawCount = GameState.CurrentPlayer.DrawPile.Count;
        Console.WriteLine($"VISIT {playerId} == {GameStateHash} == {coins} {power} {prestige} == {handCount} {cooldownCount} {drawCount} == {AppliedMove} == {TotalScore} == {VisitCount}");
        // Node Metrics

        if (MCTSHyperparameters.SET_MAX_EXPANSION_DEPTH)
        {
            if (Depth >= MCTSHyperparameters.CHOSEN_MAX_EXPANSION_DEPTH)
            {
                score = Score();
                TotalScore += score;
                VisitCount++;
                Console.WriteLine($"\tMAX DEPTH {GameStateHash}");
                return;
            }
        }

        if (GameState.GameEndState == null)
        {
            if (VisitCount == 0)
            {
                ApplyAllDeterministicAndObviousMoves();
                score = Score();
            }
            else if (PossibleMoves.Count > ChildNodes.Count)
            {
                var expandedChild = Expand();
                expandedChild.Visit(out score);
            }
            else
            {
                var selectedChild = Select();
                selectedChild.Visit(out score);

                if (selectedChild.GameState.CurrentPlayer.PlayerID != playerId)
                {
                    score *= -1;
                }
            }
        }
        else if (GameState.GameEndState.Winner == PlayerEnum.NO_PLAYER_SELECTED)
        {
            score = 0;
        }
        else if (GameState.GameEndState.Winner == GameState.CurrentPlayer.PlayerID)
        {
            score = 1;
        }
        else
        {
            score = -1;
        }

        TotalScore += score;
        VisitCount++;
    }


    internal Node Expand()
    {
        foreach (var move in PossibleMoves)
        {
            if (!ChildNodes.Any(child => child.AppliedMove == move))
            {
                if ((MCTSHyperparameters.INCLUDE_PLAY_MOVE_CHANCE_NODES && move.IsNonDeterministic())
                    || MCTSHyperparameters.INCLUDE_END_TURN_CHANCE_NODES && move.Command == CommandEnum.END_TURN)
                {
                    var newChild = new ChanceNode(GameState, this, move, Depth + 1);
                    ChildNodes.Add(newChild);
                    return newChild;
                }
                else
                {
                    ulong randomSeed = (ulong)Utility.Rng.Next();
                    var (newGameState, newPossibleMoves) = GameState.ApplyMove(move, randomSeed);
                    var newChild = new Node(newGameState, this, newPossibleMoves, move, Depth + 1);
                    ChildNodes.Add(newChild);
                    // Console.WriteLine($"New child added with Depth level: {newChild.Depth}");
                    Console.WriteLine($"\tEXPAND {newChild.GameStateHash}");
                    return newChild;
                }
            }
        }

        throw new Exception("Expand was unexpectedly called on a node that was fully expanded");
    }

    private double Score()
    {
        Console.WriteLine($"\tSCORE {GameStateHash}");
        switch (MCTSHyperparameters.CHOSEN_SCORING_METHOD)
        {
            case ScoringMethod.Rollout:
                return Rollout();
            case ScoringMethod.Heuristic:
                return Utility.UseBestMCTS3Heuristic(GameState);
            case ScoringMethod.RolloutTurnsCompletionsThenHeuristic:
                return RolloutTillTurnsEndThenHeuristic(MCTSHyperparameters.ROLLOUT_TURNS_BEFORE_HEURSISTIC);
            default:
                throw new NotImplementedException("Tried to applied non-implemented scoring method: " + MCTSHyperparameters.CHOSEN_SCORING_METHOD);
        }
    }

    private double RolloutTillTurnsEndThenHeuristic(int turnsToComplete)
    {
        int turnsCompleted = 0;
        var currentPlayer = GameState.CurrentPlayer;
        var currentGameState = GameState;
        var currentPossibleMoves = PossibleMoves;

        while (turnsCompleted < turnsToComplete && currentGameState.GameEndState == null)
        {
            if (MCTSHyperparameters.FORCE_DELAY_TURN_END_IN_ROLLOUT)
            {
                if (currentPossibleMoves.Count > 1)
                {
                    currentPossibleMoves.RemoveAll(move => move.Command == CommandEnum.END_TURN);
                }
            }

            var chosenIndex = Utility.Rng.Next(currentPossibleMoves.Count);
            var moveToMake = currentPossibleMoves[chosenIndex];

            var (newGameState, newPossibleMoves) = currentGameState.ApplyMove(moveToMake);

            if (newGameState.CurrentPlayer != currentPlayer)
            {
                turnsCompleted++;
                currentPlayer = newGameState.CurrentPlayer;
            }

            currentGameState = newGameState;
            currentPossibleMoves = newPossibleMoves;
        }

        return Utility.UseBestMCTS3Heuristic(GameState);
    }

    internal double Rollout()
    {
        double result = 0;
        var rolloutGameState = GameState;
        var rolloutPlayerId = rolloutGameState.CurrentPlayer.PlayerID;
        var rolloutPossibleMoves = new List<Move>(PossibleMoves);

        for (int i = 0; i < MCTSHyperparameters.NUMBER_OF_ROLLOUTS; i++)
        {
            // TODO also apply the playing obvious moves in here
            while (rolloutGameState.GameEndState == null)
            {
                // Choosing here to remove the "end turn" move before its the last move. This is done to make the random plays a bit more realistic
                if (MCTSHyperparameters.FORCE_DELAY_TURN_END_IN_ROLLOUT)
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
        var highestConfidenceChild = ChildNodes[0];

        foreach (var childNode in ChildNodes)
        {
            double confidence = childNode.GetConfidenceScore();
            if (confidence > maxConfidence)
            {
                maxConfidence = confidence;
                highestConfidenceChild = childNode;
            }
        }
        Console.WriteLine($"\tSELECT {highestConfidenceChild.GameStateHash}");

        return highestConfidenceChild;
    }

    public double GetConfidenceScore()
    {
        switch (MCTSHyperparameters.CHOSEN_EVALUATION_FUNCTION)
        {
            case EvaluationFunction.UCB1:
                double exploitation = TotalScore / VisitCount;
                double exploration = MCTSHyperparameters.UCB1_EXPLORATION_CONSTANT * Math.Sqrt(Math.Log(Parent.VisitCount) / VisitCount);
                return exploitation + exploration;
            case EvaluationFunction.UCT:
                // TODO implement
                return 0;
            case EvaluationFunction.Custom:
                //TODO replace this with something meaningful or remove it
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
        Console.WriteLine($"\tOBVIOUS {GameStateHash}");
    }
}
