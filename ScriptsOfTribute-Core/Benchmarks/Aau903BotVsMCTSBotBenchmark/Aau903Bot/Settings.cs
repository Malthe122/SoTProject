using Microsoft.Extensions.Configuration;

namespace Aau903Bot;

public static class MCTSHyperparameters
{
    // Here we split up, which parameters will only be changed for benchmarking purposes to proof their viability. These will have the default value
    // provided at initialisation underneath, and should only be modified when this specific parameter is being benchmarked for showcasing. 
    // Underneath we have the parameters than are eligable for being modified by a hyper parameter optimization algorithm

    // Fixed parameters:
        public static bool DYNAMIC_MOVE_TIME_DISTRIBUTION { get; set; } = true; // if this is false, the fixed amount of.
        public static int ITERATIONS { get; set; } = 50; // This is only used when DYNAMIC_MOVE_TIME_DISTRIBUTION is set to false. Needs to be set to a number where it wont timeout

    // Parameters for optmization:
        /// <summary>
        /// This variable is an upper threshold, telling how much time we need to have left for our move and still complete an iteration. This is to avoid time checking, saying that we did not exceed
        /// time usage, but then we exceed it during an iteration. If complete rollouts are completely or partly replaced by Heuristic scoring, this can be lower while with full rollouts it should be big
        /// </summary>
        public static double ITERATION_COMPLETION_MILLISECONDS_BUFFER { get; set; }
        public static EvaluationFunction CHOSEN_EVALUATION_FUNCTION { get; set; }
        /// <summary>
        /// Default value is 1.41421356237, which is the square root of 2
        /// </summary>
        public static double UCB1_EXPLORATION_CONSTANT { get; set; }
        public static int NUMBER_OF_ROLLOUTS { get; set; }
        /// <summary>
        /// Tells in the rollout whether the agents plays all their possible moves before ending turn or if end turn is an allowed move in any part of their turn
        /// Idea is that setting this to true will first of all be closer to a realistic simulation and also it should end the game quicker, making the simulation
        /// faster than if agents were allowed to spend moves ending turns without really doing anything in the game
        /// </summary>
        public static bool FORCE_DELAY_TURN_END_IN_ROLLOUT { get; set; }
        public static bool INCLUDE_PLAY_MOVE_CHANCE_NODES { get; set; }
        public static bool INCLUDE_END_TURN_CHANCE_NODES { get; set; }
        public static HashGenerationType CHOSEN_HASH_GENERATION_TYPE { get; set; }
        public static bool SET_MAX_EXPANSION_DEPTH { get; set; }
        public static int CHOSEN_MAX_EXPANSION_DEPTH { get; set; }
        public static ScoringMethod CHOSEN_SCORING_METHOD { get; set; }
        public static int ROLLOUT_TURNS_BEFORE_HEURSISTIC { get; set; }
        public static bool SHARED_MCTS_TREE { get; set; }

