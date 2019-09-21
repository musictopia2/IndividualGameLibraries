using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace ChessCP
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

    public class PlayerSpace
    {
        public EnumPieceType Piece { get; set; }
        public int Index { get; set; }
    }

    public class PreviousMove // 0 and 0 means no move was made for previous move
    {
        public int SpaceFrom { get; set; }
        public int SpaceTo { get; set; }
        public string PlayerColor { get; set; } = cs.Transparent; // this is the color for the previous player.
    }

    public class MoveInfo
    {
        public int SpaceFrom { get; set; }
        public int SpaceTo { get; set; }
        public EnumPieceType Piece { get; set; } // this is the piece used for the move.
        public EnumStatusType Results { get; set; }
    }
}
