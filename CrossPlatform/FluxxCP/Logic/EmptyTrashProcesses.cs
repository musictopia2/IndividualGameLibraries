using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using FluxxCP.Cards;
using FluxxCP.Containers;
using System.Threading.Tasks;

namespace FluxxCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class EmptyTrashProcesses : IEmptyTrashProcesses
    {
        private readonly FluxxGameContainer _gameContainer;
        private readonly FluxxVMData _model;
        private readonly IAnalyzeProcesses _processes;

        public EmptyTrashProcesses(FluxxGameContainer gameContainer, FluxxVMData model, IAnalyzeProcesses processes)
        {
            _gameContainer = gameContainer;
            _model = model;
            _processes = processes;
        }

        public async Task EmptyTrashAsync()
        {
            await UIPlatform.ShowMessageAsync("Empty the trash was played.  Therefore; the cards are being reshuffled");
            if (_gameContainer.BasicData!.MultiPlayer && _gameContainer.SingleInfo!.CanSendMessage(_gameContainer.BasicData) == false)
            {
                _gameContainer.Check!.IsEnabled = true;
                return;
            }
            var thisList = _model.Pile1!.DiscardList();
            thisList.AddRange(_model.Deck1!.DeckList());
            thisList.ShuffleList();
            if (_gameContainer.BasicData.MultiPlayer)
                await _gameContainer.Network!.SendAllAsync("emptytrash", thisList.GetDeckListFromObjectList());
            await FinishEmptyTrashAsync(thisList);
        }

        public async Task FinishEmptyTrashAsync(IEnumerableDeck<FluxxCardInformation> cardList)
        {
            _model!.Deck1!.OriginalList(cardList);
            _model.Pile1!.CardsReshuffled();
            await _processes.AnalyzeQueAsync();
        }
    }
}
