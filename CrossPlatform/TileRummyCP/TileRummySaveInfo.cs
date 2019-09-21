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
using BasicGameFramework.DrawableListsViewModels;

namespace TileRummyCP
{
    [SingletonGame]
    public class TileRummySaveInfo : BasicSavedGameClass<TileRummyPlayerItem>
    { //anything needed for autoresume is here.
        public CustomBasicList<SavedSet> SetList { get; set; } = new CustomBasicList<SavedSet>();
        public CustomBasicList<SavedSet> BeginningList { get; set; } = new CustomBasicList<SavedSet>(); //this is at the beginning.
        public SavedScatteringPieces<TileInfo>? PoolData { get; set; }
        public int FirstPlayedLast { get; set; }
        public CustomBasicList<int> TilesFromField { get; set; } = new CustomBasicList<int>();
        public CustomBasicList<int> YourTiles { get; set; } = new CustomBasicList<int>();
        public bool DidExpand { get; set; }
    }
}