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
using FillOrBustCP.Cards;
using BasicGameFrameworkLibrary.Dice;

namespace FillOrBustCP.Data
{
    [SingletonGame]
    public class FillOrBustSaveInfo : BasicSavedCardClass<FillOrBustPlayerItem, FillOrBustCardInformation>
    { //anything needed for autoresume is here.
        private FillOrBustVMData? _model; //this is needed so it can hook up.
        public int FillsRequired { get; set; }
        public EnumGameStatusList GameStatus { get; set; }
        private int _tempScore;
        public int TempScore
        {
            get { return _tempScore; }
            set
            {
                if (SetProperty(ref _tempScore, value))
                {
                    //can decide what to do when property changes
                    if (_model != null)
                        _model.TempScore = value;
                }

            }
        }
        private int _diceScore;
        public int DiceScore
        {
            get { return _diceScore; }
            set
            {
                if (SetProperty(ref _diceScore, value))
                {
                    //can decide what to do when property changes
                    if (_model != null)
                        _model.DiceScore = value;
                }

            }
        }

        public DiceList<SimpleDice> DiceList = new DiceList<SimpleDice>();
        public void LoadMod(FillOrBustVMData model)
        {
            _model = model;
            _model.DiceScore = DiceScore;
            _model.TempScore = TempScore;
        }
    }
}