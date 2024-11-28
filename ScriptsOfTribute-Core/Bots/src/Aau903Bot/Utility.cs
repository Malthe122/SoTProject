using System.Diagnostics;
using ExternalHeuristic;
using ScriptsOfTribute;
using ScriptsOfTribute.Board.Cards;
using ScriptsOfTribute.Serializers;

public static class Utility
{
    public static Random Rng = new Random();

    public static Dictionary<int,List<Node>> NodeGameStateHashMap = new Dictionary<int, List<Node>>();

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

    // public static int GenerateHash(this SeededGameState state)
    // {

    //     // Here i chose to do a quick "hash" code to save performance, meaning we can run more iterations. I view the likelyhood of 2 unequal states counting as equal being extremely low
    //     // even with this basic method is extremely low and should it happen the loss in evaluation precision also being minor compared to how much we can gain by running 
    //     // more iterations. I added it as an option though in case we change our minds
    //     switch (MCTSHyperparameters.CHOSEN_HASH_GENERATION_TYPE)
    //     {
    //         case HashGenerationType.Quick:
    //             int handHash = 1;
    //             foreach (var currCard in state.CurrentPlayer.Hand)
    //             {
    //                 handHash *= 2 * (int)currCard.CommonId;
    //             }

    //             int tavernHash = 1;
    //             foreach (var currCard in state.TavernAvailableCards)
    //             {
    //                 tavernHash *= 2 * ((int)currCard.CommonId);
    //             }
    //             foreach (var currCard in state.TavernCards)
    //             {
    //                 tavernHash *= 3 * ((int)currCard.CommonId);
    //             }

    //             int cooldownHash = 1;
    //             foreach (var currCard in state.CurrentPlayer.CooldownPile)
    //             {
    //                 cooldownHash *= 4 * ((int)currCard.CommonId);
    //             }
    //             foreach (var currCard in state.EnemyPlayer.CooldownPile)
    //             {
    //                 cooldownHash *= 5 * ((int)currCard.CommonId);
    //             }

    //             int upcomingDrawsHash = 1;
    //             foreach (var currCard in state.CurrentPlayer.KnownUpcomingDraws)
    //             {
    //                 upcomingDrawsHash *= 6 * ((int)currCard.CommonId);
    //             }
    //             foreach (var currCard in state.EnemyPlayer.KnownUpcomingDraws)
    //             {
    //                 cooldownHash *= 7 * ((int)currCard.CommonId);
    //             }

    //             int drawPileHash = 1;
    //             foreach (var currCard in state.CurrentPlayer.DrawPile)
    //             {
    //                 drawPileHash *= 8 * ((int)currCard.CommonId);
    //             }
    //             foreach (var currCard in state.EnemyPlayer.DrawPile)
    //             {
    //                 drawPileHash *= 9 * ((int)currCard.CommonId);
    //             }

    //             int commingEffectsHash = 1; //TODO

    //             int resourceHash = state.CurrentPlayer.Coins * 10 + state.CurrentPlayer.Prestige * 11 + state.CurrentPlayer.Power * 12 + state.EnemyPlayer.Prestige * 13;

    //             int agentHash = 1;

    //             foreach (var currAgent in state.CurrentPlayer.Agents)
    //             {
    //                 agentHash *= 14 * (currAgent.Activated ? 2 : 3);
    //                 agentHash *= 15 * currAgent.CurrentHp;
    //             }

    //             foreach (var currAgent in state.EnemyPlayer.Agents)
    //             {
    //                 agentHash *= 16 * (currAgent.Activated ? 2 : 3);
    //                 agentHash *= 17 * currAgent.CurrentHp;
    //             }

    //             // TODO patron hash
    //             int patronHash = 1;
    //             // TODO pending choice hash
    //             int pendingChoiceHash = 1;

    //             return handHash + tavernHash + cooldownHash + upcomingDrawsHash + drawPileHash + commingEffectsHash + resourceHash + agentHash + patronHash + pendingChoiceHash;
    //         case HashGenerationType.Precise:
    //             //TODO implement
    //             throw new NotImplementedException();
    //         default:
    //             throw new NotImplementedException();
    //     }
    // }

