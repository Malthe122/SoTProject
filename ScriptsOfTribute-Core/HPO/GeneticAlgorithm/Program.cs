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

        var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
        ga.Termination = new GenerationNumberTermination(100);

        Console.WriteLine("GA running...");
        ga.Start();
        Console.WriteLine($"GA done in {ga.GenerationsNumber} generations.");

        var bestChromosome = ga.BestChromosome as Chromosome;
        if (bestChromosome != null)
        {
            Console.WriteLine($@"[SUCCESS] Best solution found is: 
            ITERATION_COMPLETION_MILLISECONDS_BUFFER: {bestChromosome.ITERATION_COMPLETION_MILLISECONDS_BUFFER}
            UCT_EXPLORATION_CONSTANT: {bestChromosome.UCT_EXPLORATION_CONSTANT}
            FORCE_DELAY_TURN_END_IN_ROLLOUT: {bestChromosome.FORCE_DELAY_TURN_END_IN_ROLLOUT}
            INCLUDE_PLAY_MOVE_CHANCE_NODES: {bestChromosome.INCLUDE_PLAY_MOVE_CHANCE_NODES}
            INCLUDE_END_TURN_CHANCE_NODES: {bestChromosome.INCLUDE_END_TURN_CHANCE_NODES}
            CHOSEN_SCORING_METHOD: {bestChromosome.CHOSEN_SCORING_METHOD}
            ROLLOUT_TURNS_BEFORE_HEURSISTIC: {bestChromosome.ROLLOUT_TURNS_BEFORE_HEURSISTIC}            
            EQUAL_CHANCE_NODE_DISTRIBUTION: {bestChromosome.EQUAL_CHANCE_NODE_DISTRIBUTION}
            REUSE_TREE: {bestChromosome.REUSE_TREE}
            ");
        }
        else
        {
            Console.WriteLine("[ERROR] Could not compute best solution");
        }
    }
}