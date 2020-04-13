namespace ClueBoardGameCP.Data
{
    public enum EnumWeaponList
    {
        None, Candlestick, Knife, Rope, Wrench, Revolver, LeadPipe
    }
    public enum EnumClueStatusList
    {
        None, LoadGame, StartTurn, DiceRolled, MoveSpaces, MakePrediction, FindClues, EndTurn, EndGame
    }
    public enum EnumPositionInfo
    {
        None, Top, Bottom, Left, Right
    }
    public enum EnumNameList
    {
        None, Peacock, Green, Plum, Scarlet, White, Mustard
    }
    public enum EnumCardType
    {
        IsRoom = 1, IsWeapon, IsCharacter
    }
    public enum EnumCardValues
    {
        None,
        Peacock,
        Green,
        Plum,
        Scarlet,
        White,
        Colonel,
        Candlestick,
        Knife,
        Rope,
        Wrench,
        Revolver,
        LeadPipe,
        Kitchen,
        BallRoom,
        Conservatory,
        BilliardRoom,
        Library,
        Study,
        Hall,
        Lounge,
        DiningRoom
    }
}