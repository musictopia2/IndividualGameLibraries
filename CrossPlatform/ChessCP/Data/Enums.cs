namespace ChessCP.Data
{
    public enum EnumPieceType
    {
        None,
        KING,
        QUEEN,
        BISHOP,
        ROOK,
        KNIGHT,
        PAWN
    }

    public enum EnumStatusType
    {
        None = 0,
        CompletelyOpen = 1,
        PlayerOwns = 2
    }

    public enum EnumGameStatus
    {
        None = 0,
        PossibleTie = 1,
        EndingTurn = 2 // once you make move, you can only end turn or undo all moves.
    }
}