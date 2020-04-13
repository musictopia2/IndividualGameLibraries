using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGameFrameworkLibrary.ViewModels;
using BladesOfSteelCP.Data;
using BladesOfSteelCP.Logic;
using CommonBasicStandardLibraries.Exceptions;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BladesOfSteelCP.ViewModels
{
    public class BladesOfSteelShellViewModel : BasicMultiplayerShellViewModel<BladesOfSteelPlayerItem>
    {
        public BladesOfSteelShellViewModel(IGamePackageResolver mainContainer,
            CommandContainer container,
            IGameInfo gameData,
            BasicData basicData,
            IMultiplayerSaveState save,
            TestOptions test,
            BladesOfSteelScreenDelegates delegates
            )
            : base(mainContainer, container, gameData, basicData, save, test)
        {
            delegates.ReloadFaceoffAsync = LoadFaceoffAsync;
            delegates.LoadMainGameAsync = LoadMainGameAsync;
        }
        public FaceoffViewModel? FaceoffScreen { get; set; }
        private async Task LoadMainGameAsync()
        {
            if (FaceoffScreen == null)
            {
                throw new BasicBlankException("Faceoff should have been loaded first.  Rethink");
            }
            await CloseSpecificChildAsync(FaceoffScreen);
            FaceoffScreen = null;
            MainVM = MainContainer.Resolve<BladesOfSteelMainViewModel>();
            await LoadScreenAsync(MainVM);
        }
        private async Task LoadFaceoffAsync()
        {
            if (MainVM != null)
            {
                await CloseSpecificChildAsync(MainVM);
                MainVM = null;
            }
            if (FaceoffScreen != null)
            {
                throw new BasicBlankException("Faceoff should be null when loading faceoffs");
            }
            FaceoffScreen = MainContainer.Resolve<FaceoffViewModel>();
            await LoadScreenAsync(FaceoffScreen);
        }
        protected override async Task StartNewGameAsync()
        {
            await LoadFaceoffAsync();
        }
        protected override IMainScreen GetMainViewModel()
        {
            throw new BasicBlankException("Something else should have happened instead of getting the main view model because of faceoffs");
            //var model = MainContainer.Resolve<BladesOfSteelMainViewModel>(); //i think this should be the first one now.
            //return model;
        }
    }
}
