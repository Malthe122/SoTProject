using ScriptsOfTribute;
using ScriptsOfTribute.Serializers;

public class ChanceNode : Node
{
    public ChanceNode(SeededGameState gameState, Node parent, Move appliedMove, int depth ) : base(gameState, parent, new List<Move>(), appliedMove, depth) { }

    public override void Visit(out double score)
    {      
        (var newState, var newMoves) = Parent.GameState.ApplyMove(AppliedMove, (ulong)Utility.Rng.Next());
        var newStateHash = newState.GenerateHash();
        // TODO add flag
        // var childWithSameHash = Utility.FindOrBuildNode(newState, this, newMoves, AppliedMove, Depth);
        // TODO wip with the chance node. I will check if it works in default first

        // since a new state will be generated each time here. We dont do the complete comparison every time because of time restriction. In case there is a false
        // hash-collision, the only problem is that we explore a wrong outcome of the random effect. This wont cause us to have a wrong rootnode in our tree and
        // therefor wont make us play an illegal move. Right now, we do full comparison for testing purposes

        var equalNode = ChildNodes.FirstOrDefault(n => n.GameStateHash == newStateHash);
        
        if (equalNode != null && equalNode.GameState.IsIdentical(newState, "Called from Chance Node. Equal Node: " + equalNode + ". With Hash: " + equalNode.GameStateHash + ". New state hash: " + newStateHash + ". Generated: " + newState.GenerateHash()))
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
            // TODO add check
            // Console.WriteLine("FOUND NO EQUAL!!");
            var newChild = Utility.FindOrBuildNode(newState, this, newMoves, AppliedMove, Depth);
            // else:
            // var newChild = new Node(newState, this, newMoves, AppliedMove, Depth);
            ChildNodes.Add(newChild);
            newChild.Visit(out score);
        }

        TotalScore += score;
        VisitCount++;
    }
}
