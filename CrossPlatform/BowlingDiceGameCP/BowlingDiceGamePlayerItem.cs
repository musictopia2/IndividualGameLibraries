using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using System.Collections.Generic;
namespace BowlingDiceGameCP
{
    public class BowlingDiceGamePlayerItem : SimplePlayer
    { //anything needed is here
        private int _TotalScore;
        public int TotalScore
        {
            get { return _TotalScore; }
            set
            {
                if (SetProperty(ref _TotalScore, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public Dictionary<int, FrameInfoCP> FrameList { get; set; } = new Dictionary<int, FrameInfoCP>();
    }
}