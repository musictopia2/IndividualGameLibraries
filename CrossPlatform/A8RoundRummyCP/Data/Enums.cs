namespace A8RoundRummyCP.Data
{
    public enum EnumRummyType
    {
        Regular = 1,
        Straight = 2,
        Kinds = 3
    }
    public enum EnumCategory
    {
        None = 0,
        Colors = 1,
        Shapes = 2,
        Both = 3
    }
    public enum EnumPlayerStatus
    {
        Regular = 0,
        Reversed = 1,
        WentOut = 2
    }
    public enum EnumCardType
    {
        None = -1,
        Regular = 0,
        Wild = 1,
        Reverse = 2
    }
    public enum EnumCardShape
    {
        Blank = -1,
        None = 0,
        Circle = 1,
        Triangle = 2,
        Square = 3
    }
    public enum EnumColor
    {
        Blank = -1, // i think
        None = 0,
        Blue = 1,
        Red = 2
    }
}