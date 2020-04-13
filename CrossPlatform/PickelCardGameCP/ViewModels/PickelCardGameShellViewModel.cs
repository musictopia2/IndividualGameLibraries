using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGameFrameworkLibrary.ViewModels;
using PickelCardGameCP.Data;
using PickelCardGameCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace PickelCardGameCP.ViewModels
{
    public class PickelCardGameShellViewModel : BasicTrickShellViewModel<PickelCardGamePlayerItem>
    {
        public PickelCardGameShellViewModel(IGamePackageResolver mainContainer,
            CommandContainer container,
            IGameInfo gameData,
            BasicData basicData,
            IMultiplayerSaveState save,
            TestOptions test,
            PickelDelegates delegates
            )
            : base(mainContainer, container, gameData, basicData, save, test)
        {
            delegates.CloseBiddingAsync = CloseBidAsync;
            delegates.LoadBiddingAsync = LoadBidAsync;
        }

        public PickelBidViewModel? BidScreen { get; set; }

        protected override IMainScreen GetMainViewModel()
        {
            var model = MainContainer.Resolve<PickelCardGameMainViewModel>();
            return model;
        }
        private async Task CloseBidAsync()
        {
            if (BidScreen == null)
            {
                return;
            }
            await CloseSpecificChildAsync(BidScreen);
            BidScreen = null;
            await StartNewGameAsync(); //hopefully this simple (?)
        }
        private async Task LoadBidAsync()
        {
            if (BidScreen != null)
            {
                await CloseSpecificChildAsync(BidScreen);
            }
            BidScreen = MainContainer.Resolve<PickelBidViewModel>();
            await LoadScreenAsync(BidScreen);

        }

        protected override async Task GetStartingScreenAsync()
        {
            await LoadBidAsync();
        }
    }
}
