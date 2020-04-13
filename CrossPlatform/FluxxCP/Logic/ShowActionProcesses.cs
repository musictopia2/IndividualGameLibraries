using BasicGameFrameworkLibrary.Attributes;
using CommonBasicStandardLibraries.CollectionClasses;
using FluxxCP.Containers;
using System.Threading.Tasks;

namespace FluxxCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class ShowActionProcesses : IShowActionProcesses
    {
        private readonly ActionContainer _actionContainer;
        private readonly BasicActionLogic _basicActionLogic;
        private readonly FluxxGameContainer _gameContainer;

        public ShowActionProcesses(ActionContainer actionContainer,
            BasicActionLogic basicActionLogic,
            FluxxGameContainer gameContainer
            )
        {
            _actionContainer = actionContainer;
            _basicActionLogic = basicActionLogic;
            _gameContainer = gameContainer;
        }


        async Task IShowActionProcesses.ChoseOtherCardSelectedAsync(int deck)
        {
            _actionContainer.OtherHand!.SelectOneFromDeck(deck);
            if (_gameContainer.Test!.NoAnimations == false)
                await _gameContainer.Delay!.DelaySeconds(.5);
            await _basicActionLogic.ShowMainScreenAgainAsync();
        }

        async Task IShowActionProcesses.ShowCardUseAsync(int deck)
        {
            _actionContainer.TempHand!.SelectOneFromDeck(deck);
            if (_gameContainer.Test!.NoAnimations == false)
                await _gameContainer.Delay!.DelaySeconds(.5);
            await _basicActionLogic.ShowMainScreenAgainAsync();
        }

        async Task IShowActionProcesses.ShowChosenForEverybodyGetsOneAsync(CustomBasicList<int> selectedList, int selectedIndex)
        {
            selectedList.ForEach(thisItem =>
            {
                _actionContainer.TempHand!.SelectOneFromDeck(thisItem);
            });
            _actionContainer.Player1!.SelectSpecificItem(selectedIndex);
            if (_gameContainer.Test!.NoAnimations == false)
                await _gameContainer.Delay!.DelaySeconds(1);
            await _basicActionLogic.ShowMainScreenAgainAsync();
        }

        async Task IShowActionProcesses.ShowDirectionAsync(int selectedIndex)
        {
            _actionContainer.Direction1!.SelectSpecificItem(selectedIndex);
            if (_gameContainer.Test!.NoAnimations == false)
                await _gameContainer.Delay!.DelaySeconds(.5);
            await _basicActionLogic.ShowMainScreenAgainAsync();
        }

        async Task IShowActionProcesses.ShowLetsDoAgainAsync(int selectedIndex)
        {
            _actionContainer.CardList1!.SelectSpecificItem(selectedIndex);
            if (_gameContainer.Test!.NoAnimations == false)
                await _gameContainer.Delay!.DelaySeconds(.5);
            await _basicActionLogic.ShowMainScreenAgainAsync();
        }

        async Task IShowActionProcesses.ShowPlayerForCardChosenAsync(int selectedIndex)
        {
            _actionContainer.IndexPlayer = selectedIndex;
            await ShowTradeHandAsync(selectedIndex);
        }

        async Task IShowActionProcesses.ShowRulesSimplifiedAsync(CustomBasicList<int> list)
        {
            if (list.Count == 0)
                return;
            _actionContainer.Rule1!.SelectSeveralItems(list);
            if (_gameContainer.Test!.NoAnimations == false)
                await _gameContainer.Delay!.DelaySeconds(1);
            await _basicActionLogic.ShowMainScreenAgainAsync();
        }

        async Task IShowActionProcesses.ShowRuleTrashedAsync(int selectedIndex)
        {
            _actionContainer.Rule1!.SelectSpecificItem(selectedIndex);
            if (_gameContainer.Test!.NoAnimations == false)
                await _gameContainer.Delay!.DelaySeconds(.75);
            await _basicActionLogic.ShowMainScreenAgainAsync();
        }
        private async Task ShowTradeHandAsync(int selectedIndex)
        {
            _actionContainer.Player1!.SelectSpecificItem(selectedIndex);
            if (_gameContainer.Test!.NoAnimations == false)
                await _gameContainer.Delay!.DelaySeconds(.75);
            await _basicActionLogic.ShowMainScreenAgainAsync();
        }
        async Task IShowActionProcesses.ShowTradeHandAsync(int selectedIndex)
        {
            await ShowTradeHandAsync(selectedIndex);
        }
    }
}