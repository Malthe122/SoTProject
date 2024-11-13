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

    public static int QuickHash(SeededGameState state)
    {
        int handHash = 0;
        foreach (var currCard in state.CurrentPlayer.Hand)
        {
            handHash *= 1 * (int)currCard.CommonId;
        }

        int tavernHash = 0;
        foreach (var currCard in state.TavernAvailableCards)
        {
            tavernHash *= 2 * ((int)currCard.CommonId);
        }
        foreach (var currCard in state.TavernCards)
        {
            tavernHash *= 3 * ((int)currCard.CommonId);
        }

        int cooldownHash = 0;
        foreach (var currCard in state.CurrentPlayer.CooldownPile)
        {
            cooldownHash *= 4 * ((int)currCard.CommonId);
        }
        foreach (var currCard in state.EnemyPlayer.CooldownPile)
        {
            cooldownHash *= 5 * ((int)currCard.CommonId);
        }

        int upcomingDrawsHash = 0;
        foreach (var currCard in state.CurrentPlayer.KnownUpcomingDraws)
        {
            upcomingDrawsHash *= 6 * ((int)currCard.CommonId);
        }
        foreach (var currCard in state.EnemyPlayer.KnownUpcomingDraws)
        {
            cooldownHash *= 7 * ((int)currCard.CommonId);
        }

        int drawPileHash = 0;
        foreach (var currCard in state.CurrentPlayer.DrawPile)
        {
            drawPileHash *= 8 * ((int)currCard.CommonId);
        }
        foreach (var currCard in state.EnemyPlayer.DrawPile)
        {
            drawPileHash *= 9 * ((int)currCard.CommonId);
        }

        int commingEffectsHash = 0; //TODO

        int resourceHash = state.CurrentPlayer.Coins * 10 + state.CurrentPlayer.Prestige * 11 + state.CurrentPlayer.Power * 12 + state.EnemyPlayer.Prestige * 13;

        int agentHash = 0;

        foreach (var currAgent in state.CurrentPlayer.Agents)
        {
            agentHash *= 14 * (currAgent.Activated ? 2 : 3);
            agentHash *= 15 * currAgent.CurrentHp;
        }

        foreach (var currAgent in state.EnemyPlayer.Agents)
        {
            agentHash *= 16 * (currAgent.Activated ? 2 : 3);
            agentHash *= 17 * currAgent.CurrentHp;
        }

        // TODO patron hash
        int patronHash = 0;
        // TODO pending choice hash
        int pendingChoiceHash = 0;

        return handHash + tavernHash + cooldownHash + upcomingDrawsHash + drawPileHash + commingEffectsHash + resourceHash + agentHash + patronHash + pendingChoiceHash;
    }

    public static int PreciseHash(SeededGameState state)
    {
        string hashString = "";
        // Decks
        var hand = string.Join("", state.CurrentPlayer.Hand.Select(card => (int)card.CommonId));
        var tavern = string.Join("", state.TavernAvailableCards.Select(card => (int)card.CommonId));
        var tavernCards = string.Join("", state.TavernCards.Select(card => (int)card.CommonId));
        var draw = string.Join("", state.TavernCards.Select(card => (int)card.CommonId));
        var player1Cooldown = string.Join("", state.CurrentPlayer.CooldownPile.Select(card => (int)card.CommonId));
        var player2Cooldown = string.Join("", state.EnemyPlayer.CooldownPile.Select(card => (int)card.CommonId));
        var player1UpcomingDraws = string.Join("", state.CurrentPlayer.KnownUpcomingDraws.Select(card => (int)card.CommonId));
        var player2UpcomingDraws = string.Join("", state.EnemyPlayer.KnownUpcomingDraws.Select(card => (int)card.CommonId));
        // Agents
        var player1Agents = string.Join("", state.TavernCards.Select(card => (int)card.CommonId));
        var player2Agents = string.Join("", state.TavernCards.Select(card => (int)card.CommonId));
        // Resources
        var player1Coins = string.Join("", state.CurrentPlayer.Coins);
        var player2Coins = string.Join("", state.EnemyPlayer.Coins);
        var player1Prestige = string.Join("", state.CurrentPlayer.Prestige);
        var player2Prestige = string.Join("", state.EnemyPlayer.Prestige);
        var player1Power = string.Join("", state.CurrentPlayer.Power);
        var player2Power = string.Join("", state.EnemyPlayer.Power);
        // Patrons
        var patronStates = string.Join("", state.Patrons.Select((patron, state) => (int)patron + (int)state));
        hashString =
            hand + tavern + tavernCards + draw + player1Cooldown + player2Cooldown +
            player1UpcomingDraws + player2UpcomingDraws + player1Agents + player2Agents +
            player1Coins + player2Coins + player1Prestige + player2Prestige + player1Power + player2Power +
            patronStates;

        var xxHash = xxHashFactory.Instance.Create();
        byte[] hashBytes = xxHash.ComputeHash(Encoding.UTF8.GetBytes(hashString)).Hash;
        return BitConverter.ToInt32(hashBytes, 0) & 0x7FFFFFFF; // ensures positive int
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

    public static double UseBestMCTS3Heuristic(SeededGameState gameState) {
        
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