using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using System.Collections.Generic;
namespace BowlingDiceGameCP.Data
{
    public class BowlingDiceGamePlayerItem : SimplePlayer
    { //anything needed is here
        private int _totalScore;
        public int TotalScore
        {
            get { return _totalScore; }
            set
            {
                if (SetProperty(ref _totalScore, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public Dictionary<int, FrameInfoCP> FrameList { get; set; } = new Dictionary<int, FrameInfoCP>();
    }
}
