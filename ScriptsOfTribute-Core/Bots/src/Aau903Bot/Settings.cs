using Microsoft.Extensions.Configuration;

namespace Aau903Bot;

public class MCTSHyperparameters
{
    /// <summary>
    /// This variable is an upper threshold, telling how much time we need to have left for our move and still complete an iteration. This is to avoid time checking, saying that we did not exceed
    /// time usage, but then we exceed it during an iteration. If complete rollouts are completely or partly replaced by Heuristic scoring, this can be lower while with full rollouts it should be big
    /// </summary>
    public int ITERATIONS { get; set; }
    public double ITERATION_COMPLETION_MILLISECONDS_BUFFER { get; set; }
    public double UCT_EXPLORATION_CONSTANT { get; set; } // sqrt 2
    /// <summary>
    /// Tells in the rollout whether the agents plays all their possible moves before ending turn or if end turn is an allowed move in any part of their turn
    /// Idea is that setting this to true will first of all be closer to a realistic simulation and also it should end the game quicker, making the simulation
    /// faster than if agents were allowed to spend moves ending turns without really doing anything in the game
    /// </summary>
    public int NUMBER_OF_ROLLOUTS { get; set; }
    public bool FORCE_DELAY_TURN_END_IN_ROLLOUT { get; set; }
    public bool INCLUDE_PLAY_MOVE_CHANCE_NODES { get; set; }
    public bool INCLUDE_END_TURN_CHANCE_NODES { get; set; }
    public EvaluationMethod CHOSEN_EVALUATION_METHOD { get; set; }
    public ScoringMethod CHOSEN_SCORING_METHOD { get; set; }
    public int ROLLOUT_TURNS_BEFORE_HEURSISTIC { get; set; }
    public bool EQUAL_CHANCE_NODE_DISTRIBUTION { get; set; }
    public bool REUSE_TREE { get; set; }


    public MCTSHyperparameters(string filePath = "environment")
    {
        var envVariables = Settings.LoadEnvFile(filePath);

        var builder = new ConfigurationBuilder()
            .AddInMemoryCollection(envVariables);
        var config = builder.Build();

        ITERATIONS = config.GetValue("ITERATIONS", 0);
        ITERATION_COMPLETION_MILLISECONDS_BUFFER = config.GetValue("ITERATION_COMPLETION_MILLISECONDS_BUFFER", 100.0);
        UCT_EXPLORATION_CONSTANT = config.GetValue("UCT_EXPLORATION_CONSTANT", 1.41421356237);
        NUMBER_OF_ROLLOUTS = config.GetValue("NUMBER_OF_ROLLOUTS", 1);
        FORCE_DELAY_TURN_END_IN_ROLLOUT = config.GetValue("FORCE_DELAY_TURN_END_IN_ROLLOUT", true);
        INCLUDE_PLAY_MOVE_CHANCE_NODES = config.GetValue("INCLUDE_PLAY_MOVE_CHANCE_NODES", false);
        INCLUDE_END_TURN_CHANCE_NODES = config.GetValue("INCLUDE_END_TURN_CHANCE_NODES", false);
        CHOSEN_EVALUATION_METHOD = Enum.Parse<EvaluationMethod>(config.GetValue("CHOSEN_EVALUATION_METHOD", "UCT")!);
        CHOSEN_SCORING_METHOD = Enum.Parse<ScoringMethod>(config.GetValue("CHOSEN_SCORING_METHOD", "Rollout")!);
        ROLLOUT_TURNS_BEFORE_HEURSISTIC = config.GetValue("ROLLOUT_TURNS_BEFORE_HEURSISTIC", 3);
        EQUAL_CHANCE_NODE_DISTRIBUTION = config.GetValue("EQUAL_CHANCE_NODE_DISTRIBUTION", true);
        REUSE_TREE = config.GetValue("REUSE_TREE", true);

        Console.WriteLine("Loaded settings:");
        Console.WriteLine($"ITERATIONS={ITERATIONS}");
        Console.WriteLine($"ITERATION_COMPLETION_MILLISECONDS_BUFFER={ITERATION_COMPLETION_MILLISECONDS_BUFFER}");
        Console.WriteLine($"UCT_EXPLORATION_CONSTANT={UCT_EXPLORATION_CONSTANT}");
        Console.WriteLine($"NUMBER_OF_ROLLOUTS={NUMBER_OF_ROLLOUTS}");
        Console.WriteLine($"FORCE_DELAY_TURN_END_IN_ROLLOUT={FORCE_DELAY_TURN_END_IN_ROLLOUT}");
        Console.WriteLine($"INCLUDE_PLAY_MOVE_CHANCE_NODES={INCLUDE_PLAY_MOVE_CHANCE_NODES}");
        Console.WriteLine($"INCLUDE_END_TURN_CHANCE_NODES={INCLUDE_END_TURN_CHANCE_NODES}");
        Console.WriteLine($"CHOSEN_EVALUATION_METHOD={CHOSEN_EVALUATION_METHOD}");
        Console.WriteLine($"CHOSEN_SCORING_METHOD={CHOSEN_SCORING_METHOD}");
        Console.WriteLine($"ROLLOUT_TURNS_BEFORE_HEURSISTIC={ROLLOUT_TURNS_BEFORE_HEURSISTIC}");
        Console.WriteLine($"EQUAL_CHANCE_NODE_DISTRIBUTION={EQUAL_CHANCE_NODE_DISTRIBUTION}");
        Console.WriteLine($"REUSE_TREE={REUSE_TREE}");
    }
}

public class Settings
{
    public static Dictionary<string, string> LoadEnvFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.Error.WriteLine($"Env file '{filePath}' not found");
            return new Dictionary<string, string>();
        }

        var envVariables = new Dictionary<string, string>();

        foreach (var line in File.ReadAllLines(filePath))
        {
            if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
                continue;

            var parts = line.Split('=', 2);
            if (parts.Length != 2)
                continue;

            var key = parts[0].Trim();
            var value = parts[1].Trim().Trim('"');

            envVariables[key] = value;
        }

        return envVariables;
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
