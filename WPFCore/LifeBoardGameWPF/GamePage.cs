using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGamingUIWPFLibrary.Shells;
using LifeBoardGameCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace LifeBoardGameWPF
{
    public class GamePage : BasicBoardGamesShellView
    {
        public GamePage(IGameInfo gameData,
            BasicData basicData,
            IStartUp start) : base(gameData, basicData, start)
        {
        }
        protected override void AddOtherStartingScreens()
        {
            ParentSingleUIContainer parent = new ParentSingleUIContainer()
            {
                Name = nameof(LifeBoardGameShellViewModel.GenderScreen)
            };
            AddMain(parent);
            base.AddOtherStartingScreens();
        }

        protected override Task PopulateUIAsync()
        {
            //if any exceptions to the shell, do here or override other things.
            return Task.CompletedTask;
        }
    }
}
