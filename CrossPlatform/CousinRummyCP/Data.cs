using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.CollectionClasses;
namespace CousinRummyCP
{
    public class SetInfo
    {
        public bool DidSucceed { get; set; }
        public int HowMany { get; set; }
    }
    public class SetList
    {
        public string Description { get; set; } = "";
        public CustomBasicList<SetInfo> PhaseSets = new CustomBasicList<SetInfo>();
    }
    public class SendExpandedSet
    {
        public int Deck { get; set; }
        public int Number { get; set; }
    }
    public struct TempInfo
    {
        public DeckRegularDict<RegularRummyCard> CardList;
    }

}