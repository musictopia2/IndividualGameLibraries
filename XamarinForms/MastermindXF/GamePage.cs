using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGamingUIXFLibrary.Shells;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using MastermindCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using static BasicXFControlsAndPages.Helpers.GridHelper;

namespace MastermindXF
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

        }
        protected override void OrganizeMainGrid()
        {
            AddAutoRows(MainGrid!, 2);
            base.OrganizeMainGrid();
        }
        protected override Task PopulateUIAsync()
        {
            ParentSingleUIContainer parent = new ParentSingleUIContainer(nameof(MastermindShellViewModel.OpeningScreen));

            AddControlToGrid(MainGrid!, parent, 0, 0);
            //looks like we need something else.  because of solution.  plus we need something for the level chosen.
            //this means the only other thing we need is the solution screen.

            parent = new ParentSingleUIContainer(nameof(MastermindShellViewModel.SolutionScreen));
            AddControlToGrid(MainGrid!, parent, 2, 0);
            return Task.CompletedTask;
        }
        protected override void AddMain(ParentSingleUIContainer game)
        {
            AddControlToGrid(MainGrid!, game, 3, 0);
        }
        protected override void AddNewGame(ParentSingleUIContainer game)
        {
            AddControlToGrid(MainGrid!, game, 1, 0);
        }
    }
}
