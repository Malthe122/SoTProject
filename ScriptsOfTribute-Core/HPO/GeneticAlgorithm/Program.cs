using GeneticSharp;

namespace Aau903Bot;

class Program
{
    static void Main(string[] args)
    {
        var selection = new RouletteWheelSelection();
        var crossover = new OnePointCrossover();
        var mutation = new UniformMutation(true);
        var fitness = new FitnessFunction();
        var chromosome = new Chromosome();
        var population = new Population(100, 100, chromosome);

        var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation
        )
        {
            CrossoverProbability = 0.9f,
            MutationProbability = 0.1f,
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

        ga.Termination = new GenerationNumberTermination(100);

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
