using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System;
using System.Reflection;
namespace CandylandCP
{
    public class CandylandCardGraphicsCP : ObservableObject, IDeckGraphicsCP
    {
        public static string TagUsed => "candylandcard"; //standard enough.
        public bool Drew { get; set; } = false; //not relevent this time.
        private int _HowMany = 1; // default to 1
        public int HowMany
        {
            get
            {
                return _HowMany;
            }

            set
            {
                if (SetProperty(ref _HowMany, value) == true)
                    MainGraphics?.PaintUI?.DoInvalidate();
            }
        }
        private EnumCandyLandType _WhichCard;
        public EnumCandyLandType WhichCard
        {
            get { return _WhichCard; }
            set
            {
                if (SetProperty(ref _WhichCard, value) == true)
                    MainGraphics?.PaintUI?.DoInvalidate();

            }
        }
        public BaseDeckGraphicsCP? MainGraphics { get; set; }
        public void DrawBorders(SKCanvas dc, SKRect rect_Card)
        {
            SKPaint thisPaint = MiscHelpers.GetStrokePaint(SKColors.Black, 3); //i think 3 is enough.
            MainGraphics!.DrawDefaultRectangles(dc, rect_Card, thisPaint);
        }
        public bool NeedsToDrawBacks => false;
        public void DrawBacks(SKCanvas canvas, SKRect rect_Card)
        {
            throw new BasicBlankException("This does not even have backs");
        }
        public bool CanStartDrawing()
        {
            return WhichCard != EnumCandyLandType.None;
        }

        public void DrawDefaultRectangles(SKCanvas dc, SKRect rect_Card, SKPaint thisPaint)
        {
            MainGraphics!.DrawDefaultRectangles(dc, rect_Card, thisPaint);
        }
        public SKColor GetFillColor()
        {
            return MainGraphics!.GetFillColor();
        }
        public SKRect GetDrawingRectangle()
        {
            return MainGraphics!.GetMainRect();
        }
        private void DrawOther(SKCanvas canvas, SKRect rect_card, string fileName)
        {
            var thisBit = ImageExtensions.GetSkBitmap(_thisAssembly, fileName);
            canvas.DrawBitmap(thisBit, rect_card, MainGraphics!.BitPaint);
        }
        private Assembly? _thisAssembly;
        private SKPaint? _borderPaint;
        public void Init()
        {
            if (MainGraphics == null)
                throw new BasicBlankException("You never sent in the main graphics for helpers");
            _thisAssembly = Assembly.GetAssembly(this.GetType());
            _borderPaint = MiscHelpers.GetStrokePaint(SKColors.Black, 2);
        }

        public void DrawImage(SKCanvas canvas, SKRect rect_Card)
        {
            switch (WhichCard)
            {
                case EnumCandyLandType.IsTree:
                    {
                        DrawOther(canvas, rect_Card, "tree.png");
                        break;
                    }

                case EnumCandyLandType.IsMagic:
                    {
                        DrawOther(canvas, rect_Card, "magic.png");
                        break;
                    }

                case EnumCandyLandType.IsGuard:
                    {
                        DrawOther(canvas, rect_Card, "guard.png");
                        break;
                    }

                case EnumCandyLandType.IsGirl:
                    {
                        DrawOther(canvas, rect_Card, "girl.png");
                        break;
                    }

                case EnumCandyLandType.IsAngel:
                    {
                        DrawOther(canvas, rect_Card, "angel.png");
                        break;
                    }

                case EnumCandyLandType.IsFairy:
                    {
                        DrawOther(canvas, rect_Card, "fairy.png");
                        break;
                    }

                default:
                    {
                        // this means it will draw the normal ones.
                        if (HowMany != 1 && HowMany != 2)
                            throw new BasicBlankException("Only 1 and 2 are supported");
                        float int_Size;
                        float int_Offset;
                        var bounds = rect_Card;
                        SKPoint pt_Center = new SKPoint(rect_Card.Left + (bounds.Width / 2), bounds.Top + (bounds.Height / 2));
                        int_Offset = bounds.Width / 10;
                        int_Size = Math.Min(bounds.Width - (int_Offset * 2), bounds.Height - (int_Offset * 2));
                        SKRect rect1;
                        SKRect rect2;
                        SKPaint thisPaint;
                        switch (WhichCard)
                        {
                            case EnumCandyLandType.IsBlue:
                                {
                                    thisPaint = MiscHelpers.GetSolidPaint(SKColors.Blue);
                                    break;
                                }

                            case EnumCandyLandType.IsGreen:
                                {
                                    thisPaint = MiscHelpers.GetSolidPaint(SKColors.Green);
                                    break;
                                }

                            case EnumCandyLandType.IsOrange:
                                {
                                    thisPaint = MiscHelpers.GetSolidPaint(SKColors.Orange);
                                    break;
                                }

                            case EnumCandyLandType.IsPurple:
                                {
                                    thisPaint = MiscHelpers.GetSolidPaint(SKColors.Purple);
                                    break;
                                }

                            case EnumCandyLandType.IsRed:
                                {
                                    thisPaint = MiscHelpers.GetSolidPaint(SKColors.Red);
                                    break;
                                }

                            case EnumCandyLandType.IsYellow:
                                {
                                    thisPaint = MiscHelpers.GetSolidPaint(SKColors.Yellow);
                                    break;
                                }

                            default:
                                {
                                    throw new BasicBlankException("Nothing found");
                                }
                        }

                        if (HowMany == 2)
                        {
                            rect1 = SKRect.Create(pt_Center.X - int_Size - (int_Offset / 2), bounds.Top + (int_Offset), int_Size, int_Size);
                            rect2 = SKRect.Create(pt_Center.X + (int_Offset / 2), bounds.Top + (int_Offset), int_Size, int_Size);
                            canvas.DrawRect(rect1, thisPaint);
                            canvas.DrawRect(rect1, _borderPaint);
                            canvas.DrawRect(rect2, thisPaint);
                            canvas.DrawRect(rect2, _borderPaint);
                        }
                        else
                        {
                            rect1 = SKRect.Create(pt_Center.X - (int_Size / 2), bounds.Top + int_Offset, int_Size, int_Size);
                            canvas.DrawRect(rect1, thisPaint);
                            canvas.DrawRect(rect1, _borderPaint);
                        }
                        break;
                    }
            }
        }
    }
}