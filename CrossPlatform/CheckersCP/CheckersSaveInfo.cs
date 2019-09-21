using System;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMHelpers;
using static CommonBasicStandardLibraries.MVVMHelpers.Command; //this is used so if you want to know if its still executing, can be done.
using System.Linq; //sometimes i do use linq.
using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using BasicGameFramework.GameGraphicsCP.GamePieces;
namespace CheckersCP
{
    [SingletonGame]
    public class CheckersSaveInfo : BasicSavedPlainBoardGameClass<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, CheckersPlayerItem>
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

        public bool ForcedToMove { get; set; } // if forced to move, then it will end turn automatically after making move.
    }
}