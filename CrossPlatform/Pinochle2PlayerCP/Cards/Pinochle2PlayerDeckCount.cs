using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using CommonBasicStandardLibraries.CollectionClasses;
namespace Pinochle2PlayerCP.Cards
{
    public class CustomDeck : IRegularDeckInfo
    {
        int IRegularDeckInfo.HowManyDecks => 2;

        bool IRegularDeckInfo.UseJokers => false;

        int IRegularDeckInfo.GetExtraJokers => 0;

        int IRegularDeckInfo.LowestNumber => 9;

        int IRegularDeckInfo.HighestNumber => 14;

        public CustomBasicList<ExcludeRCard> ExcludeList => new CustomBasicList<ExcludeRCard>();

        public CustomBasicList<EnumSuitList> SuitList => Helpers.GetCompleteSuitList;

        int IDeckCount.GetDeckCount()
        {
            return 48; //i think.
        }
    }
}