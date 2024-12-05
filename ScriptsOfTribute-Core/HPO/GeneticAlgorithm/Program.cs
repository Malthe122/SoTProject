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
        var population = new Population(50, 50, chromosome);

        var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation)
        {
            TaskExecutor = new ParallelTaskExecutor()
        };
        ga.Termination = new GenerationNumberTermination(1);

        Console.WriteLine("GA running...");
        ga.Start();
        Console.WriteLine($"GA done in {ga.GenerationsNumber} generations.");

        var bestChromosome = ga.BestChromosome as Chromosome;

        ShowChromosome(bestChromosome);
    }

    private static void ShowChromosome(Chromosome? c)
    {
        if (c != null)
        {
            Console.WriteLine($@"[SUCCESS] Best solution found");
            Console.WriteLine("Fitness: {0:n2}", c.Fitness);
            Console.WriteLine("Cities: {0:n0}", c.Length);

            var cities = c.GetGenes().Select(g => g.Value.ToString()).ToArray();
            Console.WriteLine("City tour: {0}", string.Join(", ", cities));

            Console.WriteLine($@"
                ITERATION_COMPLETION_MILLISECONDS_BUFFER: {c.ITERATION_COMPLETION_MILLISECONDS_BUFFER}
                UCT_EXPLORATION_CONSTANT: {c.UCT_EXPLORATION_CONSTANT}
                FORCE_DELAY_TURN_END_IN_ROLLOUT: {c.FORCE_DELAY_TURN_END_IN_ROLLOUT}
                INCLUDE_PLAY_MOVE_CHANCE_NODES: {c.INCLUDE_PLAY_MOVE_CHANCE_NODES}
                INCLUDE_END_TURN_CHANCE_NODES: {c.INCLUDE_END_TURN_CHANCE_NODES}
                CHOSEN_SCORING_METHOD: {c.CHOSEN_SCORING_METHOD}
                ROLLOUT_TURNS_BEFORE_HEURSISTIC: {c.ROLLOUT_TURNS_BEFORE_HEURSISTIC}            
                EQUAL_CHANCE_NODE_DISTRIBUTION: {c.EQUAL_CHANCE_NODE_DISTRIBUTION}
                REUSE_TREE: {c.REUSE_TREE}
            ");
        }
        else
        {
            Console.WriteLine("[ERROR] Could not compute best solution");            
        }
    }
}