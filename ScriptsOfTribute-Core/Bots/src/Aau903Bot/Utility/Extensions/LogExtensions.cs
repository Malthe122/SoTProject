using System.ComponentModel;
using ScriptsOfTribute;
using ScriptsOfTribute.Board.Cards;
using ScriptsOfTribute.Serializers;

public static class LogExtensions{

    public static void Log(this SeededGameState gameState)
    {
        Console.WriteLine("GameState " + "(" + gameState.GenerateHash() + "):-----------------------------");
        Console.WriteLine("Combo states:");
        gameState.ComboStates.Log();
        Console.WriteLine("Current Player:");
        gameState.CurrentPlayer.Log();
        Console.WriteLine("---");
        Console.WriteLine("Enemy Player:");
        gameState.EnemyPlayer.Log();
        Console.WriteLine("---");
        Console.WriteLine("Patron states:");
        gameState.PatronStates.Log();
        Console.WriteLine("Pending choice:");
        gameState.PendingChoice?.Log();
        Console.WriteLine("Start of next turn effects:");
        gameState.StartOfNextTurnEffects.Log();
        Console.WriteLine("Tavern available cards:");
        gameState.TavernAvailableCards.Log();
        Console.WriteLine("Tavern cards size: " + gameState.TavernCards.Count);
        Console.WriteLine("Upcoming effects:");
        gameState.UpcomingEffects.Log();
        Console.WriteLine("-----------------------------------------------------");
    }

    public static void Log(this List<UniqueBaseEffect> list){
        // TODO expand if neccessary
        Console.WriteLine("Amount: " + list.Count);
    }

    public static void Log (this List<UniqueCard> list) {
        list.ForEach(card => Console.WriteLine(card.CommonId));
    }

    public static void Log(this SerializedChoice choice) {
        if (choice == null) {
            Console.WriteLine("N/A");
            return;
        }

        Console.WriteLine("Choice follow up: " + choice.ChoiceFollowUp);
        Console.WriteLine("Max choices: " + choice.MaxChoices);
        Console.WriteLine("Min choices: " + choice.MinChoices);
        Console.WriteLine("Type: " + choice.Type);

        switch(choice.Type) {
            case Choice.DataType.EFFECT:
                Console.WriteLine("Possible effects:");
                choice.PossibleEffects.ForEach(effect => {
                    Console.WriteLine("Parent card: " + effect.ParentCard.CommonId);
                    Console.WriteLine("Amount: " + effect.Amount);
                    Console.WriteLine("Combo: " + effect.Combo);
                });
                break;
            case Choice.DataType.CARD:
                Console.WriteLine("Possible cards:");
                choice.PossibleCards.ForEach(card => {
                    Console.WriteLine(card.CommonId);
                });
                break;
        }
    }

    public static void Log(this PatronStates patronStates) {
        foreach(var currKey in patronStates.All.Keys) {
            Console.WriteLine(currKey + ":");
            Console.WriteLine(patronStates.All[currKey]);
        }
    }

    public static void Log(this ComboStates comboStates) {
        foreach(var currKey in comboStates.All.Keys) {
            Console.WriteLine(currKey + ":");
            Console.WriteLine("Current combo: " + comboStates.All[currKey].CurrentCombo);
        }
    }

    public static void Log(this SerializedPlayer player)
    {
        Console.WriteLine("Coins: " + player.Coins);
        Console.WriteLine("Power: " + player.Power);
        Console.WriteLine("Prestige: " + player.Prestige);
        Console.WriteLine("Patron calls: " + player.PatronCalls);
        Console.WriteLine("Cards in hand:");
        player.Hand.ForEach(h => Console.WriteLine(h.CommonId));
        Console.WriteLine("Played cards:");
        player.Played.ForEach(h => Console.WriteLine(h.CommonId));
        Console.WriteLine("Known upcoming draws:");
        player.KnownUpcomingDraws.ForEach(h => Console.WriteLine(h.CommonId));
        Console.WriteLine("Cooldown size: " + player.CooldownPile.Count);
        Console.WriteLine("Drawpile size: " + player.DrawPile.Count);
        Console.WriteLine("Agents:");
        player.Agents.Log();
    }

    public static void Log(this List<SerializedAgent> list){
        list.ForEach(agent => {
            Console.WriteLine(agent.RepresentingCard.CommonId + ":");
            Console.WriteLine("Activated: " + agent.Activated);
            Console.WriteLine("HP: " + agent.CurrentHp);
        });
    }

    public static void Log(this Move move)
    {
        switch (move.Command)
        {
            case CommandEnum.PLAY_CARD:
                Console.WriteLine("Play card: " + (move as SimpleCardMove).Card.Name);
                break;
            case CommandEnum.ACTIVATE_AGENT:
                Console.WriteLine("Activating agent: " + (move as SimpleCardMove).Card.Name);
                break;
            case CommandEnum.ATTACK:
                Console.WriteLine("Attacking: " + (move as SimpleCardMove).Card.Name);
                break;
            case CommandEnum.BUY_CARD:
                Console.WriteLine("Buying card: " + (move as SimpleCardMove).Card.Name);
                break;
            case CommandEnum.CALL_PATRON:
                Console.WriteLine("Calling patron: " + (move as SimplePatronMove).PatronId);
                break;
            case CommandEnum.MAKE_CHOICE:
                Console.WriteLine("Making choice some choice");
                break;
            case CommandEnum.END_TURN:
                Console.WriteLine("Ending turn");
                break;
        }
    }


}