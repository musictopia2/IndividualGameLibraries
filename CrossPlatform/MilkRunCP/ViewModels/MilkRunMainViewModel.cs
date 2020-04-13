using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using MilkRunCP.Cards;
using MilkRunCP.Data;
using MilkRunCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace MilkRunCP.ViewModels
{
    [InstanceGame]
    public class MilkRunMainViewModel : BasicCardGamesVM<MilkRunCardInformation>
    {
        private readonly MilkRunMainGameClass _mainGame; //if we don't need, delete.
        private readonly MilkRunVMData _model;
        private readonly MilkRunGameContainer _gameContainer; //if not needed, delete.

        public MilkRunMainViewModel(CommandContainer commandContainer,
            MilkRunMainGameClass mainGame,
            MilkRunVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            MilkRunGameContainer gameContainer
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _mainGame = mainGame;
            _model = viewModel;
            _gameContainer = gameContainer;
            _model.Deck1.NeverAutoDisable = true;
            _model.PlayerHand1.Maximum = 8;
        }
        //anything else needed is here.
        protected override bool CanEnableDeck()
        {
            return _mainGame!.SaveRoot!.CardsDrawn != 2;
        }

        protected override bool CanEnablePile1()
        {
            return true; //otherwise, can't compile.
        }

        protected override async Task ProcessDiscardClickedAsync()
        {
            int newDeck = _model.PlayerHand1!.ObjectSelected();
            if (newDeck > 0)
            {
                if (_mainGame!.SaveRoot!.CardsDrawn < 2)
                {
                    await UIPlatform.ShowMessageAsync("Sorry, must draw the 2 cards first before discarding");
                    return;
                }
                if (newDeck == _gameContainer.PreviousCard)
                {
                    await UIPlatform.ShowMessageAsync("Cannot discard the same card you picked up");
                    return;
                }
                await _gameContainer.SendDiscardMessageAsync(newDeck);
                await _mainGame.DiscardAsync(newDeck);
                return;
            }
            if (_mainGame!.SaveRoot!.DrawnFromDiscard == true)
            {
                await UIPlatform.ShowMessageAsync("Sorry, you already picked up one card from discard.  Cannot pickup another one.  If you want to discard, then choose a card to discard");
                return;
            }
            if (_model.Pile1!.PileEmpty())
            {
                await UIPlatform.ShowMessageAsync("Sorry, there is no card to pickup from the discard.");
                return;
            }
            await _mainGame.PickupFromDiscardAsync();
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
    }
}