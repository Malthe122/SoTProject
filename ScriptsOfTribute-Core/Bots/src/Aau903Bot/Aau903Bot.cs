using ScriptsOfTribute;
using ScriptsOfTribute.AI;
using ScriptsOfTribute.Board;
using ScriptsOfTribute.Serializers;

public class Aau903Bot : AI
{
    public override void GameEnd(EndGameState state, FullGameState? finalBoardState)
    {
        Console.WriteLine("@@@ Game ended because of " + state.Reason + " @@@");
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

            ulong randomSeed = (ulong)Utility.Rng.Next();
            var seededGameState = gameState.ToSeededGameState(randomSeed);
            var rootNode = new Node(seededGameState, null, possibleMoves, null);

            for (int i = 0; i <= MCTSHyperparameters.ITERATIONS; i++)
            {
                rootNode.Visit(out double score);
            }

            var bestChildNode = rootNode.ChildNodes
                .OrderByDescending(child => (child.TotalScore / child.VisitCount))
                .FirstOrDefault();

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

    public override PatronId SelectPatron(List<PatronId> availablePatrons, int round)
    {
        return availablePatrons[0];
    }
}
