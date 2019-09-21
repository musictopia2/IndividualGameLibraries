using BasicGameFramework.Attributes;
using BasicGameFramework.GameGraphicsCP.Animations;
using CommonBasicStandardLibraries.CollectionClasses;
namespace ChessCP
{
    [SingletonGame]
    public class GlobalClass
    {
        public AnimateSkiaSharpGameBoard? Animates;
        internal bool CurrentCrowned;
        internal CustomBasicList<MoveInfo> CompleteMoveList { get; set; } = new CustomBasicList<MoveInfo>();
        internal CustomBasicList<MoveInfo> CurrentMoveList { get; set; } = new CustomBasicList<MoveInfo>();
        public CustomBasicList<SpaceCP>? SpaceList;
        internal EnumPieceType CurrentPiece;
    }
}
