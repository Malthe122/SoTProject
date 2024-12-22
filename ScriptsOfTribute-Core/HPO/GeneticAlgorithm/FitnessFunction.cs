using Bots;
using GeneticSharp;
using ScriptsOfTribute.Serializers;
using ScriptsOfTribute.Board;
using ScriptsOfTribute;
using System.Globalization;
using ScriptsOfTribute.AI;

namespace Aau903Bot;

class FitnessFunction : IFitness
{
    public double Evaluate(IChromosome chromosome)
    {
        // Generate a unique filename using a GUID
        string uniqueFileName = $"environment_{Guid.NewGuid()}";

        var ITERATION_COMPLETION_MILLISECONDS_BUFFER = (double)chromosome.GetGene(0).Value;
        var UCT_EXPLORATION_CONSTANT = (double)chromosome.GetGene(1).Value;
        var FORCE_DELAY_TURN_END_IN_ROLLOUT = (bool)chromosome.GetGene(2).Value;
        var INCLUDE_PLAY_MOVE_CHANCE_NODES = (bool)chromosome.GetGene(3).Value;
        var INCLUDE_END_TURN_CHANCE_NODES = (bool)chromosome.GetGene(4).Value;
        var CHOSEN_SCORING_METHOD = (string)chromosome.GetGene(5).Value;
        var ROLLOUT_TURNS_BEFORE_HEURSISTIC = (int)chromosome.GetGene(6).Value;
        var REUSE_TREE = (bool)chromosome.GetGene(7).Value;

        var data = new Dictionary<string, string>
        {
            {"ITERATION_COMPLETION_MILLISECONDS_BUFFER", ITERATION_COMPLETION_MILLISECONDS_BUFFER.ToString(CultureInfo.InvariantCulture)},
            {"UCT_EXPLORATION_CONSTANT", UCT_EXPLORATION_CONSTANT.ToString(CultureInfo.InvariantCulture)},
            {"FORCE_DELAY_TURN_END_IN_ROLLOUT", FORCE_DELAY_TURN_END_IN_ROLLOUT.ToString(CultureInfo.InvariantCulture)},
            {"INCLUDE_PLAY_MOVE_CHANCE_NODES", INCLUDE_PLAY_MOVE_CHANCE_NODES.ToString(CultureInfo.InvariantCulture)},
            {"INCLUDE_END_TURN_CHANCE_NODES", INCLUDE_END_TURN_CHANCE_NODES.ToString(CultureInfo.InvariantCulture)},
            {"CHOSEN_SCORING_METHOD", CHOSEN_SCORING_METHOD},
            {"ROLLOUT_TURNS_BEFORE_HEURSISTIC", ROLLOUT_TURNS_BEFORE_HEURSISTIC.ToString(CultureInfo.InvariantCulture)},
            {"REUSE_TREE", REUSE_TREE.ToString(CultureInfo.InvariantCulture)},
        };
        Settings.SaveEnvFile(uniqueFileName, data);

        double score = 0.0;
        try {
            var timeout = 10;
            var aauBot = new Aau903Bot();
            aauBot.Params = new MCTSHyperparameters(uniqueFileName);

            // built-in bots ranked by winrate
            var randomBot = new RandomBot();
            var maxPrestigeBot = new MaxPrestigeBot();
            var decisionTreeBot = new DecisionTreeBot();
            var mctsBot = new MCTSBot();

            // competition bots ranked by winrate
            // var sakkirinBot = new Sakkirin();
            // // ToT skipped for now, cause it needs to be used differently cause its written in Python
            // var soisMctsBot = new SOISMCTS();
            // var hqlBot = new HQL_BOT();
            // var bestMcts3 = new BestMCTS3();


            // Games
            for(int i = 0; i < 2; i++) {
                aauBot = new Aau903Bot();
                aauBot.Params = new MCTSHyperparameters(uniqueFileName);
                var gameResult = PlayGame(aauBot, randomBot, timeout);
                if (gameResult.Winner == PlayerEnum.PLAYER1) {
                    score += 1;
                }
            }

            if (score < 1) { //if bot cant beat random bot, there is no reason spending time playing the other bots
                return score;
            }

            for(int i = 0; i < 2; i++) {
                aauBot = new Aau903Bot();
                aauBot.Params = new MCTSHyperparameters(uniqueFileName);
                var gameResult = PlayGame(aauBot, maxPrestigeBot, timeout);
                if (gameResult.Winner == PlayerEnum.PLAYER1) {
                    score += 10;
                }
            }

            if (score < 10) { //if bot cant beat max prestige bot, there is no reason spending time playing the other bots
                return score;
            }

            for(int i = 0; i < 2; i++) {
                aauBot = new Aau903Bot();
                aauBot.Params = new MCTSHyperparameters(uniqueFileName);
                var gameResult = PlayGame(aauBot, decisionTreeBot, timeout);
                if (gameResult.Winner == PlayerEnum.PLAYER1) {
                    score += 100;
                }
            }

            if (score < 100) { //if bot cant beat decision tree bot, there is no reason spending time playing the other bots
                return score;
            }

            for(int i = 0; i < 2; i++) {
                aauBot = new Aau903Bot();
                aauBot.Params = new MCTSHyperparameters(uniqueFileName);
                var gameResult = PlayGame(aauBot, mctsBot, timeout);
                if (gameResult.Winner == PlayerEnum.PLAYER1) {
                    score += 1000;
                }
            }

            return score;

            // if (score < 1000) { //if bot cant beat mctsBot bot, there is no reason spending time playing the other bots
            //     return score;
            // }

            // TODO add the compition bots here, for a new GA (and remove the built in ones or most of them), when we have a solution that beats mcts
        }
        finally
        {
            Settings.RemoveEnvFile(uniqueFileName);
        }

        return score;
    }

