namespace MilkRunCP
{
    public enum EnumMilkType
    {
        None = 0, // needs 0 as well otherwise, phone version could have problems.
        Strawberry = 1,
        Chocolate = 2
    }
    public enum EnumCardCategory
    {
        None = 0,
        Points = 1,
        Stop = 2,
        Go = 3,
        Joker = 4
    }
    public enum EnumPileType
    {
        Limit = 1,
        Go = 2,
        Deliveries = 3
    }
    public class SendPlay : PileInfo
    {
        public int Deck { get; set; }
        public int Player { get; set; }
    }
    public class PileInfo
    {
        public EnumMilkType Milk { get; set; }
        public EnumPileType Pile { get; set; }
    }
}