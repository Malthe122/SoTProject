using System.Diagnostics;
using System.Reflection;

using ScriptsOfTribute;
using ScriptsOfTribute.AI;
using ScriptsOfTribute.Board;

using CsvLoggerLibrary;

namespace Benchmarks;

class Program
{
    static void Main(string[] args)
    {
        // GLOBAL CONFIG
        var logger = new CsvBenchmarkLogger();
        int runs = 100;
        int noOfThreads = 10; // Match the number of logical processors
        var timeout = 10;

        var gamesPerThread = runs / noOfThreads;
        var gamesPerThreadRemainder = runs % noOfThreads;

        List<EndGameState> PlayGames(int amount, int threadNo, ulong seed)
        {
            var results = new EndGameState[amount];
            var timeMeasurements = new long[amount];
            var watch = new Stopwatch();

            for (var i = 0; i < amount; i++)
            {
                // BOTS CONFIG
                var bot1 = new Aau903Bot.Aau903Bot();
                var bot2 = new MCTSBot.MCTSBot();

                var game = new ScriptsOfTribute.AI.ScriptsOfTribute(bot1, bot2, TimeSpan.FromSeconds(timeout));
                game.Seed = seed;
                seed += 1;

                watch.Reset();
                watch.Start();
                var (endReason, _) = game.Play();
                var data = new Dictionary<string, object>
                {
                    { "Winner", endReason.Winner },
                    { "Reason", endReason.Reason }
                };
                logger.Log(data);

                watch.Stop();
                results[i] = endReason;
                timeMeasurements[i] = watch.ElapsedMilliseconds;
            }

            Console.WriteLine($"Thread #{threadNo} finished. Total: {timeMeasurements.Sum()}ms, average: {timeMeasurements.Average()}ms.");
            return results.ToList();
        }

        // Set process priority for better performance
        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

        var watch = Stopwatch.StartNew();
        var threads = new Thread[noOfThreads];

        for (var i = 0; i < noOfThreads; i++)
        {
            var spawnAdditionalGame = gamesPerThreadRemainder <= 0 ? 0 : 1;
            gamesPerThreadRemainder -= 1;
            var gamesToPlay = gamesPerThread + spawnAdditionalGame;
            var threadNo = i;
            var seed = (ulong)new Random().NextInt64();

            Console.WriteLine($"Playing {gamesToPlay} games in thread #{threadNo}");
            Console.WriteLine($"\nInitial seed used for thread: {seed}");

            // Create and start a dedicated thread
            threads[i] = new Thread(() => PlayGames(gamesToPlay, threadNo, seed));
            threads[i].Start();
        }

        // Wait for all threads to complete
        foreach (var thread in threads)
        {
            thread.Join();
        }

        var timeTaken = watch.ElapsedMilliseconds;
        Console.WriteLine($"Total time taken: {timeTaken}ms");
    }
}
