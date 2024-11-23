using ExternalHeuristic;
using ScriptsOfTribute;
using ScriptsOfTribute.Serializers;
using System.Text;
using System.Data.HashFunction.xxHash;

namespace Aau903Bot;

public static class Utility
{
    public static Random Rng = new Random();

    // TODO consider making an evaluation function at start of the game, that populates these lists
    // based on their effects, rather than relying on manually categorising them correctly
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

    public static readonly List<CardId> RANDOM_ACTION_EFFECTS = new List<CardId>(){
        CardId.PILFER,
        CardId.SQUAWKING_ORATORY,
        CardId.THE_DREAMING_CAVE
    };
    public static readonly List<CardId> RANDOM_ACTION_COMBO_EFFECTS = new List<CardId>(){
        CardId.TOLL_OF_FLESH,
        CardId.TOLL_OF_SILVER,
        CardId.PILFER,
        CardId.SQUAWKING_ORATORY,
        CardId.RALLY,
        CardId.TWILIGHT_REVELRY
    };

    public static readonly List<CardId> RANDOM_CONTRACT_ACTION_EFFECTS = new List<CardId>(){
        CardId.HARVEST_SEASON
    };

    public static readonly List<CardId> RANDOM_CONTRACT_ACTION_COMBO_EFFECTS = new List<CardId>(){
        CardId.BLOOD_SACRIFICE,
        CardId.BLOODY_OFFERING,
        CardId.LAW_OF_SOVEREIGN_ROOST,
        CardId.BAG_OF_TRICKS,
        CardId.RINGS_GUILE,
    };

    public static readonly List<CardId> RANDOM_AGENT_COMBO_EFFECTS = new List<CardId>(){
        CardId.BLACKFEATHER_KNAVE,
        CardId.BLACKFEATHER_BRIGAND
    };

    public static readonly List<PatronId> RANDOM_PATRON_CALL_EFFECT = new List<PatronId>(){
        PatronId.RED_EAGLE
    };

