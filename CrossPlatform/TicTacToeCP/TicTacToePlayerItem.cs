using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
namespace TicTacToeCP
{
    public class TicTacToePlayerItem : SimplePlayer
    { //anything needed is here
        private EnumSpaceType _Piece;

        public EnumSpaceType Piece
        {
            get { return _Piece; }
            set
            {
                if (SetProperty(ref _Piece, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
    }
}