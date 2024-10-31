using Microsoft.Extensions.Configuration;

public static class MCTSHyperparameters
{
    public static EvaluationFunction CHOSEN_EVALUATION_FUNCTION { get; set; }
    /// <summary>
    /// Default value is 1.41421356237, which is the square root of 2
    /// </summary>
    public static double UCB1_EXPLORATION_CONSTANT { get; set; } = 1.41421356237;
    public static int NUMBER_OF_ROLLOUTS { get; set; } = 10;
    public static int ITERATIONS { get; set; } = 20;
    /// <summary>
    /// Tells in the rollout whether the agents plays all their possible moves before ending turn or if end turn is an allowed move in any part of their turn
    /// Idea is that setting this to true will first of all be closer to a realistic simulation and also it should end the game quicker, making the simulation
    /// faster than if agents were allowed to spend moves ending turns without really doing anything in the game
    /// </summary>
    public static bool FORCE_DELAY_TURN_END_IN_ROLLOUT { get; set; } = true;
    public static bool INCLUDE_CHANCE_NODES { get; set; } = true;
    public static HashGenerationType CHOSEN_HASH_GENERATION_TYPE { get; set; } = HashGenerationType.Quick;

    static MCTSHyperparameters()
    {
        Settings.LoadEnvFile("environment");
        var config = Settings.GetConfiguration();

        ITERATIONS = config.GetValue("ITERATIONS", 20);
        NUMBER_OF_ROLLOUTS = config.GetValue("NUMBER_OF_ROLLOUTS", 10);

        UCB1_EXPLORATION_CONSTANT = config.GetValue("UCB1_EXPLORATION_CONSTANT", 1.41421356237);
        FORCE_DELAY_TURN_END_IN_ROLLOUT = config.GetValue("FORCE_DELAY_TURN_END_IN_ROLLOUT", true);
        INCLUDE_CHANCE_NODES = config.GetValue("INCLUDE_CHANCE_NODES", true);

        var chosen_evaluation_function = config.GetValue<string>("CHOSEN_EVALUATION_FUNCTION");
        CHOSEN_EVALUATION_FUNCTION = EnumHelper.ToEvaluationFnction(chosen_evaluation_function);

        var chosen_hash_generation_type = config.GetValue<string>("CHOSEN_HASH_GENERATION_TYPE");
        CHOSEN_HASH_GENERATION_TYPE = EnumHelper.ToHashGenerationType(chosen_hash_generation_type);
    }
}

public class Settings
{
    public static void LoadEnvFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.Error.WriteLine($"Env file '{filePath}' not found");
            return;
        }

        foreach (var line in File.ReadAllLines(filePath))
        {
            if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
                continue;

            var parts = line.Split('=', 2);
            if (parts.Length != 2)
                continue;

            var key = parts[0].Trim();
            var value = parts[1].Trim().Trim('"');

            Environment.SetEnvironmentVariable(key, value);
        }
    }

    public static IConfiguration GetConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .AddEnvironmentVariables();

        IConfiguration configuration = builder.Build();

        return configuration;
    }
}
