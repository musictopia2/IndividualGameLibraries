using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.CollectionClasses;
namespace Rummy500CP
{
    public enum EnumWhatSets
    {
        kinds = 1,
        runs = 2
    }
    public struct PointInfo
    {
        public int Points;
        public int NumberOfCards;
    }
    public class SendAddSet
    {
        public int Index { get; set; }
        public int Deck { get; set; }
        public int Position { get; set; }
    }
    public class SendNewSet
    {
        public CustomBasicList<int> DeckList { get; set; } = new CustomBasicList<int>(); // i think this is fine (?)
        public EnumWhatSets SetType { get; set; }
        public bool UseSecond { get; set; }
    }
    public static class CardExtensions
    {
        public static int NegativePoints(this RegularRummyCard ThisCard)
        {
            //i think it should just show the minus amounts.
            if (ThisCard.Value == EnumCardValueList.HighAce)
                return -15;
            if (ThisCard.Value < EnumCardValueList.Eight)
                return -5;
            return -10;
        }
    }
}