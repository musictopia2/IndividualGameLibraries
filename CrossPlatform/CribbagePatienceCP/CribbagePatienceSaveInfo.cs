using System;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMHelpers;
using static CommonBasicStandardLibraries.MVVMHelpers.Command; //this is used so if you want to know if its still executing, can be done.
using System.Linq; //sometimes i do use linq.
using BasicGameFramework.Attributes;
using BasicGameFramework.DrawableListsViewModels;

namespace CribbagePatienceCP
{
    [SingletonGame]
    public class CribbagePatienceSaveInfo : ObservableObject
    {
        public CustomBasicList<int> DeckList { get; set; } = new CustomBasicList<int>(); //hopefully now its okay to use decklist since we don't have globals anymore
        //anything else needed to save a game will be here.

        public CustomBasicCollection<int> ScoreList { get; set; } = new CustomBasicCollection<int>();
        public SavedDiscardPile<CribbageCard>? StartCard { get; set; } //this is the data for the start card.

        private CustomBasicList<ScoreHandCP> _HandScores = new CustomBasicList<ScoreHandCP>();

        public CustomBasicList<ScoreHandCP> HandScores
        {
            get { return _HandScores; }
            set
            {
                if (SetProperty(ref _HandScores, value))
                {
                    //can decide what to do when property changes
                    if (_thisMod == null)
                        return;
                    _thisMod.HandScores = value;
                }

            }
        }



        private CribbagePatienceViewModel? _thisMod;
        public void LoadMod(CribbagePatienceViewModel thisMod)
        {
            _thisMod = thisMod;
            _thisMod.HandScores = HandScores;

        }
    }
}
