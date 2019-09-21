using CommonBasicStandardLibraries.MVVMHelpers;
namespace BowlingDiceGameCP
{
    public class SectionInfoCP : ObservableObject
    {
        private string _Score = "";
        public string Score
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