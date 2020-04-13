using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using System;

namespace RollEmCP.Data
{
    public class RollEmPlayerItem : SimplePlayer
    { //anything needed is here
        private int _scoreRound;

        public int ScoreRound
        {
            get { return _scoreRound; }
            set
            {
                if (SetProperty(ref _scoreRound, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _scoreGame;

        public int ScoreGame
        {
            get { return _scoreGame; }
            set
            {
                if (SetProperty(ref _scoreGame, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        
    }
}
