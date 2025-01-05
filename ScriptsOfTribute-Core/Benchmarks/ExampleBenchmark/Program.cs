using ScriptsOfTribute;
using ScriptsOfTribute.AI;
using ScriptsOfTribute.Board;
using ScriptsOfTribute.Serializers;
using CsvLoggerLibrary;

namespace Benchmarks;

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