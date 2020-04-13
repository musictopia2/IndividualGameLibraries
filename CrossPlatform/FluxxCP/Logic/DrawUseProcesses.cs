using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using FluxxCP.Containers;
using FluxxCP.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FluxxCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class DrawUseProcesses : IDrawUseProcesses
    {
        private readonly FluxxGameContainer _gameContainer;
        private readonly IAnalyzeProcesses _analyzeProcesses;

        public DrawUseProcesses(FluxxGameContainer gameContainer, IAnalyzeProcesses analyzeProcesses)
        {
            _gameContainer = gameContainer;
            _analyzeProcesses = analyzeProcesses;
        }

        async Task IDrawUseProcesses.DrawUsedAsync(int deck)
        {
            if (_gameContainer.SingleInfo!.CanSendMessage(_gameContainer.BasicData!))
                await _gameContainer.Network!.SendAllAsync("drawuse", deck);
            _gameContainer!.TempActionHandList.RemoveSpecificItem(deck);
            var thisCard = _gameContainer.DeckList!.GetSpecificItem(deck);
            _gameContainer.QuePlayList.Add(thisCard);
            if (_gameContainer.TempActionHandList.Count == 1)
            {
                if (_gameContainer.CurrentAction!.Deck == EnumActionMain.Draw3Play2OfThem)
                {
                    _gameContainer.SaveRoot!.SavedActionData.TempDiscardList.Add(_gameContainer.TempActionHandList.Single());
                }
                else
                {
                    thisCard = _gameContainer.DeckList.GetSpecificItem(_gameContainer.TempActionHandList.Single());
                    _gameContainer.QuePlayList.Add(thisCard);
                }
                _gameContainer.TempActionHandList.Clear();
                _gameContainer.CurrentAction = null;
                await _analyzeProcesses.AnalyzeQueAsync();
                return;
            }
            await _gameContainer.ContinueTurnAsync!.Invoke();
        }
    }
}