    public static int GenerateHash(this SeededGameState state) {
        var timer = new Stopwatch();
        timer.Start();

        var hashCode = new HashCode();

        foreach(var comboState in state.ComboStates.All) {
            hashCode.Add(((int)comboState.Key) * 10);
            hashCode.Add(comboState.Value.CurrentCombo * 2);
        }

        hashCode.Add(state.CurrentPlayer.GenerateHash());
        hashCode.Add(state.EnemyPlayer.GenerateHash());

        foreach(var currPatron in state.Patrons) {
            hashCode.Add(((int)currPatron) * 10);
        }

        foreach(var patronState in state.PatronStates.All) {
            hashCode.Add(((int)patronState.Key) * 200);
            hashCode.Add(patronState.Value);
        }

        if (state.PendingChoice != null) {
            hashCode.Add((int)state.PendingChoice.ChoiceFollowUp * 100);
            hashCode.Add(state.PendingChoice.MaxChoices);
            hashCode.Add(state.PendingChoice.MinChoices);
            hashCode.Add(state.PendingChoice.Type);

            switch(state.PendingChoice.Type) {
                case Choice.DataType.CARD:
                    foreach(var currCard in state.PendingChoice.PossibleCards) {
                        hashCode.Add((int)currCard.UniqueId * 100);
                    }
                    break;
                case Choice.DataType.EFFECT:
                    foreach(var currEffect in state.PendingChoice.PossibleEffects) {
                        hashCode.Add(currEffect.Amount);
                        hashCode.Add(currEffect.Combo);
                        hashCode.Add(currEffect.Type);
                        hashCode.Add(currEffect.ParentCard.CommonId);
                    }
                    break;
            }
        }

        foreach(var currEffect in state.StartOfNextTurnEffects){
            hashCode.Add(currEffect.GenerateHash());
        }

        foreach(var currCard in state.TavernAvailableCards) {
            hashCode.Add((int)currCard.CommonId);
        }

        foreach(var currCard in state.TavernCards) {
            hashCode.Add((int)currCard.CommonId * 100);
        }

        foreach(var currEffect in state.UpcomingEffects){
            hashCode.Add(currEffect.GenerateHash());
        }

        int result = hashCode.ToHashCode();

        timer.Stop();
        // Console.WriteLine("Hash generation took: " + timer.ElapsedMilliseconds + " seconds");

        return result;
    }

    public static int GenerateHash(this UniqueBaseEffect effect) {
        
        var hashCode = new HashCode();

        var uniqueEffect = effect as UniqueEffect;
        var uniqueEffectOr = effect as UniqueEffectOr;
        var uniqueEffectComposite = effect as UniqueEffectComposite;

        if (uniqueEffect != null) {
            hashCode.Add(uniqueEffect.Amount);
            hashCode.Add(uniqueEffect.Combo);
            hashCode.Add(uniqueEffect.ParentCard.UniqueId);
        }
        else if (uniqueEffectOr != null) {
            hashCode.Add(uniqueEffectOr.Combo * 100);
            hashCode.Add(((int)uniqueEffectOr.ParentCard.CommonId) * 100);
        }
        else if (uniqueEffectComposite != null) {
            hashCode.Add(((int)uniqueEffectComposite.ParentCard.CommonId) * 1000);
        }
        
        return hashCode.ToHashCode();
    }

    public static int GenerateHash(this SerializedPlayer player) {
        
        var hashCode = new HashCode();

        foreach(var currAgent in player.Agents) {
            hashCode.Add(currAgent.Activated);
            hashCode.Add(currAgent.CurrentHp);
            hashCode.Add(currAgent.RepresentingCard.CommonId);
        }

        hashCode.Add(player.Coins);

        foreach(var currCard in player.CooldownPile) {
            hashCode.Add(currCard.CommonId);
        }

        foreach(var currCard in player.DrawPile) {
            hashCode.Add(((int)currCard.CommonId) * 1000);
        }

        foreach(var currCard in player.KnownUpcomingDraws) {
            hashCode.Add(((int)currCard.CommonId) * 1_000_000);
        }

        hashCode.Add(player.PatronCalls);

        foreach(var currCard in player.Played) {
            hashCode.Add(currCard.CommonId);
        }

        hashCode.Add(player.Power);
        hashCode.Add(player.Prestige);

        return hashCode.ToHashCode();
    }

