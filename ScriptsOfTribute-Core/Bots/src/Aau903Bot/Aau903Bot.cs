using System.Diagnostics;
using ScriptsOfTribute;
using ScriptsOfTribute.AI;
using ScriptsOfTribute.Board;
using ScriptsOfTribute.Serializers;

namespace Aau903Bot;

public class Aau903Bot : AI
{
    private Node? rootNode = null;
    private Move? previouselyPlayedMove = null;
    private TreeLogger treeLogger = new TreeLogger();

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
            double millisecondsForMove = (remainingTime.TotalMilliseconds / estimatedRemainingMovesInTurn) - MCTSHyperparameters.ITERATION_COMPLETION_MILLISECONDS_BUFFER;

            ulong randomSeed = (ulong)Utility.Rng.Next();
            var seededGameState = gameState.ToSeededGameState(randomSeed);
            var seededGameStateHash = seededGameState.GenerateHash();
            if (MCTSHyperparameters.SHARED_MCTS_TREE)
            {
                if (rootNode is ChanceNode)
                {
                    // foreach (var childNode in rootNode.ChildNodes)
                    // {
                    //     if (childNode.GameStateHash == gameStateHash)
                    //     {
                    //         rootNode = childNode;
                    //         break;
                    //     }
                    // }
                }
                else
                {
                    var playerId = rootNode?.GameState.CurrentPlayer.PlayerID;
                    var coins = rootNode?.GameState.CurrentPlayer.Coins;
                    var power = rootNode?.GameState.CurrentPlayer.Power;
                    var prestige = rootNode?.GameState.CurrentPlayer.Prestige;
                    var handCount = rootNode?.GameState.CurrentPlayer.Hand.Count;
                    var cooldownCount = rootNode?.GameState.CurrentPlayer.CooldownPile.Count;
                    var drawCount = rootNode?.GameState.CurrentPlayer.DrawPile.Count;
                    var appliedMove = rootNode?.AppliedMove;
                    var totalScore = rootNode?.TotalScore;
                    var visitCount = rootNode?.VisitCount;
                    var tavernCards = rootNode?.GameState.TavernCards.Count;
                    var tavernAvailableCards = rootNode?.GameState.TavernAvailableCards.Count;
                    var patronStates = string.Join(",", gameState.PatronStates.All.Select((patron, player) => patron));
                    Console.WriteLine($"EXPECTING {playerId} == {rootNode?.GameStateHash} == {coins} {power} {prestige} == {handCount} {cooldownCount} {drawCount} == {tavernCards} {tavernAvailableCards} {patronStates} == {appliedMove}");
                    playerId = gameState.CurrentPlayer.PlayerID;
                    coins = gameState.CurrentPlayer.Coins;
                    power = gameState.CurrentPlayer.Power;
                    prestige = gameState.CurrentPlayer.Prestige;
                    handCount = gameState.CurrentPlayer.Hand.Count;
                    cooldownCount = gameState.CurrentPlayer.CooldownPile.Count;
                    drawCount = gameState.CurrentPlayer.DrawPile.Count;
                    tavernCards = gameState.TavernCards.Count;
                    tavernAvailableCards = gameState.TavernAvailableCards.Count;
                    patronStates = string.Join(",", gameState.PatronStates.All.Select((patron, player) => patron));
                    Console.WriteLine($"GOT       {playerId} == {seededGameStateHash} == {coins} {power} {prestige} == {handCount} {cooldownCount} {drawCount} == {tavernCards} {tavernAvailableCards} {patronStates}");

                    if (seededGameStateHash != rootNode?.GameStateHash)
                    {
                        rootNode = null;
                    }
                }
            }

            if (rootNode == null)
            {
                Console.WriteLine($"RESETTED ROOT     == {seededGameStateHash}");
                rootNode = new Node(seededGameState, null, possibleMoves, null, 0);
            }

            int iterationCounter = 0;

            if (MCTSHyperparameters.DYNAMIC_MOVE_TIME_DISTRIBUTION)
            {
                while (moveTimer.ElapsedMilliseconds < millisecondsForMove)
                {
                    Console.WriteLine($"==============={iterationCounter}===============");
                    iterationCounter++;
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
