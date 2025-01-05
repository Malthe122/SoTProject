using ScriptsOfTribute;
using ScriptsOfTribute.AI;
using ScriptsOfTribute.Board;
using ScriptsOfTribute.Serializers;

namespace RandomBot;

public static class Extensions
{
    public static int RandomK(int lowerBound, int upperBoaund, SeededRandom rng)
    {
        return (rng.Next() % (upperBoaund - lowerBound)) + lowerBound;
    }
    public static T PickRandom<T>(this List<T> source, SeededRandom rng)
    {
        return source[rng.Next() % source.Count];
    }
}

public class RandomBot : AI
{
    private readonly SeededRandom rng = new(123);

    public override PatronId SelectPatron(List<PatronId> availablePatrons, int round)
        => availablePatrons.PickRandom(rng);

    public override Move Play(GameState gameState, List<Move> possibleMoves, TimeSpan remainingTime)
    {
        return possibleMoves.PickRandom(rng);
    }

    public override void GameEnd(EndGameState state, FullGameState? finalBoardState)
    {
    }
}
