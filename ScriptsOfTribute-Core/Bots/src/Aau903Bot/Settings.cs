using Microsoft.Extensions.Configuration;

namespace Aau903Bot;

public class MCTSHyperparameters
{
    /// <summary>
    /// This variable is an upper threshold, telling how much time we need to have left for our move and still complete an iteration. This is to avoid time checking, saying that we did not exceed
    /// time usage, but then we exceed it during an iteration. If complete rollouts are completely or partly replaced by Heuristic scoring, this can be lower while with full rollouts it should be big
    /// </summary>
    public double ITERATION_COMPLETION_MILLISECONDS_BUFFER { get; set; }
    public double UCT_EXPLORATION_CONSTANT { get; set; } // sqrt 2
    /// <summary>
    /// Tells in the rollout whether the agents plays all their possible moves before ending turn or if end turn is an allowed move in any part of their turn
    /// Idea is that setting this to true will first of all be closer to a realistic simulation and also it should end the game quicker, making the simulation
    /// faster than if agents were allowed to spend moves ending turns without really doing anything in the game
    /// </summary>
    public bool FORCE_DELAY_TURN_END_IN_ROLLOUT { get; set; }
    public bool INCLUDE_PLAY_MOVE_CHANCE_NODES { get; set; }
    public bool INCLUDE_END_TURN_CHANCE_NODES { get; set; }
    public int CHOSEN_MAX_EXPANSION_DEPTH { get; set; }
    public ScoringMethod CHOSEN_SCORING_METHOD { get; set; }
    public int ROLLOUT_TURNS_BEFORE_HEURSISTIC { get; set; }

    public MCTSHyperparameters(string filePath = "environment")
    {
        Settings.LoadEnvFile(filePath);
        var config = Settings.GetConfiguration();

        ITERATION_COMPLETION_MILLISECONDS_BUFFER = config.GetValue("ITERATION_COMPLETION_MILLISECONDS_BUFFER", 100.0);
        UCT_EXPLORATION_CONSTANT = config.GetValue("UCT_EXPLORATION_CONSTANT", 1.41421356237);
        FORCE_DELAY_TURN_END_IN_ROLLOUT = config.GetValue("FORCE_DELAY_TURN_END_IN_ROLLOUT", true);
        INCLUDE_PLAY_MOVE_CHANCE_NODES = config.GetValue("INCLUDE_PLAY_MOVE_CHANCE_NODES", false);
        INCLUDE_END_TURN_CHANCE_NODES = config.GetValue("INCLUDE_END_TURN_CHANCE_NODES", false);
        CHOSEN_MAX_EXPANSION_DEPTH = config.GetValue("CHOSEN_MAX_EXPANSION_DEPTH", 99999);
        var chosenScoringMethodString = config.GetValue("CHOSEN_SCORING_METHOD", "Rollout");
        CHOSEN_SCORING_METHOD = Enum.Parse<ScoringMethod>(chosenScoringMethodString!);
        ROLLOUT_TURNS_BEFORE_HEURSISTIC = config.GetValue("ROLLOUT_TURNS_BEFORE_HEURSISTIC", 3);

        Console.WriteLine($"ITERATION_COMPLETION_MILLISECONDS_BUFFER: {ITERATION_COMPLETION_MILLISECONDS_BUFFER}");
        Console.WriteLine($"UCT_EXPLORATION_CONSTANT: {UCT_EXPLORATION_CONSTANT}");
        Console.WriteLine($"FORCE_DELAY_TURN_END_IN_ROLLOUT: {FORCE_DELAY_TURN_END_IN_ROLLOUT}");
        Console.WriteLine($"INCLUDE_PLAY_MOVE_CHANCE_NODES: {INCLUDE_PLAY_MOVE_CHANCE_NODES}");
        Console.WriteLine($"INCLUDE_END_TURN_CHANCE_NODES: {INCLUDE_END_TURN_CHANCE_NODES}");
        Console.WriteLine($"CHOSEN_MAX_EXPANSION_DEPTH: {CHOSEN_MAX_EXPANSION_DEPTH}");
        Console.WriteLine($"CHOSEN_SCORING_METHOD: {CHOSEN_SCORING_METHOD}");
        Console.WriteLine($"ROLLOUT_TURNS_BEFORE_HEURSISTIC: {ROLLOUT_TURNS_BEFORE_HEURSISTIC}");
        Console.WriteLine("---");
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

    public static void SaveEnvFile(string filePath, Dictionary<string, string> values)
    {
        // Create or overwrite the environment file
        using (var writer = new StreamWriter(filePath, false))
        {
            foreach (var kvp in values)
            {
                string key = kvp.Key.Trim();
                string value = kvp.Value.Trim();

                writer.WriteLine($"{key}={value}");
            }
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
