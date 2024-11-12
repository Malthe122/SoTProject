using ScriptsOfTribute;
using ScriptsOfTribute.AI;
using ScriptsOfTribute.Board;
using ScriptsOfTribute.Serializers;
using CsvLoggerLibrary;

namespace Aau903Bot;

public class Aau903Bot : AI
{
    // Required attributes for the benchmark to work
    private CsvBenchmarkLogger logger;
    private TimeSpan timeout;
    private readonly SeededRandom rng;

    public Aau903Bot(TimeSpan timeout, SeededRandom rng, CsvBenchmarkLogger logger)
    {
        this.logger = logger;
        this.timeout = timeout;
        this.rng = rng;
    }

    public override void GameEnd(EndGameState state, FullGameState? finalBoardState) { }

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
