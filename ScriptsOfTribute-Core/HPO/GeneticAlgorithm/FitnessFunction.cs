using Bots;
using GeneticSharp;
using ScriptsOfTribute.Serializers;
using System.Globalization;

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
        var CHOSEN_SCORING_METHOD = (string)chromosome.GetGene(5).Value;
        var ROLLOUT_TURNS_BEFORE_HEURSISTIC = (int)chromosome.GetGene(6).Value;
        var EQUAL_CHANCE_NODE_DISTRIBUTION = (bool)chromosome.GetGene(7).Value;
        var REUSE_TREE = (bool)chromosome.GetGene(8).Value;

        var data = new Dictionary<string, string>
        {
            {"ITERATION_COMPLETION_MILLISECONDS_BUFFER", ITERATION_COMPLETION_MILLISECONDS_BUFFER.ToString(CultureInfo.InvariantCulture)},
            {"UCT_EXPLORATION_CONSTANT", UCT_EXPLORATION_CONSTANT.ToString(CultureInfo.InvariantCulture)},
            {"FORCE_DELAY_TURN_END_IN_ROLLOUT", FORCE_DELAY_TURN_END_IN_ROLLOUT.ToString(CultureInfo.InvariantCulture)},
            {"INCLUDE_PLAY_MOVE_CHANCE_NODES", INCLUDE_PLAY_MOVE_CHANCE_NODES.ToString(CultureInfo.InvariantCulture)},
            {"INCLUDE_END_TURN_CHANCE_NODES", INCLUDE_END_TURN_CHANCE_NODES.ToString(CultureInfo.InvariantCulture)},
            {"CHOSEN_SCORING_METHOD", CHOSEN_SCORING_METHOD},
            {"ROLLOUT_TURNS_BEFORE_HEURSISTIC", ROLLOUT_TURNS_BEFORE_HEURSISTIC.ToString(CultureInfo.InvariantCulture)},
            {"EQUAL_CHANCE_NODE_DISTRIBUTION", EQUAL_CHANCE_NODE_DISTRIBUTION.ToString(CultureInfo.InvariantCulture)},
            {"REUSE_TREE", REUSE_TREE.ToString(CultureInfo.InvariantCulture)},
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

    private double ScoreEndOfGame(SeededGameState gameState)
    {
        double score = 1.0;
        // We need to make sure the players haven't been swapped out
        // when extracting the seededGameState out of the fullGameState
        var currentPlayer = gameState.CurrentPlayer.PlayerID == PlayerEnum.PLAYER1 ? gameState.CurrentPlayer : gameState.EnemyPlayer;
        var enemyPlayer = gameState.EnemyPlayer.PlayerID == PlayerEnum.PLAYER2 ? gameState.EnemyPlayer : gameState.CurrentPlayer;

        var prestigeDifference = currentPlayer.Prestige - enemyPlayer.Prestige;
        var currentPlayerFavors = gameState.PatronStates.All.Count(patronState => patronState.Value == currentPlayer.PlayerID);
        var enemyPlayerFavors = gameState.PatronStates.All.Count(patronState => patronState.Value == enemyPlayer.PlayerID);

        // Winning state patron favors
        if (currentPlayerFavors == 4 && prestigeDifference > 0)
        {
            var prestigeWeight = currentPlayer.Prestige >= 40 ? 1.0 : 0.5;
            score += prestigeWeight * Math.Min(prestigeDifference / 80.0, prestigeWeight);
        }
        // Loosing state patron favors
        else if (enemyPlayerFavors == 4)
        {
            score -= 0.5;
            if (prestigeDifference < 0)
            {
                var prestigeWeight = enemyPlayer.Prestige >= 40 ? 0.5 : 0.25;
                score -= prestigeWeight * Math.Min(-prestigeDifference / 80.0, prestigeWeight);
            }
            else
            {
                var prestigeWeight = currentPlayer.Prestige >= 40 ? 0.5 : 0.25;
                score += prestigeWeight * Math.Min(prestigeDifference / 80.0, prestigeWeight);
            }
        }
        // Winning state prestige
        else if (currentPlayer.Prestige >= 40 && prestigeDifference > 0)
        {
            var weight = 0.5;
            score += Math.Min(currentPlayerFavors / 4.0, weight) * weight;
            score += Math.Min(prestigeDifference / 80.0, weight) * weight;
        }
        // Loosing state prestige
        else if (enemyPlayer.Prestige >= 40 && prestigeDifference < 0)
        {
            var weight = 0.5;
            score -= Math.Min(enemyPlayerFavors / 4.0, weight) * weight;
            score -= Math.Min(-prestigeDifference / 80.0, weight) * weight;
        }
        // Draw state
        else
        {
            var weight = 0.5;
            score -= 0.5;
            score += Math.Min(prestigeDifference / 80.0, weight) * weight;
        }

        return score;
    }
}