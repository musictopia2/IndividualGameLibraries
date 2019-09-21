using System;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMHelpers;
using static CommonBasicStandardLibraries.MVVMHelpers.Command; //this is used so if you want to know if its still executing, can be done.
using System.Linq; //sometimes i do use linq.
using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;

namespace AccordianSolitaireCP
{
    [SingletonGame]
    public class AccordianSolitaireSaveInfo : ObservableObject
    {
        public CustomBasicList<int> DeckList { get; set; } = new CustomBasicList<int>(); //hopefully now its okay to use decklist since we don't have globals anymore
        //anything else needed to save a game will be here.
        public DeckRegularDict<AccordianSolitaireCardInfo> HandList = new DeckRegularDict<AccordianSolitaireCardInfo>();
        public int DeckSelected { get; set; }
        public int NewestOne { get; set; }
        private int _Score;

        public int Score
        {
            get { return _Score; }
            set
            {
                if (SetProperty(ref _Score, value))
                {
                    //can decide what to do when property changes
                    if (_thisMod != null)
                        _thisMod.Score = value;
                }

            }
        }
        private AccordianSolitaireViewModel? _thisMod;
        public void LoadMod(AccordianSolitaireViewModel thisMod)
        {
            _thisMod = thisMod;
            _thisMod.Score = Score;
        }
    }
}
