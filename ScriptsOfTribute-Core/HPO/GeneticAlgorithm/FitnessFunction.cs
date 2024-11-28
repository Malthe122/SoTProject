using Bots;
using GeneticSharp;
using ScriptsOfTribute;

namespace Aau903Bot;

class FitnessFunction : IFitness
{
    public double Evaluate(IChromosome chromosome)
    {
        // Correctly cast the chromosome's genes to the appropriate types
        var ITERATION_COMPLETION_MILLISECONDS_BUFFER = (double)chromosome.GetGene(0).Value;
        var UCB1_EXPLORATION_CONSTANT = (double)chromosome.GetGene(1).Value;
        var NUMBER_OF_ROLLOUTS = (int)chromosome.GetGene(2).Value;
        var FORCE_DELAY_TURN_END_IN_ROLLOUT = (bool)chromosome.GetGene(3).Value;
        var INCLUDE_PLAY_MOVE_CHANCE_NODES = (bool)chromosome.GetGene(4).Value;
        var INCLUDE_END_TURN_CHANCE_NODES = (bool)chromosome.GetGene(5).Value;
        var CHOSEN_HASH_GENERATION_TYPE = (string)chromosome.GetGene(6).Value;
        var CHOSEN_MAX_EXPANSION_DEPTH = (int)chromosome.GetGene(7).Value; // Set high for no effective limit
        var CHOSEN_SCORING_METHOD = (string)chromosome.GetGene(8).Value;
        var ROLLOUT_TURNS_BEFORE_HEURSISTIC = (int)chromosome.GetGene(9).Value;
        var DYNAMIC_MOVE_TIME_DISTRIBUTION = (bool)chromosome.GetGene(10).Value;

        // Prepare data for environment file
        var data = new Dictionary<string, string>
        {
            {"ITERATION_COMPLETION_MILLISECONDS_BUFFER", ITERATION_COMPLETION_MILLISECONDS_BUFFER.ToString()},
            {"UCB1_EXPLORATION_CONSTANT", UCB1_EXPLORATION_CONSTANT.ToString()},
            {"NUMBER_OF_ROLLOUTS", NUMBER_OF_ROLLOUTS.ToString()},
            {"FORCE_DELAY_TURN_END_IN_ROLLOUT", FORCE_DELAY_TURN_END_IN_ROLLOUT.ToString()},
            {"INCLUDE_PLAY_MOVE_CHANCE_NODES", INCLUDE_PLAY_MOVE_CHANCE_NODES.ToString()},
            {"INCLUDE_END_TURN_CHANCE_NODES", INCLUDE_END_TURN_CHANCE_NODES.ToString()},
            {"CHOSEN_HASH_GENERATION_TYPE", CHOSEN_HASH_GENERATION_TYPE},
            {"CHOSEN_MAX_EXPANSION_DEPTH", CHOSEN_MAX_EXPANSION_DEPTH.ToString()},
            {"CHOSEN_SCORING_METHOD", CHOSEN_SCORING_METHOD},
            {"ROLLOUT_TURNS_BEFORE_HEURSISTIC", ROLLOUT_TURNS_BEFORE_HEURSISTIC.ToString()},
            {"DYNAMIC_MOVE_TIME_DISTRIBUTION", DYNAMIC_MOVE_TIME_DISTRIBUTION.ToString()}
        };
        Settings.SaveEnvFile("environment",data);

        var timeout = 10;
        ulong seed = 42;

        // BOTS CONFIG
        var bot1 = new Aau903Bot();
        var bot2 = new RandomBot();

        // GAME CONFIG
        var game = new ScriptsOfTribute.AI.ScriptsOfTribute(bot1, bot2, TimeSpan.FromSeconds(timeout));
        game.Seed = seed;

        var (endGameState, fullGameState) = game.Play();

        return endGameState.Winner == PlayerEnum.PLAYER1 ? 1.0 : 0.0;
    }
}