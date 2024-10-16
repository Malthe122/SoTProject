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

    private Random rng = new Random();
    public override void GameEnd(EndGameState state, FullGameState? finalBoardState)
    {
    }

    public override Move Play(GameState gameState, List<Move> possibleMoves, TimeSpan remainingTime)
    {
        try{
            Console.WriteLine("Available Moves:\n------");
            possibleMoves.ForEach(move => Console.WriteLine(move));
            Console.WriteLine("------");
            //TODO implement
            var rootNode = new Node(gameState.ToSeededGameState((ulong)rng.Next()), null, possibleMoves, null);
            for(int i = 0; i <= Utility.ITERATIONS; i++) {
                rootNode.Simulate(out double score);
            }

            var bestNode = rootNode.ChildNodes.OrderByDescending(child => (child.TotalScore / child.VisitCount)).First();

            Console.WriteLine("Trying to play move:");
            Console.WriteLine(bestNode.AppliedMove.ToString());
            
            return bestNode.AppliedMove;
        } catch(Exception e) {
            Console.WriteLine("Something went wrong while trying to compute move. Playing random move instead. Exception:");
            Console.WriteLine("Message: " + e.Message);
            Console.WriteLine("Stacktrace: " + e.StackTrace);
            return possibleMoves[0];
        }
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
}