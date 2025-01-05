namespace Aau903Bot;

public enum ScoringMethod
{
    Rollout,
    Heuristic, // Note this one is not optimal with the heuristic function provided by the previous winners, that we use now, as they only use it at the end of turn, so it wont reward stuff like extra coins
    RolloutTurnsCompletionsThenHeuristic,
}