using BasicGameFramework.GameGraphicsCP.CheckersChessHelpers;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace CheckersCP
{
    public class SpaceCP : CheckersChessSpace<CheckerPiecesCP>
    {

        public int PlayerOwns { get; set; }
        public bool IsCrowned { get; set; }
        public string PlayerColor { get; set; } = cs.Transparent; //hopefully the risk pays off here.  if not, then put back to empty string.
        public override void ClearSpace()
        {
            PlayerOwns = 0;
            IsCrowned = false;
            PlayerColor = cs.Transparent;
        }

        protected override EnumGame GetGame()
        {
            return EnumGame.Checkers;
        }

        protected override CheckerPiecesCP? GetGamePiece()
        {
            if (PlayerOwns == 0)
                return null;
            CheckerPiecesCP output = new CheckerPiecesCP();
            output.MainColor = PlayerColor;
            output.IsCrowned = IsCrowned;
            return output;
        }
    }
}