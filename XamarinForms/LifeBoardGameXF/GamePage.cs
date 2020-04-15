using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGamingUIXFLibrary.Shells;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using LifeBoardGameCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace LifeBoardGameXF
{
    public class GamePage : BasicBoardGamesShellView
    {
        public GamePage(
            IGamePlatform customPlatform,
            IGameInfo gameData,
            BasicData basicData,
            IStartUp start,
            IStandardScreen screen) : base(customPlatform, gameData, basicData, start, screen)
        {
            
        }

        protected override void AddOtherStartingScreens()
        {
            ParentSingleUIContainer parent = new ParentSingleUIContainer(nameof(LifeBoardGameShellViewModel.GenderScreen));
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
