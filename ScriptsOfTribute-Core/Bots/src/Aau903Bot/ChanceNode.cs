using ScriptsOfTribute;
using ScriptsOfTribute.Serializers;

namespace Aau903Bot;

public class ChanceNode : Node
{
    public ChanceNode(SeededGameState gameState, Node parent, Move appliedMove) : base(gameState, parent, null, appliedMove) { }

    public override void Visit(out double score)
    {

        (var newState, var newMoves) = Parent.GameState.ApplyMove(AppliedMove, (ulong)Utility.Rng.Next());
        var newStateHash = newState.GenerateHash();
        if (ChildNodes.Any(n => n.GameStateHash == newStateHash))
        {
            // TODO consider if we should visit the child we hit instead of equal distribution. With equal distribution
            // we assume that each outcome has the same chance
            int lowestVisitCount = int.MaxValue;
            var leastVisitedChild = ChildNodes[0];

            foreach (var currChild in ChildNodes)
            {
                if (currChild.VisitCount < lowestVisitCount)
                {
                    leastVisitedChild = currChild;
                    lowestVisitCount = currChild.VisitCount;
                }
            }

            leastVisitedChild.Visit(out score);
        }
        else
        {
            var newChild = new Node(newState, this, newMoves, AppliedMove);
            ChildNodes.Add(newChild);
            newChild.Visit(out score);
        }

        TotalScore += score;
        VisitCount++;
    }
}
