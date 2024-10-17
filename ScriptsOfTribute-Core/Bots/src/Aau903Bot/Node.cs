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
        public Node Parent;
        

        public Node(SeededGameState gameState, Move appliedMove, List<Move> availableMoves, Node parent){
            GameState = gameState;
            TotalScore = 0;
            VisitCount = 0;
            GameStateHash = GameState.GenerateHash();
            AppliedMove = appliedMove;
            AvailableMoves = availableMoves;
            ChildNodes = new List<Node>();
            Parent = parent;
        }

        public virtual Node? Visit(out double score){
            Node visitedChild = null;

            if (GameState.GameEndState == null){
                if (VisitCount == 0){
                    ApplyAllDeterministicAndObviousMoves(); //TODO maybe move this call to constructor
                    score = Rollout();
                }
                else if(AvailableMoves.Count > ChildNodes.Count){
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
                    visitedChild = new Node(newState, moveToExplore, newMoves, this);
                    ChildNodes.Add(visitedChild);
                    visitedChild.Visit(out score);
                }
                else{
                    visitedChild = GetHighestConfidenceChild();
                    visitedChild.Visit(out score);
                    if(visitedChild.GameState.CurrentPlayer.PlayerID != GameState.CurrentPlayer.PlayerID) {
                        score *= -1;
                    }
                }
            }
            else if(GameState.GameEndState.Winner == PlayerEnum.NO_PLAYER_SELECTED){
                score = 0;
            }
            else if(GameState.GameEndState.Winner == GameState.CurrentPlayer.PlayerID) {
                score = 1;
            }
            else{
                score = -1;
            }

            TotalScore += score;
            VisitCount++;
            return visitedChild;
        }

    private double Rollout()
    {
        double result = 0;
        for(int i = 0; i <= MCTSSettings.NUMBER_OF_ROLLOUTS; i++){
            SeededGameState rollOutGameState = GameState;
            List<Move> rolloutAvailableMoves = new List<Move>(AvailableMoves);
            while(rollOutGameState.GameEndState == null) {
                // TODO also apply the playing obvious moves in here
                // Choosing here to remove the "end turn" move before its the last move. This is done to make the random plays a bit more realistic
                if (MCTSSettings.FORCE_DELAY_TURN_END_IN_ROLLOUT){
                    if(rolloutAvailableMoves.Count > 1) {
                        rolloutAvailableMoves.RemoveAll(move => move.Command == CommandEnum.END_TURN);
                    }
                }
                Move moveToMake = rolloutAvailableMoves[Utility.Rng.Next(rolloutAvailableMoves.Count)];
                (rollOutGameState, rolloutAvailableMoves) = rollOutGameState.ApplyMove(moveToMake, (ulong)Utility.Rng.Next());
            }
            if(rollOutGameState.GameEndState.Winner != PlayerEnum.NO_PLAYER_SELECTED) { //TODO here i assume that winner = NO_PLAYER_SELECTED is how they show a draw. Need to confirm this
                if(rollOutGameState.GameEndState.Winner == GameState.CurrentPlayer.PlayerID){
                    result += 1;
                }
                else{
                    result -= 1;
                }
            }
        }
        return result;
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
            switch(MCTSSettings.CHOSEN_EVALUATION_FUNCTION){
                case EvaluationFunction.UCB1:
                    double exploitation = TotalScore / VisitCount;
                    double exploration = MCTSSettings.UCB1_EXPLORATION_CONSTANT * Math.Sqrt(Math.Log(Parent.VisitCount) / VisitCount);
                    return exploitation + exploration;
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
                        // TODO consider if some of the choice cards are also obvious moves, since the choice will be a new move
                        // or how to handle this issue
                        (GameState, AvailableMoves) = GameState.ApplyMove(currMove,  (ulong)Utility.Rng.Next());
                        ApplyAllDeterministicAndObviousMoves();
                        break;
                    }
                }
                else if(currMove.Command == CommandEnum.ACTIVATE_AGENT) {
                    if (Utility.OBVIOUS_AGENT_EFFECTS.Contains(((SimpleCardMove)currMove).Card.CommonId)){
                        (GameState, AvailableMoves) = GameState.ApplyMove(currMove,  (ulong)Utility.Rng.Next());
                        ApplyAllDeterministicAndObviousMoves();
                        break;
                    }
                }
            }
        }
    }