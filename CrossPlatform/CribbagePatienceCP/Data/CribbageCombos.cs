using CommonBasicStandardLibraries.MVVMFramework.ViewModels;

namespace CribbagePatienceCP.Data
{
    public class CribbageCombos : ObservableObject
    {
        private string _description = ""; // this is used for display so its better to show property change safe than sorry
        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                if (SetProperty(ref _description, value) == true)
                {
                }
            }
        }

        // Public Property HandCategory As EnumHandCategory 'needs this so it can be linked to the ui.

        // Public Property CanFromHand As Boolean 'maybe this is fine (?)

        // don't even need this.



        public int NumberNeeded { get; set; }
        public int CardsToUse { get; set; }
        public int NumberInStraight { get; set; }
        public bool IsFlush { get; set; }
        public int NumberForKind { get; set; }
        public bool IsFullHouse { get; set; }
        private int _points; // this is used for display so its better safe than sorry
        public int Points
        {
            get
            {
                return _points;
            }

            set
            {
                if (SetProperty(ref _points, value) == true)
                {
                }
            }
        }



        public bool DoublePairNeeded { get; set; }
        public EnumScoreGroup Group { get; set; } = EnumScoreGroup.NoGroup;
        public EnumJackType JackStatus { get; set; } = EnumJackType.None;
    }
}