    public static bool Equals(SeededGameState state1, SeededGameState state2)
    {
        var player1 = state1.CurrentPlayer;
        var player2 = state2.CurrentPlayer;
        var enemy1 = state1.EnemyPlayer;
        var enemy2 = state2.EnemyPlayer;

        var hand1 = player1.Hand.OrderBy(card => card.CommonId).ToList();
        var hand2 = player2.Hand.OrderBy(card => card.CommonId).ToList();
        if (hand1.Count != hand2.Count) return false;
        for (int i = 0; i < hand1.Count; i++)
        {
            if (hand1[i].CommonId != hand2[i].CommonId) return false;
        }

        var tavernCards1 = state1.TavernAvailableCards.OrderBy(card => card.CommonId).ToList();
        var tavernCards2 = state2.TavernAvailableCards.OrderBy(card => card.CommonId).ToList();
        if (tavernCards1.Count != tavernCards2.Count) return false;
        for (int i = 0; i < tavernCards1.Count; i++)
        {
            if (tavernCards1[i].CommonId != tavernCards2[i].CommonId) return false;
        }

        var tavernDeck1 = state1.TavernCards.OrderBy(card => card.CommonId).ToList();
        var tavernDeck2 = state2.TavernCards.OrderBy(card => card.CommonId).ToList();
        if (tavernDeck1.Count != tavernDeck2.Count) return false;
        for (int i = 0; i < tavernDeck1.Count; i++)
        {
            if (tavernDeck1[i].CommonId != tavernDeck2[i].CommonId) return false;
        }

        var cooldownPlayer1 = player1.CooldownPile.OrderBy(card => card.CommonId).ToList();
        var cooldownPlayer2 = player2.CooldownPile.OrderBy(card => card.CommonId).ToList();
        if (cooldownPlayer1.Count != cooldownPlayer2.Count) return false;
        for (int i = 0; i < cooldownPlayer1.Count; i++)
        {
            if (cooldownPlayer1[i].CommonId != cooldownPlayer2[i].CommonId) return false;
        }

        var cooldownEnemy1 = enemy1.CooldownPile.OrderBy(card => card.CommonId).ToList();
        var cooldownEnemy2 = enemy2.CooldownPile.OrderBy(card => card.CommonId).ToList();
        if (cooldownEnemy1.Count != cooldownEnemy2.Count) return false;
        for (int i = 0; i < cooldownEnemy1.Count; i++)
        {
            if (cooldownEnemy1[i].CommonId != cooldownEnemy2[i].CommonId) return false;
        }

        var upcomingPlayerDraws1 = player1.KnownUpcomingDraws.OrderBy(card => card.CommonId).ToList();
        var upcomingPlayerDraws2 = player2.KnownUpcomingDraws.OrderBy(card => card.CommonId).ToList();
        if (upcomingPlayerDraws1.Count != upcomingPlayerDraws2.Count) return false;
        for (int i = 0; i < upcomingPlayerDraws1.Count; i++)
        {
            if (upcomingPlayerDraws1[i].CommonId != upcomingPlayerDraws2[i].CommonId) return false;
        }

        var upcomingEnemyDraws1 = enemy1.KnownUpcomingDraws.OrderBy(card => card.CommonId).ToList();
        var upcomingEnemyDraws2 = enemy2.KnownUpcomingDraws.OrderBy(card => card.CommonId).ToList();
        if (upcomingEnemyDraws1.Count != upcomingEnemyDraws2.Count) return false;
        for (int i = 0; i < upcomingEnemyDraws1.Count; i++)
        {
            if (upcomingEnemyDraws1[i].CommonId != upcomingEnemyDraws2[i].CommonId) return false;
        }

        var drawPlayer1 = player1.DrawPile.OrderBy(card => card.CommonId).ToList();
        var drawPlayer2 = player2.DrawPile.OrderBy(card => card.CommonId).ToList();
        if (drawPlayer1.Count != drawPlayer2.Count) return false;
        for (int i = 0; i < drawPlayer1.Count; i++)
        {
            if (drawPlayer1[i].CommonId != drawPlayer2[i].CommonId) return false;
        }

        var drawEnemy1 = enemy1.DrawPile.OrderBy(card => card.CommonId).ToList();
        var drawEnemy2 = enemy2.DrawPile.OrderBy(card => card.CommonId).ToList();
        if (drawEnemy1.Count != drawEnemy2.Count) return false;
        for (int i = 0; i < drawEnemy1.Count; i++)
        {
            if (drawEnemy1[i].CommonId != drawEnemy2[i].CommonId) return false;
        }

        if (player1.Coins != player2.Coins || player1.Prestige != player2.Prestige || player1.Power != player2.Power) return false;
        if (enemy1.Coins != enemy2.Coins || enemy1.Prestige != enemy2.Prestige || enemy1.Power != enemy2.Power) return false;

        var agentPlayer1 = player1.Agents.OrderBy(agent => agent.RepresentingCard.CommonId).ToList();
        var agentPlayer2 = player2.Agents.OrderBy(agent => agent.RepresentingCard.CommonId).ToList();
        if (agentPlayer1.Count != agentPlayer2.Count) return false;
        for (int i = 0; i < agentPlayer1.Count; i++)
        {
            if (agentPlayer1[i].RepresentingCard.CommonId != agentPlayer2[i].RepresentingCard.CommonId ||
                agentPlayer1[i].Activated != agentPlayer2[i].Activated ||
                agentPlayer1[i].CurrentHp != agentPlayer2[i].CurrentHp) return false;
        }

        var agentEnemy1 = player1.Agents.OrderBy(agent => agent.RepresentingCard.CommonId).ToList();
        var agentEnemy2 = player2.Agents.OrderBy(agent => agent.RepresentingCard.CommonId).ToList();
        if (agentEnemy1.Count != agentEnemy2.Count) return false;
        for (int i = 0; i < agentEnemy1.Count; i++)
        {
            if (agentEnemy1[i].RepresentingCard.CommonId != agentEnemy2[i].RepresentingCard.CommonId ||
                agentEnemy1[i].Activated != agentEnemy2[i].Activated ||
                agentEnemy1[i].CurrentHp != agentEnemy2[i].CurrentHp) return false;
        }

        foreach (var (patronId, patronState) in state1.PatronStates.All)
        {
            if (state2.PatronStates.All.Any(
                p => p.Key != patronId || p.Value != patronState)
            ) return false;
        }

        if (state1.PendingChoice != null && state1.PendingChoice == state2.PendingChoice)
        {
            if (state1.PendingChoice.MaxChoices != state2.PendingChoice.MaxChoices) return false;
            if (state1.PendingChoice.MinChoices != state2.PendingChoice.MinChoices) return false;
            if (state1.PendingChoice.ChoiceFollowUp != state2.PendingChoice.ChoiceFollowUp) return false;
        }

        return true;
    }

