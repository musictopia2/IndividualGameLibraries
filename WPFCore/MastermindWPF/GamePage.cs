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
using BasicGamingUIWPFLibrary.Shells;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using MastermindCP.Logic;
using MastermindCP.Data;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.ViewModels;
using MastermindCP.ViewModels;
using static BasicControlsAndWindowsCore.Helpers.GridHelper;
using System.Windows;
//should not need the view models though.  if i am wrong, rethink.
//i think this is the most common things i like to do
namespace MastermindWPF
{
    public class GamePage : SinglePlayerShellView
    {
        //we may have to think about loading information.
        //until i have some experience, not sure what to do (?)

        public GamePage(IGameInfo gameData, BasicData basicData, IStartUp start) : base(gameData, basicData, start)
        {

        }
        protected override void OrganizeMainGrid()
        {
            AddAutoRows(MainGrid!, 2);
            base.OrganizeMainGrid();
        }
        protected override Task PopulateUIAsync()
        {
            ParentSingleUIContainer parent = new ParentSingleUIContainer()
            {
                Name = nameof(MastermindShellViewModel.OpeningScreen)
            };
            AddControlToGrid(MainGrid!, parent, 0, 0);
            //looks like we need something else.  because of solution.  plus we need something for the level chosen.
            //this means the only other thing we need is the solution screen.

            parent = new ParentSingleUIContainer()
            {
                Name = nameof(MastermindShellViewModel.SolutionScreen),
                Margin = new Thickness(10)
            };
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