namespace SkuckCardGameCP
{
    public enum EnumStatusList
    {
        None = 0,
        ChooseTrump = 1, // because once it loads, the computer will already evaluate strength
        ChooseBid = 2,
        ChoosePlay = 3, // this means whoever called the trump because of the strength, gets to decide to play or pass
        NormalPlay = 4,
        WaitForOtherPlayers = 5
    }
    public enum EnumChoiceOption
    {
        None = 0,
        Play = 1,
        Pass = 2
    }
}