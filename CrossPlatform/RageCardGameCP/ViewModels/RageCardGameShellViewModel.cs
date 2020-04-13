using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BasicGameFrameworkLibrary.ViewModels;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using RageCardGameCP.Data;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using RageCardGameCP.Logic;
//i think this is the most common things i like to do
namespace RageCardGameCP.ViewModels
{
    public class RageCardGameShellViewModel : BasicTrickShellViewModel<RageCardGamePlayerItem>
    {
        public RageCardGameShellViewModel(IGamePackageResolver mainContainer,
            CommandContainer container, 
            IGameInfo gameData, 
            BasicData basicData,
            IMultiplayerSaveState save,
            TestOptions test,
            RageDelgates delgates
            )
            : base(mainContainer, container, gameData, basicData, save, test)
        {
            delgates.CloseBidScreenAsync = CloseBidScreenAsync;
            delgates.LoadBidScreenAsync = LoadBidScreenAsync;
            delgates.CloseColorScreenAsync = CloseColorScreenAsync;
            delgates.LoadColorScreenAsync = LoadColorScreenAsync;
        }

        protected override IMainScreen GetMainViewModel()
        {
            var model = MainContainer.Resolve<RageCardGameMainViewModel>();
            return model;
        }

        public async Task LoadColorScreenAsync()
        {
            if (ColorScreen != null)
            {
                return;
            }
            await CloseMainAsync("Already closed main to load colors.  Rethink");
            ColorScreen = MainContainer.Resolve<RageColorViewModel>();
            await LoadScreenAsync(ColorScreen);
        }
        public async Task CloseColorScreenAsync()
        {
            if (ColorScreen == null)
            {
                return;
            }
            await CloseSpecificChildAsync(ColorScreen);
            ColorScreen = null;
            await StartNewGameAsync();
        }

        public async Task LoadBidScreenAsync()
        {
            if (BidScreen != null)
            {
                return;
            }
            await CloseMainAsync("Already closed main to load bidding.  Rethink");
            BidScreen = MainContainer.Resolve<RageBiddingViewModel>();
            await LoadScreenAsync(BidScreen);
        }
        public async Task CloseBidScreenAsync()
        {
            if (BidScreen == null)
            {
                return;
            }
            await CloseSpecificChildAsync(BidScreen);
            BidScreen = null;
            await StartNewGameAsync();
        }

        public RageColorViewModel? ColorScreen { get; set; }
        public RageBiddingViewModel? BidScreen { get; set; }

    }
}
