using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.GameGraphicsCP.BasicGameBoards;
using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using PaydayCP.Data;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PaydayCP.Graphics
{
    [SingletonGame]
    [AutoReset]
    public class GameBoardGraphicsCP : BaseGameBoardCP<PawnPiecesCP<EnumColorChoice>>
    {
        private readonly PaydayGameContainer _gameContainer;

        public GameBoardGraphicsCP(PaydayGameContainer gameContainer) : base(gameContainer.Resolver)
        {
            _thisAssembly = Assembly.GetAssembly(this.GetType());
            _gameContainer = gameContainer;
        }
        public override string TagUsed => "main";
        protected override SKSize OriginalSize { get; set; } = new SKSize(550, 550); // can adjust as needed
        #region "Paint Processes"
        protected override PawnPiecesCP<EnumColorChoice> GetGamePiece(string color, SKPoint location)
        {
            var thisPiece = base.GetGamePiece(color, location);
            thisPiece.NeedsToClear = false; //because you are drawing on the board.
            return thisPiece;
        }
        protected override bool CanStartPaint()
        {
            return true;
        }
        private SKPaint? _lightBluePaint;
        private SKPaint? _whitePaint;
        private SKPaint? _darkSlateGrayPaint;
        private SKPaint? _redPaint;
        private SKPaint? _borderPaint;
        private SKPaint? _bluePaint;
        private SKPaint? _blackPaint;
        private SKPaint? _greenPaint;
        private SKPaint? _lightGrayPaint;
        private SKPaint? _firstHatch;
        private SKPaint? _secondHatch;
        private SKPaint? _thirdHatch;
        private SKPaint? _brownPaint;
        private SKPaint? _fourthHatch;
        private SKPaint? _orangePaint;
        private SKPaint? _blueBorder;
        private readonly Assembly? _thisAssembly;

        private SKPaint GetPaint(string path)
        {
            var thisPaint = MiscHelpers.GetBitmapPaint();
            thisPaint.Shader = ImageExtensions.GetSkShader(_thisAssembly!, path);
            return thisPaint;
        }
        protected override void SetUpPaints()
        {
            _lightBluePaint = MiscHelpers.GetSolidPaint(SKColors.LightBlue);
            _whitePaint = MiscHelpers.GetSolidPaint(SKColors.White);
            _darkSlateGrayPaint = MiscHelpers.GetSolidPaint(SKColors.DarkSlateGray);
            _redPaint = MiscHelpers.GetSolidPaint(SKColors.Red);
            _borderPaint = MiscHelpers.GetStrokePaint(SKColors.Black, 1);
            _bluePaint = MiscHelpers.GetSolidPaint(SKColors.Blue);
            _blackPaint = MiscHelpers.GetSolidPaint(SKColors.Black);
            _greenPaint = MiscHelpers.GetSolidPaint(SKColors.Green);
            _lightGrayPaint = MiscHelpers.GetSolidPaint(SKColors.LightGray);
            _firstHatch = GetPaint("blacklightgray.png");
            _secondHatch = GetPaint("whiteredtrellis.png");
            _thirdHatch = GetPaint("lightgrayblacktrellis.png");
            _brownPaint = MiscHelpers.GetSolidPaint(SKColors.Brown);
            _fourthHatch = GetPaint("blackgreennarrowhorizontal.png");
            _orangePaint = MiscHelpers.GetSolidPaint(SKColors.Orange);
            _blueBorder = MiscHelpers.GetStrokePaint(SKColors.Blue, 4);
        }
        private bool _paint = false;
        internal bool DidPaint()
        {
            return _paint;
        }
        protected override void DrawBoard(SKCanvas canvas)
        {
            canvas.Clear();
            var thisRect = GetBounds();
            DrawDates(canvas, thisRect);
            _gameContainer.PrivateSpaceList.ForEach(thisSpace =>
            {
                thisSpace.PieceList.ForEach(thisPiece =>
                {
                    thisPiece.DrawImage(canvas);
                });
            });
            if (_gameContainer.SaveRoot!.NumberHighlighted > 0)
            {
                if (_spaceList!.Keys.Contains(_gameContainer.SaveRoot.NumberHighlighted.ToString()))
                {
                    var thisSpace = _spaceList[_gameContainer.SaveRoot.NumberHighlighted.ToString()];
                    canvas.DrawRect(thisSpace.Bounds, _blueBorder);
                }
            }
            _paint = true;
        }
        #endregion
        #region "Positioning Info"
        private Dictionary<string, SpaceCP>? _spaceList;
        public SKRect SpaceRectangle(int day) //can't do as property unfortunately.
        {
            if (_spaceList!.Keys.Contains(day.ToString()))
                return _spaceList[day.ToString()].Bounds;
            else
                return default;
        }
        public SKRect StartingRectangle
        {
            get
            {
                if (_spaceList!.Keys.Contains("Start"))
                    return _spaceList["Start"].Bounds;
                else
                    return default;
            }
        }
        public SKRect FinishRectangle
        {
            get
            {
                if (_spaceList!.Keys.Contains("Finish"))
                    return _spaceList["Finish"].Bounds;
                else
                    return default;
            }
        }
        protected override void CreateSpaces() { }
        #endregion
        #region "Click Processes"
        protected override async Task ClickProcessAsync(SKPoint thisPoint)
        {
            if (_gameContainer.Command.IsExecuting || _gameContainer.SaveRoot.GameStatus != EnumStatus.MakeMove)
            {
                return;
            }
            if (_gameContainer.SpaceClickedAsync == null)
            {
                throw new BasicBlankException("Nobody is handling the space clicked async.  Rethink");
            }
            foreach (var thisSpace in _spaceList!.Values)
            {
                if (MiscHelpers.DidClickRectangle(thisSpace.Bounds, thisPoint))
                {
                    if (thisSpace.Number != _gameContainer.SaveRoot!.NumberHighlighted)
                        return;
                    await _gameContainer.ProcessCustomCommandAsync(_gameContainer.SpaceClickedAsync, thisSpace.Number);
                    return;
                }
            }
        }
        #endregion
        #region "Drawing Processes"
        private void DrawDates(SKCanvas canvas, SKRect bounds)
        {
            int int_Row;
            int int_Col;
            SKRect rect;
            SpaceCP obj_Space;
            int int_Day = 0;
            var pn_Red = MiscHelpers.GetStrokePaint(SKColors.Red, bounds.Width / 100);
            SKPaint pn_Border;
            SKPaint br_Fill;
            _spaceList = new Dictionary<string, SpaceCP>(); // has to be string so it can be supported by payday.
            for (int_Row = 0; int_Row <= 4; int_Row++)
            {
                for (int_Col = 0; int_Col <= 6; int_Col++)
                {
                    obj_Space = new SpaceCP(); // maybe this was forgotten.
                    rect = SKRect.Create(bounds.Left + ((bounds.Width / 7) * int_Col), bounds.Top + ((bounds.Height / 5) * int_Row), bounds.Width / 7, bounds.Height / 5);
                    br_Fill = MiscHelpers.GetLinearGradientPaint(SKColors.Gray, SKColors.White, rect, MiscHelpers.EnumLinearGradientPercent.Angle45);
                    rect = SKRect.Create(rect.Left + (rect.Width / 20), rect.Top + (rect.Height / 20), (rect.Width * 9) / 10, (rect.Height * 9) / 10);
                    obj_Space.Number = int_Day;
                    obj_Space.Bounds = rect;
                    if (int_Day == 0)
                        _spaceList.Add("Start", obj_Space);
                    else
                        _spaceList.Add(int_Day.ToString(), obj_Space);
                    if (int_Day <= 31)
                    {
                        canvas.DrawRect(rect, _lightBluePaint);
                        DrawDateImage(canvas, rect, int_Day);
                        pn_Border = br_Fill;
                        pn_Border.IsStroke = true;
                        pn_Border.StrokeWidth = bounds.Width / 150;
                        canvas.DrawRect(rect, pn_Border);
                        if (int_Day > 0)
                        {
                            var FirstTextPaint = MiscHelpers.GetTextPaint(SKColors.Black, bounds.Height / 20, "Times New Roman");
                            canvas.DrawText(int_Day.ToString(), rect.Left, rect.Top, false, FirstTextPaint);
                        }
                    }
                    int_Day += 1;
                }
            }
            var tempWidth = bounds.Width / 100;
            for (int_Col = 0; int_Col <= 7; int_Col++)
            {
                if (int_Col == 0)
                    canvas.DrawLine(new SKPoint(bounds.Left + ((bounds.Width / 7) * int_Col) + (tempWidth / 2), bounds.Top), new SKPoint(bounds.Left + ((bounds.Width / 7) * int_Col) + (tempWidth / 2), bounds.Top + bounds.Height), pn_Red);
                else if (int_Col == 7)
                    canvas.DrawLine(new SKPoint((bounds.Left + ((bounds.Width / 7) * int_Col)) - (tempWidth / 2), bounds.Top), new SKPoint((bounds.Left + ((bounds.Width / 7) * int_Col)) - (tempWidth / 2), bounds.Top + bounds.Height), pn_Red);
                else if (int_Col > 4)
                    canvas.DrawLine(new SKPoint(bounds.Left + ((bounds.Width / 7) * int_Col), bounds.Top), new SKPoint(bounds.Left + ((bounds.Width / 7) * int_Col), bounds.Top + ((bounds.Height / 5) * 4)), pn_Red);
                else
                    canvas.DrawLine(new SKPoint(bounds.Left + ((bounds.Width / 7) * int_Col), bounds.Top), new SKPoint(bounds.Left + ((bounds.Width / 7) * int_Col), bounds.Top + bounds.Height), pn_Red);
            }
            for (int_Row = 0; int_Row <= 5; int_Row++)
            {
                if (int_Row == 5)
                    canvas.DrawLine(new SKPoint(bounds.Left, (bounds.Top + ((bounds.Height / 5) * int_Row)) - (tempWidth / 2)), new SKPoint(bounds.Left + bounds.Width, (bounds.Top + ((bounds.Height / 5) * int_Row)) - (tempWidth / 2)), pn_Red);
                else
                    canvas.DrawLine(new SKPoint(bounds.Left, bounds.Top + ((bounds.Height / 5) * int_Row)), new SKPoint(bounds.Left + bounds.Width, bounds.Top + ((bounds.Height / 5) * int_Row)), pn_Red);
            }
            rect = SKRect.Create(bounds.Left + ((bounds.Width / 7) * 4), bounds.Top + ((bounds.Height / 5) * 4), (bounds.Width * 3) / 7, bounds.Height / 5); // *** Draw jackpot
            rect = SKRect.Create(rect.Left + (rect.Width / 50), rect.Top + (rect.Height / 20), rect.Width - (rect.Width / 30), (rect.Height * 9) / 10);
            obj_Space = new SpaceCP();
            obj_Space.Bounds = rect;
            obj_Space.Number = 32;
            _spaceList.Add("Finish", obj_Space);
            SKColor firstColor;
            SKColor secondColor;
            firstColor = new SKColor(255, 255, 255, 100); // 60 instead of 100 seems to do the trick
            secondColor = new SKColor(0, 0, 0, 100);
            br_Fill = MiscHelpers.GetLinearGradientPaint(firstColor, secondColor, SKRect.Create(rect.Left, rect.Top, rect.Width, rect.Height / 2), MiscHelpers.EnumLinearGradientPercent.Angle180);
            canvas.DrawRect(rect, _lightBluePaint);
            canvas.DrawRect(rect, br_Fill);
            SKPaint secondTextPaint;
            secondTextPaint = MiscHelpers.GetTextPaint(SKColors.Black, bounds.Height * 0.06f, "Ariel");
            float lefts;
            lefts = rect.Left + (rect.Width / 2);
            float tops;
            tops = rect.Top + (rect.Height * 0.05f);
            canvas.DrawText("$ Jackpot $", lefts, tops, true, secondTextPaint);
            var finalRect = SKRect.Create(rect.Left, rect.Top + (rect.Height / 2), rect.Width, rect.Height / 2);
            string thisText;
            thisText = _gameContainer.SaveRoot!.LotteryAmount.ToCurrency(0);
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Aqua, finalRect.Height * 0.8f);
            canvas.DrawBorderText(thisText, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, _borderPaint!, finalRect);
            var firstRect = StartingRectangle;
            var secondRect = FinishRectangle;
            if (firstRect.Width == secondRect.Width)
                throw new BasicBlankException("The first and second rectangle is the same.");
        }
        private void DrawDateImage(SKCanvas canvas, SKRect bounds, int int_Day)
        {
            switch (int_Day)
            {
                case 0:
                    {
                        var gp = new SKPath();
                        gp.AddLine(new SKPoint(bounds.Left + (bounds.Width / 2), bounds.Top), new SKPoint(bounds.Left + bounds.Width, bounds.Top + (bounds.Height / 2)), true);
                        gp.AddLine(new SKPoint(bounds.Left + (bounds.Width / 2), bounds.Top + bounds.Height), new SKPoint(bounds.Left, bounds.Top + (bounds.Height / 2)));
                        gp.Close();
                        canvas.DrawPath(gp, _whitePaint);
                        var TextPaint = MiscHelpers.GetTextPaint(SKColors.Red, bounds.Height / 3);
                        float Lefts;
                        float Tops;
                        Lefts = bounds.Left + (bounds.Width / 2);
                        Tops = bounds.Top + (bounds.Height * 0.6f);
                        canvas.DrawText("Start", Lefts, Tops, true, TextPaint);
                        break;
                    }

                case 1:
                    {
                        DrawMailbox(canvas, bounds);
                        break;
                    }

                case 2:
                    {
                        DrawSweepstakes(canvas, bounds);
                        break;
                    }

                case 3:
                    {
                        DrawMailbox(canvas, bounds);
                        break;
                    }

                case 4:
                    {
                        DrawDeal(canvas, bounds);
                        break;
                    }

                case 5:
                    {
                        DrawMailbox(canvas, bounds);
                        break;
                    }

                case 6:
                    {
                        DrawLottery(canvas, bounds);
                        break;
                    }

                case 7:
                    {
                        DrawSki(canvas, bounds);
                        break;
                    }

                case 8:
                    {
                        DrawRadio(canvas, bounds);
                        break;
                    }

                case 9:
                    {
                        DrawFoundABuyer(canvas, bounds);
                        break;
                    }

                case 10:
                    {
                        DrawHappyBirthday(canvas, bounds);
                        break;
                    }

                case 11:
                    {
                        DrawMailbox(canvas, bounds);
                        break;
                    }

                case 12:
                    {
                        DrawDeal(canvas, bounds);
                        break;
                    }

                case 13:
                    {
                        DrawLottery(canvas, bounds);
                        break;
                    }

                case 14:
                    {
                        DrawConcert(canvas, bounds);
                        break;
                    }

                case 15:
                    {
                        DrawDeal(canvas, bounds);
                        break;
                    }

                case 16:
                    {
                        DrawMailbox(canvas, bounds);
                        break;
                    }

                case 17:
                    {
                        DrawFoundABuyer(canvas, bounds);
                        break;
                    }

                case 18:
                    {
                        DrawFood(canvas, bounds);
                        break;
                    }

                case 19:
                    {
                        DrawMailbox(canvas, bounds);
                        break;
                    }

                case 20:
                    {
                        DrawLottery(canvas, bounds);
                        break;
                    }

                case 21:
                    {
                        DrawYardSale(canvas, bounds);
                        break;
                    }

                case 22:
                    {
                        DrawMailbox(canvas, bounds);
                        break;
                    }

                case 23:
                    {
                        DrawFoundABuyer(canvas, bounds);
                        break;
                    }

                case 24:
                    {
                        DrawMailbox(canvas, bounds);
                        break;
                    }

                case 25:
                    {
                        DrawDeal(canvas, bounds);
                        break;
                    }

                case 26:
                    {
                        DrawFoundABuyer(canvas, bounds);
                        break;
                    }

                case 27:
                    {
                        DrawLottery(canvas, bounds);
                        break;
                    }

                case 28:
                    {
                        DrawGoingToTheMall(canvas, bounds);
                        break;
                    }

                case 29:
                    {
                        DrawFoundABuyer(canvas, bounds);
                        break;
                    }

                case 30:
                    {
                        DrawCharityWalk(canvas, bounds);
                        break;
                    }

                case 31:
                    {
                        DrawPayday(canvas, bounds);
                        break;
                    }
            }
        }
        private void DrawPayday(SKCanvas canvas, SKRect bounds)
        {
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Red, bounds.Height * 0.17f, "Times New Roman");
            var rect = SKRect.Create(bounds.Left + (bounds.Width * 0.4f), bounds.Top + (bounds.Height * 0.1f), bounds.Width * 0.55f, bounds.Height * 0.5f);
            canvas.DrawOval(rect, _bluePaint);
            rect = SKRect.Create(bounds.Left + (bounds.Width * 0.4f), bounds.Top + (bounds.Height * 0.11f), bounds.Width * 0.55f, bounds.Height * 0.5f);
            var otherPaint = MiscHelpers.GetTextPaint(SKColors.Yellow, bounds.Height * 0.4f, "Ariel");
            canvas.DrawCustomText("$", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, otherPaint, rect, out _);
            textPaint.FakeBoldText = true;
            var tempRect = SKRect.Create(bounds.Left, bounds.Top + (bounds.Height * 0.5f), bounds.Width, bounds.Height * 0.3f);
            canvas.DrawCustomText("Its", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, tempRect, out _);
            tempRect = SKRect.Create(bounds.Left, bounds.Top + (bounds.Height * 0.8f), bounds.Width, bounds.Height * 0.2f);
            canvas.DrawCustomText("Payday", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, tempRect, out _);
        }
        private void DrawCharityWalk(SKCanvas canvas, SKRect bounds)
        {
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Red, bounds.Height * 0.18f, "Times New Roman");
            SKPoint[] pts = new SKPoint[26];
            SKPath gp;
            var rect = SKRect.Create(bounds.Left + (bounds.Width * 0.2f), bounds.Top + (bounds.Height / 10), bounds.Width * 0.6f, bounds.Height * 0.5f);
            pts[0] = new SKPoint(rect.Left + (0.342857142857143f * rect.Width), rect.Top + (0.296296296296296f * rect.Height));
            pts[1] = new SKPoint(rect.Left + (0.114285714285714f * rect.Width), rect.Top + (0.555555555555556f * rect.Height));
            pts[2] = new SKPoint(rect.Left + (0 * rect.Width), rect.Top + (0.555555555555556f * rect.Height));
            pts[3] = new SKPoint(rect.Left + (0.0857142857142857f * rect.Width), rect.Top + (0.740740740740741f * rect.Height));
            pts[4] = new SKPoint(rect.Left + (0.314285714285714f * rect.Width), rect.Top + (0.518518518518518f * rect.Height));
            pts[5] = new SKPoint(rect.Left + (0.314285714285714f * rect.Width), rect.Top + (0.592592592592593f * rect.Height));
            pts[6] = new SKPoint(rect.Left + (0.0571428571428571f * rect.Width), rect.Top + (0.851851851851852f * rect.Height));
            pts[7] = new SKPoint(rect.Left + (0.2f * rect.Width), rect.Top + (1 * rect.Height));
            pts[8] = new SKPoint(rect.Left + (0.285714285714286f * rect.Width), rect.Top + (1 * rect.Height));
            pts[9] = new SKPoint(rect.Left + (0.2f * rect.Width), rect.Top + (0.888888888888889f * rect.Height));
            pts[10] = new SKPoint(rect.Left + (0.4f * rect.Width), rect.Top + (0.777777777777778f * rect.Height));
            pts[11] = new SKPoint(rect.Left + (0.571428571428571f * rect.Width), rect.Top + (0.777777777777778f * rect.Height));
            pts[12] = new SKPoint(rect.Left + (0.8f * rect.Width), rect.Top + (1 * rect.Height));
            pts[13] = new SKPoint(rect.Left + (1 * rect.Width), rect.Top + (0.777777777777778f * rect.Height));
            pts[14] = new SKPoint(rect.Left + (0.914285714285714f * rect.Width), rect.Top + (0.740740740740741f * rect.Height));
            pts[15] = new SKPoint(rect.Left + (0.8f * rect.Width), rect.Top + (0.777777777777778f * rect.Height));
            pts[16] = new SKPoint(rect.Left + (0.657142857142857f * rect.Width), rect.Top + (0.62962962962963f * rect.Height));
            pts[17] = new SKPoint(rect.Left + (0.8f * rect.Width), rect.Top + (0.592592592592593f * rect.Height));
            pts[18] = new SKPoint(rect.Left + (0.971428571428571f * rect.Width), rect.Top + (0.444444444444444f * rect.Height));
            pts[19] = new SKPoint(rect.Left + (0.942857142857143f * rect.Width), rect.Top + (0.37037037037037f * rect.Height));
            pts[20] = new SKPoint(rect.Left + (0.771428571428571f * rect.Width), rect.Top + (0.444444444444444f * rect.Height));
            pts[21] = new SKPoint(rect.Left + (0.571428571428571f * rect.Width), rect.Top + (0.296296296296296f * rect.Height));
            pts[22] = new SKPoint(rect.Left + (0.6f * rect.Width), rect.Top + (0.111111111111111f * rect.Height));
            pts[23] = new SKPoint(rect.Left + (0.4f * rect.Width), rect.Top + (0 * rect.Height));
            pts[24] = new SKPoint(rect.Left + (0.2f * rect.Width), rect.Top + (0.148148148148148f * rect.Height));
            pts[25] = new SKPoint(rect.Left + (0.342857142857143f * rect.Width), rect.Top + (0.259259259259259f * rect.Height));
            gp = new SKPath();
            gp.AddPoly(pts);
            canvas.DrawPath(gp, _bluePaint);
            canvas.DrawPath(gp, _borderPaint);
            textPaint.FakeBoldText = true;
            var TempRect = SKRect.Create(bounds.Left, bounds.Top + (bounds.Height * 0.6f), bounds.Width, bounds.Height * 0.2f);
            canvas.DrawCustomText("Charity", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, TempRect, out _);
            TempRect = SKRect.Create(bounds.Left, bounds.Top + (bounds.Height * 0.8f), bounds.Width, bounds.Height * 0.2f);
            canvas.DrawCustomText("Walk", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, TempRect, out _);
        }
        private void DrawGoingToTheMall(SKCanvas canvas, SKRect bounds)
        {
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Red, bounds.Height * 0.16f, "Times New Roman");
            textPaint.FakeBoldText = true;
            var tempRect = SKRect.Create(bounds.Left, bounds.Top + (bounds.Height * 0.2f), bounds.Width, bounds.Height * 0.2f);
            canvas.DrawCustomText("Going to", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, tempRect, out _);
            tempRect = SKRect.Create(bounds.Left, bounds.Top + (bounds.Height * 0.45f), bounds.Width, bounds.Height * 0.2f);
            canvas.DrawCustomText("the Mall", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, tempRect, out _);
            tempRect = SKRect.Create(bounds.Left, bounds.Top + (bounds.Height * 0.7f), bounds.Width, bounds.Height * 0.2f);
            canvas.DrawCustomText("Pay $500", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, tempRect, out _);
        }
        private void DrawYardSale(SKCanvas canvas, SKRect bounds)
        {
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Red, bounds.Height * 0.2f, "Times New Roman");
            textPaint.FakeBoldText = true;
            var rect = SKRect.Create(bounds.Left + (bounds.Width * 0.45f), bounds.Top + (bounds.Height * 0.2f), bounds.Width * 0.1f, bounds.Height * 0.8f);
            canvas.DrawRect(rect, _whitePaint);
            canvas.DrawRect(rect, _borderPaint);
            rect = SKRect.Create(bounds.Left + (bounds.Width * 0.1f), bounds.Top + (bounds.Height * 0.3f), bounds.Width * 0.8f, bounds.Height * 0.5f);
            canvas.DrawRect(rect, _whitePaint);
            canvas.DrawRect(rect, _borderPaint);
            var TempRect = SKRect.Create(bounds.Left, bounds.Top + (bounds.Height * 0.35f), bounds.Width, bounds.Height * 0.8f);
            canvas.DrawCustomText("Yard", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Start, textPaint, TempRect, out _);
            TempRect = SKRect.Create(bounds.Left, bounds.Top + (bounds.Height * 0.55f), bounds.Width, bounds.Height * 0.8f);
            canvas.DrawCustomText("Sale", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Start, textPaint, TempRect, out _);
        }
        private void DrawFood(SKCanvas canvas, SKRect bounds)
        {
            SKPoint[] pts = new SKPoint[13];
            SKPath gp;
            SKPaint br_Fill;
            var rect = SKRect.Create(bounds.Left + (bounds.Width * 0.35f), bounds.Top + (bounds.Height * 0.1f), bounds.Width * 0.39f, bounds.Height * 0.3f);
            gp = new SKPath();
            gp.AddOval(rect);
            SKColor firstColor;
            SKColor secondColor;
            firstColor = new SKColor(255, 255, 255, 150); // 60 instead of 100 seems to do the trick
            secondColor = new SKColor(0, 0, 0, 150);
            br_Fill = MiscHelpers.GetLinearGradientPaint(firstColor, secondColor, gp.Bounds, MiscHelpers.EnumLinearGradientPercent.Angle45);
            canvas.DrawPath(gp, _orangePaint);
            canvas.DrawPath(gp, br_Fill);
            canvas.DrawPath(gp, _borderPaint);
            rect = SKRect.Create(bounds.Left + (bounds.Width * 0.5f), bounds.Top + (bounds.Height * 0.2f), bounds.Width * 0.39f, bounds.Height * 0.3f);
            pts[0] = new SKPoint(rect.Left + (0.576923076923077f * rect.Width), rect.Top + (0.3f * rect.Height));
            pts[1] = new SKPoint(rect.Left + (0.307692307692308f * rect.Width), rect.Top + (0.233333333333333f * rect.Height));
            pts[2] = new SKPoint(rect.Left + (0.0384615384615385f * rect.Width), rect.Top + (0.366666666666667f * rect.Height));
            pts[3] = new SKPoint(rect.Left + (0 * rect.Width), rect.Top + (0.666666666666667f * rect.Height));
            pts[4] = new SKPoint(rect.Left + (0.269230769230769f * rect.Width), rect.Top + (1 * rect.Height));
            pts[5] = new SKPoint(rect.Left + (0.423076923076923f * rect.Width), rect.Top + (1 * rect.Height));
            pts[6] = new SKPoint(rect.Left + (0.538461538461538f * rect.Width), rect.Top + (0.866666666666667f * rect.Height));
            pts[7] = new SKPoint(rect.Left + (0.653846153846154f * rect.Width), rect.Top + (0.933333333333333f * rect.Height));
            pts[8] = new SKPoint(rect.Left + (0.807692307692308f * rect.Width), rect.Top + (0.833333333333333f * rect.Height));
            pts[9] = new SKPoint(rect.Left + (1 * rect.Width), rect.Top + (0.4f * rect.Height));
            pts[10] = new SKPoint(rect.Left + (0.576923076923077f * rect.Width), rect.Top + (0.166666666666667f * rect.Height));
            pts[11] = new SKPoint(rect.Left + (0.5f * rect.Width), rect.Top + (0.233333333333333f * rect.Height));
            pts[12] = new SKPoint(rect.Left + (0.384615384615385f * rect.Width), rect.Top + (0 * rect.Height));
            gp = new SKPath();
            gp.AddPoly(pts);
            br_Fill = MiscHelpers.GetLinearGradientPaint(firstColor, secondColor, gp.Bounds, MiscHelpers.EnumLinearGradientPercent.Angle45);
            canvas.DrawPath(gp, _redPaint);
            canvas.DrawPath(gp, br_Fill);
            canvas.DrawPath(gp, _borderPaint);
            var TextPaint = MiscHelpers.GetTextPaint(SKColors.Red, bounds.Height * 0.2f, "Times New Roman");
            TextPaint.FakeBoldText = true;
            var TempRect = SKRect.Create(bounds.Left, bounds.Top + (bounds.Height * 0.5f), bounds.Width, bounds.Height * 0.2f);
            canvas.DrawCustomText("Food", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, TextPaint, TempRect, out _);
            TempRect = SKRect.Create(bounds.Left, bounds.Top + (bounds.Height * 0.75f), bounds.Width, bounds.Height * 0.2f);
            TextPaint = MiscHelpers.GetTextPaint(SKColors.Red, bounds.Height * 0.15f, "Times New Roman");
            TextPaint.FakeBoldText = true;
            canvas.DrawCustomText("Pay $600", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, TextPaint, TempRect, out _);
        }
        private void DrawConcert(SKCanvas canvas, SKRect bounds)
        {
            SKPoint[] pts = new SKPoint[10];
            SKPath gp = new SKPath();
            var rect = SKRect.Create(bounds.Left + (bounds.Width * 0.5f), bounds.Top + (bounds.Height / 10), bounds.Width * 0.39f, bounds.Height * 0.6f);
            pts[0] = new SKPoint(rect.Left + (0.592592592592593f * rect.Width), rect.Top + (0 * rect.Height));
            pts[1] = new SKPoint(rect.Left + (0.518518518518518f * rect.Width), rect.Top + (0.638888888888889f * rect.Height));
            pts[2] = new SKPoint(rect.Left + (0.333333333333333f * rect.Width), rect.Top + (0.527777777777778f * rect.Height));
            pts[3] = new SKPoint(rect.Left + (0 * rect.Width), rect.Top + (0.666666666666667f * rect.Height));
            pts[4] = new SKPoint(rect.Left + (0.037037037037037f * rect.Width), rect.Top + (0.916666666666667f * rect.Height));
            pts[5] = new SKPoint(rect.Left + (0.592592592592593f * rect.Width), rect.Top + (1 * rect.Height));
            pts[6] = new SKPoint(rect.Left + (0.666666666666667f * rect.Width), rect.Top + (0.194444444444444f * rect.Height));
            pts[7] = new SKPoint(rect.Left + (0.777777777777778f * rect.Width), rect.Top + (0.25f * rect.Height));
            pts[8] = new SKPoint(rect.Left + (1 * rect.Width), rect.Top + (0.277777777777778f * rect.Height));
            pts[9] = new SKPoint(rect.Left + (1 * rect.Width), rect.Top + (0.0555555555555556f * rect.Height));
            gp.AddPoly(pts);
            canvas.DrawPath(gp, _blackPaint);
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Red, bounds.Height * 0.15f);
            textPaint.FakeBoldText = true;
            var tempRect = SKRect.Create(bounds.Left, bounds.Top + (bounds.Height * 0.2f), bounds.Width, bounds.Height * 0.2f);
            canvas.DrawCustomText("Charity", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, tempRect, out _);
            tempRect = SKRect.Create(bounds.Left, bounds.Top + (bounds.Height * 0.45f), bounds.Width, bounds.Height * 0.2f);
            canvas.DrawCustomText("Concert", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, tempRect, out _);
            tempRect = SKRect.Create(bounds.Left, bounds.Top + (bounds.Height * 0.7f), bounds.Width, bounds.Height * 0.2f);
            canvas.DrawCustomText("Pay $400", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, tempRect, out _);
        }
        private void DrawHappyBirthday(SKCanvas canvas, SKRect bounds)
        {
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Red, bounds.Height * 0.18f);
            textPaint.FakeBoldText = true;
            var tempRect = SKRect.Create(bounds.Left, bounds.Top + (bounds.Height * 0.2f), bounds.Width, bounds.Height * 0.2f);
            canvas.DrawCustomText("Happy", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, tempRect, out _);
            tempRect = SKRect.Create(bounds.Left, bounds.Top + (bounds.Height * 0.4f), bounds.Width, bounds.Height * 0.2f);
            canvas.DrawCustomText("Birthday", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, tempRect, out _);
            tempRect = SKRect.Create(bounds.Left, bounds.Top + (bounds.Height * 0.6f), bounds.Width, bounds.Height * 0.2f);
            canvas.DrawCustomText("Collect", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, tempRect, out _);
            tempRect = SKRect.Create(bounds.Left, bounds.Top + (bounds.Height * 0.8f), bounds.Width, bounds.Height * 0.2f);
            canvas.DrawCustomText("$100", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, tempRect, out _);
        }
        private void DrawFoundABuyer(SKCanvas canvas, SKRect bounds)
        {
            var pn_Line = _borderPaint;
            SKPath gp;
            var rect = SKRect.Create(bounds.Left + (bounds.Width * 0.25f), bounds.Top + (bounds.Height * 0.15f), bounds.Width * 0.6f, (bounds.Height * 0.3f));
            SKRect rect2;
            var br_Fill = _fourthHatch;
            rect2 = SKRect.Create(rect.Left + (rect.Width * 0.2f), rect.Top + (rect.Height * 0.2f), rect.Width * 0.8f, rect.Height * 0.8f);
            canvas.DrawRect(rect2, br_Fill);
            canvas.DrawRect(rect2, pn_Line);
            gp = new SKPath();
            gp.AddLine(new SKPoint(rect.Left, rect.Top), new SKPoint(rect.Left + (rect.Width * 0.2f), rect.Top + (rect.Height * 0.2f)), true);
            gp.AddLine(new SKPoint(rect.Left + (rect.Width * 0.2f), rect.Top + rect.Height), new SKPoint(rect.Left, rect.Top + (rect.Height * 0.8f)));
            gp.Close();
            canvas.DrawPath(gp, _blackPaint);
            gp = new SKPath();
            gp.AddLine(new SKPoint(rect.Left, rect.Top), new SKPoint(rect.Left + (rect.Width * 0.8f), rect.Top), true);
            gp.AddLine(new SKPoint(rect.Left + (rect.Width), rect.Top + (rect.Height * 0.2f)), new SKPoint(rect.Left + (rect.Width * 0.2f), rect.Top + (rect.Height * 0.2f)));
            gp.Close();
            canvas.DrawPath(gp, _greenPaint);
            canvas.DrawPath(gp, pn_Line);
            gp = new SKPath();
            gp.AddLine(new SKPoint(rect.Left + (rect2.Width * 0.4f), rect.Top), new SKPoint(rect.Left + (rect2.Width * 0.6f), rect.Top), true);
            gp.AddLine(new SKPoint(rect2.Left + (rect2.Width * 0.6f), rect.Top + (rect.Height * 0.2f)), new SKPoint(rect2.Left + (rect2.Width * 0.4f), rect.Top + (rect.Height * 0.2f)));
            gp.Close();
            canvas.DrawPath(gp, _brownPaint);
            canvas.DrawPath(gp, pn_Line);
            rect2 = SKRect.Create(rect2.Left + (rect2.Width * 0.4f), rect2.Top, rect2.Width * 0.2f, rect2.Height);
            canvas.DrawRect(rect2, _brownPaint);
            canvas.DrawRect(rect2, pn_Line);
            SKRect tempRect;
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Red, bounds.Height * 0.2f);
            textPaint.FakeBoldText = true;
            tempRect = SKRect.Create(bounds.Left, bounds.Top + (bounds.Height * 0.55f), bounds.Width, bounds.Height * 0.25f);
            canvas.DrawCustomText("Found a", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, tempRect, out _);
            tempRect = SKRect.Create(bounds.Left, bounds.Top + (bounds.Height * 0.75f), bounds.Width, bounds.Height * 0.25f);
            canvas.DrawCustomText("Buyer", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, tempRect, out _);
        }
        private void DrawRadio(SKCanvas canvas, SKRect bounds)
        {
            SKRect rect;
            rect = SKRect.Create(bounds.Left + (bounds.Width * 0.25f), bounds.Top + (bounds.Height * 0.15f), bounds.Width * 0.6f, (bounds.Height * 0.4f));
            SKRect rect2;
            SKPaint br_Fill;
            var pn_Line = _borderPaint;
            canvas.DrawLine(new SKPoint(rect.Left + (rect.Width / 10), rect.Top), new SKPoint((rect.Left + (rect.Width)) - (rect.Width / 10), bounds.Top + (bounds.Height * 0.05f)), pn_Line);
            canvas.DrawRect(rect, _lightGrayPaint);
            canvas.DrawRect(rect, pn_Line);
            rect2 = SKRect.Create(rect.Left + (rect.Width / 2), rect.Top + (rect.Height / 6), rect.Width * 0.45f, rect.Width * 0.45f);
            br_Fill = _firstHatch!;
            canvas.DrawRect(rect2, br_Fill);
            canvas.DrawRect(rect2, pn_Line);
            rect2 = SKRect.Create(rect.Left + (rect.Width / 10), rect.Top + (rect.Height / 6), rect.Width * 0.3f, rect.Height * 0.2f);
            br_Fill = _secondHatch!;
            canvas.DrawRect(rect2, br_Fill);
            canvas.DrawRect(rect2, pn_Line);
            rect2 = SKRect.Create(rect.Left + (rect.Width / 10), rect.Top + (rect.Height * 0.5f), rect.Width * 0.3f, rect.Height * 0.4f);
            br_Fill = _thirdHatch!;
            canvas.DrawRect(rect2, br_Fill);
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Red, bounds.Height * 0.2f);
            var tempRect = SKRect.Create(bounds.Left, bounds.Top + (bounds.Height * 0.55f), bounds.Width, bounds.Height * 0.25f);
            textPaint.FakeBoldText = true;
            canvas.DrawCustomText("Radio", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, tempRect, out _);
            tempRect = SKRect.Create(bounds.Left, bounds.Top + (bounds.Height * 0.75f), bounds.Width, bounds.Height * 0.25f);
            canvas.DrawCustomText("Contest", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, tempRect, out _);
        }
        private void DrawSki(SKCanvas canvas, SKRect bounds)
        {
            SKPath gp = new SKPath();
            SKPoint[] pts = new SKPoint[25];
            var rect = SKRect.Create(bounds.Left, bounds.Top + (bounds.Height / 4), bounds.Width * 0.75f, bounds.Height * 0.7f);
            pts[0] = new SKPoint(rect.Left + (0.490566037735849f * rect.Width), rect.Top + (0.722222222222222f * rect.Height));
            pts[1] = new SKPoint(rect.Left + (0.207547169811321f * rect.Width), rect.Top + (0.611111111111111f * rect.Height));
            pts[2] = new SKPoint(rect.Left + (0.264150943396226f * rect.Width), rect.Top + (0.694444444444444f * rect.Height));
            pts[3] = new SKPoint(rect.Left + (0.509433962264151f * rect.Width), rect.Top + (0.75f * rect.Height));
            pts[4] = new SKPoint(rect.Left + (0.849056603773585f * rect.Width), rect.Top + (1 * rect.Height));
            pts[5] = new SKPoint(rect.Left + (1 * rect.Width), rect.Top + (0.944444444444444f * rect.Height));
            pts[6] = new SKPoint(rect.Left + (0.867924528301887f * rect.Width), rect.Top + (0.944444444444444f * rect.Height));
            pts[7] = new SKPoint(rect.Left + (0.566037735849057f * rect.Width), rect.Top + (0.75f * rect.Height));
            pts[8] = new SKPoint(rect.Left + (0.69811320754717f * rect.Width), rect.Top + (0.583333333333333f * rect.Height));
            pts[9] = new SKPoint(rect.Left + (0.509433962264151f * rect.Width), rect.Top + (0.416666666666667f * rect.Height));
            pts[10] = new SKPoint(rect.Left + (0.60377358490566f * rect.Width), rect.Top + (0.277777777777778f * rect.Height));
            pts[11] = new SKPoint(rect.Left + (0.679245283018868f * rect.Width), rect.Top + (0.277777777777778f * rect.Height));
            pts[12] = new SKPoint(rect.Left + (0.622641509433962f * rect.Width), rect.Top + (0.5f * rect.Height));
            pts[13] = new SKPoint(rect.Left + (0.735849056603774f * rect.Width), rect.Top + (0.25f * rect.Height));
            pts[14] = new SKPoint(rect.Left + (0.60377358490566f * rect.Width), rect.Top + (0.194444444444444f * rect.Height));
            pts[15] = new SKPoint(rect.Left + (0.660377358490566f * rect.Width), rect.Top + (0.111111111111111f * rect.Height));
            pts[16] = new SKPoint(rect.Left + (0.490566037735849f * rect.Width), rect.Top + (0 * rect.Height));
            pts[17] = new SKPoint(rect.Left + (0.415094339622642f * rect.Width), rect.Top + (0.0555555555555556f * rect.Height));
            pts[18] = new SKPoint(rect.Left + (0.452830188679245f * rect.Width), rect.Top + (0.138888888888889f * rect.Height));
            pts[19] = new SKPoint(rect.Left + (0 * rect.Width), rect.Top + (0.333333333333333f * rect.Height));
            pts[20] = new SKPoint(rect.Left + (0.377358490566038f * rect.Width), rect.Top + (0.222222222222222f * rect.Height));
            pts[21] = new SKPoint(rect.Left + (0.358490566037736f * rect.Width), rect.Top + (0.388888888888889f * rect.Height));
            pts[22] = new SKPoint(rect.Left + (0.415094339622642f * rect.Width), rect.Top + (0.527777777777778f * rect.Height));
            pts[23] = new SKPoint(rect.Left + (0.509433962264151f * rect.Width), rect.Top + (0.527777777777778f * rect.Height));
            pts[24] = new SKPoint(rect.Left + (0.490566037735849f * rect.Width), rect.Top + (0.722222222222222f * rect.Height));
            gp.AddPoly(pts);
            canvas.DrawPath(gp, _blackPaint);
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Red, bounds.Height * 0.15f);
            textPaint.FakeBoldText = true;
            var tempRect = SKRect.Create(bounds.Left + (bounds.Width / 2), bounds.Top + (bounds.Height * 0.2f), bounds.Width / 2, bounds.Height / 3);
            canvas.DrawCustomText("Ski", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, tempRect, out _);
            tempRect = SKRect.Create(bounds.Left + (bounds.Width / 2), bounds.Top + (bounds.Height * 0.4f), bounds.Width / 2, bounds.Height / 3);
            canvas.DrawCustomText("Trip", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, tempRect, out _);
            float tops;
            float lefts;
            lefts = bounds.Left + (bounds.Width / 2);
            tops = bounds.Top + (bounds.Height * 0.75f);
            canvas.DrawText("Pay $500", lefts, tops, true, textPaint);
        }
        private void DrawLottery(SKCanvas canvas, SKRect bounds)
        {
            SKRect rect;
            SKPoint[] pts = new SKPoint[9];
            SKPath gp = new SKPath();
            rect = SKRect.Create(bounds.Left + (bounds.Width / 4), bounds.Top + (bounds.Height / 5), bounds.Width / 2, bounds.Height / 2);
            pts[0] = new SKPoint(rect.Left + (0.2f * rect.Width), rect.Top + (0 * rect.Height));
            pts[1] = new SKPoint(rect.Left + (0.25f * rect.Width), rect.Top + (0.263157894736842f * rect.Height));
            pts[2] = new SKPoint(rect.Left + (0 * rect.Width), rect.Top + (0.684210526315789f * rect.Height));
            pts[3] = new SKPoint(rect.Left + (0.2f * rect.Width), rect.Top + (1 * rect.Height));
            pts[4] = new SKPoint(rect.Left + (1 * rect.Width), rect.Top + (0.947368421052632f * rect.Height));
            pts[5] = new SKPoint(rect.Left + (1 * rect.Width), rect.Top + (0.473684210526316f * rect.Height));
            pts[6] = new SKPoint(rect.Left + (0.85f * rect.Width), rect.Top + (0.315789473684211f * rect.Height));
            pts[7] = new SKPoint(rect.Left + (0.95f * rect.Width), rect.Top + (0.0526315789473684f * rect.Height));
            pts[8] = new SKPoint(rect.Left + (0.5f * rect.Width), rect.Top + (0.0526315789473684f * rect.Height));
            gp.AddPoly(pts);
            canvas.DrawPath(gp, _greenPaint);
            var textPaint = MiscHelpers.GetTextPaint(SKColors.White, bounds.Height / 5.0f, "Times New Roman");
            textPaint.FakeBoldText = true;
            float lefts;
            float tops;
            lefts = bounds.Left + (bounds.Width / 2);
            tops = bounds.Top + (bounds.Height * 0.35f);
            canvas.DrawText(" $", lefts, tops, true, textPaint);
            textPaint.Color = SKColors.Red;
            tops = bounds.Top + (bounds.Height * 0.7f);
            canvas.DrawText("Lottery", lefts, tops, true, textPaint);
        }
        private void DrawDeal(SKCanvas canvas, SKRect bounds)
        {
            SKRect rect;
            SKPoint[] pts = new SKPoint[9];
            SKPath gp = new SKPath();
            SKRegion reg_Temp;
            SKPaint br_Fill;
            var rect_Circle = SKRect.Create(bounds.Left + (bounds.Width / 10), bounds.Top + (bounds.Height / 6), (bounds.Width * 4) / 5, (bounds.Height * 2) / 3);
            var pn_Temp = MiscHelpers.GetStrokePaint(SKColors.Black, bounds.Width / 30);
            canvas.DrawOval(rect_Circle, pn_Temp);
            rect = SKRect.Create(bounds.Left + (bounds.Width * 0.2f), bounds.Top + (bounds.Height * 0.4f), bounds.Width * 0.5f, bounds.Height * 0.3f);
            pts[0] = new SKPoint(rect.Left + (0.119047619047619f * rect.Width), rect.Top + (0 * rect.Height));
            pts[1] = new SKPoint(rect.Left + (0.5f * rect.Width), rect.Top + (0.0476190476190476f * rect.Height));
            pts[2] = new SKPoint(rect.Left + (1 * rect.Width), rect.Top + (0.666666666666667f * rect.Height));
            pts[3] = new SKPoint(rect.Left + (0.952380952380952f * rect.Width), rect.Top + (0.952380952380952f * rect.Height));
            pts[4] = new SKPoint(rect.Left + (0.785714285714286f * rect.Width), rect.Top + (0.666666666666667f * rect.Height));
            pts[5] = new SKPoint(rect.Left + (0.928571428571429f * rect.Width), rect.Top + (0.952380952380952f * rect.Height));
            pts[6] = new SKPoint(rect.Left + (0.666666666666667f * rect.Width), rect.Top + (1 * rect.Height));
            pts[7] = new SKPoint(rect.Left + (0.30952380952381f * rect.Width), rect.Top + (0.904761904761905f * rect.Height));
            pts[8] = new SKPoint(rect.Left + (0 * rect.Width), rect.Top + (0.571428571428571f * rect.Height));
            gp.AddPoly(pts);
            canvas.DrawPath(gp, _whitePaint);
            br_Fill = MiscHelpers.GetLinearGradientPaint(SKColors.White, SKColors.Black, gp.Bounds, MiscHelpers.EnumLinearGradientPercent.Angle180);
            canvas.DrawPath(gp, br_Fill);
            canvas.DrawPath(gp, _borderPaint);
            rect = SKRect.Create(bounds.Left + (bounds.Width * 0.4f), bounds.Top + (bounds.Height * 0.4f), bounds.Width * 0.4f, bounds.Height * 0.2f);
            gp = new SKPath();
            pts = new SKPoint[8];
            pts[0] = new SKPoint(rect.Left + (0.794871794871795f * rect.Width), rect.Top + (0.357142857142857f * rect.Height));
            pts[1] = new SKPoint(rect.Left + (0.666666666666667f * rect.Width), rect.Top + (0.357142857142857f * rect.Height));
            pts[2] = new SKPoint(rect.Left + (0.487179487179487f * rect.Width), rect.Top + (0 * rect.Height));
            pts[3] = new SKPoint(rect.Left + (0 * rect.Width), rect.Top + (0.0714285714285714f * rect.Height));
            pts[4] = new SKPoint(rect.Left + (0.0256410256410256f * rect.Width), rect.Top + (0.285714285714286f * rect.Height));
            pts[5] = new SKPoint(rect.Left + (0.435897435897436f * rect.Width), rect.Top + (0.357142857142857f * rect.Height));
            pts[6] = new SKPoint(rect.Left + (0.82051282051282f * rect.Width), rect.Top + (1 * rect.Height));
            pts[7] = new SKPoint(rect.Left + (1 * rect.Width), rect.Top + (0.928571428571429f * rect.Height));
            gp.AddPoly(pts);
            canvas.DrawPath(gp, _whitePaint);
            br_Fill = MiscHelpers.GetLinearGradientPaint(SKColors.White, SKColors.Black, gp.Bounds, MiscHelpers.EnumLinearGradientPercent.Angle180);
            canvas.DrawPath(gp, br_Fill);
            canvas.DrawPath(gp, _borderPaint);
            reg_Temp = MiscHelpers.GetNewRegion(rect_Circle);
            gp = new SKPath();
            gp.AddLine(new SKPoint(bounds.Left + (bounds.Width / 4), bounds.Top + (bounds.Height * 0.4f)), new SKPoint(bounds.Left + (bounds.Width * 0.2f), bounds.Top + (bounds.Height * 0.7f)));
            gp.AddLine(new SKPoint(bounds.Left, bounds.Top + (bounds.Height * 0.5f)), new SKPoint(bounds.Left, bounds.Top + (bounds.Height * 0.4f)));
            gp.Close();
            reg_Temp.Intersect(gp);
            reg_Temp = MiscHelpers.GetNewRegion(rect_Circle);
            gp = new SKPath();
            gp.AddLine(new SKPoint(bounds.Left + (bounds.Width * 0.7f), bounds.Top + (bounds.Height * 0.45f)), new SKPoint(bounds.Left + (bounds.Width * 0.8f), bounds.Top + (bounds.Height * 0.7f)));
            gp.AddLine(new SKPoint(bounds.Left + bounds.Width, bounds.Top + (bounds.Height * 0.5f)), new SKPoint(bounds.Left + bounds.Width, bounds.Top + (bounds.Height * 0.4f)));
            gp.Close();
            reg_Temp.Intersect(gp);
            float lefts;
            float tops;
            lefts = bounds.Left + (bounds.Width / 2);
            tops = bounds.Top + (bounds.Height * 0.68f);
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Red, bounds.Height / 4);
            textPaint.FakeBoldText = true;
            canvas.DrawText("Deal", lefts, tops, true, textPaint);
        }
        private void DrawSweepstakes(SKCanvas canvas, SKRect bounds)
        {
            var firstTextPaint = MiscHelpers.GetTextPaint(SKColors.LimeGreen, bounds.Height * 0.2f, "Times New Roman");
            firstTextPaint.FakeBoldText = true;
            var otherBorder = MiscHelpers.GetStrokePaint(SKColors.LimeGreen, 1);
            SKRect tempRect;
            tempRect = SKRect.Create(bounds.Left + (bounds.Width * 0.2f), bounds.Top + (bounds.Height * 0.1f), bounds.Width * 0.6f, bounds.Height * 0.4f);
            canvas.DrawOval(tempRect, _bluePaint);
            canvas.DrawBorderText("$$$", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, firstTextPaint, otherBorder, tempRect);
            tempRect = SKRect.Create(bounds.Left, bounds.Top + (bounds.Height / 2), bounds.Width, bounds.Height / 4);
            firstTextPaint.Color = SKColors.Red;
            canvas.DrawCustomText("Sweep-", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, firstTextPaint, tempRect, out _);
            var finalRect = SKRect.Create(bounds.Left, tempRect.Bottom, bounds.Width, bounds.Height / 4);
            canvas.DrawCustomText("stakes", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, firstTextPaint, finalRect, out _);
        }
        private void DrawMailbox(SKCanvas canvas, SKRect bounds)
        {
            SKRect rect;
            SKPath gp = new SKPath();
            SKPaint br_Fill;
            gp.AddArc(SKRect.Create(bounds.Left + (bounds.Width / 30), bounds.Top + (bounds.Height / 2), bounds.Width / 5, bounds.Height / 5), 180, 180);
            gp.AddLine(new SKPoint(bounds.Left + (bounds.Width / 30) + (bounds.Width / 5), bounds.Top + ((bounds.Height * 29) / 30)), new SKPoint(bounds.Left + (bounds.Width / 30), bounds.Top + ((bounds.Height * 5) / 6)));
            gp.Close();
            canvas.DrawPath(gp, _darkSlateGrayPaint);
            gp = new SKPath();
            gp.AddArc(SKRect.Create(bounds.Left + (bounds.Width / 30), bounds.Top + (bounds.Height / 2), bounds.Width / 5, bounds.Height / 5), 270, 90);
            gp.AddLine(new SKPoint(bounds.Left + (bounds.Width / 30) + (bounds.Width / 5), bounds.Top + ((bounds.Height * 29) / 30)), new SKPoint(bounds.Left + ((bounds.Width * 29) / 30), bounds.Top + ((bounds.Height * 29) / 30)), false);
            gp.ArcTo(SKRect.Create((bounds.Left + ((bounds.Width * 29) / 30)) - (bounds.Width / 5), bounds.Top + (bounds.Height / 2), bounds.Width / 5, bounds.Height / 5), 0, -90, false); // iffy
            gp.Close();
            rect = SKRect.Create(bounds.Left + (bounds.Width / 30), bounds.Top + (bounds.Height / 2) + (bounds.Height / 8), bounds.Width / 2, bounds.Height / 5);
            canvas.DrawRect(rect, _whitePaint);
            canvas.DrawRect(rect, _borderPaint);
            rect = SKRect.Create(bounds.Left + (bounds.Width / 10), bounds.Top + (bounds.Height / 2) + (bounds.Height / 5), bounds.Width / 2, bounds.Height / 5);
            canvas.DrawRect(rect, _whitePaint);
            canvas.DrawRect(rect, _borderPaint);
            rect = SKRect.Create(rect.Left + (rect.Width / 20), rect.Top + (rect.Width / 20), rect.Width / 8, rect.Width / 8);
            canvas.DrawRect(rect, _lightBluePaint);
            canvas.DrawRect(rect, _borderPaint);
            br_Fill = MiscHelpers.GetLinearGradientPaint(SKColors.White, SKColors.SlateGray, gp.Bounds, MiscHelpers.EnumLinearGradientPercent.Angle180);
            canvas.DrawPath(gp, br_Fill);
            gp = new SKPath();
            gp.AddLine(new SKPoint(bounds.Left + (bounds.Width / 3), bounds.Top + ((bounds.Height * 3) / 4)), new SKPoint(bounds.Left + (bounds.Width / 3), bounds.Top + (bounds.Height / 5)), true);
            gp.AddLine(new SKPoint(bounds.Left + ((bounds.Width * 5) / 8), bounds.Top + (bounds.Height / 5)), new SKPoint(bounds.Left + ((bounds.Width * 5) / 8), (bounds.Top + (bounds.Height / 2)) - (bounds.Height / 10)));
            gp.AddLine(new SKPoint(bounds.Left + (bounds.Width / 3) + (bounds.Width / 10), (bounds.Top + (bounds.Height / 2)) - (bounds.Height / 10)), new SKPoint(bounds.Left + (bounds.Width / 3) + (bounds.Width / 10), bounds.Top + ((bounds.Height * 3) / 4)));
            gp.Close();
            canvas.DrawPath(gp, _redPaint);
        }
        #endregion
    }
}
