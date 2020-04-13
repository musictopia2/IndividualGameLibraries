using BasicGameFrameworkLibrary.Attributes;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using CribbageCP.Data;
using System.Threading.Tasks;

namespace CribbageCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class HiddenBoard : ObservableObject
    {
        private readonly CribbageGameContainer _gameContainer;
        public HiddenBoard(CribbageGameContainer gameContainer)
        {
            _gameContainer = gameContainer;
        }

        public async Task PegAsync(int howMany)
        {
            if (_gameContainer.WhoTurn == 0)
            {
                throw new BasicBlankException("The whoturn cannot be 0");
            }
            if (_gameContainer.NextStepAsync == null)
            {
                throw new BasicBlankException("Nobody is handling the next step");
            }
            if (howMany == 0)
            {
                await _gameContainer.NextStepAsync();
                return;
            }
            int oldPosition;
            bool useFirsts;
            if (_gameContainer.SingleInfo!.FirstPosition == 0 && _gameContainer.SingleInfo.SecondPosition == 0)
            {
                oldPosition = 0;
                useFirsts = true;
            }
            else if (_gameContainer.SingleInfo.FirstPosition > _gameContainer.SingleInfo.SecondPosition)
            {
                oldPosition = _gameContainer.SingleInfo.FirstPosition;
                useFirsts = false;
            }
            else
            {
                oldPosition = _gameContainer.SingleInfo.SecondPosition;
                useFirsts = true;
            }
            int newPosition = oldPosition + howMany;
            if (newPosition > 121)
            {
                int diffs = newPosition - 121;
                howMany -= diffs;
                newPosition = 121;
            }
            _gameContainer.SingleInfo.ScoreRound += howMany;
            int oldTotal = _gameContainer.SingleInfo.TotalScore;
            _gameContainer.SingleInfo.TotalScore = newPosition;
            if (_gameContainer.SingleInfo.TotalScore < oldTotal)
                throw new BasicBlankException("Can't lower score.  Rethink");
            if (newPosition > 90)
                _gameContainer.SingleInfo.IsSkunk = false;
            if (useFirsts == true)
                _gameContainer.SingleInfo.FirstPosition = newPosition;
            else
                _gameContainer.SingleInfo.SecondPosition = newPosition;
            if (newPosition == 121 || _gameContainer.Test.ImmediatelyEndGame)
            {
                await _gameContainer.ShowWinAsync!.Invoke();
                return;
            }
            await _gameContainer.NextStepAsync.Invoke();
        }
    }
}
