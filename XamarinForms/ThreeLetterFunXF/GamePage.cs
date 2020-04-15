using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGamingUIXFLibrary.Shells;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ThreeLetterFunCP.ViewModels;

namespace ThreeLetterFunXF
{
    public class GamePage : MultiplayerBasicShellView
    {
        public GamePage(
            IGamePlatform customPlatform,
            IGameInfo gameData,
            BasicData basicData,
            IStartUp start,
            IStandardScreen screen) : base(customPlatform, gameData, basicData, start, screen)
        {
        }



        protected override Task PopulateUIAsync()
        {
            ParentSingleUIContainer firsts = new ParentSingleUIContainer(nameof(ThreeLetterFunShellViewModel.FirstScreen));
            AddMain(firsts);
            ParentSingleUIContainer advanced = new ParentSingleUIContainer(nameof(ThreeLetterFunShellViewModel.AdvancedScreen));
            AddMain(advanced);
            ParentSingleUIContainer cards = new ParentSingleUIContainer(nameof(ThreeLetterFunShellViewModel.CardsScreen));
            AddMain(cards);
            return Task.CompletedTask;
        }
    }
}
