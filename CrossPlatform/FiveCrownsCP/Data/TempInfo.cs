using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using FiveCrownsCP.Cards;

namespace FiveCrownsCP.Data
{
    public struct TempInfo
    {
        public DeckRegularDict<FiveCrownsCardInformation> CardList;
        public int SetNumber { get; set; }
    }
}