    public static bool IsIdentical(this SeededGameState instance, SeededGameState other, string info) {
        
        var timer = new Stopwatch();
        timer.Start();
        var result = ( 
                instance.ComboStates.IsIdentical(other.ComboStates)
                // Current player
            &&  instance.CurrentPlayer.Agents.IsIdentical(other.CurrentPlayer.Agents)
            &&  instance.CurrentPlayer.Coins == other.CurrentPlayer.Coins
            &&  instance.CurrentPlayer.CooldownPile.IsIdentical(other.CurrentPlayer.CooldownPile)
            &&  instance.CurrentPlayer.DrawPile.IsIdentical(other.CurrentPlayer.DrawPile)
            &&  instance.CurrentPlayer.Hand.IsIdentical(other.CurrentPlayer.Hand)
            &&  instance.CurrentPlayer.KnownUpcomingDraws.IsIdentical(other.CurrentPlayer.KnownUpcomingDraws)
            &&  instance.CurrentPlayer.PatronCalls == other.CurrentPlayer.PatronCalls
            &&  instance.CurrentPlayer.Played.IsIdentical(other.CurrentPlayer.Played)
            &&  instance.CurrentPlayer.Power == other.CurrentPlayer.Power
            &&  instance.CurrentPlayer.Prestige == other.CurrentPlayer.Prestige
                // Enemy player
            &&  instance.EnemyPlayer.Agents.IsIdentical(other.EnemyPlayer.Agents)
            &&  instance.EnemyPlayer.Coins == other.EnemyPlayer.Coins
            &&  instance.EnemyPlayer.CooldownPile.IsIdentical(other.EnemyPlayer.CooldownPile)
            &&  instance.EnemyPlayer.DrawPile.IsIdentical(other.EnemyPlayer.DrawPile)
            &&  instance.EnemyPlayer.Hand.IsIdentical(other.EnemyPlayer.Hand)
            &&  instance.EnemyPlayer.KnownUpcomingDraws.IsIdentical(other.EnemyPlayer.KnownUpcomingDraws)
            &&  instance.EnemyPlayer.PatronCalls == instance.EnemyPlayer.PatronCalls
            &&  instance.EnemyPlayer.Played.IsIdentical(other.EnemyPlayer.Played)
            &&  instance.EnemyPlayer.Power == instance.EnemyPlayer.Power
            &&  instance.EnemyPlayer.Prestige == instance.EnemyPlayer.Prestige

            &&  instance.Patrons.IsIdentical(other.Patrons)
            &&  instance.PatronStates.IsIdentical(other.PatronStates)
            &&  instance.PendingChoice.IsIdentical(other.PendingChoice)
            &&  instance.StartOfNextTurnEffects.IsIdentical(other.StartOfNextTurnEffects)
            &&  instance.TavernAvailableCards.IsIdentical(other.TavernAvailableCards)
            &&  instance.TavernCards.IsIdentical(other.TavernCards)
            &&  instance.UpcomingEffects.IsIdentical(other.UpcomingEffects)
            );

        timer.Stop();
        if (!result) {
            Console.WriteLine("hash collison for unequal states: ");
            Console.WriteLine(info);
            Console.WriteLine("instance hash: " + instance.GenerateHash());
            Console.WriteLine("other hash: " + other.GenerateHash());
            Console.WriteLine();
            Console.WriteLine("INSTANCE:");
            instance.Log();
            Console.WriteLine("OTHER:");
            other.Log();
        }

        return result;
        // real code:
        // return ( 
        //         instance.ComboStates.IsIdentical(other.ComboStates)
        //         // Current player
        //     &&  instance.CurrentPlayer.Agents.IsIdentical(other.CurrentPlayer.Agents)
        //     &&  instance.CurrentPlayer.Coins == other.CurrentPlayer.Coins
        //     &&  instance.CurrentPlayer.CooldownPile.IsIdentical(other.CurrentPlayer.CooldownPile)
        //     &&  instance.CurrentPlayer.DrawPile.IsIdentical(other.CurrentPlayer.DrawPile)
        //     &&  instance.CurrentPlayer.Hand.IsIdentical(other.CurrentPlayer.Hand)
        //     &&  instance.CurrentPlayer.KnownUpcomingDraws.IsIdentical(other.CurrentPlayer.KnownUpcomingDraws)
        //     &&  instance.CurrentPlayer.PatronCalls == other.CurrentPlayer.PatronCalls
        //     &&  instance.CurrentPlayer.Played.IsIdentical(other.CurrentPlayer.Played)
        //     &&  instance.CurrentPlayer.Power == other.CurrentPlayer.Power
        //     &&  instance.CurrentPlayer.Prestige == other.CurrentPlayer.Prestige
        //         // Enemy player
        //     &&  instance.EnemyPlayer.Agents.IsIdentical(other.EnemyPlayer.Agents)
        //     &&  instance.EnemyPlayer.Coins == other.EnemyPlayer.Coins
        //     &&  instance.EnemyPlayer.CooldownPile.IsIdentical(other.EnemyPlayer.CooldownPile)
        //     &&  instance.EnemyPlayer.DrawPile.IsIdentical(other.EnemyPlayer.DrawPile)
        //     &&  instance.EnemyPlayer.Hand.IsIdentical(other.EnemyPlayer.Hand)
        //     &&  instance.EnemyPlayer.KnownUpcomingDraws.IsIdentical(other.EnemyPlayer.KnownUpcomingDraws)
        //     &&  instance.EnemyPlayer.PatronCalls == instance.EnemyPlayer.PatronCalls
        //     &&  instance.EnemyPlayer.Played.IsIdentical(other.EnemyPlayer.Played)
        //     &&  instance.EnemyPlayer.Power == instance.EnemyPlayer.Power
        //     &&  instance.EnemyPlayer.Prestige == instance.EnemyPlayer.Prestige

        //     &&  instance.Patrons.IsIdentical(other.Patrons)
        //     &&  instance.PatronStates.IsIdentical(other.PatronStates)
        //     &&  instance.PendingChoice.IsIdentical(other.PendingChoice)
        //     &&  instance.StartOfNextTurnEffects.IsIdentical(other.StartOfNextTurnEffects)
        //     &&  instance.TavernAvailableCards.IsIdentical(other.TavernAvailableCards)
        //     &&  instance.TavernCards.IsIdentical(other.TavernCards)
        //     &&  instance.UpcomingEffects.IsIdentical(other.UpcomingEffects)
        //     );
    }
    // TODO check if this will throw a nullpointer or not when instance is null
    public static bool IsIdentical(this SerializedChoice? instance, SerializedChoice? other) {
        if (instance == null) {
            if (other == null) {
                return true;
            }
            else {
                return false;
            }
        }
        else if (other == null) {
            return false;
        }
        else{
            return (
                    instance.ChoiceFollowUp == other.ChoiceFollowUp
                // &&  instance.Context == other.Context // content should not matter if you have the same options
                &&  instance.MaxChoices == other.MaxChoices
                &&  instance.MinChoices == other.MinChoices
                &&  instance.Type == other.Type
                &&  (instance.Type == Choice.DataType.CARD && instance.PossibleCards.IsIdentical(other.PossibleCards) || instance.Type == Choice.DataType.EFFECT && instance.PossibleEffects.IsIdentical(other.PossibleEffects))
            );
        }
    }

