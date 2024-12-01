using Bots;
using GeneticSharp;
using ScriptsOfTribute;
using ScriptsOfTribute.Serializers;

namespace Aau903Bot;

class FitnessFunction : IFitness
{
    public double Evaluate(IChromosome chromosome)
    {
        var ITERATION_COMPLETION_MILLISECONDS_BUFFER = (double)chromosome.GetGene(0).Value;
        var UCT_EXPLORATION_CONSTANT = (double)chromosome.GetGene(1).Value;
        var FORCE_DELAY_TURN_END_IN_ROLLOUT = (bool)chromosome.GetGene(2).Value;
        var INCLUDE_PLAY_MOVE_CHANCE_NODES = (bool)chromosome.GetGene(3).Value;
        var INCLUDE_END_TURN_CHANCE_NODES = (bool)chromosome.GetGene(4).Value;
        var CHOSEN_MAX_EXPANSION_DEPTH = (int)chromosome.GetGene(5).Value;
        var CHOSEN_SCORING_METHOD = (string)chromosome.GetGene(6).Value;
        var ROLLOUT_TURNS_BEFORE_HEURSISTIC = (int)chromosome.GetGene(7).Value;

        var data = new Dictionary<string, string>
        {
            {"ITERATION_COMPLETION_MILLISECONDS_BUFFER", ITERATION_COMPLETION_MILLISECONDS_BUFFER.ToString()},
            {"UCT_EXPLORATION_CONSTANT", UCT_EXPLORATION_CONSTANT.ToString()},
            {"FORCE_DELAY_TURN_END_IN_ROLLOUT", FORCE_DELAY_TURN_END_IN_ROLLOUT.ToString()},
            {"INCLUDE_PLAY_MOVE_CHANCE_NODES", INCLUDE_PLAY_MOVE_CHANCE_NODES.ToString()},
            {"INCLUDE_END_TURN_CHANCE_NODES", INCLUDE_END_TURN_CHANCE_NODES.ToString()},
            {"CHOSEN_MAX_EXPANSION_DEPTH", CHOSEN_MAX_EXPANSION_DEPTH.ToString()},
            {"CHOSEN_SCORING_METHOD", CHOSEN_SCORING_METHOD},
            {"ROLLOUT_TURNS_BEFORE_HEURSISTIC", ROLLOUT_TURNS_BEFORE_HEURSISTIC.ToString()},
        };
        Settings.SaveEnvFile("environment", data);

        var timeout = 10;

        // BOTS CONFIG
        var bot1 = new Aau903Bot();
        var bot2 = new RandomBot();

        // GAME CONFIG
        var game = new ScriptsOfTribute.AI.ScriptsOfTribute(bot1, bot2, TimeSpan.FromSeconds(timeout));

        var (endGameState, fullGameState) = game.Play();
        var seededGameState = new SeededGameState(fullGameState);
        return Utility.ScoreEndOfGame(seededGameState);
    }
}