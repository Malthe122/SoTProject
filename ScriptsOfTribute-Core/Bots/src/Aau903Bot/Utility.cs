using System.Linq.Expressions;
using ScriptsOfTribute;
using ScriptsOfTribute.Serializers;

public static class Utility {
    // TODO probably move these to some MCTS setting static class or something as these are the ones we want to try to modify when benchmarking
    public const EvaluationFunction CHOSEN_EVALUATION_FUNCTION = EvaluationFunction.Custom;
    public const int NUMBER_OF_ROLLOUTS = 10; //Set low for initial tests
    /// <summary>
    /// Tells in the rollout whether the agents plays all their possible moves before ending turn or if end turn is an allowed move in any part of their turn
    /// Idea is that setting this to true will first of all be closer to a realistic simulation and also it should end the game quicker, making the simulation
    /// faster than if agents were allowed to spend moves ending turns without really doing anything in the game
    /// </summary>
    public const bool FORCE_DELAY_TURN_END_IN_ROLLOUT = true;

    public static Random Rng = new Random();

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


    public static int GenerateHash(this SeededGameState seededGameState){
    //TODO implement
    throw new NotImplementedException();
    }
    
}