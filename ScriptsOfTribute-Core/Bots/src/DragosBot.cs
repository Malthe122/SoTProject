using ScriptsOfTribute;
using ScriptsOfTribute.AI;
using ScriptsOfTribute.Board;
using ScriptsOfTribute.Board.Cards;
using ScriptsOfTribute.Serializers;

namespace Bots;

public class DragosBot : AI
{
    private List<PatronId> patronsInPlay = new List<PatronId>();

    private readonly SeededRandom rng = new(123);

    public override PatronId SelectPatron(List<PatronId> availablePatrons, int round) {
        return availablePatrons.PickRandom(rng);
    }

    public override Move Play(GameState gameState, List<Move> possibleMoves, TimeSpan remainingTime)
    {
        Dictionary<PatronId, PlayerEnum> patronFavours = gameState.PatronStates.All;
        var playerId = gameState.CurrentPlayer.PlayerID;

        foreach (var move in possibleMoves) {
            switch (move.Command) {
                case CommandEnum.CALL_PATRON:
                    var nextPatronMove = move as SimplePatronMove;
                    var patronId = nextPatronMove.PatronId;

                    if (playerId != patronFavours[patronId] && patronId != PatronId.TREASURY) {
                        Console.WriteLine($"PLAYER {playerId} ACTIVATED PATRON {nextPatronMove.PatronId}");
                        return nextPatronMove;
                    }
                    break;

                case CommandEnum.PLAY_CARD:
                    var nextCardMove = move as SimpleCardMove;
                    if (nextCardMove.Card.Type == CardType.STARTER && nextCardMove.Card.Name == "Gold") {
                        Console.WriteLine($"PLAYER {playerId} PLAYED CARD {nextCardMove.Card.Name} OF TYPE {nextCardMove.Card.Type}");
                        return nextCardMove;
                    }
                    break;

                default:
                    return move;
            }
        }
        return possibleMoves.PickRandom(rng);
    }

    public override void GameEnd(EndGameState state, FullGameState? finalBoardState) { }
}
