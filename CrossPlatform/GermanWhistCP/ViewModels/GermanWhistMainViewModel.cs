using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using GermanWhistCP.Cards;
using GermanWhistCP.Data;
using GermanWhistCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace GermanWhistCP.ViewModels
{
    [InstanceGame]
    public class GermanWhistMainViewModel : TrickCardGamesVM<GermanWhistCardInformation, EnumSuitList>
    {
        private readonly GermanWhistVMData _model;

        public GermanWhistMainViewModel(CommandContainer commandContainer,
            GermanWhistMainGameClass mainGame,
            GermanWhistVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _model = viewModel;
            _model.Deck1.NeverAutoDisable = true;
            _model.PlayerHand1.Maximum = 13;
            _model.Deck1.DeckStyle = DeckObservablePile<GermanWhistCardInformation>.EnumStyleType.AlwaysKnown;
        }
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