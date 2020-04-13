namespace BackgammonCP.Data
{
    public enum EnumStatusType
    {
        CompletelyOpen = 1,
        PlayerOwns = 2,
        KnockOtherPlayer = 3,
        PlayerHasOne = 4,
        Closed = 5,
        Stackup = 6
    }
    public enum EnumGameStatus
    {
        None = 0,
        // RollDice = 1
        MakingMoves = 2,
        EndingTurn = 3 // this means no more moves left.  so the choices are either to undo moves or end turn
    }
}