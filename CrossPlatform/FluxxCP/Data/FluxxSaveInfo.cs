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
using FluxxCP.Cards;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using FluxxCP.Containers;
//i think this is the most common things i like to do
namespace FluxxCP.Data
{
    [SingletonGame]
    public class FluxxSaveInfo : BasicSavedCardClass<FluxxPlayerItem, FluxxCardInformation>
    { //anything needed for autoresume is here.
        public DeckObservableDict<GoalCard> GoalList { get; set; } = new DeckObservableDict<GoalCard>();
        public DeckObservableDict<RuleCard> RuleList { get; set; } = new DeckObservableDict<RuleCard>();
        public CustomBasicList<int> DelayedPlayList { get; set; } = new CustomBasicList<int>();
        public bool DoAnalyze { get; set; }
        public CustomBasicList<int> QueList { get; set; } = new CustomBasicList<int>();
        //i still need the savedactionclass for autoresume though.

        public SavedActionClass SavedActionData { get; set; } = new SavedActionClass(); //forgot this part.
        public int CurrentAction { get; set; }
        private int _playsLeft;
        public int PlaysLeft
        {
            get { return _playsLeft; }
            set
            {
                if (SetProperty(ref _playsLeft, value))
                {
                    if (_model != null)
                        _model.PlaysLeft = value;
                }
            }
        }
        private int _handLimit;
        public int HandLimit
        {
            get { return _handLimit; }
            set
            {
                if (SetProperty(ref _handLimit, value))
                {
                    if (_model != null)
                        _model.HandLimit = value;
                }
            }
        }
        private int _keeperLimit;
        public int KeeperLimit
        {
            get { return _keeperLimit; }
            set
            {
                if (SetProperty(ref _keeperLimit, value))
                {
                    if (_model != null)
                        _model.KeeperLimit = value;
                }
            }
        }
        private int _playLimit;
        public int PlayLimit
        {
            get { return _playLimit; }
            set
            {
                if (SetProperty(ref _playLimit, value))
                {
                    if (_model != null)
                        _model.PlayLimit = value;
                }
            }
        }
        private bool _anotherTurn;
        public bool AnotherTurn
        {
            get { return _anotherTurn; }
            set
            {
                if (SetProperty(ref _anotherTurn, value))
                {
                    if (_model != null)
                        _model.AnotherTurn = value;
                }
            }
        }
        private int _drawBonus;
        public int DrawBonus
        {
            get { return _drawBonus; }
            set
            {
                if (SetProperty(ref _drawBonus, value))
                {
                    if (_model != null)
                        _model.DrawBonus = value;
                }
            }
        }
        private int _playBonus;
        public int PlayBonus
        {
            get { return _playBonus; }
            set
            {
                if (SetProperty(ref _playBonus, value))
                {
                    if (_model != null)
                        _model.PlayBonus = value;
                }
            }
        }
        private int _cardsDrawn;
        public int CardsDrawn
        {
            get { return _cardsDrawn; }
            set
            {
                if (SetProperty(ref _cardsDrawn, value))
                {
                    if (_model != null)
                        _model.CardsDrawn = value;
                }
            }
        }
        private int _cardsPlayed;
        public int CardsPlayed
        {
            get { return _cardsPlayed; }
            set
            {
                if (SetProperty(ref _cardsPlayed, value))
                {
                    if (_model != null)
                        _model.CardsPlayed = value;
                }
            }
        }
        private int _drawRules;
        public int DrawRules
        {
            get { return _drawRules; }
            set
            {
                if (SetProperty(ref _drawRules, value))
                {
                    //can decide what to do when property changes
                    if (_model != null)
                        _model.DrawRules = value;
                }
            }
        }
        private int _previousBonus;
        public int PreviousBonus
        {
            get { return _previousBonus; }
            set
            {
                if (SetProperty(ref _previousBonus, value))
                {
                    //can decide what to do when property changes
                    if (_model != null)
                        _model.PreviousBonus = value;
                }
            }
        }
        private FluxxVMData? _model;
        internal void LoadMod(FluxxVMData model)
        {
            model.PlaysLeft = PlaysLeft;
            model.HandLimit = HandLimit;
            model.KeeperLimit = KeeperLimit;
            model.PlayLimit = PlayLimit;
            model.AnotherTurn = AnotherTurn;
            model.DrawBonus = DrawBonus;
            model.PlayBonus = PlayBonus;
            model.CardsDrawn = CardsDrawn;
            model.CardsPlayed = CardsPlayed;
            model.DrawRules = DrawRules;
            model.PreviousBonus = PreviousBonus;
            _model = model;
        }
    }

}