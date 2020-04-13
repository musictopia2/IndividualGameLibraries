using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using CommonBasicStandardLibraries.CollectionClasses;

namespace CousinRummyCP.Logic
{
    public class CustomDeck : IRegularDeckInfo //use smaller cards
    {
        int IRegularDeckInfo.HowManyDecks => 2; //ace should be high on this one.

        bool IRegularDeckInfo.UseJokers => true;

        int IRegularDeckInfo.GetExtraJokers => 2;

        int IRegularDeckInfo.LowestNumber => 2;

        int IRegularDeckInfo.HighestNumber => 14;

        CustomBasicList<ExcludeRCard> IRegularDeckInfo.ExcludeList => new CustomBasicList<ExcludeRCard>();

        CustomBasicList<EnumSuitList> IRegularDeckInfo.SuitList => Helpers.GetCompleteSuitList;
        int IDeckCount.GetDeckCount()
        {
            return 110;  //decided 2 extra jokers.
        }
    }
}