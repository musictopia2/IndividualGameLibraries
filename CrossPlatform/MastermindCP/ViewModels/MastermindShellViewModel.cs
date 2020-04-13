using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.ViewModels;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace MastermindCP.ViewModels
{
    public class MastermindShellViewModel : SinglePlayerShellViewModel
    {
        

        public MastermindShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo GameData, ISaveSinglePlayerClass saves) : base(mainContainer, container, GameData, saves)
        {
        }

        protected override bool AlwaysNewGame => false; //most games allow new game always.
        protected override bool AutoStartNewGame => false;

        public IScreen? OpeningScreen { get; set; }
        public IScreen? SolutionScreen { get; set; }

        protected override IMainScreen GetMainViewModel()
        {
            var model = MainContainer.Resolve<MastermindMainViewModel>();
            return model;
        }
        protected override async Task NewGameRequestedAsync()
        {
            if (OpeningScreen == null)
            {
                throw new BasicBlankException("There was no opening screen.  Rethink");
            }
            if (SolutionScreen != null)
            {
                await CloseSpecificChildAsync(SolutionScreen);
                SolutionScreen = null;
            }
            await CloseSpecificChildAsync(OpeningScreen);
        }
        protected override Task GameOverScreenAsync()
        {
            SolutionScreen = MainContainer.Resolve<SolutionViewModel>();
            return LoadScreenAsync(SolutionScreen);
        }
        protected override async Task OpenStartingScreensAsync()
        {
            OpeningScreen = MainContainer.Resolve<MastermindOpeningViewModel>(); //i think has to be this way so its fresh everytime.
            await LoadScreenAsync(OpeningScreen); //try this way.
            await ShowNewGameAsync();
            FinishInit();
        }
    }
}