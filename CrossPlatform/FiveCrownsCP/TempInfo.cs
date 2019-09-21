using BasicGameFramework.BasicDrawables.Dictionary;
namespace FiveCrownsCP
{
    public struct TempInfo
    {
        public DeckRegularDict<FiveCrownsCardInformation> CardList;
        public int SetNumber { get; set; }
    }
}