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
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.DrawableListsObservable;
//i think this is the most common things i like to do
namespace CribbageCP.Data
{
    [SingletonGame]
    public class CribbageSaveInfo : BasicSavedCardClass<CribbagePlayerItem, CribbageCard>
    { //anything needed for autoresume is here.
        public bool IsStart { get; set; }
        public bool StartOver { get; set; }
        public int NewTurn { get; set; }
        public int Dealer { get; set; }
        public bool IsCorrect { get; set; }
        public DeckRegularDict<CribbageCard> CribList { get; set; } = new DeckRegularDict<CribbageCard>();
        public DeckRegularDict<CribbageCard> MainList { get; set; } = new DeckRegularDict<CribbageCard>();
        public DeckRegularDict<CribbageCard> MainFrameList { get; set; } = new DeckRegularDict<CribbageCard>();
        private CribbageVMData? _model;
        private EnumGameStatus _whatStatus;
        public EnumGameStatus WhatStatus
        {
            get { return _whatStatus; }
            set
            {
                if (SetProperty(ref _whatStatus, value))
                {
                    //can decide what to do when property changes
                    ProcessModStatus();
                }
            }
        }
        private void ProcessModStatus()
        {
            if (_model == null)
                return;
            if (_model.CribFrame == null)
                throw new BasicBlankException("I think the crib frame should have been created first");
            _model.CribFrame.Visible = WhatStatus == EnumGameStatus.GetResultsCrib;
            if (WhatStatus == EnumGameStatus.CardsForCrib && _gameContainer!.PlayerList!.Count == 2)
                _model.PlayerHand1!.AutoSelect = HandObservable<CribbageCard>.EnumAutoType.SelectAsMany;
            else
                _model.PlayerHand1!.AutoSelect = HandObservable<CribbageCard>.EnumAutoType.SelectOneOnly;
        }
        private CribbageGameContainer? _gameContainer;
        public void LoadMod(CribbageVMData model, CribbageGameContainer gameContainer)
        {
            _gameContainer = gameContainer;
            _model = model;
            ProcessModStatus();
        }
    }
}