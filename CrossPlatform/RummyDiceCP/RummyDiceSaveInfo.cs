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
namespace RummyDiceCP
{
    [SingletonGame]
    public class RummyDiceSaveInfo : BasicSavedGameClass<RummyDicePlayerItem>
    { //anything needed for autoresume is here.
        public CustomBasicCollection<RummyDiceInfo> DiceList = new CustomBasicCollection<RummyDiceInfo>();
        private int _RollNumber;

        public int RollNumber
        {
            get { return _RollNumber; }
            set
            {
                if (SetProperty(ref _RollNumber, value))
                {
                    //can decide what to do when property changes
                    if (_thisMod == null)
                        return;
                    _thisMod.RollNumber = value;
                }

            }
        }
        private RummyDiceViewModel? _thisMod;
        public void LoadMod(RummyDiceViewModel thisMod)
        {
            _thisMod = thisMod;
            thisMod.RollNumber = RollNumber;
        }
        public bool SomeoneFinished { get; set; }
        public CustomBasicList<CustomBasicList<RummyDiceInfo>> TempSets { get; set; } = new CustomBasicList<CustomBasicList<RummyDiceInfo>>();
    }
}