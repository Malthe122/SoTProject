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
        int runs = 1000;
        int noOfThreads = 128;
        var timeout = 10;

        var gamesPerThread = runs / noOfThreads;
        var gamesPerThreadRemainder = runs % noOfThreads;
        var threads = new Task<List<EndGameState>>[noOfThreads];
        
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
                    {"Winner", endReason.Winner}
                };
                logger.Log(data);

                watch.Stop();
                results[i] = endReason;
                timeMeasurements[i] = watch.ElapsedMilliseconds;
            }
        
            Console.WriteLine($"Thread #{threadNo} finished. Total: {timeMeasurements.Sum()}ms, average: {timeMeasurements.Average()}ms.");
        
            return results.ToList();
        }

        var watch = Stopwatch.StartNew();
        for (var i = 0; i < noOfThreads; i++)
        {
            var spawnAdditionalGame = gamesPerThreadRemainder <= 0 ? 0 : 1;
            gamesPerThreadRemainder -= 1;
            var gamesToPlay = gamesPerThread + spawnAdditionalGame;
            Console.WriteLine($"Playing {gamesToPlay} games in thread #{i}");
            var threadNo = i;
            var seed = (ulong)new Random().NextInt64();
            threads[i] = Task.Factory.StartNew(() => PlayGames(gamesToPlay, threadNo, seed));
        }
        Task.WaitAll(threads.ToArray<Task>());
        
        var timeTaken = watch.ElapsedMilliseconds;
        
        Console.WriteLine($"\nInitial seed used: {actualSeed}");
        Console.WriteLine($"Total time taken: {timeTaken}ms");
    }
}
