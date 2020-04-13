using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using YahtzeeHandsDownCP.Cards;
using YahtzeeHandsDownCP.Data;
using YahtzeeHandsDownCP.Logic;
namespace YahtzeeHandsDownCP.ViewModels
{
    [InstanceGame]
    public class YahtzeeHandsDownMainViewModel : BasicCardGamesVM<YahtzeeHandsDownCardInformation>
    {
        private readonly YahtzeeHandsDownMainGameClass _mainGame; //if we don't need, delete.
        private readonly YahtzeeHandsDownVMData _model;
        private readonly YahtzeeHandsDownGameContainer _gameContainer; //if not needed, delete.

        public YahtzeeHandsDownMainViewModel(CommandContainer commandContainer,
            YahtzeeHandsDownMainGameClass mainGame,
            YahtzeeHandsDownVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            YahtzeeHandsDownGameContainer gameContainer
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _mainGame = mainGame;
            _model = viewModel;
            _gameContainer = gameContainer;
            _model.Deck1.NeverAutoDisable = true;
            _model.PlayerHand1.Maximum = 5;
            _model.PlayerHand1.AutoSelect = HandObservable<YahtzeeHandsDownCardInformation>.EnumAutoType.SelectAsMany;

        }
        protected override bool CanEnableDeck()
        {
            return false; //otherwise, can't compile.
        }

        protected override bool CanEnablePile1()
        {
            if (_mainGame!.PlayerList.Count() > 2)
                return !_gameContainer.AlreadyDrew;
            if (_mainGame.SaveRoot!.ExtraTurns < 4)
                return !_gameContainer.AlreadyDrew;
            return false;
        }

        protected override async Task ProcessDiscardClickedAsync()
        {
            var thisList = _model.PlayerHand1!.ListSelectedObjects();
            if (thisList.Count == 0)
            {
                await UIPlatform.ShowMessageAsync("Must choose at least one card to discard to get new cards");
                return;
            }
            if (_mainGame.BasicData!.MultiPlayer)
            {
                var nextList = thisList.GetDeckListFromObjectList();
                await _mainGame.Network!.SendAllAsync("replacecards", nextList);
            }
            await _mainGame!.ReplaceCardsAsync(thisList);
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
        public override bool CanEndTurn()
        {
            return !CanEnablePile1();
        }
        [Command(EnumCommandCategory.Game)]
        public async Task GoOutAsync()
        {
            var results = _mainGame!.GetResults();
            if (results.Count == 0)
            {
                await UIPlatform.ShowMessageAsync("Cannot go out");
                return;
            }
            if (_mainGame.BasicData!.MultiPlayer)
                await _mainGame.Network!.SendAllAsync("wentout", results.First());
            await _mainGame.PlayerGoesOutAsync(results.First());
        }
    }
}