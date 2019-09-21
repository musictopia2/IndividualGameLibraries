namespace CribbageCP
{
    public enum EnumGameStatus
    {
        // choosecolor = 1
        None = 0,
        CardsForCrib = 1,
        PlayCard = 2, // only for when playing card that it will save the info
        GetResultsHand = 3,
        GetResultsCrib = 4
    }
    public enum EnumCribbagEequals
    {
        ToEqual = 1,
        ToLessThan = 2,
        ToGreaterThan = 3,
        NA = 4
    }
    public enum EnumScoreGroups
    {
        ScoreFlush = 1,
        ScorePairRuns = 2,
        NoGroup = 0
    }
    public enum EnumPlayType
    {
        InHand = 1,
        InPlay = 2,
        InHandAndCrib = 3,
        AllCombos = 4
    }
}