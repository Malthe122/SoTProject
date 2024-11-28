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
        var population = new Population(50,50, chromosome);

        var ga = new GeneticAlgorithm(population,fitness,selection,crossover,mutation);
        ga.Termination = new GenerationNumberTermination(100);

        Console.WriteLine("GA running...");
        ga.Start();
        Console.WriteLine($"GA done in {ga.GenerationsNumber} generations.");

        var bestChromosome = ga.BestChromosome as Chromosome;
        Console.WriteLine($@"Best solution found is: 
        ITERATION_COMPLETION_MILLISECONDS_BUFFER: {bestChromosome.ITERATION_COMPLETION_MILLISECONDS_BUFFER}
        UCB1_EXPLORATION_CONSTANT: {bestChromosome.UCB1_EXPLORATION_CONSTANT}
        NUMBER_OF_ROLLOUTS: {bestChromosome.NUMBER_OF_ROLLOUTS}
        FORCE_DELAY_TURN_END_IN_ROLLOUT: {bestChromosome.FORCE_DELAY_TURN_END_IN_ROLLOUT}
        INCLUDE_PLAY_MOVE_CHANCE_NODES: {bestChromosome.INCLUDE_PLAY_MOVE_CHANCE_NODES}
        INCLUDE_END_TURN_CHANCE_NODES: {bestChromosome.INCLUDE_END_TURN_CHANCE_NODES}
        CHOSEN_HASH_GENERATION_TYPE: {bestChromosome.CHOSEN_HASH_GENERATION_TYPE}
        CHOSEN_MAX_EXPANSION_DEPTH: {bestChromosome.CHOSEN_MAX_EXPANSION_DEPTH}
        CHOSEN_SCORING_METHOD: {bestChromosome.CHOSEN_SCORING_METHOD}
        ROLLOUT_TURNS_BEFORE_HEURSISTIC: {bestChromosome.ROLLOUT_TURNS_BEFORE_HEURSISTIC}
        ");
        Console.ReadKey();
    }
}