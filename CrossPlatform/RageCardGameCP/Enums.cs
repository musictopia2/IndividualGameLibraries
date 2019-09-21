namespace RageCardGameCP
{
    public enum EnumStatus
    {
        Bidding = 1, Regular, ChooseColor //choosing color is like trump.
    }
    public enum EnumColor
    {
        None, Blue, Red, Green, Yellow, Purple, Orange
    }
    public enum EnumSpecialType
    {
        Blank = -1, None, Out, Change, Wild, Bonus, Mad
    }
}