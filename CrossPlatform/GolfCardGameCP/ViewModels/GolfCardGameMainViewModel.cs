using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using GolfCardGameCP.Data;
using GolfCardGameCP.Logic;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace GolfCardGameCP.ViewModels
{
    [InstanceGame]
    public class GolfCardGameMainViewModel : BasicCardGamesVM<RegularSimpleCard>
    {
        private readonly GolfCardGameMainGameClass _mainGame; //if we don't need, delete.
        private readonly GolfCardGameVMData _model;
        private readonly GolfCardGameGameContainer _gameContainer;

        public GolfCardGameMainViewModel(CommandContainer commandContainer,
            GolfCardGameMainGameClass mainGame,
            GolfCardGameVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            GolfCardGameGameContainer gameContainer
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _mainGame = mainGame;
            _model = viewModel;
            _gameContainer = gameContainer;
            _model.Deck1.NeverAutoDisable = true;

            _model.OtherPile!.PileClickedAsync += OtherPile_PileClickedAsync;
            _model.OtherPile.SendEnableProcesses(this, (() => gameContainer.AlreadyDrew));
            _model.GolfHand1.SendEnableProcesses(this, () => gameContainer.AlreadyDrew);
            _model.HiddenCards1.SendEnableProcesses(this, () => gameContainer.AlreadyDrew);
        }
        protected override Task TryCloseAsync()
        {
            _model.OtherPile!.PileClickedAsync -= OtherPile_PileClickedAsync;
            return base.TryCloseAsync();
        }
        private async Task OtherPile_PileClickedAsync()
        {
            var tempCard = _model.OtherPile!.GetCardInfo();
            if (_gameContainer!.PreviousCard == tempCard.Deck)
            {
                await UIPlatform.ShowMessageAsync("Sorry; you cannot throwaway the same card you picked up from the discard pile");
                return;
            }
            await _gameContainer.SendDiscardMessageAsync(tempCard.Deck);
            await _mainGame.DiscardAsync(tempCard.Deck);
        }

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
        private int _round;
        [VM]
        public int Round
        {
            get { return _round; }
            set
            {
                if (SetProperty(ref _round, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private string _instructions = "";
        [VM]
        public string Instructions
        {
            get { return _instructions; }
            set
            {
                if (SetProperty(ref _instructions, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public bool CanKnock => !_mainGame.PlayerList.Any(items => items.Knocked == true) && !_gameContainer.AlreadyDrew;
        [Command(EnumCommandCategory.Game)]
        public async Task KnockAsync()
        {
            if (_mainGame.BasicData!.MultiPlayer == true)
                await _mainGame.Network!.SendAllAsync("knock");
            await _mainGame.KnockAsync();
        }
    }
}