    static MCTSHyperparameters()
    {
        Settings.LoadEnvFile("environment");
        var config = Settings.GetConfiguration();

        DYNAMIC_MOVE_TIME_DISTRIBUTION = config.GetRequiredSection("DYNAMIC_MOVE_TIME_DISTRIBUTION").Get<bool>();
        ITERATION_COMPLETION_MILLISECONDS_BUFFER = config.GetRequiredSection("ITERATION_COMPLETION_MILLISECONDS_BUFFER").Get<double>();
        ITERATIONS = config.GetRequiredSection("ITERATIONS").Get<int>();
        NUMBER_OF_ROLLOUTS = config.GetRequiredSection("NUMBER_OF_ROLLOUTS").Get<int>();
        UCB1_EXPLORATION_CONSTANT = config.GetRequiredSection("UCB1_EXPLORATION_CONSTANT").Get<float>();
        FORCE_DELAY_TURN_END_IN_ROLLOUT = config.GetRequiredSection("FORCE_DELAY_TURN_END_IN_ROLLOUT").Get<bool>();
        INCLUDE_PLAY_MOVE_CHANCE_NODES = config.GetRequiredSection("INCLUDE_PLAY_MOVE_CHANCE_NODES").Get<bool>();
        INCLUDE_END_TURN_CHANCE_NODES = config.GetRequiredSection("INCLUDE_END_TURN_CHANCE_NODES").Get<bool>();

        var chosen_evaluation_function = config.GetRequiredSection("CHOSEN_EVALUATION_FUNCTION").Get<string>();
        CHOSEN_EVALUATION_FUNCTION = Enum.Parse<EvaluationFunction>(chosen_evaluation_function);

        var chosen_hash_generation_type = config.GetRequiredSection("CHOSEN_HASH_GENERATION_TYPE").Get<string>();
        CHOSEN_HASH_GENERATION_TYPE = Enum.Parse<HashGenerationType>(chosen_hash_generation_type);

        var chosenScoringMethodString = config.GetRequiredSection("CHOSEN_SCORING_METHOD").Get<string>();
        CHOSEN_SCORING_METHOD = Enum.Parse<ScoringMethod>(chosenScoringMethodString);

        ROLLOUT_TURNS_BEFORE_HEURSISTIC = config.GetRequiredSection("ROLLOUT_TURNS_BEFORE_HEURSISTIC").Get<int>();
        SET_MAX_EXPANSION_DEPTH = config.GetRequiredSection("SET_MAX_EXPANSION_DEPTH").Get<bool>();
        CHOSEN_MAX_EXPANSION_DEPTH = config.GetRequiredSection("CHOSEN_MAX_EXPANSION_DEPTH").Get<int>();
        SHARED_MCTS_TREE = config.GetRequiredSection("SHARED_MCTS_TREE").Get<bool>();

        Console.WriteLine("Loaded settings:");
        Console.WriteLine($"NUMBER_OF_ROLLOUTS: {NUMBER_OF_ROLLOUTS}");
        Console.WriteLine($"UCB1_EXPLORATION_CONSTANT: {UCB1_EXPLORATION_CONSTANT}");
        Console.WriteLine($"FORCE_DELAY_TURN_END_IN_ROLLOUT: {FORCE_DELAY_TURN_END_IN_ROLLOUT}");
        Console.WriteLine($"INCLUDE_PLAY_MOVE_CHANCE_NODES: {INCLUDE_PLAY_MOVE_CHANCE_NODES}");
        Console.WriteLine($"INCLUDE_END_TURN_CHANCE_NODES: {INCLUDE_END_TURN_CHANCE_NODES}");
        Console.WriteLine($"CHOSEN_EVALUATION_FUNCTION: {CHOSEN_EVALUATION_FUNCTION}");
        Console.WriteLine($"CHOSEN_HASH_GENERATION_TYPE: {CHOSEN_HASH_GENERATION_TYPE}");
        Console.WriteLine($"DYNAMIC_MOVE_TIME_DISTRIBUTION: {DYNAMIC_MOVE_TIME_DISTRIBUTION}");
        Console.WriteLine($"ITERATIONS: {ITERATIONS}");
        Console.WriteLine($"ITERATION_COMPLETION_MILLISECONDS_BUFFER: {ITERATION_COMPLETION_MILLISECONDS_BUFFER}");
        Console.WriteLine($"CHOSEN_SCORING_METHOD: {CHOSEN_SCORING_METHOD}");
        Console.WriteLine($"ROLLOUT_TURNS_BEFORE_HEURSISTIC: {ROLLOUT_TURNS_BEFORE_HEURSISTIC}");
        Console.WriteLine($"SET_MAX_EXPANSION_DEPTH: {SET_MAX_EXPANSION_DEPTH}");
        Console.WriteLine($"CHOSEN_EXPANSION_DEPTH: {CHOSEN_MAX_EXPANSION_DEPTH}");
        Console.WriteLine($"SHARED_MCTS_TREE: {SHARED_MCTS_TREE}");
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
