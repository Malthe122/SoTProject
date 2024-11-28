using Bots;
using GeneticSharp;
using ScriptsOfTribute;

namespace Aau903Bot;

class FitnessFunction : IFitness
{
    public double Evaluate(IChromosome chromosome)
    {
        var ITERATION_COMPLETION_MILLISECONDS_BUFFER = (string)chromosome.GetGene(0).Value;
        var UCB1_EXPLORATION_CONSTANT = (string)chromosome.GetGene(1).Value;
        var NUMBER_OF_ROLLOUTS = (string)chromosome.GetGene(2).Value;
        var FORCE_DELAY_TURN_END_IN_ROLLOUT = (string)chromosome.GetGene(3).Value;
        var INCLUDE_PLAY_MOVE_CHANCE_NODES = (string)chromosome.GetGene(4).Value;
        var INCLUDE_END_TURN_CHANCE_NODES = (string)chromosome.GetGene(5).Value;
        var CHOSEN_HASH_GENERATION_TYPE = (string)chromosome.GetGene(6).Value;
        var CHOSEN_MAX_EXPANSION_DEPTH = (string)chromosome.GetGene(7).Value;
        var CHOSEN_SCORING_METHOD = (string)chromosome.GetGene(8).Value;
        var ROLLOUT_TURNS_BEFORE_HEURSISTIC = (string)chromosome.GetGene(9).Value;

        var data = new Dictionary<string, string>
        {
            {"ITERATION_COMPLETION_MILLISECONDS_BUFFER", ITERATION_COMPLETION_MILLISECONDS_BUFFER},
            {"UCB1_EXPLORATION_CONSTANT", UCB1_EXPLORATION_CONSTANT},
            {"NUMBER_OF_ROLLOUTS", NUMBER_OF_ROLLOUTS},
            {"FORCE_DELAY_TURN_END_IN_ROLLOUT",FORCE_DELAY_TURN_END_IN_ROLLOUT },
            {"INCLUDE_PLAY_MOVE_CHANCE_NODES",INCLUDE_PLAY_MOVE_CHANCE_NODES },
            {"INCLUDE_END_TURN_CHANCE_NODES",INCLUDE_END_TURN_CHANCE_NODES },
            {"CHOSEN_HASH_GENERATION_TYPE", CHOSEN_HASH_GENERATION_TYPE},
            {"CHOSEN_MAX_EXPANSION_DEPTH", CHOSEN_MAX_EXPANSION_DEPTH},
            {"CHOSEN_SCORING_METHOD", CHOSEN_SCORING_METHOD},
            {"ROLLOUT_TURNS_BEFORE_HEURSISTIC", ROLLOUT_TURNS_BEFORE_HEURSISTIC}
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