using System;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMHelpers;
using static CommonBasicStandardLibraries.MVVMHelpers.Command; //this is used so if you want to know if its still executing, can be done.
using System.Linq; //sometimes i do use linq.
using BasicGameFramework.Attributes;
using BaseSolitaireClassesCP.TriangleClasses;
using BaseSolitaireClassesCP.Cards;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.BasicDrawables.Dictionary;

namespace PyramidSolitaireCP
{
    [SingletonGame]
    public class PyramidSolitaireSaveInfo : ObservableObject
    {
        public CustomBasicList<int> DeckList { get; set; } = new CustomBasicList<int>(); //hopefully now its okay to use decklist since we don't have globals anymore

        private PyramidSolitaireViewModel? _thisMod;

        public SavedDiscardPile<SolitaireCard>? DiscardPileData { get; set; }
        public SavedDiscardPile<SolitaireCard>? CurrentPileData { get; set; }
        public SavedTriangle? TriangleData { get; set; }
        //public DeckRegularDict<SolitaireCard> TriangleList { get; set; } = new DeckRegularDict<SolitaireCard>();
        public DeckRegularDict<SolitaireCard> PlayList { get; set; } = new DeckRegularDict<SolitaireCard>();

        public SolitaireCard RecentCard { get; set; } = new SolitaireCard();

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
        private bool _CanEnableGame;

        public bool CanEnableGame
        {
            get { return _CanEnableGame; }
            set
            {
                if (SetProperty(ref _CanEnableGame, value))
                {
                    //can decide what to do when property changes
                    if (_thisMod != null)
                        _thisMod.CanEnableGame = value;
                }

            }
        }
        public int FirstDeck { get; set; }
        public int SecondDeck { get; set; }

        public void LoadMod(PyramidSolitaireViewModel thisMod)
        {
            _thisMod = thisMod;
            _thisMod.Score = Score;
            _thisMod.CanEnableGame = CanEnableGame;
        }
    }
}
