using System.Linq.Expressions;
using ScriptsOfTribute;
using ScriptsOfTribute.Serializers;

public static class Utility {
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


    public static int GenerateHash(this SeededGameState state){

        // Here i chose to do a quick "hash" code to save performance, meaning we can run more iterations. I view the likelyhood of 2 unequal states counting as equal
        // even with this basic method is extremely low and should it happen the loss in evaluation precision also being minor compared to how much we can gain by running 
        // more iterations. I added it as an option though in case we change our minds
        switch(MCTSSettings.CHOSEN_HASH_GENERATION_TYPE){
            case HashGenerationType.Quick:
                int handHash = 0;
                foreach(var currCard in state.CurrentPlayer.Hand){
                    handHash *= 1 * (int)currCard.CommonId;
                }

                int tavernHash = 0;
                foreach(var currCard in state.TavernAvailableCards){
                    tavernHash *= 2 * ((int)currCard.CommonId);
                }
                foreach(var currCard in state.TavernCards){
                    tavernHash *= 3 * ((int)currCard.CommonId);
                }

                int cooldownHash = 0;
                foreach(var currCard in state.CurrentPlayer.CooldownPile){
                    cooldownHash *= 4 * ((int)currCard.CommonId);
                }
                foreach(var currCard in state.EnemyPlayer.CooldownPile){
                    cooldownHash *= 5 * ((int)currCard.CommonId);
                }

                int upcomingDrawsHash = 0;
                foreach(var currCard in state.CurrentPlayer.KnownUpcomingDraws){
                    upcomingDrawsHash *= 6 * ((int)currCard.CommonId);
                }
                foreach(var currCard in state.EnemyPlayer.KnownUpcomingDraws){
                    cooldownHash *= 7 * ((int)currCard.CommonId);
                }

                int drawPileHash = 0;
                foreach(var currCard in state.CurrentPlayer.DrawPile){
                    drawPileHash *= 8 * ((int)currCard.CommonId);
                }
                foreach(var currCard in state.EnemyPlayer.DrawPile){
                    drawPileHash *= 9 * ((int)currCard.CommonId);
                }

                int commingEffectsHash = 0; //TODO

                int resourceHash = state.CurrentPlayer.Coins * 10 + state.CurrentPlayer.Prestige * 11 + state.CurrentPlayer.Power * 12 + state.EnemyPlayer.Prestige * 13;

                int agentHash = 0;

                foreach(var currAgent in state.CurrentPlayer.Agents) {
                    agentHash *= 14 * (currAgent.Activated ? 2 : 3);
                    agentHash *= 15 * currAgent.CurrentHp;
                }

                foreach(var currAgent in state.EnemyPlayer.Agents){
                   agentHash *= 16 * (currAgent.Activated ? 2 : 3);
                   agentHash *= 17 * currAgent.CurrentHp; 
                }

                // TODO patron hash
                int patronHash = 0;
                // TODO pending choice hash
                int pendingChoiceHash = 0;


                // return HashCode.Combine(handHash, tavernHash, cooldownHash, upcomingDrawsHash, drawPileHash, commingEffectsHash, resourceHash, agentHash, patronHash, pendingChoiceHash);
                return handHash + tavernHash + cooldownHash + upcomingDrawsHash + drawPileHash + commingEffectsHash + resourceHash + agentHash + patronHash + pendingChoiceHash;
            case HashGenerationType.Precise:
            //TODO implement
            throw new NotImplementedException(); 
        }
    }
    
}