    public static bool IsIdentical(this List<UniqueBaseEffect> instance, List<UniqueBaseEffect> other) {
        List<UniqueEffect> instanceUniqueEffects = new List<UniqueEffect>();
        List<UniqueEffectOr> instanceUniqueEffectOrs = new List<UniqueEffectOr>();
        List<UniqueEffectComposite> instanceUniqueEffectComposites = new List<UniqueEffectComposite>();

        List<UniqueEffect> otherUniqueEffects = new List<UniqueEffect>();
        List<UniqueEffectOr> otherUniqueEffectOrs = new List<UniqueEffectOr>();
        List<UniqueEffectComposite> otherUniqueEffectComposites = new List<UniqueEffectComposite>();

        foreach(var currEffect in instance){
            var uniqueEffect = currEffect as UniqueEffect;
            var uniqueEffectOr = currEffect as UniqueEffectOr;
            var uniqueEffectComposite = currEffect as UniqueEffectComposite;

            if (uniqueEffect != null) {
                instanceUniqueEffects.Add(uniqueEffect);
            }
            else if (uniqueEffectOr != null) {
                instanceUniqueEffectOrs.Add(uniqueEffectOr);
            }
            else if (uniqueEffectComposite != null) {
                instanceUniqueEffectComposites.Add(uniqueEffectComposite);
            }
            else {
                throw new Exception("TROUBLE HANDLING EFFECT COMPARISON");
            }
        }

        foreach(var currEffect in other){
            var uniqueEffect = currEffect as UniqueEffect;
            var uniqueEffectOr = currEffect as UniqueEffectOr;
            var uniqueEffectComposite = currEffect as UniqueEffectComposite;

            if (uniqueEffect != null) {
                otherUniqueEffects.Add(uniqueEffect);
            }
            else if (uniqueEffectOr != null) {
                otherUniqueEffectOrs.Add(uniqueEffectOr);
            }
            else if (uniqueEffectComposite != null) {
                otherUniqueEffectComposites.Add(uniqueEffectComposite);
            }
            else {
                throw new Exception("TROUBLE HANDLING EFFECT COMPARISON");
            }
        }

        return (
                instanceUniqueEffects.IsIdentical(otherUniqueEffects)
            &&  instanceUniqueEffectOrs.IsIdentical(otherUniqueEffectOrs)
            &&  instanceUniqueEffectComposites.IsIdentical(otherUniqueEffectComposites)
        );
    }

