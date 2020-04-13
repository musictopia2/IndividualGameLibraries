namespace BladesOfSteelCP.Data
{
    public enum EnumAttackGroup
    {
        HighCard = 0,
        Flush = 1,
        OneTimer = 2,
        BreakAway = 3,
        GreatOne = 4
    }
    public enum EnumDefenseGroup
    {
        HighCard = 0,
        Flush = 1,
        StarDefense = 2,
        StarGoalie = 3
    }
    //computer part.
    public enum EnumFirstStep
    {
        ThrowAwayAllCards = 0,
        PlayAttack = 1,
        PlayDefense = 2
    }
    public enum EnumDefenseStep
    {
        Pass = 0,
        Hand = 1,
        Board = 2
    }
}