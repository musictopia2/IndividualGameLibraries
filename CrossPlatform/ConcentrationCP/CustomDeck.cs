using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.CollectionClasses;
namespace ConcentrationCP
{
    public class CustomDeck : IRegularDeckInfo
    {
        public int HowManyDecks => 1;
        public bool UseJokers => false;
        public int GetExtraJokers => 0;
        public int LowestNumber => 1; //ordinary, ace will be low.  in cases wheer
        public int HighestNumber => 13;
        CustomBasicList<ExcludeRCard> IRegularDeckInfo.ExcludeList
        {
            get
            {
                CustomBasicList<ExcludeRCard> output = new CustomBasicList<ExcludeRCard>();
                output.AppendExclude(EnumSuitList.Clubs, 1)
                    .AppendExclude(EnumSuitList.Spades, 1);
                return output;
            }
        }
        public CustomBasicList<EnumSuitList> SuitList => Helpers.GetCompleteSuitList;
        public int GetDeckCount()
        {
            return 50; //this has 52 cards in a standard deck.
        }
    }
}