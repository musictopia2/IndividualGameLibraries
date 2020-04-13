using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using CommonBasicStandardLibraries.CollectionClasses;

namespace OldMaidCP.Data
{
    public class OldMaidDeck : IRegularDeckInfo
    {
        int IRegularDeckInfo.HowManyDecks => 1;

        bool IRegularDeckInfo.UseJokers => false;

        int IRegularDeckInfo.GetExtraJokers => 0;

        int IRegularDeckInfo.LowestNumber => 2;

        int IRegularDeckInfo.HighestNumber => 14;

        CustomBasicList<ExcludeRCard> IRegularDeckInfo.ExcludeList
        {
            get
            {
                CustomBasicCollection<ExcludeRCard> output = new CustomBasicCollection<ExcludeRCard>();
                output.AppendExclude(EnumSuitList.Clubs, 12)
                    .AppendExclude(EnumSuitList.Diamonds, 12)
                    .AppendExclude(EnumSuitList.Hearts, 12);
                return output;
            }
        }
        CustomBasicList<EnumSuitList> IRegularDeckInfo.SuitList => Helpers.GetCompleteSuitList;
        int IDeckCount.GetDeckCount()
        {
            return 49;
        }
    }
}
