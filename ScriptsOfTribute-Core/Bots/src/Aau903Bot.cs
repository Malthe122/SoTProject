using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Bots;
using ScriptsOfTribute;
using ScriptsOfTribute.AI;
using ScriptsOfTribute.Board;
using ScriptsOfTribute.Board.Cards;
using ScriptsOfTribute.Serializers;


public class Aau903Bot : AI
{
public static readonly List<CardId> OBVIOUS_ACTION_PLAYS = new List<CardId>(){
    CardId.LUXURY_EXPORTS,
    CardId.GOODS_SHIPMENT,
    CardId.MIDNIGHT_RAID,
    CardId.WAR_SONG,
    CardId.PLUNDER,
    CardId.TOLL_OF_FLESH,
    CardId.TOLL_OF_SILVER,
    CardId.MURDER_OF_CROWS,
    CardId.PILFER,
    CardId.SQUAWKING_ORATORY,
    CardId.LAW_OF_SOVEREIGN_ROOST,
    CardId.POOL_OF_SHADOW,
    CardId.SCRATCH,
    CardId.PECK,
    CardId.MAINLAND_INQUIRIES,
    CardId.RALLY,
    CardId.SIEGE_WEAPON_VOLLEY,
    CardId.THE_ARMORY,
    CardId.REINFORCEMENTS,
    CardId.ARCHERS_VOLLEY,
    CardId.LEGIONS_ARRIVAL,
    CardId.THE_PORTCULLIS,
    CardId.FORTIFY,
    CardId.BEWILDERMENT,
    CardId.SWIPE,
    CardId.GHOSTSCALE_SEA_SERPENT,
    CardId.MAORMER_BOARDING_PARTY,
    CardId.PYANDONEAN_WAR_FLEET,
    CardId.SEA_ELF_RAID,
    CardId.SEA_SERPENT_COLOSSUS,
    CardId.SERPENTPROW_SCHOONER,
    CardId.SUMMERSET_SACKING,
    CardId.GOLD,
    CardId.WRIT_OF_COIN
};

public static readonly List<CardId> OBVIOUS_AGENT_EFFECTS = new List<CardId>(){
    CardId.BLACKFEATHER_KNAVE,
    CardId.BLACKFEATHER_BRIGAND,
    CardId.BANNERET,
    CardId.KNIGHT_COMMANDER,
    CardId.SHIELD_BEARER,
    CardId.BANGKORAI_SENTRIES,
    CardId.KNIGHTS_OF_SAINT_PELIN,
    CardId.JEERING_SHADOW,
    CardId.PROWLING_SHADOW,
    CardId.STUBBORN_SHADOW,
};
    public override void GameEnd(EndGameState state, FullGameState? finalBoardState)
    {
        throw new NotImplementedException();
    }

    public override Move Play(GameState gameState, List<Move> possibleMoves, TimeSpan remainingTime)
    {
        Log("Trying to take my turn");
        StateMachine machine = new StateMachine(gameState);

        Log("Game state is:");
        LogState(gameState);
        Log("----------------");
        Log("");
        foreach(var move in possibleMoves) {
            Log("If i play the move,");
            LogMove(move);
            Log("We will transfer to this game state:");
            var newState = machine.SimulateMove(move, this);
            LogState(newState);
            Log("");
        }

        return possibleMoves[0];
    }

    public void LogFromThis(string log){
        Log(log);
    }

    private void LogMove(Move move)
    {
        switch (move.Command)
        {
            case CommandEnum.PLAY_CARD:
                Log("Play card: " + (move as SimpleCardMove).Card.Name);
                break;
            case CommandEnum.ACTIVATE_AGENT:
                Log("Activating agent: " + (move as SimpleCardMove).Card.Name);
                break;
            case CommandEnum.ATTACK:
                Log("Attacking: " + (move as SimpleCardMove).Card.Name);
                break;
            case CommandEnum.BUY_CARD:
                Log("Buying card: " + (move as SimpleCardMove).Card.Name);
                break;
            case CommandEnum.CALL_PATRON:
                Log("Calling patron: " + (move as SimplePatronMove).PatronId);
                break;
            case CommandEnum.MAKE_CHOICE:
                Log("Making choice some choice. TODO log this better"); //TODO log this better
                break;
            case CommandEnum.END_TURN:
                Log("Ending turn");
                break;
        }
    }

    private void LogState(GameState gameState)
    {
        Log("State:");
        Log("You:");
        LogPlayerState(gameState.CurrentPlayer);
        Log("Opponent:");
        LogPlayerState(gameState.EnemyPlayer);
        Log("Tavern:");
        Log("Cards in tavern: " + gameState.TavernAvailableCards.Count);
    }

    private void LogPlayerState(FairSerializedEnemyPlayer player)
    {
        Log("Coins: " + player.Coins);
        Log("Power: " + player.Power);
        Log("Prestige: " + player.Prestige);
        Log("Cards in cooldown: " + player.CooldownPile.Count);
    }

    private void LogPlayerState(FairSerializedPlayer player)
    {
        Log("Coins: " + player.Coins);
        Log("Power: " + player.Power);
        Log("Prestige: " + player.Prestige);
        Log("Cards in cooldown: " + player.CooldownPile.Count);
    }

    public override PatronId SelectPatron(List<PatronId> availablePatrons, int round)
    {
        return availablePatrons[0];
    }

    private class Node{
        public GameState GameState; //Might need to be change to seeded game state
        public List<Node> ChildNodes;
        public int GameStateHash;
        public double TotalScore;
        public int VisitCount;
        public Move AppliedMove;
        public List<Move> AvailableMoves;
        

        public Node(GameState gameState, Move appliedMove, List<Move> availableMoves){
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
            GameStateHash = GameState.SerializeGameState().GetHashCode();
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
                        (GameState, AvailableMoves) = GameState.ApplyMove(currMove,  (ulong)(new Random().Next()));
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

    private class ChanceNode : Node
    {
        private Random rng = new Random();
        public List<Node> NodeVersions;
        public ChanceNode(GameState gameState) : base(gameState)
        {
            NodeVersions = new List<Node>();
        }

        override public Node Simulate(out double score){
            var chosenNode = NodeVersions.ElementAt(rng.Next(NodeVersions.Count));
            var resultingNode = chosenNode.Simulate(out double chosenNodeScore);

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
}