using System.Globalization;
using GeneticSharp;
using System.Globalization;

namespace Aau903Bot;

public class Chromosome : ChromosomeBase
{
    public Chromosome(
        double? iterationCompletionMillisecondsBuffer = null,
        double? uctExplorationConstant = null,
        bool? forceDelayTurnEndInRollout = null,
        bool? includePlayMoveChanceNodes = null,
        bool? includeEndTurnChanceNodes = null,
        string? chosenScoringMethod = null,
        int? rolloutTurnsBeforeHeuristic = null,
        bool? reuseTree = null) : base(8)
    {
        ReplaceGene(0, iterationCompletionMillisecondsBuffer.HasValue 
        ? new Gene(iterationCompletionMillisecondsBuffer.Value) 
        : GenerateGene(0));

        ReplaceGene(1, uctExplorationConstant.HasValue 
            ? new Gene(uctExplorationConstant.Value) 
            : GenerateGene(1));

        ReplaceGene(2, forceDelayTurnEndInRollout.HasValue 
            ? new Gene(forceDelayTurnEndInRollout.Value) 
            : GenerateGene(2));

        ReplaceGene(3, includePlayMoveChanceNodes.HasValue 
            ? new Gene(includePlayMoveChanceNodes.Value) 
            : GenerateGene(3));

        ReplaceGene(4, includeEndTurnChanceNodes.HasValue 
            ? new Gene(includeEndTurnChanceNodes.Value) 
            : GenerateGene(4));

        ReplaceGene(5, chosenScoringMethod != null 
            ? new Gene(chosenScoringMethod) 
            : GenerateGene(5));

        ReplaceGene(6, rolloutTurnsBeforeHeuristic.HasValue 
            ? new Gene(rolloutTurnsBeforeHeuristic.Value) 
            : GenerateGene(6));

        ReplaceGene(7, reuseTree.HasValue 
            ? new Gene(reuseTree.Value) 
            : GenerateGene(7));
    }

    public double ITERATION_COMPLETION_MILLISECONDS_BUFFER
    {
        get { return (double)GetGene(0).Value; }
    }

    public double UCT_EXPLORATION_CONSTANT
    {
        get { return (double)GetGene(1).Value; }
    }

    public bool FORCE_DELAY_TURN_END_IN_ROLLOUT
    {
        get { return (bool)GetGene(2).Value; }
    }

    public bool INCLUDE_PLAY_MOVE_CHANCE_NODES
    {
        get { return (bool)GetGene(3).Value; }
    }

    public bool INCLUDE_END_TURN_CHANCE_NODES
    {
        get { return (bool)GetGene(4).Value; }
    }

    public string CHOSEN_SCORING_METHOD
    {
        get { return (string)GetGene(5).Value; }
    }

    public int ROLLOUT_TURNS_BEFORE_HEURSISTIC
    {
        get { return (int)GetGene(6).Value; }
    }

    public bool REUSE_TREE
    {
        get { return (bool)GetGene(7).Value; }
    }

    public override Gene GenerateGene(int geneIndex)
    {
        //This module's Get_ is exclusive on the right
        switch (geneIndex)
        {
            case 0: //ITERATION_COMPLETION_MILLISECONDS_BUFFER
                double gene0Min = 50.0;
                double gene0Max = 500.0;
                return new Gene(Math.Round(RandomizationProvider.Current.GetDouble(gene0Min, gene0Max), 3));
            case 1: //UCT_EXPLORATION_CONSTANT
                double gene1Min = 0.5;
                double gene1Max = 4.0;
                return new Gene(Math.Round(RandomizationProvider.Current.GetDouble(gene1Min, gene1Max), 3));
            case 2: //FORCE_DELAY_TURN_END_IN_ROLLOUT
                return new Gene(RandomizationProvider.Current.GetInt(0, 2) == 1);
            case 3: //INCLUDE_PLAY_MOVE_CHANCE_NODE
                return new Gene(RandomizationProvider.Current.GetInt(0, 2) == 1);
            case 4: //INCLUDE_END_TURN_CHANCE_NODES
                return new Gene(RandomizationProvider.Current.GetInt(0, 2) == 1);
            case 5: //CHOSEN_SCORING_METHOD
                var scoringMethodTypes = new List<string> { "Rollout", "Heuristic", "RolloutTurnsCompletionsThenHeuristic" };
                return new Gene(scoringMethodTypes[RandomizationProvider.Current.GetInt(0, scoringMethodTypes.Count)]);
            case 6: //ROLLOUT_TURNS_BEFORE_HEURSISTIC
                var gene7Min = 1;
                var gene7Max = 10;
                return new Gene(RandomizationProvider.Current.GetInt(gene7Min, gene7Max + 1));
            case 7: //REUSE_TREE
                return new Gene(RandomizationProvider.Current.GetInt(0, 2) == 1);
            default:
                throw new ArgumentOutOfRangeException(nameof(geneIndex), "Invalid gene index.");
        }
    }

    public override IChromosome CreateNew()
    {
        return new Chromosome();
    }

    public override string ToString()
    {
        return @$"
            ITERATION_COMPLETION_MILLISECONDS_BUFFER={ITERATION_COMPLETION_MILLISECONDS_BUFFER}
            UCT_EXPLORATION_CONSTANT={UCT_EXPLORATION_CONSTANT}
            FORCE_DELAY_TURN_END_IN_ROLLOUT={FORCE_DELAY_TURN_END_IN_ROLLOUT}
            INCLUDE_PLAY_MOVE_CHANCE_NODES={INCLUDE_PLAY_MOVE_CHANCE_NODES}
            INCLUDE_END_TURN_CHANCE_NODES={INCLUDE_END_TURN_CHANCE_NODES}
            CHOSEN_SCORING_METHOD={CHOSEN_SCORING_METHOD}
            ROLLOUT_TURNS_BEFORE_HEURSISTIC={ROLLOUT_TURNS_BEFORE_HEURSISTIC}
            REUSE_TREE={REUSE_TREE}
            ";
    }

    public void SaveGenes(string fileName)
    {
        var data = new Dictionary<string, string>
        {
            {"ITERATION_COMPLETION_MILLISECONDS_BUFFER", ITERATION_COMPLETION_MILLISECONDS_BUFFER.ToString(CultureInfo.InvariantCulture)},
            {"UCT_EXPLORATION_CONSTANT", UCT_EXPLORATION_CONSTANT.ToString(CultureInfo.InvariantCulture)},
            {"FORCE_DELAY_TURN_END_IN_ROLLOUT", FORCE_DELAY_TURN_END_IN_ROLLOUT.ToString(CultureInfo.InvariantCulture)},
            {"INCLUDE_PLAY_MOVE_CHANCE_NODES", INCLUDE_PLAY_MOVE_CHANCE_NODES.ToString(CultureInfo.InvariantCulture)},
            {"INCLUDE_END_TURN_CHANCE_NODES", INCLUDE_END_TURN_CHANCE_NODES.ToString(CultureInfo.InvariantCulture)},
            {"CHOSEN_SCORING_METHOD", CHOSEN_SCORING_METHOD},
            {"ROLLOUT_TURNS_BEFORE_HEURSISTIC", ROLLOUT_TURNS_BEFORE_HEURSISTIC.ToString(CultureInfo.InvariantCulture)},
            {"REUSE_TREE", REUSE_TREE.ToString(CultureInfo.InvariantCulture)},
        };
        Settings.SaveEnvFile(fileName, data);
    }
}