    public static int QuickHash(SeededGameState state)
    {
        const int MODULO = 2147483647; // 2^31 - 1, a large prime
        var player = state.CurrentPlayer;
        var enemy = state.EnemyPlayer;

        int playerHand = 0;
        foreach (var currCard in player.Hand)
        {
            playerHand = (playerHand + 31 * (int)currCard.CommonId) % MODULO;
        }

        int tavernCards = 0;
        foreach (var currCard in state.TavernAvailableCards)
        {
            tavernCards = (tavernCards + 37 * ((int)currCard.CommonId)) % MODULO;
        }
        int tavernDeck = 0;
        foreach (var currCard in state.TavernCards)
        {
            tavernDeck = (tavernDeck + 41 * ((int)currCard.CommonId)) % MODULO;
        }

        int playerCooldown = 0;
        foreach (var currCard in player.CooldownPile)
        {
            playerCooldown = (playerCooldown + 43 * ((int)currCard.CommonId)) % MODULO;
        }
        int enemyCooldown = 0;
        foreach (var currCard in enemy.CooldownPile)
        {
            enemyCooldown = (enemyCooldown + 47 * ((int)currCard.CommonId)) % MODULO;
        }

        int playerUpcomingDraws = 0;
        foreach (var currCard in player.KnownUpcomingDraws)
        {
            playerUpcomingDraws = (playerUpcomingDraws + 53 * ((int)currCard.CommonId)) % MODULO;
        }
        int enemyUpcomingDraws = 0;
        foreach (var currCard in enemy.KnownUpcomingDraws)
        {
            enemyUpcomingDraws = (enemyUpcomingDraws + 59 * ((int)currCard.CommonId)) % MODULO;
        }

        int playerDraw = 0;
        foreach (var currCard in player.DrawPile)
        {
            playerDraw = (playerDraw + 61 * ((int)currCard.CommonId)) % MODULO;
        }
        int enemyDraw = 0;
        foreach (var currCard in enemy.DrawPile)
        {
            enemyDraw = (enemyDraw + 67 * ((int)currCard.CommonId)) % MODULO;
        }

        int playerPlayed = 0;
        foreach (var currCard in player.Played)
        {
            playerPlayed = (playerPlayed + 71 * ((int)currCard.CommonId)) % MODULO;
        }
        int enemyPlayed = 0;
        foreach (var currCard in enemy.Played)
        {
            enemyPlayed = (enemyPlayed + 73 * ((int)currCard.CommonId)) % MODULO;
        }

        int playerResources = ((player.Coins * 79) + (player.Prestige * 83) + (player.Power * 89)) % MODULO;
        int enemyResources = ((enemy.Coins * 97) + (enemy.Prestige * 101) + (enemy.Power * 103)) % MODULO;

        int playerAgents = 0;
        foreach (var currAgent in player.Agents)
        {
            playerAgents = (playerAgents + 109 * (currAgent.Activated ? 1 : 2) + 113 * currAgent.CurrentHp) % MODULO;
        }
        int enemyAgents = 0;
        foreach (var currAgent in enemy.Agents)
        {
            enemyAgents = (enemyAgents + 127 * (currAgent.Activated ? 1 : 2) + 131 * currAgent.CurrentHp) % MODULO;
        }

        int patrons = 0;
        foreach (var (patronId, patronState) in state.PatronStates.All)
        {
            patrons = (patrons + 137 * (int)patronId * (int)patronState) % MODULO;
        }

        var playerPatronCalls = (int)player.PatronCalls % MODULO;
        var enemyPatronCalls = (int)enemy.PatronCalls % MODULO;

        var playerId = (int)player.PlayerID % MODULO;
        var enemyId = (int)enemy.PlayerID % MODULO;

        int pendingChoice = 0;
        if (state.PendingChoice != null)
        {
            pendingChoice = (pendingChoice + 149 * (state.PendingChoice.MaxChoices + state.PendingChoice.MinChoices) +
                             151 * (int)state.PendingChoice.ChoiceFollowUp) % MODULO;
        }

        return (
            playerHand +
            tavernDeck + tavernCards +
            playerCooldown + enemyCooldown +
            playerUpcomingDraws + enemyUpcomingDraws +
            playerDraw + enemyDraw +
            playerResources + enemyResources +
            playerPatronCalls + enemyPatronCalls +
            playerId + enemyId +
            playerAgents + patrons + pendingChoice
        ) % MODULO;
    }


