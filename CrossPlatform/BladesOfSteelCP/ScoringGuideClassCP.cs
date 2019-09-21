using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMHelpers;
namespace BladesOfSteelCP
{
    public class ScoringGuideClassCP : ObservableObject
    {
        private string _OffenseText = "";
        public string OffenseText
        {
            get
            {
                return _OffenseText;
            }

            set
            {
                if (SetProperty(ref _OffenseText, value) == true)
                {
                }
            }
        }
        private string _DefenseText = "";
        public string DefenseText
        {
            get
            {
                return _DefenseText;
            }

            set
            {
                if (SetProperty(ref _DefenseText, value) == true)
                {
                }
            }
        }
        private string _BorderText = "";
        public string BorderText
        {
            get
            {
                return _BorderText;
            }
            set
            {
                if (SetProperty(ref _BorderText, value) == true)
                {
                }
            }
        }
        public CustomBasicList<string> OffenseList()
        {
            return new CustomBasicList<string>() { "The Great One:  2 red nines", "Breakaway:  Red Ace plus card of same suit", "One-Timer:  2 red cards of same rank", "3 card flush:  3 red cards of same suit", "High Card:  Any 3 red cards" };
        }
        public CustomBasicList<string> DefenseList()
        {
            return new CustomBasicList<string>() { "(The Great One is an automatic goal)", "Star goalie:  1 black ace", "Star defense:  2 black cards of same rank", "3 card flush:  3 black cards", "High Card:  Any 3 black cards" };
        }

        public ScoringGuideClassCP()
        {
            OffenseText = "Offense Levels:";
            DefenseText = "Defense Levels:";
        }
    }
}