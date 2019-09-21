using BasicGameFramework.Attributes;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace SolitaireBoardGameCP
{
    [SingletonGame]
    public class SolitaireGameEventHandler : ISolitaireBoardEvents
    {

        private readonly ISimpleUI _thisMessage;
        private readonly SolitaireBoardGameMainGameClass _thisGame;
        //this is good so i can mock more.

        public SolitaireGameEventHandler(ISimpleUI thisMessage, SolitaireBoardGameMainGameClass thisGame)
        {
            _thisMessage = thisMessage;
            _thisGame = thisGame;
        }

        void ISolitaireBoardEvents.MoveCompleted()
        {
            _thisGame.IsEnabled = true;
        }

        async Task ISolitaireBoardEvents.PiecePlacedAsync(GameSpace thisSpace)
        {
            _thisGame.IsEnabled = false;
            if (_thisGame.IsValidMove(thisSpace) == false)
            {
                await _thisMessage.ShowMessageBox("Illegal Move");
                await _thisGame.UnselectPieceAsync(thisSpace);
                _thisGame.IsEnabled = true;
                return;
            }
            await _thisGame.MakeMoveAsync(thisSpace);
        }

        async Task ISolitaireBoardEvents.PieceSelectedAsync(GameSpace ThisSpace)
        {
            _thisGame.IsEnabled = false;
            if (ThisSpace.Vector.Equals(_thisGame.PreviousPiece) == false)
                await _thisGame.HightlightSpaceAsync(ThisSpace);
            else
                _thisGame.SelectUnSelectSpace(ThisSpace);
            _thisGame.IsEnabled = true;
        }

        async Task ISolitaireBoardEvents.WonAsync() //i think this is it.
        {
            _thisGame.IsEnabled = false;
            await _thisMessage.ShowMessageBox("You won");
        }
    }
}