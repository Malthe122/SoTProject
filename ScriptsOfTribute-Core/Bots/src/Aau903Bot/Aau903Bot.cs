using System.Diagnostics;
using ScriptsOfTribute;
using ScriptsOfTribute.AI;
using ScriptsOfTribute.Board;
using ScriptsOfTribute.Serializers;

namespace Aau903Bot;

public class Aau903Bot : AI
{
    private Node? rootNode = null;
    private TreeLogger treeLogger = new TreeLogger();
    private int reused = 0;
    private int reset = 0;

    public override void GameEnd(EndGameState state, FullGameState? finalBoardState)
    {
        Console.WriteLine("@@@ Game ended because of " + state.Reason + " @@@");
        Console.WriteLine("@@@ Winner was " + state.Winner + " @@@");
        Console.WriteLine($"REUSED: {reused}");
        Console.WriteLine($"RESET: {reset}");
        reused = 0;
        reset = 0;
    }

    public override Move Play(GameState gameState, List<Move> possibleMoves, TimeSpan remainingTime)
    {
        try
        {
            ulong randomSeed = (ulong)Utility.Rng.Next();
            var seededGameState = gameState.ToSeededGameState(randomSeed);

            var obviousMove = FindObviousMove(possibleMoves);
            if (obviousMove != null)
            {
                return obviousMove;
            }
            if (possibleMoves.Count == 1)
            {
                return possibleMoves[0];
            }

            var moveTimer = new Stopwatch();
            moveTimer.Start();
            int estimatedRemainingMovesInTurn = EstimateRemainingMovesInTurn(gameState, possibleMoves);
            double millisecondsForMove = (remainingTime.TotalMilliseconds / estimatedRemainingMovesInTurn) - MCTSHyperparameters.ITERATION_COMPLETION_MILLISECONDS_BUFFER;

            if (rootNode != null && MCTSHyperparameters.SHARED_MCTS_TREE)
            {
                var seededGameStateHash = seededGameState.GenerateHash();
                if (rootNode is ChanceNode)
                {
                    bool found = false;
                    foreach (var childNode in rootNode.ChildNodes)
                    {
                        if (seededGameStateHash == childNode.GameStateHash)
                        {
                            rootNode = childNode;
                            rootNode.Depth = 0;
                            found = true;
                            break;
                        }
                    }
                    if (found == false)
                    {
                        rootNode = null;
                    }
                }
                else if (seededGameStateHash != rootNode.GameStateHash)
                {
                    rootNode = null;
                }
            }

            if (rootNode == null)
            {
                reset++;
                rootNode = new Node(seededGameState, null, possibleMoves, null, 0);
            }
            else
            {
                reused++;
            }

            int iterationCounter = 0;
            if (MCTSHyperparameters.DYNAMIC_MOVE_TIME_DISTRIBUTION)
            {
                while (moveTimer.ElapsedMilliseconds < millisecondsForMove)
                {
                    // iterationCounter++;
                    rootNode.Visit(out double score);
                    // this.treeLogger.LogTree(rootNode);
                }
            }
            else
            {
                while (iterationCounter <= MCTSHyperparameters.ITERATIONS)
                {
                    rootNode.Visit(out double score);
                    iterationCounter++;
                }
            }


            Move bestMoveToPlay;
            if (rootNode.ChildNodes.Count == 0)
            {
                bestMoveToPlay = possibleMoves[0];
                rootNode = null;
            }
            else
            {
                var bestChildNode = rootNode.ChildNodes
                    .OrderByDescending(child => (child.TotalScore / child.VisitCount))
                    .FirstOrDefault();
                bestMoveToPlay = bestChildNode.AppliedMove!;

                if (MCTSHyperparameters.SHARED_MCTS_TREE)
                {
                    rootNode = bestChildNode;
                    rootNode.Depth = 0;
                }
                else
                {
                    rootNode = null;
                }
            }

            return bestMoveToPlay;
        }
        catch (Exception e)
        {
            Console.WriteLine("Something went wrong while trying to compute move. Playing random move instead. Exception:");
            Console.WriteLine("Message: " + e.Message);
            Console.WriteLine("Stacktrace: " + e.StackTrace);
            Console.WriteLine("Data: " + e.Data);
            if (e.InnerException != null)
            {
                Console.WriteLine("Inner excpetion: " + e.InnerException.Message);
                Console.WriteLine("Inner stacktrace: " + e.InnerException.StackTrace);
            }
            return possibleMoves[0];
        }
    }

    private int EstimateRemainingMovesInTurn(GameState inputState, List<Move> inputPossibleMoves)
    {
        return EstimateRemainingMovesInTurn(inputState.ToSeededGameState((ulong)Utility.Rng.Next()), inputPossibleMoves);
    }

    private int EstimateRemainingMovesInTurn(SeededGameState inputState, List<Move> inputPossibleMoves)
    {

        if (inputPossibleMoves.Count == 1 && inputPossibleMoves[0].Command == CommandEnum.END_TURN)
        {
            return 0;
        }

        inputPossibleMoves.RemoveAll(x => x.Command == CommandEnum.END_TURN);

        int result = 1;
        SeededGameState currentState = inputState;
        List<Move> currentPossibleMoves = inputPossibleMoves;

        while (currentPossibleMoves.Count > 0)
        {

            var obviousMove = FindObviousMove(currentPossibleMoves);
            if (obviousMove != null)
            {
                (currentState, currentPossibleMoves) = currentState.ApplyMove(obviousMove);
            }
            else if (currentPossibleMoves.Count == 1)
            {
                // TODO add this to ovious moves instead
                // we already checked that its not end turn, so this is make choice in cases where there is only one choice
                (currentState, currentPossibleMoves) = currentState.ApplyMove(currentPossibleMoves[0]);
            }
            else
            {
                result++;
                (currentState, currentPossibleMoves) = currentState.ApplyMove(currentPossibleMoves[0]);
            }

            currentPossibleMoves.RemoveAll(x => x.Command == CommandEnum.END_TURN);
        }

        return result;
    }

    private Move FindObviousMove(List<Move> possibleMoves)
    {
        foreach (Move currMove in possibleMoves)
        {
            if (currMove.Command == CommandEnum.PLAY_CARD)
            {
                if (Utility.OBVIOUS_ACTION_PLAYS.Contains(((SimpleCardMove)currMove).Card.CommonId))
                {
                    return currMove;
                }
            }
            else if (currMove.Command == CommandEnum.ACTIVATE_AGENT)
            {
                if (Utility.OBVIOUS_AGENT_EFFECTS.Contains(((SimpleCardMove)currMove).Card.CommonId))
                {
                    return currMove;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Used for logging when debugging. Do not delete even though it has no references
    /// </summary>
    private double GetTimeSpentBeforeTurn(TimeSpan remainingTime)
    {
        return 10_000d - remainingTime.TotalMilliseconds;
    }
    public override PatronId SelectPatron(List<PatronId> availablePatrons, int round)
    {
        return availablePatrons[0];
    }
}
