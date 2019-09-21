using BasicGameFramework.Attributes;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers;
using System.Threading.Tasks;
namespace CribbageCP
{
    [SingletonGame]
    public class HiddenBoard : ObservableObject
    {
        private readonly CribbageMainGameClass _mainGame;
        public HiddenBoard(CribbageMainGameClass mainGame)
        {
            _mainGame = mainGame;
        }

        public async Task PegAsync(int howMany)
        {
            if (_mainGame.WhoTurn == 0)
                throw new BasicBlankException("The whoturn cannot be 0");
            if (howMany == 0)
            {
                await _mainGame.NextStepAsync();
                return;
            }
            int oldPosition;
            bool useFirsts;
            if (_mainGame.SingleInfo!.FirstPosition == 0 && _mainGame.SingleInfo.SecondPosition == 0)
            {
                oldPosition = 0;
                useFirsts = true;
            }
            else if (_mainGame.SingleInfo.FirstPosition > _mainGame.SingleInfo.SecondPosition)
            {
                oldPosition = _mainGame.SingleInfo.FirstPosition;
                useFirsts = false;
            }
            else
            {
                oldPosition = _mainGame.SingleInfo.SecondPosition;
                useFirsts = true;
            }
            int newPosition = oldPosition + howMany;
            if (newPosition > 121)
            {
                int diffs = newPosition - 121;
                howMany -= diffs;
                newPosition = 121;
            }
            _mainGame.SingleInfo.ScoreRound += howMany;
            int oldTotal = _mainGame.SingleInfo.TotalScore;
            _mainGame.SingleInfo.TotalScore = newPosition;
            if (_mainGame.SingleInfo.TotalScore < oldTotal)
                throw new BasicBlankException("Can't lower score.  Rethink");
            if (newPosition > 90)
                _mainGame.SingleInfo.IsSkunk = false;
            if (useFirsts == true)
                _mainGame.SingleInfo.FirstPosition = newPosition;
            else
                _mainGame.SingleInfo.SecondPosition = newPosition;
            if (newPosition == 121)
            {
                await _mainGame.GameOverAsync();
                return;
            }
            await _mainGame.NextStepAsync();
        }
    }
}