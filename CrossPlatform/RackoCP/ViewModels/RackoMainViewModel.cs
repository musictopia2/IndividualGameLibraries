using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using RackoCP.Cards;
using RackoCP.Data;
using RackoCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace RackoCP.ViewModels
{
    [InstanceGame]
    public class RackoMainViewModel : BasicCardGamesVM<RackoCardInformation>
    {
        private readonly RackoMainGameClass _mainGame; //if we don't need, delete.
        private readonly RackoVMData _model;
        private readonly RackoGameContainer _gameContainer; //if not needed, delete.

        public RackoMainViewModel(CommandContainer commandContainer,
            RackoMainGameClass mainGame,
            RackoVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            RackoGameContainer gameContainer
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _mainGame = mainGame;
            _model = viewModel;
            _gameContainer = gameContainer;
            _model.Deck1.NeverAutoDisable = true;
        }
        //anything else needed is here.
        protected override bool CanEnableDeck()
        {
            return !_gameContainer!.AlreadyDrew;
        }

        protected override bool CanEnablePile1()
        {
            return !_gameContainer!.AlreadyDrew;
        }

        protected override async Task ProcessDiscardClickedAsync()
        {
            await _mainGame!.PickupFromDiscardAsync();
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
        public bool CanPlayOnPile => _gameContainer!.AlreadyDrew;
        public async Task PlayOnPileAsync(RackoCardInformation card)
        {
            await _gameContainer!.SendDiscardMessageAsync(card.Deck); //i think
            await _mainGame.DiscardAsync(card);
        }
        public bool CanDiscardCurrent => _gameContainer!.AlreadyDrew;
        [Command(EnumCommandCategory.Game)]
        public async Task DiscardCurrentAsync()
        {
            if (_model.OtherPile!.PileEmpty() == true)
            {
                await UIPlatform.ShowMessageAsync("You must have a card to discard");
                return;
            }
            if (_mainGame.BasicData!.MultiPlayer == true)
                await _mainGame.Network!.SendAllAsync("discardcurrent");
            await _mainGame!.DiscardCurrentAsync();
        }
        public bool CanRacko => !_gameContainer!.AlreadyDrew;
        [Command(EnumCommandCategory.Game)]
        public async Task RackoAsync()
        {
            if (_mainGame!.HasRacko() == false)
            {
                await UIPlatform.ShowMessageAsync("There is no Racko");
                return;
            }
            if (_mainGame.BasicData!.MultiPlayer == true)
                await _mainGame.Network!.SendAllAsync("racko");
            await _mainGame.EndRoundAsync();
        }
    }
}