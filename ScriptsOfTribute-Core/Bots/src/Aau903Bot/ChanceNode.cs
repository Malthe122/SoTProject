using ScriptsOfTribute;
using ScriptsOfTribute.Serializers;

public class ChanceNode : Node
    {
        private Random rng = new Random();
        public List<Node> NodeVersions;
        public ChanceNode(SeededGameState gameState, Move appliedMove, List<Move> availableMoves, Node parentNode) : base(gameState, appliedMove, availableMoves, parentNode)
        {
            NodeVersions = new List<Node>();
        }

        override public Node Visit(out double score){
            var chosenNode = NodeVersions.ElementAt(rng.Next(NodeVersions.Count));
            var resultingNode = chosenNode.Visit(out double chosenNodeScore);

            score = chosenNodeScore;
            TotalScore += score;
            VisitCount++;

            var equalGameStateNode = NodeVersions.FirstOrDefault(s => s.GameStateHash == resultingNode.GameStateHash);
            if (equalGameStateNode != null){
                equalGameStateNode.VisitCount++;
                equalGameStateNode.TotalScore += chosenNodeScore;
                return equalGameStateNode;
            }
            else {
                NodeVersions.Add(resultingNode);
                return resultingNode;
            }
        }
    }