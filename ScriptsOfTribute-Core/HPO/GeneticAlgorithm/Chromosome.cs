using GeneticSharp;

namespace Aau903Bot;

public class Chromosome : ChromosomeBase
{
    public Chromosome() : base(8)
    {
        ReplaceGene(0, GenerateGene(0));
        ReplaceGene(1, GenerateGene(1));
        ReplaceGene(2, GenerateGene(2));
        ReplaceGene(3, GenerateGene(3));
        ReplaceGene(4, GenerateGene(4));
        ReplaceGene(5, GenerateGene(5));
        ReplaceGene(6, GenerateGene(6));
        ReplaceGene(7, GenerateGene(7));
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

    public int CHOSEN_MAX_EXPANSION_DEPTH
    {
        get { return (int)GetGene(5).Value; }
    }

    public string CHOSEN_SCORING_METHOD
    {
        get { return (string)GetGene(6).Value; }
    }

    public int ROLLOUT_TURNS_BEFORE_HEURSISTIC
    {
        get { return (int)GetGene(7).Value; }
    }

    public override Gene GenerateGene(int geneIndex)
    {
        //This module's Get_ is exclusive on the right
        switch (geneIndex)
        {
            case 0:
                double gene0Min = 50.0;
                double gene0Max = 150.0;
                return new Gene(Math.Round(RandomizationProvider.Current.GetDouble(gene0Min, gene0Max), 3));
            case 1:
                double gene1Min = 0.5;
                double gene1Max = 2.0; // Might want to just set it to root2
                return new Gene(Math.Round(RandomizationProvider.Current.GetDouble(gene1Min, gene1Max), 3));
            case 2:
                return new Gene(RandomizationProvider.Current.GetInt(0, 2) == 1);
            case 3:
                return new Gene(RandomizationProvider.Current.GetInt(0, 2) == 1);
            case 4:
                return new Gene(RandomizationProvider.Current.GetInt(0, 2) == 1);
            case 5:
                int gene5Min = 1;
                int gene5Max = 10; // Maybe this has to be increased
                return new Gene(RandomizationProvider.Current.GetInt(gene5Min, gene5Max + 1));
            case 6:
                var scoringMethodTypes = new List<string> { "Rollout", "Heuristic", "RolloutTurnsCompletionsThenHeuristic" };
                return new Gene(scoringMethodTypes[RandomizationProvider.Current.GetInt(0, scoringMethodTypes.Count)]);
            case 7:
                var gene7Min = 1;
                var gene7Max = 5;
                return new Gene(RandomizationProvider.Current.GetInt(gene7Min, gene7Max + 1));
            default:
                throw new ArgumentOutOfRangeException(nameof(geneIndex), "Invalid gene index.");
        }
    }

    public override IChromosome CreateNew()
    {
        return new Chromosome();
    }
}
