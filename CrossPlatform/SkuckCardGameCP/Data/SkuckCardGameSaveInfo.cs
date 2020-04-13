using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using SkuckCardGameCP.Cards;
namespace SkuckCardGameCP.Data
{
    [SingletonGame]
    public class SkuckCardGameSaveInfo : BasicSavedTrickGamesClass<EnumSuitList, SkuckCardGameCardInformation, SkuckCardGamePlayerItem>
    { //anything needed for autoresume is here.
        private int _roundNumber;

        public int RoundNumber
        {
            get { return _roundNumber; }
            set
            {
                if (SetProperty(ref _roundNumber, value))
                {
                    //can decide what to do when property changes
                    if (_model != null)
                        _model.RoundNumber = value;
                }

            }
        }

        private EnumStatusList _whatStatus;

        public EnumStatusList WhatStatus
        {
            get { return _whatStatus; }
            set
            {
                if (SetProperty(ref _whatStatus, value))
                {
                    //can decide what to do when property changes
                    if (_model != null)
                        _model.GameStatus = WhatStatus;
                }
            }
        }
        private SkuckCardGameVMData? _model;
        public void LoadMod(SkuckCardGameVMData model)
        {
            _model = model;
            _model.RoundNumber = RoundNumber;
            _model.GameStatus = WhatStatus;
        }
    }
}