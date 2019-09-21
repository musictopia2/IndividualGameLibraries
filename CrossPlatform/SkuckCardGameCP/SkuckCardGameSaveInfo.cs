using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using BasicGameFramework.RegularDeckOfCards;
namespace SkuckCardGameCP
{
    [SingletonGame]
    public class SkuckCardGameSaveInfo : BasicSavedTrickGamesClass<EnumSuitList, SkuckCardGameCardInformation, SkuckCardGamePlayerItem>
    { //anything needed for autoresume is here.
        private int _RoundNumber;

        public int RoundNumber
        {
            get { return _RoundNumber; }
            set
            {
                if (SetProperty(ref _RoundNumber, value))
                {
                    //can decide what to do when property changes
                    if (_thisMod != null)
                        _thisMod.RoundNumber = value;
                }

            }
        }

        private EnumStatusList _WhatStatus;

        public EnumStatusList WhatStatus
        {
            get { return _WhatStatus; }
            set
            {
                if (SetProperty(ref _WhatStatus, value))
                {
                    //can decide what to do when property changes
                    if (_thisMod != null)
                        _thisMod.ShowVisibleChange();
                }
            }
        }
        private SkuckCardGameViewModel? _thisMod;
        public void LoadMod(SkuckCardGameViewModel ThisMod)
        {
            _thisMod = ThisMod;
            ThisMod.RoundNumber = RoundNumber;
            ThisMod.ShowVisibleChange(); //i think here too.
        }
    }
}