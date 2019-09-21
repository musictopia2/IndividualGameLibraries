using BasicGameFramework.BasicDrawables.Dictionary;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers;
using System.Linq;
namespace CribbagePatienceCP
{
    public class ScoreHandCP : ObservableObject
    {
        public DeckObservableDict<CribbageCard> CardList { get; set; } = new DeckObservableDict<CribbageCard>();
        public CustomBasicCollection<CribbageCombos> Scores { get; set; } = new CustomBasicCollection<CribbageCombos>();
        public DeckRegularDict<CribbageCard> TempList { get; set; } = new DeckRegularDict<CribbageCard>();
        private EnumHandCategory _HandCategory;

        public EnumHandCategory HandCategory
        {
            get { return _HandCategory; }
            set
            {
                if (SetProperty(ref _HandCategory, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        // decided to not do converter becauase otherwise, i have to do twice.  so i might as well have here.
        public string Text
        {
            get
            {
                if (HandCategory == EnumHandCategory.Crib)
                    return "Crib Score";
                if (HandCategory == EnumHandCategory.Hand1)
                    return "Hand 1 Score";
                if (HandCategory == EnumHandCategory.Hand2)
                    return "Hand 2 Score";
                throw new BasicBlankException("Text Not Found");
            }
        }

        public string HandData()
        {
            if (HandCategory == EnumHandCategory.Hand1)
                return "Hand 1";
            else if (HandCategory == EnumHandCategory.Hand2)
                return "Hand 2";
            else if (HandCategory == EnumHandCategory.Crib)
                return "Crib So Far";
            else
                throw new BasicBlankException("Nothing found");
        }

        public (int Row, int Column) GetRowColumn()
        {
            // this is a suggestion.  can change it if necessary
            if (HandCategory == EnumHandCategory.Crib)
                return (0, 1);
            if (HandCategory == EnumHandCategory.Hand1)
                return (1, 1);
            if (HandCategory == EnumHandCategory.Hand2)
                return (1, 2);
            throw new BasicBlankException("Nothing found for the row, column information");
        }
        public int TotalScore()
        {
            return Scores.Sum(x => x.Points);
        }

        public void NewRound() // this should tell the ui to clear (if everything works).
        {
            Scores.Clear();
            CardList.Clear();
            TempList.Clear();
        }
    }
}