using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using CommonBasicStandardLibraries.CollectionClasses;
using FluxxCP.Cards;
using FluxxCP.Containers;
using System.Threading.Tasks;

namespace FluxxCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class DiscardProcesses : IDiscardProcesses
    {
        private readonly FluxxGameContainer _gameContainer;
        private readonly IAnalyzeProcesses _analyzeProcesses;


        //if this requires play processes, then rethink because otherwise, overflows;

        public DiscardProcesses(FluxxGameContainer gameContainer, IAnalyzeProcesses analyzeProcesses)
        {
            _gameContainer = gameContainer;
            _analyzeProcesses = analyzeProcesses;
        }

        public async Task DiscardFromHandAsync(CustomBasicList<int> list)
        {
            var newList = list.GetFluxxCardListFromDeck(_gameContainer);
            await DiscardFromHandAsync(newList);
        }

        public async Task DiscardFromHandAsync(FluxxCardInformation thisCard)
        {
            _gameContainer.RemoveFromHandOnly(thisCard);
            await _gameContainer.AnimatePlayAsync!(thisCard);
        }

        public async Task DiscardFromHandAsync(IDeckDict<FluxxCardInformation> list)
        {
            if (_gameContainer.SingleInfo!.CanSendMessage(_gameContainer.BasicData!))
                await _gameContainer.Network!.SendAllAsync("discardfromhand", list.GetDeckListFromObjectList());
            await list.ForEachAsync(async thisCard =>
            {
                await DiscardFromHandAsync(thisCard);
                if (_gameContainer.Test!.NoAnimations == false)
                    await _gameContainer.Delay!.DelaySeconds(.1);
            });
            await _analyzeProcesses.AnalyzeQueAsync();
        }

        public async Task DiscardFromHandAsync(int deck)
        {
            var thisCard = _gameContainer.SingleInfo!.MainHandList.GetSpecificItem(deck);
            await DiscardFromHandAsync(thisCard);
        }

        public async Task DiscardGoalAsync(int deck)
        {
            if (_gameContainer.SingleInfo!.CanSendMessage(_gameContainer.BasicData!))
                await _gameContainer.Network!.SendAllAsync("discardgoal", deck);
            var thisGoal = (GoalCard)_gameContainer.DeckList!.GetSpecificItem(deck);
            await DiscardGoalAsync(thisGoal);
        }

        public async Task DiscardGoalAsync(GoalCard thisCard)
        {
            _gameContainer.SaveRoot!.GoalList.RemoveObjectByDeck((int)thisCard.Deck);
            await _gameContainer.AnimatePlayAsync!(thisCard);
        }

        public async Task DiscardKeeperAsync(int deck)
        {
            var thisKeeper = (KeeperCard)_gameContainer.DeckList!.GetSpecificItem(deck);
            await DiscardKeeperAsync(thisKeeper);
        }

        public async Task DiscardKeeperAsync(FluxxCardInformation thisCard)
        {
            _gameContainer.SingleInfo!.KeeperList.RemoveObjectByDeck(thisCard.Deck);
            await _gameContainer.AnimatePlayAsync!(thisCard);
        }

        public async Task DiscardKeepersAsync(IDeckDict<FluxxCardInformation> list)
        {
            if (_gameContainer.SingleInfo!.CanSendMessage(_gameContainer.BasicData!))
                await _gameContainer.Network!.SendAllAsync("discardkeepers", list.GetDeckListFromObjectList());
            await list.ForEachAsync(async thisCard =>
            {
                await DiscardKeeperAsync(thisCard);
                if (_gameContainer.Test!.NoAnimations == false)
                    await _gameContainer.Delay!.DelaySeconds(.1);
            });
            await _analyzeProcesses.AnalyzeQueAsync();
        }

        public async Task DiscardKeepersAsync(CustomBasicList<int> list)
        {
            var newList = list.GetFluxxCardListFromDeck(_gameContainer);
            await DiscardKeepersAsync(newList);
        }
    }
}