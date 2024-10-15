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
            GameStateHash = GameState.GenerateHash();
            AppliedMove = appliedMove;
            AvailableMoves = availableMoves;
            ChildNodes = new List<Node>();
        }

        private void GenerateGameStateHash()
        {
            GameStateHash = GameState.GenerateHash();
        }

        public virtual Node? Simulate(out double score){
            Node visitedChild = null;


            if (VisitCount == 0){
                ApplyAllDeterministicAndObviousMoves(); //TODO maybe move this call to constructor
                score = Rollout();
            }
            else if(AvailableMoves.Count > AvailableMoves.Count()){
                Move moveToExplore = null;
                foreach(Move currMove in AvailableMoves){
                    if(!ChildNodes.Any(n => n.AppliedMove == currMove)) {
                        //TODO insert here checks that if a move can lead to several different stages, we need to create a chance node
                        moveToExplore = currMove;
                        break;
                    }
                }
                (var newState, var newMoves) = GameState.ApplyMove(moveToExplore!, (ulong)Utility.Rng.Next());
                // TODO check here if it is a chanceNode and in that case create one of those instead
                visitedChild = new Node(newState, moveToExplore, newMoves);
                visitedChild.Simulate(out score);
            }
            else{
                visitedChild = GetHighestConfidenceChild();
                visitedChild.Simulate(out score);
            }

            TotalScore += score;
            VisitCount++;
            return visitedChild;
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
            switch(Utility.CHOSEN_EVALUATION_FUNCTION){ //TODO i do it like this, so we can edit this and try to benchmark them against each other
                case EvaluationFunction.UCB1:
                    // TODO implement
                    return 0;
                case EvaluationFunction.UCT:
                    // TODO implement
                    return 0;
                case EvaluationFunction.Custom:
                    //TODO replace this with something meaningful or remove it
                    return TotalScore - VisitCount;
            }            
        }

        private void ApplyAllDeterministicAndObviousMoves()
        {
            foreach(Move currMove in AvailableMoves) {
                if(currMove.Command == CommandEnum.PLAY_CARD) {
                    if (Utility.OBVIOUS_ACTION_PLAYS.Contains(((SimpleCardMove)currMove).Card.CommonId)){
                        // TODO do not generate a new random here. We should use a global one
                        // TODO maybe we need to substitute our gamestate for seeded game state
                        // TODO consider if some of the choice cards are also obvious moves, since the choice will be a new move
                        // or how to handle this issue
                        (GameState, AvailableMoves) = GameState.ApplyMove(currMove,  (ulong)(new Random().Next()));
                        ApplyAllDeterministicAndObviousMoves();
                        break;
                    }
                }
                else if(currMove.Command == CommandEnum.ACTIVATE_AGENT) {
                    if (Utility.OBVIOUS_AGENT_EFFECTS.Contains(((SimpleCardMove)currMove).Card.CommonId){
                        (GameState, AvailableMoves) = GameState.ApplyMove(currMove,  (ulong)(new Random().Next()));
                        ApplyAllDeterministicAndObviousMoves();
                        break;
                    }
                }
            }
        }
    }