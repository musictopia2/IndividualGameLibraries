using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using System.Collections.Generic;
namespace BowlingDiceGameCP.Data
{
    public class FrameInfoCP : ObservableObject
    {
        public Dictionary<int, SectionInfoCP> SectionList = new Dictionary<int, SectionInfoCP>();
        private int _score = -1;
        public int Score
        {
            get { return _score; }
            set
            {
                if (SetProperty(ref _score, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
    }
}
