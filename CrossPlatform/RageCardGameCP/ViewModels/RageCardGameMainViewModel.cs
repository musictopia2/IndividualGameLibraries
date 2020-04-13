using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.TestUtilities;
using RageCardGameCP.Cards;
using RageCardGameCP.Data;
using RageCardGameCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace RageCardGameCP.ViewModels
{
    [InstanceGame]
    public class RageCardGameMainViewModel : TrickCardGamesVM<RageCardGameCardInformation, EnumColor>
    {
        private readonly RageCardGameVMData _model;

        public RageCardGameMainViewModel(CommandContainer commandContainer,
            RageCardGameMainGameClass mainGame,
            RageCardGameVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _model = viewModel;
            _model.Deck1.NeverAutoDisable = true;
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

        private string _lead = "";
        [VM]
        public string Lead
        {
            get { return _lead; }
            set
            {
                if (SetProperty(ref _lead, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

    }
}