using GeneticSharp;

namespace Aau903Bot;

class Program
{
    static void Main(string[] args)
    {
        var selection = new EliteSelection();
        var crossover = new OnePointCrossover();
        var mutation = new UniformMutation(true);
        var fitness = new FitnessFunction(); // WE MADE THIS WOO
        var chromosome = new Chromosome(); // ALSO THIS YIPPIE
        var population = new Population(100, 100, chromosome);

        var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation)
        {
            TaskExecutor = new ParallelTaskExecutor()
        };
        ga.GenerationRan += (sender, e) =>
        {
            var bestChromosome = ga.BestChromosome as Chromosome;
            if (bestChromosome != null)
            {
                var currentGeneration = ga.GenerationsNumber;
                var fitness = (double)ga.BestChromosome.Fitness!;
                string fileName = $"generations/gen_{currentGeneration}_{fitness.ToString("F2")}";
                bestChromosome.SaveGenes(fileName);
            }
        };

        ga.Termination = new GenerationNumberTermination(200);

        Console.WriteLine("GA running...");
        ga.Start();
        Console.WriteLine($"GA done in {ga.GenerationsNumber} generations.");

        var bestChromosome = ga.BestChromosome as Chromosome;
        if (bestChromosome != null)
        {
            var bestFitness = (double)bestChromosome.Fitness!;
            string fileName = $"best_{bestFitness.ToString("F2")}";
            Console.WriteLine(bestChromosome);
            bestChromosome.SaveGenes(fileName);
        }
    }
}
