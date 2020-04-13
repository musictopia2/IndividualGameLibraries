using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.MultiplePilesObservable;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using ConcentrationCP.Data;
using ConcentrationCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace ConcentrationCP.ViewModels
{
    [InstanceGame]
    public class ConcentrationMainViewModel : BasicCardGamesVM<RegularSimpleCard>
    {
        private readonly ConcentrationMainGameClass _mainGame; //if we don't need, delete.
        private readonly ConcentrationVMData _model;
        public ConcentrationMainViewModel(CommandContainer commandContainer,
            ConcentrationMainGameClass mainGame,
            ConcentrationVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver) : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _mainGame = mainGame;
            _model = viewModel;
            viewModel.Deck1.NeverAutoDisable = true;
            viewModel.GameBoard1.PileClickedAsync += GameBoard1_PileClickedAsync;
        }
        protected override Task TryCloseAsync()
        {
            _model.GameBoard1.PileClickedAsync -= GameBoard1_PileClickedAsync;
            return base.TryCloseAsync();
        }
        private async Task GameBoard1_PileClickedAsync(int index, BasicPileInfo<RegularSimpleCard> thisPile)
        {
            await _mainGame.SelectCardAsync(thisPile.ThisObject.Deck);
        }

        protected override bool CanEnableDeck()
        {
            return false;
        }
        protected override bool CanEnablePile1()
        {
            return false;
        }
        protected override async Task ProcessDiscardClickedAsync()
        {
            //if we have anything, will be here.
            await Task.CompletedTask;
        }
        public override bool CanEnableAlways()
        {
            return true;
        }

    }
}