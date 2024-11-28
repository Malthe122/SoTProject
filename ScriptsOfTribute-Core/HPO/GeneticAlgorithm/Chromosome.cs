using Aau903Bot;
using GeneticSharp;

namespace Aau903Bot;

public class Chromosome : ChromosomeBase
{
    public Chromosome() : base(11)
    {
        ReplaceGene(0, GenerateGene(0));
        ReplaceGene(1, GenerateGene(1));
        ReplaceGene(2, GenerateGene(2));
        ReplaceGene(3, GenerateGene(3));
        ReplaceGene(4, GenerateGene(4));
        ReplaceGene(5, GenerateGene(5));
        ReplaceGene(6, GenerateGene(6));
        ReplaceGene(7, GenerateGene(7));
        ReplaceGene(8, GenerateGene(8));
        ReplaceGene(9, GenerateGene(9));
        ReplaceGene(10, GenerateGene(10));
    }

    public double ITERATION_COMPLETION_MILLISECONDS_BUFFER
    {
        get { return (double)GetGene(0).Value; }
    }

    public double UCB1_EXPLORATION_CONSTANT
    {
        get { return (double)GetGene(1).Value; }
    }

    public int NUMBER_OF_ROLLOUTS
    {
        get { return (int)GetGene(2).Value; }
    }

    public bool FORCE_DELAY_TURN_END_IN_ROLLOUT
    {
        get { return (bool)GetGene(3).Value; }
    }

    public bool INCLUDE_PLAY_MOVE_CHANCE_NODES
    {
        get { return (bool)GetGene(4).Value; }
    }

    public bool INCLUDE_END_TURN_CHANCE_NODES
    {
        get { return (bool)GetGene(5).Value; }
    }

    public string CHOSEN_HASH_GENERATION_TYPE
    {
        get { return (string)GetGene(6).Value; }
    }

    //Remove Set_Max_expansion_depth, and code implementaiton too, so it always checks. Setting a
    // high value like 999 essentially would mean there is no depth limit
    public int CHOSEN_MAX_EXPANSION_DEPTH
    {
        get { return (int)GetGene(7).Value; }
    }

    public string CHOSEN_SCORING_METHOD
    {
        get { return (string)GetGene(8).Value; }
    }

    public int ROLLOUT_TURNS_BEFORE_HEURSISTIC
    {
        get { return (int)GetGene(9).Value; }
    }

    public int DYNAMIC_MOVE_TIME_DISTRIBUTION
    {
        get { return (int)GetGene(10).Value; }
    }

    public override Gene GenerateGene(int geneIndex)
    {
        //This module's Get_ is exclusive on the right
        switch (geneIndex)
        {
            case 0:
                double gene0Min = 50.0; // Quesiton: what is the step ? is it 0.1? is it 0.01?
                double gene0Max = 150.0;
                return new Gene(Math.Round(RandomizationProvider.Current.GetDouble(gene0Min, gene0Max), 3));
            case 1:
                double gene1Min = 0.5;
                double gene1Max = 2.0; //Might want to just set it to root2
                return new Gene(Math.Round(RandomizationProvider.Current.GetDouble(gene1Min, gene1Max), 3));
            case 2:
                int gene2Min = 1;
                int gene2Max = 5;
                return new Gene(RandomizationProvider.Current.GetInt(gene2Min, gene2Max + 1));
            case 3:
                return new Gene(RandomizationProvider.Current.GetInt(0, 2) == 1);
            case 4:
                return new Gene(RandomizationProvider.Current.GetInt(0, 2) == 1);
            case 5:
                return new Gene(RandomizationProvider.Current.GetInt(0, 2) == 1);
            case 6:
                var hashGenerationTypes = new List<string> { "Quick", "Precise" };
                return new Gene(hashGenerationTypes[RandomizationProvider.Current.GetInt(0, hashGenerationTypes.Count)]);
            case 7:
                int gene7Min = 1;
                int gene7Max = 10; //Maybe this has to be increased
                return new Gene(RandomizationProvider.Current.GetInt(gene7Min, gene7Max + 1));
            case 8:
                var scoringMethodTypes = new List<string> { "Rollout", "Heuristic", "RolloutTurnsCompletionsThenHeuristic" };
                return new Gene(scoringMethodTypes[RandomizationProvider.Current.GetInt(0, scoringMethodTypes.Count)]);
            case 9:
                var gene9Min = 1;
                var gene9Max = 5;
                return new Gene(RandomizationProvider.Current.GetInt(gene9Min, gene9Max + 1));
            case 10:
                return new Gene(RandomizationProvider.Current.GetInt(0, 2) == 1);
            default:
                Console.WriteLine("Hello!");
                throw new ArgumentOutOfRangeException(nameof(geneIndex), "Invalid gene index.");

                //TODO : Finish all parameters
        }
    }

    public override IChromosome CreateNew()
    {
        return new Chromosome();
    }

}