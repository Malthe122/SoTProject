using ScriptsOfTribute;
using ScriptsOfTribute.AI;
using ScriptsOfTribute.Board;
using ScriptsOfTribute.Serializers;
using CsvLoggerLibrary;

namespace Benchmarks;

public static class Extensions
{
    public static int RandomK(int lowerBound, int upperBoaund, SeededRandom rng)
    {
        return (rng.Next() % (upperBoaund - lowerBound)) + lowerBound;
    }
    public static T PickRandom<T>(this List<T> source, SeededRandom rng)
    {
        return source[rng.Next() % source.Count];
    }
}

public class RandomBot : AI
{
    // Required attributes for the benchmark to work
    private CsvBenchmarkLogger logger;
    private TimeSpan timeout;
    private readonly SeededRandom rng;

    // Your own attributes here
    private int movesPlayedDuringTurn = 0;
    private PlayerEnum playerId = PlayerEnum.NO_PLAYER_SELECTED;

    public RandomBot(TimeSpan timeout, SeededRandom rng, CsvBenchmarkLogger logger)
    {
        this.logger = logger;
        this.timeout = timeout;
        this.rng = rng;
    }

    public override PatronId SelectPatron(List<PatronId> availablePatrons, int round)
        => availablePatrons.PickRandom(rng);

    public override Move Play(GameState gameState, List<Move> possibleMoves, TimeSpan remainingTime)
    {
        if (playerId == PlayerEnum.NO_PLAYER_SELECTED)
        {
            playerId = gameState.CurrentPlayer.PlayerID;
        }
        // var playerId = gameState.CurrentPlayer.PlayerID;
        var randomMove = possibleMoves.PickRandom(rng);

        // if (randomMove.Command == CommandEnum.END_TURN)
        // {
        //     var totalMilliseconds = (timeout - remainingTime).TotalMicroseconds;
        //     var data = new Dictionary<string, object>
        //     {
        //         {"Player", playerId},
        //         {"Turn Time (ms)", totalMilliseconds},
        //         {"Moves played", this.movesPlayedDuringTurn},
        //         {"Winner", "-"}
        //     };
        //     this.logger.Log(data);
        //     this.movesPlayedDuringTurn = 0;
        // }
        // else
        // {
        //     this.movesPlayedDuringTurn++;
        // }
        return randomMove;
    }

    public override void GameEnd(EndGameState endGameState, FullGameState? finalBoardState)
    {

    }
}

class Program
{
    static void Main(string[] args)
    {
        // GLOBAL CONFIG
        var logger = new CsvBenchmarkLogger("some_results.csv");
        var timeout = 30;
        ulong seed = 42;

        // BOTS CONFIG
        var bot1 = new RandomBot(TimeSpan.FromSeconds(timeout), new SeededRandom(seed), logger);
        var bot2 = new RandomBot(TimeSpan.FromSeconds(timeout), new SeededRandom(seed), logger);

        // GAME CONFIG
        var game = new ScriptsOfTribute.AI.ScriptsOfTribute(bot1, bot2, TimeSpan.FromSeconds(timeout));
        game.Seed = seed;
        // game.P1LoggerEnabled = true;
        // game.P2LoggerEnabled = true;

        // RUN BENCHMARK
        for (int i = 0; i < 100; i++)
        {
            var (endGameState, fullGameState) = game.Play();

            var data = new Dictionary<string, object>
            {
                {"Run", i},
                {"Winner", endGameState.Winner}
            };
            logger.Log(data);
        }
    }
}