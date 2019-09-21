using CommonBasicStandardLibraries.CollectionClasses;
namespace LifeCardGameCP
{
    public class TradeCard
    {
        public int YourCard { get; set; }
        public int OtherCard { get; set; }
    }
    public class Swap
    {
        public int Player { get; set; }
        public CustomBasicList<int> YourCards { get; set; } = new CustomBasicList<int>();
        public CustomBasicList<int> OtherCards { get; set; } = new CustomBasicList<int>();
    }
}