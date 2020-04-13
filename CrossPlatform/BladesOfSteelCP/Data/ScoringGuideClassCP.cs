using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;

namespace BladesOfSteelCP.Data
{
    public class ScoringGuideClassCP : ObservableObject
    {
        private string _offenseText = "";
        public string OffenseText
        {
            get
            {
                return _offenseText;
            }

            set
            {
                if (SetProperty(ref _offenseText, value) == true)
                {
                }
            }
        }
        private string _defenseText = "";
        public string DefenseText
        {
            get
            {
                return _defenseText;
            }

            set
            {
                if (SetProperty(ref _defenseText, value) == true)
                {
                }
            }
        }
        private string _borderText = "";
        public string BorderText
        {
            get
            {
                return _borderText;
            }
            set
            {
                if (SetProperty(ref _borderText, value) == true)
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