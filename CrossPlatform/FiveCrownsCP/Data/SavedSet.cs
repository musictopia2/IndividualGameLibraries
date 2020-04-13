using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using FiveCrownsCP.Cards;

namespace FiveCrownsCP.Data
{
    public class SavedSet
    {
        public DeckRegularDict<FiveCrownsCardInformation> CardList = new DeckRegularDict<FiveCrownsCardInformation>();
    }
}