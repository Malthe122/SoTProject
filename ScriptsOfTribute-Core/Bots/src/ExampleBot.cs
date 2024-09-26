using Bots;
using ScriptsOfTribute;
using ScriptsOfTribute.AI;
using ScriptsOfTribute.Board;
using ScriptsOfTribute.Serializers;

public class ExampleBot : AI
{
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
}