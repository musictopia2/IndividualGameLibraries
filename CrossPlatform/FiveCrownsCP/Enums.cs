namespace FiveCrownsCP
{
    public enum EnumSuitList
    {
        None = 0, // needs none.  so if suit is none, then may not even draw.
        Clubs = 1,
        Diamonds = 2,
        Spades = 3,
        Hearts = 4,
        Stars = 5
    }
    public enum EnumCardTypeList
    {
        Regular,
        Joker
    }
    public enum EnumCardValueList
    {
        None = 0,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13,
        Joker = 50 // i think this is needed to prevent error
    }
    public enum EnumColorList
    {
        None = 0,
        Red = 1,
        Black = 2,
        Green = 3,
        Blue = 4,
        Yellow = 5
    }
}