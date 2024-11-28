using System.Diagnostics;
using ScriptsOfTribute;
using ScriptsOfTribute.Board.Cards;
using ScriptsOfTribute.Serializers;

public static class IsIdenticalExtensions{
        public static bool IsIdentical(this SeededGameState instance, SeededGameState other, string info) {
        
        var timer = new Stopwatch();
        timer.Start();
            
            bool bool1 = false, bool2 = false, bool3 = false, bool4 = false, bool5 = false, bool6 = false, bool7 = false, bool8 = false, bool9 = false, bool10 = false, bool11 = false;
            bool bool12 = false, bool13 = false, bool14 = false, bool15 = false, bool16 = false, bool17 = false, bool18 = false, bool19 = false, bool20 = false, bool21 = false;
            bool bool22 = false, bool23 = false, bool24 = false, bool25 = false, bool26 = false, bool27 = false, bool28 = false;

            try { bool1 = instance.ComboStates.IsIdentical(other.ComboStates); }
            catch (Exception ex) { Console.WriteLine($"Exception in ComboStates.IsIdentical: {ex}"); }

            try { bool2 = instance.CurrentPlayer.Agents.IsIdentical(other.CurrentPlayer.Agents); }
            catch (Exception ex) { Console.WriteLine($"Exception in CurrentPlayer.Agents.IsIdentical: {ex}"); }

            try { bool3 = instance.CurrentPlayer.Coins == other.CurrentPlayer.Coins; }
            catch (Exception ex) { Console.WriteLine($"Exception in CurrentPlayer.Coins comparison: {ex}"); }

            try { bool4 = instance.CurrentPlayer.CooldownPile.IsIdentical(other.CurrentPlayer.CooldownPile); }
            catch (Exception ex) { Console.WriteLine($"Exception in CurrentPlayer.CooldownPile.IsIdentical: {ex}"); }

            try { bool5 = instance.CurrentPlayer.DrawPile.IsIdentical(other.CurrentPlayer.DrawPile); }
            catch (Exception ex) { Console.WriteLine($"Exception in CurrentPlayer.DrawPile.IsIdentical: {ex}"); }

            try { bool6 = instance.CurrentPlayer.Hand.IsIdentical(other.CurrentPlayer.Hand); }
            catch (Exception ex) { Console.WriteLine($"Exception in CurrentPlayer.Hand.IsIdentical: {ex}"); }

            try { bool7 = instance.CurrentPlayer.KnownUpcomingDraws.IsIdentical(other.CurrentPlayer.KnownUpcomingDraws); }
            catch (Exception ex) { Console.WriteLine($"Exception in CurrentPlayer.KnownUpcomingDraws.IsIdentical: {ex}"); }

            try { bool8 = instance.CurrentPlayer.PatronCalls == other.CurrentPlayer.PatronCalls; }
            catch (Exception ex) { Console.WriteLine($"Exception in CurrentPlayer.PatronCalls comparison: {ex}"); }

            try { bool9 = instance.CurrentPlayer.Played.IsIdentical(other.CurrentPlayer.Played); }
            catch (Exception ex) { Console.WriteLine($"Exception in CurrentPlayer.Played.IsIdentical: {ex}"); }

            try { bool10 = instance.CurrentPlayer.Power == other.CurrentPlayer.Power; }
            catch (Exception ex) { Console.WriteLine($"Exception in CurrentPlayer.Power comparison: {ex}"); }

            try { bool11 = instance.CurrentPlayer.Prestige == other.CurrentPlayer.Prestige; }
            catch (Exception ex) { Console.WriteLine($"Exception in CurrentPlayer.Prestige comparison: {ex}"); }

            try { bool12 = instance.EnemyPlayer.Agents.IsIdentical(other.EnemyPlayer.Agents); }
            catch (Exception ex) { Console.WriteLine($"Exception in EnemyPlayer.Agents.IsIdentical: {ex}"); }

            try { bool13 = instance.EnemyPlayer.Coins == other.EnemyPlayer.Coins; }
            catch (Exception ex) { Console.WriteLine($"Exception in EnemyPlayer.Coins comparison: {ex}"); }

            try { bool14 = instance.EnemyPlayer.CooldownPile.IsIdentical(other.EnemyPlayer.CooldownPile); }
            catch (Exception ex) { Console.WriteLine($"Exception in EnemyPlayer.CooldownPile.IsIdentical: {ex}"); }

            try { bool15 = instance.EnemyPlayer.DrawPile.IsIdentical(other.EnemyPlayer.DrawPile); }
            catch (Exception ex) { Console.WriteLine($"Exception in EnemyPlayer.DrawPile.IsIdentical: {ex}"); }

            try { bool16 = instance.EnemyPlayer.Hand.IsIdentical(other.EnemyPlayer.Hand); }
            catch (Exception ex) { Console.WriteLine($"Exception in EnemyPlayer.Hand.IsIdentical: {ex}"); }

            try { bool17 = instance.EnemyPlayer.KnownUpcomingDraws.IsIdentical(other.EnemyPlayer.KnownUpcomingDraws); }
            catch (Exception ex) { Console.WriteLine($"Exception in EnemyPlayer.KnownUpcomingDraws.IsIdentical: {ex}"); }

            try { bool18 = instance.EnemyPlayer.PatronCalls == instance.EnemyPlayer.PatronCalls; }
            catch (Exception ex) { Console.WriteLine($"Exception in EnemyPlayer.PatronCalls comparison: {ex}"); }

            try { bool19 = instance.EnemyPlayer.Played.IsIdentical(other.EnemyPlayer.Played); }
            catch (Exception ex) { Console.WriteLine($"Exception in EnemyPlayer.Played.IsIdentical: {ex}"); }

            try { bool20 = instance.EnemyPlayer.Power == instance.EnemyPlayer.Power; }
            catch (Exception ex) { Console.WriteLine($"Exception in EnemyPlayer.Power comparison: {ex}"); }

            try { bool21 = instance.EnemyPlayer.Prestige == instance.EnemyPlayer.Prestige; }
            catch (Exception ex) { Console.WriteLine($"Exception in EnemyPlayer.Prestige comparison: {ex}"); }

            try { bool22 = instance.Patrons.IsIdentical(other.Patrons); }
            catch (Exception ex) { Console.WriteLine($"Exception in Patrons.IsIdentical: {ex}"); }

            try { bool23 = instance.PatronStates.IsIdentical(other.PatronStates); }
            catch (Exception ex) { Console.WriteLine($"Exception in PatronStates.IsIdentical: {ex}"); }

            try { bool24 = instance.PendingChoice.IsIdentical(other.PendingChoice); }
            catch (Exception ex) { Console.WriteLine($"Exception in PendingChoice.IsIdentical: {ex}"); }

            try { bool25 = instance.StartOfNextTurnEffects.IsIdentical(other.StartOfNextTurnEffects); }
            catch (Exception ex) { Console.WriteLine($"Exception in StartOfNextTurnEffects.IsIdentical: {ex}"); }

            try { bool26 = instance.TavernAvailableCards.IsIdentical(other.TavernAvailableCards); }
            catch (Exception ex) { Console.WriteLine($"Exception in TavernAvailableCards.IsIdentical: {ex}"); }

            try { bool27 = instance.TavernCards.IsIdentical(other.TavernCards); }
            catch (Exception ex) { Console.WriteLine($"Exception in TavernCards.IsIdentical: {ex}"); }

            try { bool28 = instance.UpcomingEffects.IsIdentical(other.UpcomingEffects); }
            catch (Exception ex) { Console.WriteLine($"Exception in UpcomingEffects.IsIdentical: {ex}"); }



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
            if (!bool1) Console.WriteLine("Bool1: " + bool1);
            if (!bool2) Console.WriteLine("Bool2: " + bool2);
            if (!bool3) Console.WriteLine("Bool3: " + bool3);
            if (!bool4) Console.WriteLine("Bool4: " + bool4);
            if (!bool5) Console.WriteLine("Bool5: " + bool5);
            if (!bool6) Console.WriteLine("Bool6: " + bool6);
            if (!bool7) Console.WriteLine("Bool7: " + bool7);
            if (!bool8) Console.WriteLine("Bool8: " + bool8);
            if (!bool9) Console.WriteLine("Bool9: " + bool9);
            if (!bool10) Console.WriteLine("Bool10: " + bool10);
            if (!bool11) Console.WriteLine("Bool11: " + bool11);
            if (!bool12) Console.WriteLine("Bool12: " + bool12);
            if (!bool13) Console.WriteLine("Bool13: " + bool13);
            if (!bool14) Console.WriteLine("Bool14: " + bool14);
            if (!bool15) Console.WriteLine("Bool15: " + bool15);
            if (!bool16) Console.WriteLine("Bool16: " + bool16);
            if (!bool17) Console.WriteLine("Bool17: " + bool17);
            if (!bool18) Console.WriteLine("Bool18: " + bool18);
            if (!bool19) Console.WriteLine("Bool19: " + bool19);
            if (!bool20) Console.WriteLine("Bool20: " + bool20);
            if (!bool21) Console.WriteLine("Bool21: " + bool21);
            if (!bool22) Console.WriteLine("Bool22: " + bool22);
            if (!bool23) Console.WriteLine("Bool23: " + bool23);
            if (!bool24) Console.WriteLine("Bool24: " + bool24);
            if (!bool25) Console.WriteLine("Bool25: " + bool25);
            if (!bool26) Console.WriteLine("Bool26: " + bool26);
            if (!bool27) Console.WriteLine("Bool27: " + bool27);
            if (!bool28) Console.WriteLine("Bool28: " + bool28);
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
                &&  ((instance.Type == Choice.DataType.CARD && instance.PossibleCards.IsIdentical(other.PossibleCards)) || (instance.Type == Choice.DataType.EFFECT && instance.PossibleEffects.IsIdentical(other.PossibleEffects)))
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

        if(instance.Count != other.Count) {
            return false;
        }

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

        if (instance.All.Keys.OrderBy(patronId => patronId).ToList().SequenceEqual(other.All.Keys.OrderBy(patronId => patronId))){
            return false;
        }

        foreach(var patron in other.All.Keys) {
            if (instance.All[patron] != other.All[patron]) {
                return false;
            }
        }

        return true;
    }
}