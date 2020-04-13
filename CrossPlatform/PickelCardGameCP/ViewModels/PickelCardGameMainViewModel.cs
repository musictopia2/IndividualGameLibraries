using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using PickelCardGameCP.Cards;
using PickelCardGameCP.Data;
using PickelCardGameCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace PickelCardGameCP.ViewModels
{
    [InstanceGame]
    public class PickelCardGameMainViewModel : TrickCardGamesVM<PickelCardGameCardInformation, EnumSuitList>
    {
        private readonly PickelCardGameVMData _model;

        public PickelCardGameMainViewModel(CommandContainer commandContainer,
            PickelCardGameMainGameClass mainGame,
            PickelCardGameVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _model = viewModel;
            _model.Deck1.NeverAutoDisable = true;
        }
        //anything else needed is here.
        protected override bool CanEnableDeck()
        {
            return false; //otherwise, can't compile.
        }

        protected override bool CanEnablePile1()
        {
            return false; //otherwise, can't compile.
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