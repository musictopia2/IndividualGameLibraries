namespace HitTheDeckCP.Data
{
    public enum EnumInstructionList
    {
        None = 1, // this means a number card has been played
        PlayNumber = 2, // this means must play a number
        PlayColor = 3, // this means must play a color
        Flip = 4, // this means will flip then resume
        Cut = 5, // this means will cut the deck then resume
        RandomDraw = 6, // this means someone will draw 4 cards
    }
    public enum EnumTypeList
    {
        None = 0, // this is needed to support mobile.  try this way.
        Regular = 1,
        Cut = 2,
        Flip = 3,
        Number = 4,
        Color = 5,
        Draw4 = 6
    }
}