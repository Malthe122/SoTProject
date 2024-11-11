using System.Diagnostics;
using ScriptsOfTribute;
using ScriptsOfTribute.AI;
using ScriptsOfTribute.Board;
using ScriptsOfTribute.Serializers;

public class Aau903Bot : AI
{
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

            if(possibleMoves.Count == 1){
                Console.WriteLine("HIT SITUATION WHERE WE ONLY HAD ONE MOVE, SO WE SKIPPED MCTS---------------------------");
                if(possibleMoves[0].Command == CommandEnum.END_TURN) {
                    Console.WriteLine("----------TURN END----------");
                    Console.WriteLine("Turn time: " + GetTimeSpentBeforeTurn(remainingTime));
                }
                // Console.WriteLine("Move was: " + possibleMoves[0].Command);
                // TODO find out how MakeChoice sometimes comes as a single move. Logic would say you would always have atleast two choices to make
                // (maybe its choosing like agent to destroy when there is only 1, but check)
                return possibleMoves[0];
            }

            var moveTimer = new Stopwatch();
            moveTimer.Start();

            int estimatedRemainingMovesInTurn = EstimateRemainingMovesInTurn(gameState, possibleMoves);
            // TODO here is a hardcoded buffer of 0.04. Could be made as an environment variable
            double millisecondsForMove = (remainingTime.TotalMilliseconds / estimatedRemainingMovesInTurn) - 40;

            // // TODO remove or explain. Rn its just for testing:
            // if ((millisecondsForMove + GetTimeSpentBeforeTurn(remainingTime)) <= 9_000) {
            //     Console.WriteLine("Added time");
            //     millisecondsForMove += 500;
            // }

            ulong randomSeed = (ulong)Utility.Rng.Next();
            var seededGameState = gameState.ToSeededGameState(randomSeed);
            var rootNode = new Node(seededGameState, null, possibleMoves, null);

            int iterationCounter = 0;
            // Console.WriteLine("current available moves: " + possibleMoves.Count);
            // Console.WriteLine("estimated remaining moves: " + estimatedRemainingMovesInTurn);
            // Console.WriteLine("milliseconds for move: " + millisecondsForMove);
            // Console.WriteLine("remaining time: " + remainingTime.TotalMilliseconds);

            while (moveTimer.ElapsedMilliseconds < millisecondsForMove)
            {
                // var iterationTimer = new Stopwatch();
                // iterationTimer.Start();
                // iterationCounter++;
                rootNode.Visit(out double score);
                // iterationTimer.Stop();
                // Console.WriteLine("Iteration took: " + iterationTimer.ElapsedMilliseconds + " milliseconds");
            }

            // Console.WriteLine("remaining time after calculating move: " + remainingTime.TotalMilliseconds);

            if (rootNode.ChildNodes.Count == 0) {
                Console.WriteLine("NO TIME FOR CALCULATING MOVE@@@@@@@@@@@@@@@");
                if (possibleMoves[0].Command == CommandEnum.END_TURN){
                Console.WriteLine("----------TURN END----------");
                Console.WriteLine("Turn time: " + GetTimeSpentBeforeTurn(remainingTime) + moveTimer.ElapsedMilliseconds);
                }
                return possibleMoves[0];
            }

            var bestChildNode = rootNode.ChildNodes
                .OrderByDescending(child => (child.TotalScore / child.VisitCount))
                .FirstOrDefault();

            if (bestChildNode.AppliedMove.Command == CommandEnum.END_TURN){
                Console.WriteLine("----------TURN END----------");
                Console.WriteLine("Turn time: " + GetTimeSpentBeforeTurn(remainingTime) + moveTimer.ElapsedMilliseconds);
            }

            return bestChildNode.AppliedMove;
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

    private int EstimateRemainingMovesInTurn(GameState inputState, List<Move> inputPossibleMoves){
        return EstimateRemainingMovesInTurn(inputState.ToSeededGameState((ulong)Utility.Rng.Next()), inputPossibleMoves);
    }

    private int EstimateRemainingMovesInTurn(SeededGameState inputState, List<Move> inputPossibleMoves)
    {

        if (inputPossibleMoves.Count == 1 && inputPossibleMoves[0].Command == CommandEnum.END_TURN) {
            return 0;
        }

        inputPossibleMoves.RemoveAll(x => x.Command == CommandEnum.END_TURN);

        int result = 1;
        SeededGameState currentState = inputState;
        List<Move> currentPossibleMoves = inputPossibleMoves;

        while(currentPossibleMoves.Count > 0) {

            var obviousMove = FindObviousMove(currentPossibleMoves);
            if (obviousMove != null) {
                (currentState, currentPossibleMoves) = currentState.ApplyMove(obviousMove);
            }
            else if (currentPossibleMoves.Count == 1) {
                // TODO add this to ovious moves instead
                // we already checked that its not end turn, so this is make choice in cases where there is only one choice
                (currentState, currentPossibleMoves) = currentState.ApplyMove(currentPossibleMoves[0]);
            }
            else {
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

    private double GetTimeSpentBeforeTurn(TimeSpan remainingTime){
        return 10_000d - remainingTime.TotalMilliseconds;
    }
    public override PatronId SelectPatron(List<PatronId> availablePatrons, int round)
    {
        return availablePatrons[0];
    }
}
