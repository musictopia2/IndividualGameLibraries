using CommonBasicStandardLibraries.MVVMHelpers;
using System.Collections.Generic;
namespace BowlingDiceGameCP
{
    public class FrameInfoCP : ObservableObject
    {
        public Dictionary<int, SectionInfoCP> SectionList = new Dictionary<int, SectionInfoCP>();
        private int _Score = -1;
        public int Score
        {
            get { return _Score; }
            set
            {
                if (SetProperty(ref _Score, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
    }
}