    public static int PreciseHash(SeededGameState state)
    {
        var player = state.CurrentPlayer;
        var enemy = state.EnemyPlayer;

        var playerHand = string.Join("", player.Hand.OrderBy(card => card.CommonId).Select(card => (int)card.CommonId));
        var enemyHand = string.Join("", player.Hand.OrderBy(card => card.CommonId).Select(card => (int)card.CommonId));

        var tavernCards = string.Join("", state.TavernAvailableCards.OrderBy(card => card.CommonId).Select(card => (int)card.CommonId));
        var tavernDeck = string.Join("", state.TavernCards.OrderBy(card => card.CommonId).Select(card => (int)card.CommonId));

        var playerCooldown = string.Join("", player.CooldownPile.OrderBy(card => card.CommonId).Select(card => (int)card.CommonId));
        var enemyCooldown = string.Join("", enemy.CooldownPile.OrderBy(card => card.CommonId).Select(card => (int)card.CommonId));

        var playerUpcomingDraws = string.Join("", player.KnownUpcomingDraws.OrderBy(card => card.CommonId).Select(card => (int)card.CommonId));
        var enemyUpcomingDraws = string.Join("", enemy.KnownUpcomingDraws.OrderBy(card => card.CommonId).Select(card => (int)card.CommonId));

        var playerDraw = string.Join("", player.DrawPile.OrderBy(card => card.CommonId).Select(card => (int)card.CommonId));
        var enemyDraw = string.Join("", enemy.DrawPile.OrderBy(card => card.CommonId).Select(card => (int)card.CommonId));

        var playerResources = string.Join("", new List<int>() { player.Coins, player.Power, player.Prestige });
        var enemyResources = string.Join("", new List<int>() { enemy.Coins, enemy.Power, enemy.Prestige });

        var playerAgents = string.Join("", player.Agents.OrderBy(agent => agent.RepresentingCard.CommonId)
            .Select(agent => $"{(int)agent.RepresentingCard.CommonId}{agent.Activated}{agent.CurrentHp}"));
        var enemyAgents = string.Join("", enemy.Agents.OrderBy(agent => agent.RepresentingCard.CommonId)
            .Select(agent => $"{(int)agent.RepresentingCard.CommonId}{agent.Activated}{agent.CurrentHp}"));

        var patrons = string.Join("", state.PatronStates.All.Select((patronId, patronState) => $"{patronId}{patronState}"));

        var pendingChoice = "";
        if (state.PendingChoice != null)
        {
            pendingChoice = $"{state.PendingChoice.MaxChoices}{state.PendingChoice.MinChoices}{state.PendingChoice.ChoiceFollowUp}";
        }

        string hashString = playerHand + enemyHand + tavernCards + tavernDeck + playerCooldown + enemyCooldown + playerUpcomingDraws + enemyUpcomingDraws + playerDraw + enemyDraw + playerResources + enemyResources + playerAgents + enemyAgents + patrons + pendingChoice;

        var xxHash = xxHashFactory.Instance.Create();
        byte[] hashBytes = xxHash.ComputeHash(Encoding.UTF8.GetBytes(hashString)).Hash;
        return BitConverter.ToInt32(hashBytes, 0) & 0x7FFFFFFF; // ensures positive int
    }

