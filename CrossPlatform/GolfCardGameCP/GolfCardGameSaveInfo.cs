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
using BasicGameFramework.RegularDeckOfCards;
namespace GolfCardGameCP
{
    [SingletonGame]
    public class GolfCardGameSaveInfo : BasicSavedCardClass<GolfCardGamePlayerItem, RegularSimpleCard>
    { //anything needed for autoresume is here.
        public EnumStatusType GameStatus { get; set; } //still needed here so it can communicate with the client when its next round, etc.
        private int _Round;

        public int Round
        {
            get { return _Round; }
            set
            {
                if (SetProperty(ref _Round, value))
                {
                    //can decide what to do when property changes
                    if (_thisMod == null)
                        return;
                    _thisMod.Round = value;
                }
            }
        }

        private GolfCardGameViewModel? _thisMod;

        internal void LoadMod(GolfCardGameViewModel thisMod)
        {
            _thisMod = thisMod;
            thisMod.Round = Round;
        }
    }
}