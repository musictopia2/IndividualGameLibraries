namespace FlinchCP.Data
{
    public enum EnumCardType
    {
        Discard = 1,
        Stock = 2,
        MyCards = 3,
        IsNone = 0
    }
    public enum EnumStatusList
    {
        DiscardAll = 1, // this means it needs to discard all (this means will not draw cards because need to discard all)
        Normal = 2, // this means normal play
        FirstOne = 3, // this means to either find a 1 or else pass (in this case, will draw the usual 5 cards)
        DiscardOneOnly = 4 // this means that a player has found a one.  all other players has to discard one (no drawing)
    }
}