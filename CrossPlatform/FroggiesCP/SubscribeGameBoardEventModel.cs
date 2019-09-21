namespace FroggiesCP
{
    public enum EnumDrawCategory
    {
        Redraw, NewLilyList
    }
    public class SubscribeGameBoardEventModel
    {
        public EnumDrawCategory DrawCategory { get; set; }

    }
}