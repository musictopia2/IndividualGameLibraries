using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BasicGameFrameworkLibrary.GameGraphicsCP.CheckersChessHelpers;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;

namespace ChessCP.Data
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
        private readonly ChessGameContainer _gameContainer;
        public SpaceCP()
        {
            _greenPaint = MiscHelpers.GetStrokePaint(SKColors.Green, 2);
            _gameContainer = Resolve<ChessGameContainer>(); //this means no unit testing.   this can't be unit tested anyways.
        }
        protected override void HighlightSpaces(SKCanvas thisCanvas)
        {
            int tempIndex;
            if (_gameContainer.BasicData!.MultiPlayer == false)
                tempIndex = ReversedIndex;
            else
                tempIndex = MainIndex;
            if (_gameContainer.SaveRoot!.PreviousMove.SpaceFrom == tempIndex || _gameContainer.SaveRoot.PreviousMove.SpaceTo == tempIndex)
            {
                var newColor = _gameContainer.SaveRoot.PreviousMove.PlayerColor.ToSKColor();
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
            if (_gameContainer.SaveRoot.SpaceHighlighted == 0)
                return;// because there is nothing todo
            if (_gameContainer.CurrentMoveList.Count == 0)
                throw new BasicBlankException("If there is a space highlighted, then must have a filtered move list");
            if (_gameContainer.CurrentMoveList.Any(items => items.SpaceTo == MainIndex))
                thisCanvas.DrawRect(ThisRect, _greenPaint);
        }
    }
}