    /// <summary>
    /// Sometimes exceptions happens in the framework. In these cases, we will replay the game. This is not bot specific exceptions, as in these cases, the game runner 
    /// will simply grant the victory to the opponent instead of rethrowing the exception
    /// </summary>
    private EndGameState PlayGame(AI bot1, AI bot2, int timeout) {
        try {
            return new ScriptsOfTribute.AI.ScriptsOfTribute(bot1, bot2, TimeSpan.FromSeconds(timeout)).Play().Item1;
        }
        catch{
            return PlayGame(bot1, bot2, timeout);
        }
    }

    private double ScoreEndOfGame(EndGameState endGameState, SeededGameState gameState)
    {
        var currentPlayer = gameState.CurrentPlayer.PlayerID == PlayerEnum.PLAYER1 ? gameState.CurrentPlayer : gameState.EnemyPlayer;
        var enemyPlayer = gameState.EnemyPlayer.PlayerID == PlayerEnum.PLAYER2 ? gameState.EnemyPlayer : gameState.CurrentPlayer;
        var currentPlayerFavorCount = gameState.PatronStates.All.Count(patronState => patronState.Value == currentPlayer.PlayerID);
        var enemyPlayerFavorCount = gameState.PatronStates.All.Count(patronState => patronState.Value == enemyPlayer.PlayerID);
        var prestigeDifference = Math.Abs(currentPlayer.Prestige - enemyPlayer.Prestige);

        var endGameReason = endGameState.Reason;
        var winner = endGameState.Winner;

        double prestigeMultiplier = 20;
        double favorMultiplier = 10;

        double score = 0.0;
        switch (endGameReason)
        {

            case GameEndReason.PRESTIGE_OVER_80: // Evaluated the same as PRESTIGE_OVER_40_NOT_MATCHED
            case GameEndReason.PRESTIGE_OVER_40_NOT_MATCHED: // PrestigeDifference will always reffer to the winning player here
                if (winner == currentPlayer.PlayerID) // Player1 wins because they gained over 40 prestige
                {
                    score += 500;
                    score += prestigeMultiplier * prestigeDifference;
                    score += Math.Pow(favorMultiplier, currentPlayerFavorCount);
                    score -= Math.Pow(favorMultiplier, enemyPlayerFavorCount);
                }
                else if (winner == enemyPlayer.PlayerID) // Player1 loses because opponent has prestige over 40
                {
                    score -= prestigeMultiplier * prestigeDifference;
                    score += Math.Pow(favorMultiplier, currentPlayerFavorCount);
                    score -= Math.Pow(favorMultiplier, enemyPlayerFavorCount);
                    score /= 2;
                }
                break;
            case GameEndReason.PATRON_FAVOR:
                if (winner == currentPlayer.PlayerID) // Player1 wins because they gained 4 patron favors
                {
                    score += 500;
                    if (currentPlayer.Prestige >= enemyPlayer.Prestige)
                    {
                        score += prestigeMultiplier * prestigeDifference;
                    }
                    else
                    {
                        score -= 0.5 * prestigeMultiplier * prestigeDifference;
                    }
                    score += 1000;
                }
                else if (winner == enemyPlayer.PlayerID) // Player1 loses because opponent has 4 patron favors
                {
                    if (currentPlayer.Prestige >= enemyPlayer.Prestige)
                    {
                        score += prestigeMultiplier * prestigeDifference;
                    }
                    else
                    {
                        score -= 0.5 * prestigeMultiplier * prestigeDifference;
                    }
                    score -= 1000;
                    score /= 2;
                }
                break;
            default:
                score -= 500;
                break;
        }

        double min = -1300; // Theoretical minimum score
        double max = 3100; // Theorectical maximum score
        double normalizedMin = 0; // Will be mapped between a min
        double normalizedMax = 1000; // and a max

        return ((score - min) * (normalizedMax - normalizedMin)) / (max - min);
    }
}