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
namespace ConnectTheDotsCP
{
    [SingletonGame]
    public class ConnectTheDotsSaveInfo : BasicSavedPlainBoardGameClass<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, ConnectTheDotsPlayerItem>
    { //anything needed for autoresume is here.
        public SavedBoardData? BoardData { get; set; }

    }
}