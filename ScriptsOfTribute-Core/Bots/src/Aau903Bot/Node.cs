using ScriptsOfTribute;
using ScriptsOfTribute.Serializers;

public class Node {
    /// <summary>
    /// Has to be stored like a seeded game state although its a bit non-intuitive, but this is the object type that applyMove method returns.
    /// However we never want to actually reuse the seed inside the object, so when we call apply move on the seeded state, we need to call it
    /// with a new random seed which is a possible argument for the applyMove method
    /// </summary>
    public Node? Parent = null;
    public List<Node> ChildNodes = new List<Node>();
    public int VisitCount = 0;
    public double TotalScore = 0;

    public int GameStateHash;
    public SeededGameState GameState;
    public Move? AppliedMove = null;
    public List<Move> PossibleMoves;
    
    public Node(SeededGameState gameState, Node parent, List<Move> possibleMoves, Move appliedMove) {
        GameState = gameState;
        Parent = parent;
        PossibleMoves = possibleMoves;
        AppliedMove = appliedMove;
        /// <summary>
        /// TODO if this takes too much performance, look into only calling this method on children of chance nodes
        /// </summary>
        GameStateHash = GameState.GenerateHash();
    }

    public virtual void Visit(out double score) {
        Node visitedChild = null;
        var playerId = GameState.CurrentPlayer.PlayerID;

        if (GameState.GameEndState == null) {
            if (VisitCount == 0) {
                ApplyAllDeterministicAndObviousMoves();
                score = Rollout();
            } else if (PossibleMoves.Count > ChildNodes.Count) {
                var expandedChild = Expand();
                expandedChild.Visit(out score);
            } else {
                var selectedChild = Select();
                selectedChild.Visit(out score);

                if (selectedChild.GameState.CurrentPlayer.PlayerID != playerId) {
                    score *= -1;
                }
            }
        } else if (GameState.GameEndState.Winner == PlayerEnum.NO_PLAYER_SELECTED) {
            score = 0;
        } else if (GameState.GameEndState.Winner == GameState.CurrentPlayer.PlayerID) {
            score = 1;
        } else {
            score = -1;
        }

        TotalScore += score;
        VisitCount++;
        // return visitedChild;
    }

    internal Node Expand() {
        foreach (var move in PossibleMoves) {
            if (!ChildNodes.Any(child => child.AppliedMove == move)) {
                //TODO insert here checks that if a move can lead to several different stages, we need to create a chance node
                ulong randomSeed = (ulong)Utility.Rng.Next();
                var (newGameState, newPossibleMoves) = GameState.ApplyMove(move, randomSeed); 
                var expandedChild = new Node(newGameState, this, newPossibleMoves, move);
                ChildNodes.Add(expandedChild);
                return expandedChild;
            }
        }

        return null;
    }

    internal double Evaluate(SeededGameState gameState, PlayerEnum playerId) {
        if (gameState.GameEndState.Winner != PlayerEnum.NO_PLAYER_SELECTED) { //TODO here i assume that winner = NO_PLAYER_SELECTED is how they show a draw. Need to confirm this
            if (gameState.GameEndState.Winner == playerId) {
                return +1;
            } else {
                return -1;
            }
        }
        return 0;
    }

    internal double Rollout() {
        double result = 0;
        var rolloutGameState = GameState;
        var rolloutPlayerId = rolloutGameState.CurrentPlayer.PlayerID;
        var rolloutPossibleMoves = new List<Move>(PossibleMoves);

        for (int i = 0; i <= MCTSSettings.NUMBER_OF_ROLLOUTS; i++) {
            while (rolloutGameState.GameEndState == null) {
                // TODO also apply the playing obvious moves in here
                // Choosing here to remove the "end turn" move before its the last move. This is done to make the random plays a bit more realistic
                if (MCTSSettings.FORCE_DELAY_TURN_END_IN_ROLLOUT) {
                    if (rolloutPossibleMoves.Count > 1) {
                        rolloutPossibleMoves.RemoveAll(move => move.Command == CommandEnum.END_TURN);
                    }
                }
                var chosenIndex = Utility.Rng.Next(rolloutPossibleMoves.Count);
                var moveToMake = rolloutPossibleMoves[chosenIndex];

                var (newGameState, newPossibleMoves) = rolloutGameState.ApplyMove(moveToMake);
                rolloutGameState = newGameState;
                rolloutPossibleMoves = newPossibleMoves;
            }

            result += Evaluate(rolloutGameState, rolloutPlayerId);
        }
        return result;
    }

    internal virtual Node Select() {
        double maxConfidence = -double.MaxValue;
        var highestConfidenceChild = ChildNodes[0];

        foreach (var childNode in ChildNodes) {
            double confidence = childNode.GetConfidenceScore();
            if (confidence > maxConfidence) {
                maxConfidence = confidence;
                highestConfidenceChild = childNode;
            }
        }

        return highestConfidenceChild;
    }

    public double GetConfidenceScore() {
        switch (MCTSSettings.CHOSEN_EVALUATION_FUNCTION) {
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

    internal void ApplyAllDeterministicAndObviousMoves() {
        foreach (Move currMove in PossibleMoves) {
            if (currMove.Command == CommandEnum.PLAY_CARD) {
                if (Utility.OBVIOUS_ACTION_PLAYS.Contains(((SimpleCardMove)currMove).Card.CommonId)) {
                    // TODO consider if some of the choice cards are also obvious moves, since the choice will be a new move
                    // or how to handle this issue
                    (GameState, PossibleMoves) = GameState.ApplyMove(currMove,  (ulong)Utility.Rng.Next());
                    ApplyAllDeterministicAndObviousMoves();
                    break;
                }
            } else if (currMove.Command == CommandEnum.ACTIVATE_AGENT) {
                if (Utility.OBVIOUS_AGENT_EFFECTS.Contains(((SimpleCardMove)currMove).Card.CommonId)) {
                    (GameState, PossibleMoves) = GameState.ApplyMove(currMove,  (ulong)Utility.Rng.Next());
                    ApplyAllDeterministicAndObviousMoves();
                    break;
                }
            }
        }
    }
}
