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
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using ChessCP.Logic;
//i think this is the most common things i like to do
namespace ChessCP.Data
{
    [SingletonGame]
    public class ChessSaveInfo : BasicSavedGameClass<ChessPlayerItem>
    { //anything needed for autoresume is here.
        public int SpaceHighlighted
        {
            get
            {
                return GameBoardGraphicsCP.SpaceSelected;
            }
            set
            {
                GameBoardGraphicsCP.SpaceSelected = value;
            }
        }
        public EnumGameStatus GameStatus { get; set; }
        public PreviousMove? PossibleMove { get; set; }
        public PreviousMove PreviousMove { get; set; } = new PreviousMove();
    }
}