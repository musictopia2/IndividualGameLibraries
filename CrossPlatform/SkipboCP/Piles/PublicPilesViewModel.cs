using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.MultiplePilesObservable;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using SkipboCP.Cards;

namespace SkipboCP.Piles
{
    public class PublicPilesViewModel : BasicMultiplePilesCP<SkipboCardInformation>
    {
        public void UnselectAllPiles()
        {
            int x;
            var loopTo = PileList!.Count;
            for (x = 1; x <= loopTo; x++)
                UnselectPile(x - 1);// because 0 based
        }
        public DeckObservableDict<SkipboCardInformation> CardsFromPile(int pile)
        {
            var output = new DeckObservableDict<SkipboCardInformation>();
            var thisPile = PileList![pile];
            output.AddRange(thisPile.ObjectList);
            thisPile.ObjectList.Clear();
            if (output.Count == 0)
                throw new BasicBlankException("Can't clear the list out");
            return output;
        }
        public int NextNumberNeeded(int whichOne) // has to send in 0 based because inherited now.
        {
            var thisPile = PileList![whichOne];
            if (thisPile.ObjectList.Count > 12)
                throw new BasicBlankException("Should have cleared out the piles");
            return thisPile.ObjectList.Count + 1;
        }
        public bool NeedToRemovePile(int whichOne)
        {
            var thisPile = PileList![whichOne];
            if (thisPile.ObjectList.Count < 12)
                return false;
            return true;
        }
        public DeckObservableDict<SkipboCardInformation> EmptyPileList(int whichOne)
        {
            var thisPile = PileList![whichOne];
            if (thisPile.ObjectList.Count != 12)
                throw new BasicBlankException($"Must have 12 cards to empty a pile; not {thisPile.ObjectList.Count}");
            var tempList = thisPile.ObjectList.ToObservableDeckDict();
            thisPile.ObjectList.Clear(); // i think this is fine
            if (tempList.Count != 12)
                throw new BasicBlankException($"Must have 12 cards in the list, not {tempList.Count}");
            return tempList;
        }
        public PublicPilesViewModel(CommandContainer command, IEventAggregator aggregator) : base(command, aggregator)
        {
            Style = EnumStyleList.HasList;
            Rows = 1;
            HasFrame = true;
            HasText = true; // does have frame and text.
            Columns = 4;
            LoadBoard();
            int x = 0;
            if (PileList!.Count != 4)
                throw new BasicBlankException("Should have had 4 piles");
            foreach (var thisPile in PileList)
            {
                x += 1;
                thisPile.Text = "Pile " + x;
            }
        }
    }
}