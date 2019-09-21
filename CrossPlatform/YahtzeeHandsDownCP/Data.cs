namespace YahtzeeHandsDownCP
{
    public enum EnumColor
    {
        None, Blue, Red, Yellow, Any
    }
    public interface ICard
    {
        int FirstValue { get; set; }
        int SecondValue { get; set; }
        bool IsWild { get; set; }
        EnumColor Color { get; set; }
    }
    public class YahtzeeResults
    {
        public int NumberUsed { get; set; }
        public int Points { get; set; }
    }
}