    public static bool IsIdentical(this List<UniqueEffect> instance, List<UniqueEffect> other) {
        instance.OrderBy(effect => effect.Amount).ThenBy(effect => effect.Combo).ThenBy(effect => effect.ParentCard.CommonId).ThenBy(effect => effect.Type);
        other.OrderBy(effect => effect.Amount).ThenBy(effect => effect.Combo).ThenBy(effect => effect.ParentCard.CommonId).ThenBy(effect => effect.Type);

        if (instance.Count != other.Count) {
            return false;
        }

        for(int i = 0; i < instance.Count; i++) {
            if (
                    instance[i].Amount != other[i].Amount
                ||  instance[i].Combo != other[i].Combo
                ||  instance[i].ParentCard.CommonId != other[i].ParentCard.CommonId
                ||  instance[i].Type != other[i].Type
            ) {
                return false;
            }
        }

        return true;
    }

    public static bool IsIdentical(this List<UniqueEffectOr> instance, List<UniqueEffectOr> other) {
        instance.OrderBy(effect => effect.Combo).ThenBy(effect => effect.ParentCard.CommonId);
        other.OrderBy(effect => effect.Combo).ThenBy(effect => effect.ParentCard.CommonId);

        if (instance.Count != other.Count) {
            return false;
        }

        for(int i = 0; i < instance.Count; i++) {
            if (
                    instance[i].Combo != other[i].Combo
                ||  instance[i].ParentCard.CommonId != other[i].ParentCard.CommonId
            ) {
                return false;
            }
        }

        return true;
    }

    public static bool IsIdentical(this List<UniqueEffectComposite> instance, List<UniqueEffectComposite> other) {
        instance.OrderBy(effect => effect.ParentCard.CommonId);
        other.OrderBy(effect => effect.ParentCard.CommonId);

        if (instance.Count != other.Count) {
            return false;
        }

        for(int i = 0; i < instance.Count; i++) {
            if (instance[i].ParentCard.CommonId != other[i].ParentCard.CommonId) {
                return false;
            }
        }

        return true;
    }
    public static bool IsIdentical(this List<SerializedAgent> instance, List<SerializedAgent> other) {
        var orderedInstanceAgents = instance.OrderBy(agent => agent.RepresentingCard.CommonId).ThenBy(agent => agent.Activated).ThenBy(agent => agent.CurrentHp).ToList();
        var orderedOtherAgents = other.OrderBy(agent => agent.RepresentingCard.CommonId).ThenBy(agent => agent.Activated).ThenBy(agent => agent.CurrentHp).ToList();

        for(int i = 0; i < instance.Count; i++) {
            if (
                    orderedInstanceAgents[i].Activated != orderedOtherAgents[i].Activated
                ||  orderedInstanceAgents[i].CurrentHp != orderedOtherAgents[i].CurrentHp
                ||  orderedInstanceAgents[i].RepresentingCard.CommonId != orderedOtherAgents[i].RepresentingCard.CommonId
            ) {
                return false;
            }
        }

        return true;
    }

