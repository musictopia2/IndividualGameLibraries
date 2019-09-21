namespace XactikaCP
{
    public enum EnumShapes
    {
        None = 0,
        Balls = 1,
        Cubes = 2,
        Cones = 3,
        Stars = 4
    }
    public enum EnumStatusList
    {
        None = 0,
        ChooseGameType = 1,
        Bidding = 2,
        Normal = 3,
        CallShape = 4 // this means a person has played a card but has not called the shape yet
    }
    public enum EnumGameMode
    {
        None = 0, // to begin with
        ToWin = 1,
        ToLose = 2,
        ToBid = 3
    }
}