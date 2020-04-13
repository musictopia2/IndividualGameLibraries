using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using CommonBasicStandardLibraries.CollectionClasses;

namespace PickelCardGameCP.Cards
{
    public class CustomDeck : IRegularDeckInfo
    {
        int IRegularDeckInfo.HowManyDecks => 1;

        bool IRegularDeckInfo.UseJokers => true;

        int IRegularDeckInfo.GetExtraJokers => 0;

        int IRegularDeckInfo.LowestNumber => 2;

        int IRegularDeckInfo.HighestNumber => 14;

        CustomBasicList<ExcludeRCard> IRegularDeckInfo.ExcludeList => new CustomBasicList<ExcludeRCard>();

        CustomBasicList<EnumSuitList> IRegularDeckInfo.SuitList => Helpers.GetCompleteSuitList;

        int IDeckCount.GetDeckCount()
        {
            return 54; //because of 2 jokers.
        }
    }
}
