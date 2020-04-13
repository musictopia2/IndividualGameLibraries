//i think this is the most common things i like to do
namespace RookCP.Data
{
    public enum EnumStatusList
    {
        None, Bidding, ChooseTrump, SelectNest, Normal
        //only the winner can choose trump.
        //after choosing trump, will get 5 cards (to choose which ones to get rid of).
    }
}