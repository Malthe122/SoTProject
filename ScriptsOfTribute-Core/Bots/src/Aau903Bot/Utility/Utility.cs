using ExternalHeuristic;
using ScriptsOfTribute;
using ScriptsOfTribute.Serializers;

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
                return false;
            default:
                return false;
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

    public static Node FindOrBuildNode(SeededGameState seededGameState, Node parent, List<Move> possibleMoves, MCTSHyperparameters parameters)
    {
        var result = new Node(seededGameState, parent, possibleMoves, parameters);

        if (result.Params.REUSE_TREE)
        {

            if (Aau903Bot.NodeGameStateHashMap.ContainsKey(result.GameStateHash))
            {
                var equalNode = Aau903Bot.NodeGameStateHashMap[result.GameStateHash].SingleOrDefault(node => node.GameState.IsIdentical(result.GameState));
                if (equalNode != null)
                {
                    result = equalNode;
                }
                else
                {
                    Aau903Bot.NodeGameStateHashMap[result.GameStateHash].Add(result);
                }
            }
            else
            {
                Aau903Bot.NodeGameStateHashMap.Add(result.GameStateHash, new List<Node>() { result });
            }
        }

        return result;
    }
}
