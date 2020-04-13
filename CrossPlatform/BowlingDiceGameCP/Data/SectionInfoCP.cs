using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
namespace BowlingDiceGameCP.Data
{
    public class SectionInfoCP : ObservableObject
    {
        private string _score = "";
        public string Score
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
