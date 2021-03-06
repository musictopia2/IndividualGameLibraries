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
using AccordianSolitaireCP.Logic;
using AccordianSolitaireCP.Data;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using BasicControlsAndWindowsCore.Controls.NavigationControls;
//should not need the view models though.  if i am wrong, rethink.
//i think this is the most common things i like to do
namespace AccordianSolitaireWPF
{
    public class GamePage : SinglePlayerShellView
    {
        protected override void AddMain(ParentSingleUIContainer game)
        {
            AddControlToMainGrid(game, 0, 0);
        }
        protected override void AddNewGame(ParentSingleUIContainer game)
        {
            AddControlToMainGrid(game, 1, 0);
        }
        public GamePage(IGameInfo gameData, BasicData basicData, IStartUp start) : base(gameData, basicData, start)
        {

            


        }
        protected override void OrganizeMainGrid()
        {
            AddAutoRowsMainGrid(2);
            //AddLeftOverRowsMainGrid(1);
        }
        protected override Task PopulateUIAsync()
        {
            //if any exceptions to the shell, do here or override other things.
            return Task.CompletedTask;
        }
    }
}
