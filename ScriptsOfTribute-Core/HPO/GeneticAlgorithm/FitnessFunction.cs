using Bots;
using GeneticSharp;
using ScriptsOfTribute.Serializers;
using ScriptsOfTribute.Board;
using ScriptsOfTribute;
using System.Globalization;
using Sakkarin;
using SOISMCTS_;
using hql_bot;
using BestMCTS3_;

namespace Aau903Bot;

class FitnessFunction : IFitness
{
    public double Evaluate(IChromosome chromosome)
    {
        // Generate a unique filename using a GUID
        string fileName = $"environments/environment_{Guid.NewGuid()}";
        (chromosome as Chromosome)!.SaveGenes(fileName);
        var bot1 = new Aau903Bot();
        bot1.Params = new MCTSHyperparameters(fileName);

        double score0 = 0, score1 = 0, score2 = 0;
        double weight0 = 0.6, weight1 = 0.3, weight2 = 0.1;
        int iterations = 1;

        try
        {
            for (int i = 0; i < iterations; i++)
            {
                var bot2 = new MCTSBot();

        double score = 0.0;
        try {
            var timeout = 10;
            var aauBot = new Aau903Bot(uniqueFileName);

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
                var gameResult = new ScriptsOfTribute.AI.ScriptsOfTribute(aauBot, randomBot, TimeSpan.FromSeconds(timeout)).Play().Item1;
                if (gameResult.Winner == PlayerEnum.PLAYER1) {
                    score += 1;
                }
            }

            if (score < 1) { //if bot cant beat random bot, there is no reason spending time playing the other bots
                return score;
            }

            for(int i = 0; i < 2; i++) {
                var gameResult = new ScriptsOfTribute.AI.ScriptsOfTribute(aauBot, maxPrestigeBot, TimeSpan.FromSeconds(timeout)).Play().Item1;
                if (gameResult.Winner == PlayerEnum.PLAYER1) {
                    score += 10;
                }
            }

            if (score < 10) { //if bot cant beat max prestige bot, there is no reason spending time playing the other bots
                return score;
            }

            for(int i = 0; i < 2; i++) {
                var gameResult = new ScriptsOfTribute.AI.ScriptsOfTribute(aauBot, decisionTreeBot, TimeSpan.FromSeconds(timeout)).Play().Item1;
                if (gameResult.Winner == PlayerEnum.PLAYER1) {
                    score += 100;
                }
            }

            if (score < 100) { //if bot cant beat decision tree bot, there is no reason spending time playing the other bots
                return score;
            }

            for(int i = 0; i < 2; i++) {
                var gameResult = new ScriptsOfTribute.AI.ScriptsOfTribute(aauBot, mctsBot, TimeSpan.FromSeconds(timeout)).Play().Item1;
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
            Settings.RemoveEnvFile(fileName);
        }

        return (score0 * weight0 + score1 * weight1 + score2 * weight2) / iterations;
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