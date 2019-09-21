using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
namespace SpiderSolitaireCP
{
    public class CustomDeck : IRegularDeckInfo
    {
        int IRegularDeckInfo.HowManyDecks
        {
            get
            {
                if (_thisMod.LevelChosen == 3)
                    return 2;
                if (_thisMod.LevelChosen == 2)
                    return 4;
                if (_thisMod.LevelChosen == 1)
                    return 8;
                throw new BasicBlankException("Needs levels 1, 2, or 3 for figuring out how many decks");
            }
        }

        bool IRegularDeckInfo.UseJokers => false;

        int IRegularDeckInfo.GetExtraJokers => 0;

        int IRegularDeckInfo.LowestNumber => 1;

        int IRegularDeckInfo.HighestNumber => 13; //aces will usually be low.

        private readonly SpiderSolitaireViewModel _thisMod;

        public CustomDeck(SpiderSolitaireViewModel thisMod)
        {
            _thisMod = thisMod;
        }
        CustomBasicList<ExcludeRCard> IRegularDeckInfo.ExcludeList => new CustomBasicList<ExcludeRCard>();

        CustomBasicList<EnumSuitList> IRegularDeckInfo.SuitList
        {
            get
            {
                if (_thisMod.LevelChosen == 1)
                    return new CustomBasicList<EnumSuitList> { EnumSuitList.Spades };
                if (_thisMod.LevelChosen == 2)
                    return new CustomBasicList<EnumSuitList> { EnumSuitList.Spades, EnumSuitList.Hearts };
                if (_thisMod.LevelChosen != 3)
                    throw new BasicBlankException("Only 3 levels are supposed for the suit list");
                var tempList = Helpers.GetCompleteSuitList;
                return tempList;
            }
        }

        int IDeckCount.GetDeckCount()
        {
            return 104;
        }
    }
}
