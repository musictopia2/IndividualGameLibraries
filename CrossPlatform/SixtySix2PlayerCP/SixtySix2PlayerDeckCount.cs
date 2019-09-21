using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.CollectionClasses;
namespace SixtySix2PlayerCP
{
    public class CustomDeck : IRegularDeckInfo
    {
        int IRegularDeckInfo.HowManyDecks => 1;

        bool IRegularDeckInfo.UseJokers => false;

        int IRegularDeckInfo.GetExtraJokers => 0;

        int IRegularDeckInfo.LowestNumber => 9;

        int IRegularDeckInfo.HighestNumber => 14;

        public CustomBasicList<ExcludeRCard> ExcludeList => new CustomBasicList<ExcludeRCard>();

        public CustomBasicList<EnumSuitList> SuitList => Helpers.GetCompleteSuitList;

        int IDeckCount.GetDeckCount()
        {
            return 24; //i think.
        }
    }
}