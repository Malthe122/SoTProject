using Newtonsoft.Json;
using ScriptsOfTribute;
using ScriptsOfTribute.AI;
using ScriptsOfTribute.Serializers;

public class StateMachine{

    public GameState GameState;
    private ulong seed = 0; //I think we should use a set seed in the simulation like this rather than generating new every time

    public StateMachine(GameState gameState){
        GameState = gameState;
    }

    public GameState SimulateMove(Move move, ExampleBot bot){
        bot.LogFromThis("Trying to deep copy state");
        var simulatedState = DeepCopyState(GameState);
        bot.LogFromThis("succesfully deep copied state");
        bot.LogFromThis("trying to apply move on deep copy");
        simulatedState.ApplyMove(move, seed);
        bot.LogFromThis("succesfully applied move on deep copy");
        return simulatedState;
    }

    public GameState DeepCopyState(GameState gamestate){
        // TODO make a manual deep copy if this way causes performance issues
        // Manual deep copy will require a big implementation because of all the nested references
        var json = JsonConvert.SerializeObject(gamestate);
        return JsonConvert.DeserializeObject<GameState>(json)!;
    }
}