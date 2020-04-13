using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.ViewModels;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace SpiderSolitaireCP.ViewModels
{
    public class SpiderSolitaireShellViewModel : SinglePlayerShellViewModel
    {
        public SpiderSolitaireShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo GameData, ISaveSinglePlayerClass saves) : base(mainContainer, container, GameData, saves)
        {
        }

        protected override bool AlwaysNewGame => false; //most games allow new game always.
        protected override bool AutoStartNewGame => false;

        protected override IMainScreen GetMainViewModel()
        {
            var model = MainContainer.Resolve<SpiderSolitaireMainViewModel>();
            return model;
        }

        public IScreen? OpeningScreen { get; set; }

        protected override async Task NewGameRequestedAsync()
        {
            if (OpeningScreen == null)
            {
                throw new BasicBlankException("There was no opening screen.  Rethink");
            }
            await CloseSpecificChildAsync(OpeningScreen);
        }

        protected override async Task OpenStartingScreensAsync()
        {
            OpeningScreen = MainContainer.Resolve<SpiderSolitaireOpeningViewModel>(); //i think has to be this way so its fresh everytime.
            await LoadScreenAsync(OpeningScreen); //try this way.
            await ShowNewGameAsync();
            FinishInit();
        }

    }
}
