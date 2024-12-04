using System.Xml.Schema;
using ScriptsOfTribute;
using ScriptsOfTribute.Serializers;

namespace Aau903Bot;

public class ChanceNode : Node
{
    public Move AppliedMove;
    private Dictionary<int, List<Node>> knownPossibleOutcomes;
    public ChanceNode(SeededGameState gameState, Node parent, Move appliedMove, MCTSHyperparameters parameters) : base(gameState, parent, new List<Move>(), parameters)
    {
        AppliedMove = appliedMove;
        knownPossibleOutcomes = new Dictionary<int, List<Node>>();
    }

    public override void Visit(out double score)
    {   
        (var newState, var newMoves) = Parent.GameState.ApplyMove(AppliedMove, (ulong)Utility.Rng.Next());

        var child = Utility.FindOrBuildNode(newState, this, newMoves, Params);

        if (Params.EQUAL_CHANCE_NODE_DISTRIBUTION)
        {
            Node? existingChild = null;
            if (knownPossibleOutcomes.Keys.Contains(child.GameStateHash)){
                existingChild = knownPossibleOutcomes[child.GameStateHash].SingleOrDefault(node => node.GameState.IsIdentical(child.GameState));
                if (existingChild == null){
                    knownPossibleOutcomes[child.GameStateHash].Add(child);
                }
            }
            else {
                knownPossibleOutcomes.Add(child.GameStateHash, new List<Node>(){child});
            }
            if (existingChild != null) {

                var leastVisitedChild = existingChild;
                var lowestVisitCount = int.MaxValue;
                foreach (var currChild in knownPossibleOutcomes.Values.SelectMany(list => list).ToList())
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
                child.Visit(out score);
            }
        }
        else
        {
            child.Visit(out score);
        }

        TotalScore += score;
        VisitCount++;
    }
}
