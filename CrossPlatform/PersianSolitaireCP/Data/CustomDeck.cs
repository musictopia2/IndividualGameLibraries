using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using CommonBasicStandardLibraries.CollectionClasses;
namespace PersianSolitaireCP.Data
{
    public class CustomDeck : IRegularDeckInfo
    {
        int IRegularDeckInfo.HowManyDecks => 2; //most games use one deck.  if there is more than one deck, put here.

        bool IRegularDeckInfo.UseJokers => false;

        int IRegularDeckInfo.GetExtraJokers => 0;

        int IRegularDeckInfo.LowestNumber => 7;

        int IRegularDeckInfo.HighestNumber => 14; //still low but needs to show 14 so it would work properly.



        CustomBasicList<ExcludeRCard> IRegularDeckInfo.ExcludeList => new CustomBasicList<ExcludeRCard>();

        CustomBasicList<EnumSuitList> IRegularDeckInfo.SuitList => Helpers.GetCompleteSuitList;

        int IDeckCount.GetDeckCount()
        {
            return 64;
        }
    }
}
