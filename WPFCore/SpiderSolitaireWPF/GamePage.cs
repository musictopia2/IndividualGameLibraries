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
using SpiderSolitaireCP.Logic;
using SpiderSolitaireCP.Data;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //since i use the grid a lot too.
using BasicControlsAndWindowsCore.Controls.NavigationControls;
using SpiderSolitaireCP.ViewModels;
//i think this is the most common things i like to do
namespace SpiderSolitaireWPF
{
    public class GamePage : SinglePlayerShellView
    {

        public GamePage(IGameInfo gameData, BasicGameFrameworkLibrary.BasicGameDataClasses.BasicData basicData, IStartUp start) : base(gameData, basicData, start)
        {

            


        }
        
        protected override Task PopulateUIAsync()
        {
            ParentSingleUIContainer parent = new ParentSingleUIContainer()
            {
                Name = nameof(SpiderSolitaireShellViewModel.OpeningScreen)
            };
            AddControlToGrid(MainGrid!, parent, 0, 0);
            
            return Task.CompletedTask;
        }
        //protected override void AddMain(ParentSingleUIContainer game)
        //{
        //    AddControlToGrid(MainGrid!, game, 3, 0);
        //}
        protected override void AddNewGame(ParentSingleUIContainer game)
        {
            AddControlToGrid(MainGrid!, game, 1, 0);
        }
    }
}
