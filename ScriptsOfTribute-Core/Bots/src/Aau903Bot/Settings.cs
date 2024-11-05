using Microsoft.Extensions.Configuration;

public static class MCTSHyperparameters
{
    public static EvaluationFunction CHOSEN_EVALUATION_FUNCTION { get; set; }
    /// <summary>
    /// Default value is 1.41421356237, which is the square root of 2
    /// </summary>
    public static double UCB1_EXPLORATION_CONSTANT { get; set; }
    public static int NUMBER_OF_ROLLOUTS { get; set; }
    public static int ITERATIONS { get; set; }
    /// <summary>
    /// Tells in the rollout whether the agents plays all their possible moves before ending turn or if end turn is an allowed move in any part of their turn
    /// Idea is that setting this to true will first of all be closer to a realistic simulation and also it should end the game quicker, making the simulation
    /// faster than if agents were allowed to spend moves ending turns without really doing anything in the game
    /// </summary>
    public static bool FORCE_DELAY_TURN_END_IN_ROLLOUT { get; set; }
    public static bool INCLUDE_PLAY_MOVE_CHANCE_NODES { get; set; }
    public static bool INCLUDE_END_TURN_CHANCE_NODES { get; set; }
    public static HashGenerationType CHOSEN_HASH_GENERATION_TYPE { get; set; }

    static MCTSHyperparameters()
    {
        Settings.LoadEnvFile("environment");
        var config = Settings.GetConfiguration();

        ITERATIONS = config.GetRequiredSection("ITERATIONS").Get<int>();
        NUMBER_OF_ROLLOUTS = config.GetRequiredSection("NUMBER_OF_ROLLOUTS").Get<int>();

        UCB1_EXPLORATION_CONSTANT = config.GetRequiredSection("UCB1_EXPLORATION_CONSTANT").Get<float>();
        FORCE_DELAY_TURN_END_IN_ROLLOUT = config.GetRequiredSection("FORCE_DELAY_TURN_END_IN_ROLLOUT").Get<bool>();
        INCLUDE_PLAY_MOVE_CHANCE_NODES = config.GetRequiredSection("INCLUDE_PLAY_MOVE_CHANCE_NODES").Get<bool>();
        INCLUDE_END_TURN_CHANCE_NODES = config.GetRequiredSection("INCLUDE_END_TURN_CHANCE_NODES").Get<bool>();

        var chosen_evaluation_function = config.GetRequiredSection("CHOSEN_EVALUATION_FUNCTION").Get<string>();
        CHOSEN_EVALUATION_FUNCTION = EnumHelper.ToEvaluationFnction(chosen_evaluation_function);

        var chosen_hash_generation_type = config.GetRequiredSection("CHOSEN_HASH_GENERATION_TYPE").Get<string>();
        CHOSEN_HASH_GENERATION_TYPE = EnumHelper.ToHashGenerationType(chosen_hash_generation_type);

        Console.WriteLine("Loaded settings:");
        Console.WriteLine($"ITERATIONS: {ITERATIONS}");
        Console.WriteLine($"NUMBER_OF_ROLLOUTS: {NUMBER_OF_ROLLOUTS}");
        Console.WriteLine($"UCB1_EXPLORATION_CONSTANT: {UCB1_EXPLORATION_CONSTANT}");
        Console.WriteLine($"FORCE_DELAY_TURN_END_IN_ROLLOUT: {FORCE_DELAY_TURN_END_IN_ROLLOUT}");
        Console.WriteLine($"INCLUDE_PLAY_MOVE_CHANCE_NODES: {INCLUDE_PLAY_MOVE_CHANCE_NODES}");
        Console.WriteLine($"INCLUDE_END_TURN_CHANCE_NODES: {INCLUDE_END_TURN_CHANCE_NODES}");
        Console.WriteLine($"CHOSEN_EVALUATION_FUNCTION: {CHOSEN_EVALUATION_FUNCTION}");
        Console.WriteLine($"CHOSEN_HASH_GENERATION_TYPE: {CHOSEN_HASH_GENERATION_TYPE}");
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
