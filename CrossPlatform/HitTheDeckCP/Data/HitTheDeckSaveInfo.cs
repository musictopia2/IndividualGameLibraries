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
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using HitTheDeckCP.Cards;
//i think this is the most common things i like to do
namespace HitTheDeckCP.Data
{
    [SingletonGame]
    public class HitTheDeckSaveInfo : BasicSavedCardClass<HitTheDeckPlayerItem, HitTheDeckCardInformation>
    { //anything needed for autoresume is here.
        public bool HasDrawn { get; set; }
        public bool WasFlipped { get; set; }
        private string _nextPlayer = "";
        public string NextPlayer
        {
            get { return _nextPlayer; }
            set
            {
                if (SetProperty(ref _nextPlayer, value))
                {
                    //can decide what to do when property changes
                    if (_model != null)
                        _model.NextPlayer = value;
                }
            }
        }
        private HitTheDeckVMData? _model;
        public void LoadMod(HitTheDeckVMData model)
        {
            _model = model;
            _model.NextPlayer = NextPlayer;
        }
    }
}