    public static int GenerateHash(this GameState state)
    {
        var seededGameState = state.ToSeededGameState((ulong)Rng.Next());
        return GenerateHash(seededGameState);
    }

    public static int GenerateHash(this SeededGameState state)
    {
        switch (MCTSHyperparameters.CHOSEN_HASH_GENERATION_TYPE)
        {
            case HashGenerationType.Quick:
                return QuickHash(state);
            case HashGenerationType.Precise:
                return PreciseHash(state);
            default:
                throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Is not taking Combo effects into account at the moment, but here we should also not look at whether the played card has an effect, but also whether there is a combo cards already played that
    /// will be activated by playing said card
    /// </summary>
    /// <param name="move"></param>
    /// <returns></returns>
    public static bool IsNonDeterministic(this Move move)
    {
        switch (move.Command)
        {
            case CommandEnum.PLAY_CARD:
                return RANDOM_ACTION_EFFECTS.Contains((move as SimpleCardMove).Card.CommonId);
            case CommandEnum.ACTIVATE_AGENT:
                return false; // None of these exists
            case CommandEnum.ATTACK:
                return false;
            case CommandEnum.BUY_CARD:
                return RANDOM_CONTRACT_ACTION_EFFECTS.Contains((move as SimpleCardMove).Card.CommonId);
            case CommandEnum.CALL_PATRON:
                return RANDOM_PATRON_CALL_EFFECT.Contains((move as SimplePatronMove).PatronId);
            case CommandEnum.MAKE_CHOICE:
                return false; //TODO we might add some check here too
            case CommandEnum.END_TURN:
                return false; //TODO we consider this false for now, but it should be considered true since the opponent will draw their cards which is random
            default:
                return false;
        }
    }

    public static void Log(this SeededGameState gameState)
    {
        Console.WriteLine("State:");
        Console.WriteLine("You:");
        gameState.CurrentPlayer.Log();
        Console.WriteLine("Opponent:");
        gameState.EnemyPlayer.Log();
        Console.WriteLine("Tavern:");
        Console.WriteLine("Cards in tavern: " + gameState.TavernAvailableCards.Count);
    }

    public static void Log(this SerializedPlayer player)
    {
        Console.WriteLine("Coins: " + player.Coins);
        Console.WriteLine("Power: " + player.Power);
        Console.WriteLine("Prestige: " + player.Prestige);
        Console.WriteLine("Cards in cooldown: " + player.CooldownPile.Count);
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

    public static double UseBestMCTS3Heuristic(SeededGameState gameState)
    {

        GameStrategy strategy;

        var currentPlayer = gameState.CurrentPlayer;
        int cardCount = currentPlayer.Hand.Count + currentPlayer.CooldownPile.Count + currentPlayer.DrawPile.Count;
        int points = gameState.CurrentPlayer.Prestige;
        if (points >= 27 || gameState.EnemyPlayer.Prestige >= 30)
        {
            strategy = new GameStrategy(cardCount, GamePhase.LateGame);
        }
        else if (points <= 10 && gameState.EnemyPlayer.Prestige <= 13)
        {
            strategy = new GameStrategy(cardCount, GamePhase.EarlyGame);
        }
        else
        {
            strategy = new GameStrategy(cardCount, GamePhase.MidGame);
        }

        return strategy.Heuristic(gameState);
    }
}