    public static bool IsIdentical(this ComboStates instance, ComboStates other) {
        if (instance.All.Count != other.All.Count) {
            return false;
        } 
        var instanceComboKeys = instance.All.OrderBy(state => state.Key).Select(state => state.Key);
        var otherComboKeys = other.All.OrderBy(state => state.Key).Select(state => state.Key);
        if (!instanceComboKeys.SequenceEqual(otherComboKeys)) {
            return false;
        }
        
        foreach(var currKey in instanceComboKeys) {
            // Since we also check which cards a played (which we need cause cards in played can be destroyed), we do not need to check which combo effects are ready for each patron
            // Since these will be equal if the cards played are equal
            if (instance.All[currKey].CurrentCombo != other.All[currKey].CurrentCombo) {
                return false;
            }
        }

        return true;
    }

    public static bool IsIdentical(this List<UniqueCard> instance, List<UniqueCard> other) {
        List<CardId> orderedInstanceCardIds = instance.OrderBy(card => card.CommonId).Select(card => card.CommonId).ToList();
        List<CardId> orderedOtherCardIds = other.OrderBy(card => card.CommonId).Select(card => card.CommonId).ToList();
        return orderedInstanceCardIds.SequenceEqual(orderedOtherCardIds);
    }

    public static bool IsIdentical(this List<PatronId> instance, List<PatronId> other) {
        var orderedInstancePatronIds = instance.OrderBy(id => id);
        var orderedOtherPatronIds = other.OrderBy(id => id);

        return orderedInstancePatronIds.SequenceEqual(orderedOtherPatronIds);
    }

    public static bool IsIdentical(this PatronStates instance, PatronStates other) {

        foreach(var patron in other.All.Keys) {
            if (instance.All[patron] != other.All[patron]) {
                return false;
            }
        }

        return true;
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

    public static Node FindOrBuildNode(SeededGameState seededGameState, Node parent, List<Move> possibleMoves, Move appliedMove, int depth)
    {
        var result = new Node(seededGameState, parent, possibleMoves, appliedMove, depth);

        if (NodeGameStateHashMap.ContainsKey(result.GameStateHash)){
            var equalNode = Utility.NodeGameStateHashMap[result.GameStateHash].SingleOrDefault(node => node.GameState.IsIdentical(result.GameState, "Called from Utlity"));
            if (equalNode != null){
                // Console.WriteLine("-------- Hash collison for equal states -------- ");
                result = equalNode;
            }
            else {
                Console.WriteLine("-------- Hash collison without states being equal -------- ");
                foreach (var node in NodeGameStateHashMap[result.GameStateHash])
                {
                    Console.WriteLine($"Hash: {node.GameStateHash}");
                    Console.WriteLine("Generated hash: " + node.GameState.GenerateHash());
                    Console.WriteLine("Generated hash2: " + node.GameState.GenerateHash());
                    Console.WriteLine("Generated hash3: " + node.GameState.GenerateHash());
                    Console.WriteLine("Generated hash4: " + node.GameState.GenerateHash());
                    Console.WriteLine("Generated hash5: " + node.GameState.GenerateHash());
                }
                if (result.GameStateHash != result.GameState.GenerateHash()) {
                    Console.WriteLine("WWWWWWWWWWTTTTTTTTFFFFFFFFFFFFF");
                }
                NodeGameStateHashMap[result.GameStateHash].Add(result);
            }
        }
        else{
            if (result.GameStateHash != result.GameState.GenerateHash()) {
                    Console.WriteLine("WWWWWWWWWWTTTTTTTTHHHHHHHHHHHH");
                }
            NodeGameStateHashMap.Add(result.GameStateHash, new List<Node>(){result});
        }

        return result;
    }
}