namespace SorryCardGameCP.Data
{
    public enum EnumCategory
    {
        Blank = -1,
        Regular = 0,
        Sorry = 5,
        Take2 = 3,
        Play2 = 2,
        Switch = 4,
        Slide = 1,
        Home = 6,
        Start = 7
    }
    public enum EnumSorry
    {
        Blank = -1,
        None = 0,
        Regular = 1,
        Dont = 3,
        At21 = 2,
        OnBoard = 4 // this means its for something else.  well see how this will work (?)
    }
    public enum EnumGameStatus
    {
        Regular = 0,
        ChoosePlayerToSorry = 1, // this is when playing a regular sorry card
        WaitForSorry21 = 2,
        HasDontBeSorry = 3
    }
    public enum EnumColorChoices
    {
        None = 0,
        Blue, Red, Green, Yellow
    }
}