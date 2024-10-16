public static class MCTSSettings {
    public const EvaluationFunction CHOSEN_EVALUATION_FUNCTION = EvaluationFunction.UCB1;
    /// <summary>
    /// Default value is 1.41421356237, which is the square root of 2
    /// </summary>
    public const double UCB1_EXPLORATION_CONSTANT = 1.41421356237;
    public const int NUMBER_OF_ROLLOUTS = 10; //I set it low for initial tests
    public const int ITERATIONS = 20; //I set it low for initial tests
    /// <summary>
    /// Tells in the rollout whether the agents plays all their possible moves before ending turn or if end turn is an allowed move in any part of their turn
    /// Idea is that setting this to true will first of all be closer to a realistic simulation and also it should end the game quicker, making the simulation
    /// faster than if agents were allowed to spend moves ending turns without really doing anything in the game
    /// </summary>
    public const bool FORCE_DELAY_TURN_END_IN_ROLLOUT = true;
}