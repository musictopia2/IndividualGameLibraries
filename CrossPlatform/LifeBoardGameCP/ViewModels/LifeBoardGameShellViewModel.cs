using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGameFrameworkLibrary.ViewModels;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using LifeBoardGameCP.Data;
using LifeBoardGameCP.EventModels;
using LifeBoardGameCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace LifeBoardGameCP.ViewModels
{
    public class LifeBoardGameShellViewModel : BasicBoardGamesShellViewModel<LifeBoardGamePlayerItem>, IHandleAsync<GenderEventModel>, IHandleAsync<StartEventModel>
    {
        public LifeBoardGameShellViewModel(IGamePackageResolver mainContainer,
            CommandContainer container,
            IGameInfo gameData,
            BasicData basicData,
            IMultiplayerSaveState save,
            TestOptions test)
            : base(mainContainer, container, gameData, basicData, save, test)
        {
        }
        protected override bool CanOpenMainAfterColors => false; //not this time.
        protected override IMainScreen GetMainViewModel()
        {
            var model = MainContainer.Resolve<LifeBoardGameMainViewModel>();
            return model;
        }
        public ChooseGenderViewModel? GenderScreen { get; set; }
        protected override async Task GetStartingScreenAsync()
        {
            LifeBoardGameSaveInfo saveRoot = MainContainer.Resolve<LifeBoardGameSaveInfo>();
            if (saveRoot.GameStatus == EnumWhatStatus.NeedChooseGender)
            {
                await LoadGenderAsync();
                return;
            }
            await base.GetStartingScreenAsync();
        }
        private async Task LoadGenderAsync()
        {
            if (GenderScreen != null)
            {
                throw new BasicBlankException("Cannot load gender because its already there.  Rethink");
            }
            GenderScreen = MainContainer.Resolve<ChooseGenderViewModel>();
            await LoadScreenAsync(GenderScreen);
        }
        async Task IHandleAsync<GenderEventModel>.HandleAsync(GenderEventModel message)
        {
            await LoadGenderAsync();            
        }

        async Task IHandleAsync<StartEventModel>.HandleAsync(StartEventModel message)
        {
            //load main screen.
            if (GenderScreen == null)
            {
                throw new BasicBlankException("Should have loaded gender first before starting this way.");
            }
            await CloseSpecificChildAsync(GenderScreen);
            GenderScreen = null;
            LifeBoardGameMainGameClass game = MainContainer.Resolve<LifeBoardGameMainGameClass>();
            game.SaveRoot.GameStatus = EnumWhatStatus.NeedChooseFirstOption; //belongs here.
            await StartNewGameAsync();

            await game.AfterChoosingGenderAsync();
        }
    }
}
