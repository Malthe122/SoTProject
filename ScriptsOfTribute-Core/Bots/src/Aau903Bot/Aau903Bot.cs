using System.Diagnostics;
using ScriptsOfTribute;
using ScriptsOfTribute.AI;
using ScriptsOfTribute.Board;
using ScriptsOfTribute.Serializers;

namespace Aau903Bot;

public class Aau903Bot : AI
{
    private Node? rootNode;
    /// <summary>
    /// TODO refactor this from being static. It works with only 1 aau-bot playing at a time. Also would work with more, but then they would share information between them,
    /// and this might not be allowed according to competetion, but we would have to figure that out. But in that case, we should not reset it when a game begins, like we do now
    /// </summary>
    public static Dictionary<int, List<Node>> NodeGameStateHashMap = new Dictionary<int, List<Node>>();
    private MCTSHyperparameters Params = new MCTSHyperparameters();

    public override void PregamePrepare()
    {
        base.PregamePrepare();
        rootNode = null;
        NodeGameStateHashMap = new Dictionary<int, List<Node>>(); //TODO move this to here
    }
    public override void GameEnd(EndGameState state, FullGameState? finalBoardState)
    {
        Console.WriteLine("@@@ Game ended because of " + state.Reason + " @@@");
        Console.WriteLine("@@@ Winner was " + state.Winner + " @@@");
    }

    public override Move Play(GameState gameState, List<Move> possibleMoves, TimeSpan remainingTime)
    {
        try
        {
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
            double millisecondsForMove = (remainingTime.TotalMilliseconds / estimatedRemainingMovesInTurn) - Params.ITERATION_COMPLETION_MILLISECONDS_BUFFER;

            ulong randomSeed = (ulong)Utility.Rng.Next();
            var seededGameState = gameState.ToSeededGameState(randomSeed);

            var rootNode = Utility.FindOrBuildNode(seededGameState, null, possibleMoves, Params);

            int iterationCounter = 0;

            while (moveTimer.ElapsedMilliseconds < millisecondsForMove)
            {
                // var iterationTimer = new Stopwatch();
                // iterationTimer.Start();
                // iterationCounter++;
                rootNode.Visit(out double score);
                // iterationTimer.Stop();
                // Console.WriteLine("Iteration took: " + iterationTimer.ElapsedMilliseconds + " milliseconds");
            }

            if (rootNode.MoveToChildNode.Count == 0)
            {
                // Console.WriteLine("NO TIME FOR CALCULATING MOVE@@@@@@@@@@@@@@@");
                return possibleMoves[0];
            }

            var bestMove = rootNode.MoveToChildNode
                .OrderByDescending(moveNodePair => (moveNodePair.Value.TotalScore / moveNodePair.Value.VisitCount))
                .FirstOrDefault()
                .Key;

            return bestMove;
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

    private void CheckMoveLegality(Move moveToCheck, Node rootNode, GameState officialGameState, List<Move> officialPossiblemoves)
    {
        if (!officialPossiblemoves.Any(move => move.IsIdentical(moveToCheck)))
        {
            Console.WriteLine("----- ABOUT TO PERFORM ILLEGAL MOVE -----");
            Console.WriteLine("Our state:");
            rootNode?.GameState.Log();
            Console.WriteLine("Actual state:");
            officialGameState.ToSeededGameState((ulong)Utility.Rng.Next()).Log();
            Console.WriteLine("@@@@ Trying to play move:");
            moveToCheck.Log();
            Console.WriteLine("@@@@@@@ But available moves were:");
            officialPossiblemoves.ForEach(m => m.Log());
            Console.WriteLine("@@@@@@ But we thought moves were:");
            rootNode.PossibleMoves.ForEach(m => m.Log());
        }
    }

    private int EstimateRemainingMovesInTurn(GameState inputState, List<Move> inputPossibleMoves)
    {
        return EstimateRemainingMovesInTurn(inputState.ToSeededGameState((ulong)Utility.Rng.Next()), inputPossibleMoves);
    }

    private int EstimateRemainingMovesInTurn(SeededGameState inputState, List<Move> inputPossibleMoves)
    {

        var possibleMoves = new List<Move>(inputPossibleMoves);

        if (possibleMoves.Count == 1 && possibleMoves[0].Command == CommandEnum.END_TURN)
        {
            return 0;
        }

        possibleMoves.RemoveAll(x => x.Command == CommandEnum.END_TURN);

        int result = 1;
        SeededGameState currentState = inputState;
        List<Move> currentPossibleMoves = possibleMoves;

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
