using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System.Reflection;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString; //maybe
namespace ChessCP
{
    public class PieceCP : BaseGraphicsCP
    {
        public EnumPieceType WhichPiece { get; set; }

        private readonly SKBitmap _redBishop;
        private readonly SKBitmap _redKing;
        private readonly SKBitmap _redKnight;
        private readonly SKBitmap _redPawn;
        private readonly SKBitmap _redQueen;
        private readonly SKBitmap _redRook;
        private readonly SKBitmap _blueBishop;
        private readonly SKBitmap _blueKing;
        private readonly SKBitmap _blueKnight;
        private readonly SKBitmap _bluePawn;
        private readonly SKBitmap _blueQueen;
        private readonly SKBitmap _blueRook;
        private readonly SKPaint _bitPaint;
        public PieceCP()
        {
            Assembly thisA = Assembly.GetAssembly(GetType());
            _blueBishop = ImageExtensions.GetSkBitmap(thisA, "BLUEBISHOP.gif");
            _blueKing = ImageExtensions.GetSkBitmap(thisA, "BLUEKING.gif");
            _blueKnight = ImageExtensions.GetSkBitmap(thisA, "BLUEKNIGHT.gif");
            _bluePawn = ImageExtensions.GetSkBitmap(thisA, "BLUEPAWN.gif");
            _blueQueen = ImageExtensions.GetSkBitmap(thisA, "BLUEQUEEN.gif");
            _blueRook = ImageExtensions.GetSkBitmap(thisA, "BLUEROOK.gif");
            _redBishop = ImageExtensions.GetSkBitmap(thisA, "REDBISHOP.gif");
            _redKing = ImageExtensions.GetSkBitmap(thisA, "REDKING.gif");
            _redKnight = ImageExtensions.GetSkBitmap(thisA, "REDKNIGHT.gif");
            _redPawn = ImageExtensions.GetSkBitmap(thisA, "REDPAWN.gif");
            _redQueen = ImageExtensions.GetSkBitmap(thisA, "REDQUEEN.gif");
            _redRook = ImageExtensions.GetSkBitmap(thisA, "REDROOK.gif");
            _bitPaint = MiscHelpers.GetBitmapPaint();
        }
        public override void DrawImage(SKCanvas dc)
        {
            SKBitmap thisBit;
            if (MainColor.Equals(cs.Blue) == true)
            {
                // blue processes

                switch (WhichPiece)
                {
                    case EnumPieceType.BISHOP:
                        {
                            thisBit = _blueBishop;
                            break;
                        }

                    case EnumPieceType.KING:
                        {
                            thisBit = _blueKing;
                            break;
                        }

                    case EnumPieceType.KNIGHT:
                        {
                            thisBit = _blueKnight;
                            break;
                        }

                    case EnumPieceType.PAWN:
                        {
                            thisBit = _bluePawn;
                            break;
                        }

                    case EnumPieceType.QUEEN:
                        {
                            thisBit = _blueQueen;
                            break;
                        }

                    case EnumPieceType.ROOK:
                        {
                            thisBit = _blueRook;
                            break;
                        }

                    default:
                        {
                            throw new BasicBlankException("Not supported");
                        }
                }
            }
            else if (MainColor.Equals(cs.Red) == true)
            {
                // red processes
                switch (WhichPiece)
                {
                    case EnumPieceType.BISHOP:
                        {
                            thisBit = _redBishop;
                            break;
                        }

                    case EnumPieceType.KING:
                        {
                            thisBit = _redKing;
                            break;
                        }

                    case EnumPieceType.KNIGHT:
                        {
                            thisBit = _redKnight;
                            break;
                        }

                    case EnumPieceType.PAWN:
                        {
                            thisBit = _redPawn;
                            break;
                        }

                    case EnumPieceType.QUEEN:
                        {
                            thisBit = _redQueen;
                            break;
                        }

                    case EnumPieceType.ROOK:
                        {
                            thisBit = _redRook;
                            break;
                        }

                    default:
                        {
                            throw new BasicBlankException("Not supported");
                        }
                }
            }
            else
                throw new BasicBlankException("Only blue and red are supported for chess pieces");
            if (thisBit == null == true)
                throw new BasicBlankException("There is no bitmap to even draw");
            var thisRect = GetMainRect();
            dc.DrawBitmap(thisBit, thisRect, _bitPaint);
        }
    }
}