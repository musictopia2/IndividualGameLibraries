using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using CommonBasicStandardLibraries.Exceptions;
namespace CribbageCP
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
        private CribbageViewModel? _thisMod;
        private EnumGameStatus _WhatStatus;
        public EnumGameStatus WhatStatus
        {
            get { return _WhatStatus; }
            set
            {
                if (SetProperty(ref _WhatStatus, value))
                {
                    //can decide what to do when property changes
                    ProcessModStatus();
                }
            }
        }
        private void ProcessModStatus()
        {
            if (_thisMod == null)
                return;
            if (_thisMod.CribFrame == null)
                throw new BasicBlankException("I think the crib frame should have been created first");
            _thisMod.CribFrame.Visible = WhatStatus == EnumGameStatus.GetResultsCrib;
            if (WhatStatus == EnumGameStatus.CardsForCrib && _thisMod.PlayerCount == 2)
                _thisMod.PlayerHand1!.AutoSelect = BasicGameFramework.DrawableListsViewModels.HandViewModel<CribbageCard>.EnumAutoType.SelectAsMany;
            else
                _thisMod.PlayerHand1!.AutoSelect = BasicGameFramework.DrawableListsViewModels.HandViewModel<CribbageCard>.EnumAutoType.SelectOneOnly;
        }
        public void LoadMod(CribbageViewModel thisMod)
        {
            _thisMod = thisMod;
            ProcessModStatus();
        }
    }
}