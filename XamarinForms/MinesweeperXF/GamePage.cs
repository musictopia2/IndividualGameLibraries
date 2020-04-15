using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGamingUIXFLibrary.Shells;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using MinesweeperCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace MinesweeperXF
{
    public class GamePage : SinglePlayerShellView
    {
        public GamePage(
            IGamePlatform customPlatform,
            IGameInfo gameData,
            BasicData basicData,
            IStartUp start,
            IStandardScreen screen) : base(customPlatform, gameData, basicData, start, screen)
        {
            //TODO:  may need to think about if we need load or update (?)
        }



        protected override Task PopulateUIAsync()
        {
            ParentSingleUIContainer parent = new ParentSingleUIContainer(nameof(MinesweeperShellViewModel.OpeningScreen));
            AddMain(parent); //hopefully this works.  since its one or the other but never both the same time.
            return Task.CompletedTask;
        }
    }
}
