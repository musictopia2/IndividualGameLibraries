using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.CollectionClasses;
namespace GolfCardGameCP
{
    public class GolfDeck : IRegularDeckInfo
    {
        int IRegularDeckInfo.HowManyDecks => 1;

        bool IRegularDeckInfo.UseJokers => true;

        int IRegularDeckInfo.GetExtraJokers => 0;

        int IRegularDeckInfo.LowestNumber => 1;

        int IRegularDeckInfo.HighestNumber => 13;

        CustomBasicList<ExcludeRCard> IRegularDeckInfo.ExcludeList => new CustomBasicList<ExcludeRCard>();

        CustomBasicList<EnumSuitList> IRegularDeckInfo.SuitList => Helpers.GetCompleteSuitList;

        int IDeckCount.GetDeckCount()
        {
            return 54;
        }
    }
}