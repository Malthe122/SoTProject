using ScriptsOfTribute;
using ScriptsOfTribute.Serializers;

public class Node{
        /// <summary>
        /// Has to be stored like a seeded game state although its a bit non-intuitive, but this is the object type that applyMove method returns.
        /// However we never want to actually reuse the seed inside the object, so when we call apply move on the seeded state, we need to call it
        /// with a new random seed which is a possible argument for the applyMove method
        /// </summary>
        public SeededGameState GameState;
        public List<Node> ChildNodes;
        public int GameStateHash;
        public double TotalScore;
        public int VisitCount;
        public Move AppliedMove;
        public List<Move> AvailableMoves;
        

        public Node(SeededGameState gameState, Move appliedMove, List<Move> availableMoves){
            GameState = gameState;
            TotalScore = 0;
            VisitCount = 0;
            GenerateGameStateHash();
            AppliedMove = appliedMove;
            AvailableMoves = availableMoves;
            ChildNodes = new List<Node>();
        }

        private void GenerateGameStateHash()
        {
            // TODO We properly need to optimize this for performance
            GameStateHash = GameState.GenerateHash();
        }

        public virtual Node Simulate(out double score){
            score = 0; //TODO update this in simulation


            if (VisitCount == 0){
                ApplyAllDeterministicAndObviousMoves(); //TODO maybe move this call to constructor
                score = Rollout();
            }
            else if(AvailableMoves.Count > AvailableMoves.Count()){
                Move moveToExplore = null;
                foreach(Move currMove in AvailableMoves){
                    if(!ChildNodes.Any(n => n.AppliedMove == currMove)) {
                        moveToExplore = currMove;
                        break;
                    }
                }
                // TODO do not generate a new random here. We should use a global one
                (var newState, var newMoves) = GameState.ApplyMove(moveToExplore!, (ulong)(new Random().Next()));
                // TODO check here if it is a chanceNode and in that case create one of those instead
                var newChild = new Node(newState, moveToExplore, newMoves); //TODO this is where i reached. Might need to change all our gamestates to seeded game states
                newChild.Simulate(out score);
            }
            else{
                var nodeToSimulate = GetHighestConfidenceChild();
                nodeToSimulate.Simulate(out score);
            }

            TotalScore += score;
            VisitCount++;
            return null; // TODO simulate and return resulting node
        }

    private double Rollout()
    {
        // TODO implement
        throw new NotImplementedException();
    }

    private Node GetHighestConfidenceChild()
        {
            double maxConfidence = -double.MaxValue;
            Node highestConfidenceChild = ChildNodes[0];

            foreach(Node currChild in ChildNodes){
                double confidence = currChild.GetConfidenceScore();
                if (confidence > maxConfidence) {
                    maxConfidence = confidence;
                    highestConfidenceChild = currChild;
                }
            }

            return highestConfidenceChild;
        }

        public double GetConfidenceScore()
        {
            //TODO use an algorithm like UCB1 or UCT to decide this using visitcount and totalscore;¨
            return TotalScore - VisitCount;
        }

        private void ApplyAllDeterministicAndObviousMoves()
        {
            foreach(Move currMove in AvailableMoves) {
                if(currMove.Command == CommandEnum.PLAY_CARD) {
                    if (OBVIOUS_ACTION_PLAYS.Contains(((SimpleCardMove)currMove).Card.CommonId)){
                        // TODO do not generate a new random here. We should use a global one
                        // TODO maybe we need to substitute our gamestate for seeded game state
                        // TODO consider if some of the choice cards are also obvious moves, since the choice will be a new move
                        // or how to handle this issue
                        (GameState.ToSeededGameState(), AvailableMoves) = GameState.ApplyMove(currMove,  (ulong)(new Random().Next()));
                        ApplyAllDeterministicAndObviousMoves();
                        break;
                    }
                }
                else if(currMove.Command == CommandEnum.ACTIVATE_AGENT) {
                    if (OBVIOUS_AGENT_EFFECTS.Contains(((SimpleCardMove)currMove).Card.CommonId){
                        (GameState, AvailableMoves) = GameState.ApplyMove(currMove,  (ulong)(new Random().Next()));
                        ApplyAllDeterministicAndObviousMoves();
                        break;
                    }
                }
            }
        }
    }