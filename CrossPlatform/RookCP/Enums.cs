namespace RookCP
{
    public enum EnumStatusList
    {
        None, Bidding, ChooseTrump, SelectNest, Normal
        //only the winner can choose trump.
        //after choosing trump, will get 5 cards (to choose which ones to get rid of).
    }
}