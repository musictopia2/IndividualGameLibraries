using CommonBasicStandardLibraries.CollectionClasses;
namespace FluxxCP
{
    public class KeeperPlayer
    {
        public int Card { get; set; } // this is the number on the collection.  needs this so can be sent to other players
        public int Player { get; set; }
    } // this may actually be it.

    public class SavedActionClass
    {
        public int SelectedIndex { get; set; }
        public CustomBasicList<int> TempHandList { get; set; } = new CustomBasicList<int>(); // this is the list of cards remaining
        public CustomBasicList<PreviousCard> PreviousList { get; set; } = new CustomBasicList<PreviousCard>();
        public CustomBasicList<int> TempDiscardList { get; set; } = new CustomBasicList<int>();
    }
    public class PreviousCard
    {
        public int Player { get; set; }
        public int Deck { get; set; }
    }
}