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
using BasicGameFramework.BasicDrawables.Dictionary;

namespace OpetongCP
{
    [SingletonGame]
    public class OpetongSaveInfo : BasicSavedCardClass<OpetongPlayerItem, RegularRummyCard>
    { //anything needed for autoresume is here.
        public DeckRegularDict<RegularRummyCard> PoolList { get; set; } = new DeckRegularDict<RegularRummyCard>();
        public CustomBasicList<SavedSet> SetList { get; set; } = new CustomBasicList<SavedSet>();
        public bool FirstTurn { get; set; }
        public int WhichPart { get; set; }
    }
}