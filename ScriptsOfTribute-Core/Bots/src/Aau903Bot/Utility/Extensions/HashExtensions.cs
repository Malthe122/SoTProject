using System.Diagnostics;
using ScriptsOfTribute;
using ScriptsOfTribute.Board.Cards;
using ScriptsOfTribute.Serializers;

public static class HashExtensions
{
    public static int GenerateHash(this SeededGameState state)
    {
        var timer = new Stopwatch();
        timer.Start();

        var hashCode = new HashCode();

        foreach (var comboState in state.ComboStates.All)
        {
            hashCode.Add(((int)comboState.Key) * 10);
            hashCode.Add(comboState.Value.CurrentCombo * 2);
        }

        hashCode.Add(state.CurrentPlayer.GenerateHash());
        hashCode.Add(state.EnemyPlayer.GenerateHash());

        foreach (var currPatron in state.Patrons)
        {
            hashCode.Add(((int)currPatron) * 10);
        }

        foreach (var patronState in state.PatronStates.All)
        {
            hashCode.Add(((int)patronState.Key) * 200);
            hashCode.Add(patronState.Value);
        }

        if (state.PendingChoice != null)
        {
            hashCode.Add((int)state.PendingChoice.ChoiceFollowUp * 100);
            hashCode.Add(state.PendingChoice.MaxChoices);
            hashCode.Add(state.PendingChoice.MinChoices);
            hashCode.Add(state.PendingChoice.Type);

            switch (state.PendingChoice.Type)
            {
                case Choice.DataType.CARD:
                    foreach (var currCard in state.PendingChoice.PossibleCards)
                    {
                        hashCode.Add((int)currCard.UniqueId * 100);
                    }
                    break;
                case Choice.DataType.EFFECT:
                    foreach (var currEffect in state.PendingChoice.PossibleEffects)
                    {
                        hashCode.Add(currEffect.Amount);
                        hashCode.Add(currEffect.Combo);
                        hashCode.Add(currEffect.Type);
                        hashCode.Add(currEffect.ParentCard.CommonId);
                    }
                    break;
            }
        }

        foreach (var currEffect in state.StartOfNextTurnEffects)
        {
            hashCode.Add(currEffect.GenerateHash());
        }

        foreach (var currCard in state.TavernAvailableCards)
        {
            hashCode.Add((int)currCard.CommonId);
        }

        foreach (var currCard in state.TavernCards)
        {
            hashCode.Add((int)currCard.CommonId * 100);
        }

        foreach (var currEffect in state.UpcomingEffects)
        {
            hashCode.Add(currEffect.GenerateHash());
        }

        int result = hashCode.ToHashCode();

        timer.Stop();
        // Console.WriteLine("Hash generation took: " + timer.ElapsedMilliseconds + " seconds");

        return result;
    }

    public static int GenerateHash(this UniqueBaseEffect effect)
    {

        var hashCode = new HashCode();

        var uniqueEffect = effect as UniqueEffect;
        var uniqueEffectOr = effect as UniqueEffectOr;
        var uniqueEffectComposite = effect as UniqueEffectComposite;

        if (uniqueEffect != null)
        {
            hashCode.Add(uniqueEffect.Amount);
            hashCode.Add(uniqueEffect.Combo * 100);
            hashCode.Add(((int)uniqueEffect.ParentCard.UniqueId) * 1_000);
        }
        else if (uniqueEffectOr != null)
        {
            hashCode.Add(uniqueEffectOr.Combo * 10_000);
            hashCode.Add(((int)uniqueEffectOr.ParentCard.CommonId) * 100_000);
        }
        else if (uniqueEffectComposite != null)
        {
            hashCode.Add(((int)uniqueEffectComposite.ParentCard.CommonId) * 1_000_000);
        }

        return hashCode.ToHashCode();
    }

    public static int GenerateHash(this SerializedPlayer player)
    {

        var hashCode = new HashCode();

        foreach (var currAgent in player.Agents)
        {
            hashCode.Add(currAgent.Activated);
            hashCode.Add(currAgent.CurrentHp);
            hashCode.Add(currAgent.RepresentingCard.CommonId);
        }

        hashCode.Add(player.Coins);

        foreach (var currCard in player.CooldownPile)
        {
            hashCode.Add(currCard.CommonId);
        }

        foreach (var currCard in player.DrawPile)
        {
            hashCode.Add(((int)currCard.CommonId) * 1000);
        }

        foreach (var currCard in player.KnownUpcomingDraws)
        {
            hashCode.Add(((int)currCard.CommonId) * 1_000_000);
        }

        hashCode.Add(player.PatronCalls);

        foreach (var currCard in player.Played)
        {
            hashCode.Add(currCard.CommonId);
        }

        hashCode.Add(player.Power);
        hashCode.Add(player.Prestige);

        return hashCode.ToHashCode();
    }
}