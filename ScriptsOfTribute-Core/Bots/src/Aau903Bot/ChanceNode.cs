using ScriptsOfTribute;
using ScriptsOfTribute.Serializers;

public class ChanceNode : Node
{
    public Move AppliedMove;
    private HashSet<Node> knownPossibleOutcomes;
    public ChanceNode(SeededGameState gameState, Node parent, Move appliedMove, int depth ) : base(gameState, parent, new List<Move>(), depth) { 
        AppliedMove = appliedMove;
        knownPossibleOutcomes = new HashSet<Node>();
    }

    public override void Visit(out double score)
    {      
        (var newState, var newMoves) = Parent.GameState.ApplyMove(AppliedMove, (ulong)Utility.Rng.Next());

        var child = Utility.FindOrBuildNode(newState, this, newMoves, Depth);

        knownPossibleOutcomes.Add(child);

        if (MCTSHyperparameters.EQUAL_CHANCE_NODE_DISTRIBUTION) {
            var existingChild = knownPossibleOutcomes.SingleOrDefault(existingChild => existingChild.GameStateHash == child.GameStateHash);
            var leastVisitedChild = existingChild;
            var lowestVisitCount = int.MaxValue;
            foreach (var currChild in knownPossibleOutcomes)
            {
                if (currChild.VisitCount < lowestVisitCount)
                {
                    leastVisitedChild = currChild;
                    lowestVisitCount = currChild.VisitCount;
                }
            }
            
            leastVisitedChild.Visit(out score);
        }
        else {
            child.Visit(out score);
        }
    
        TotalScore += score;
        VisitCount++;
    }
}
