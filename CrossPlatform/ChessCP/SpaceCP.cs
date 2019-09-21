using BasicGameFramework.GameGraphicsCP.CheckersChessHelpers;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System.Linq;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace ChessCP
{
    public class SpaceCP : CheckersChessSpace<PieceCP>
    {
        public int PlayerOwns { get; set; }
        public string PlayerColor { get; set; } = cs.Transparent;
        public EnumPieceType PlayerPiece { get; set; }

        public override void ClearSpace()
        {
            PlayerOwns = 0;
            PlayerPiece = EnumPieceType.None;
            PlayerColor = cs.Transparent;
        }

        protected override EnumGame GetGame()
        {
            return EnumGame.Chess;
        }

        protected override PieceCP? GetGamePiece()
        {
            if (PlayerOwns == 0)
                return null;
            PieceCP output = new PieceCP();
            output.MainColor = PlayerColor;
            output.WhichPiece = PlayerPiece;
            return output;
        }
        private readonly SKPaint _greenPaint;
        private readonly ChessMainGameClass _mainGame;
        public SpaceCP()
        {
            _greenPaint = MiscHelpers.GetStrokePaint(SKColors.Green, 2);
            _mainGame = Resolve<ChessMainGameClass>(); //this means no unit testing.   this can't be unit tested anyways.
        }
        protected override void HighlightSpaces(SKCanvas thisCanvas)
        {
            int tempIndex;
            if (_mainGame.ThisData!.MultiPlayer == false)
                tempIndex = ReversedIndex;
            else
                tempIndex = MainIndex;
            if (_mainGame.SaveRoot!.PreviousMove.SpaceFrom == tempIndex || _mainGame.SaveRoot.PreviousMove.SpaceTo == tempIndex)
            {
                var newColor = _mainGame.SaveRoot.PreviousMove.PlayerColor.ToSKColor();
                var thisStroke = MiscHelpers.GetStrokePaint(newColor, 2); // i think 2
                thisStroke.StrokeCap = SKStrokeCap.Butt;
                float[] thisSingle;
                thisSingle = new float[2];
                thisSingle[0] = 5;
                thisSingle[1] = 5;
                thisStroke.PathEffect = SKPathEffect.CreateDash(thisSingle, 5);
                thisCanvas.DrawRect(ThisRect, thisStroke);
                return;
            }
            if (_mainGame.SaveRoot.SpaceHighlighted == 0)
                return;// because there is nothing todo
            if (_mainGame.ThisGlobal!.CurrentMoveList.Count == 0)
                throw new BasicBlankException("If there is a space highlighted, then must have a filtered move list");
            if (_mainGame.ThisGlobal.CurrentMoveList.Any(items => items.SpaceTo == MainIndex))
                thisCanvas.DrawRect(ThisRect, _greenPaint);
        }
    }
}