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
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using LifeBoardGameCP.Cards;

namespace LifeBoardGameCP.Data
{
    [SingletonGame]
    public class LifeBoardGameSaveInfo : BasicSavedGameClass<LifeBoardGamePlayerItem>
    { //anything needed for autoresume is here.


        private EnumWhatStatus _GameStatus;
        public EnumWhatStatus GameStatus
        {
            get { return _GameStatus; }
            set
            {
                if (SetProperty(ref _GameStatus, value))
                {
                    //can decide what to do when property changes
                    if (_model != null)
                    {
                        _model.GameStatus = value;
                    }

                }
            }
        }
        public CustomBasicList<TileInfo> TileList { get; set; } = new CustomBasicList<TileInfo>();
        public DeckRegularDict<HouseInfo> HouseList { get; set; } = new DeckRegularDict<HouseInfo>();
        public bool WasMarried { get; set; }
        public bool GameStarted { get; set; }
        public bool WasNight { get; set; }
        public int MaxChosen { get; set; }
        public int NewPosition { get; set; }
        public bool EndAfterSalary { get; set; }
        public bool EndAfterStock { get; set; }
        public int NumberRolled { get; set; } // i think this is needed as well
        public int SpinPosition { get; set; }
        public int ChangePosition { get; set; }
        public CustomBasicList<int> SpinList { get; set; } = new CustomBasicList<int>(); // needs this.  so based on career chosen, they can get the 100,000.
        public int TemporarySpaceSelected { get; set; }
        private LifeBoardGameVMData? _model;
        internal void LoadMod(LifeBoardGameVMData model)
        {
            _model = model;
            _model.GameStatus = GameStatus;
        }
    }
}