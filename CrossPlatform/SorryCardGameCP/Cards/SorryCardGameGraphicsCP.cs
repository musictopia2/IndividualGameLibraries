using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using SorryCardGameCP.Data;
using System;
using System.Linq;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace SorryCardGameCP.Cards
{
    public class SorryCardGameGraphicsCP : ObservableObject, IDeckGraphicsCP
    {
        public BaseDeckGraphicsCP? MainGraphics { get; set; }
        private EnumCategory _category = EnumCategory.Blank;
        public EnumCategory Category
        {
            get
            {
                return _category;
            }
            set
            {
                if (SetProperty(ref _category, value) == true)
                    MainGraphics?.PaintUI?.DoInvalidate();
            }
        }
        private int _value;
        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (SetProperty(ref _value, value) == true)
                    MainGraphics?.PaintUI?.DoInvalidate();
            }
        }
        private EnumSorry _sorry = EnumSorry.Blank;
        public EnumSorry Sorry
        {
            get
            {
                return _sorry;
            }
            set
            {
                if (SetProperty(ref _sorry, value) == true)
                    MainGraphics?.PaintUI?.DoInvalidate();
            }
        }
        private bool _drew;

        public bool Drew
        {
            get { return _drew; }
            set
            {
                if (SetProperty(ref _drew, value))
                {
                    MainGraphics?.PaintUI?.DoInvalidate();
                }

            }
        }

        private string _color = "";

        public string Color
        {
            get { return _color; }
            set
            {
                if (SetProperty(ref _color, value))
                {
                    MainGraphics?.PaintUI?.DoInvalidate();
                }
            }
        }
        public bool NeedsToDrawBacks => true; //if you don't need to draw backs, then set false.
        public bool CanStartDrawing()
        {
            if (MainGraphics!.IsUnknown)
                return true; //default to true.  change to what you need to start drawing.
            if (Sorry == EnumSorry.Blank || Category == EnumCategory.Blank)
                return false;
            if (Sorry == EnumSorry.OnBoard)
            {
                if (Color == "")
                    return false;
                if (Category == EnumCategory.Home || Category == EnumCategory.Start)
                    return true;
                return false;
            }
            return true; //maybe it was okay.
        }
        public void DrawDefaultRectangles(SKCanvas dc, SKRect rect_Card, SKPaint thisPaint)
        {
            MainGraphics!.DrawDefaultRectangles(dc, rect_Card, thisPaint);
        }
        public void DrawBorders(SKCanvas dc, SKRect rect_Card)
        {
            SKPaint thisPaint = MiscHelpers.GetStrokePaint(SKColors.Black, 3); //i think 3 is enough.
            MainGraphics!.DrawDefaultRectangles(dc, rect_Card, thisPaint);
            DrawHighLights(dc, rect_Card);
        }
        private void DrawHighLights(SKCanvas dc, SKRect rect_Card) //on this game, has to be at the end.
        {
            if (MainGraphics!.IsSelected == true)
                MainGraphics.DrawDefaultRectangles(dc, rect_Card, _selectPaint!);
            else if (Drew == true)
                MainGraphics.DrawDefaultRectangles(dc, rect_Card, _pDrewPaint!);
        }
        public void DrawBacks(SKCanvas Canvas, SKRect rect_Card)
        {
            var firstRect = MainGraphics!.GetActualRectangle(3, 3, 60, 19);
            var fontSize = firstRect.Height * 0.4f; // can experiment
            var secondRect = MainGraphics.GetActualRectangle(3, 22, 60, 19);
            var thirdRect = MainGraphics.GetActualRectangle(3, 41, 60, 19);
            var fourthRect = MainGraphics.GetActualRectangle(3, 60, 60, 19);
            var thisPaint = MiscHelpers.GetTextPaint(SKColors.Black, fontSize);
            thisPaint.FakeBoldText = true;
            Canvas.DrawCustomText("Sorry!", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, thisPaint, firstRect, out _);
            Canvas.DrawCustomText("Revenge", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, thisPaint, secondRect, out _);
            Canvas.DrawCustomText("Card", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, thisPaint, thirdRect, out _);
            Canvas.DrawCustomText("Game", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, thisPaint, fourthRect, out _);
        }
        public SKColor GetFillColor()
        {
            if (MainGraphics!.IsUnknown == true)
                return SKColors.LightBlue; //this is the color of the back card if we have nothing else to it.
            return MainGraphics.GetFillColor();
        }
        public SKRect GetDrawingRectangle()
        {
            return MainGraphics!.GetDrawingRectangle();
        }
        private SKPaint? _selectPaint;
        private SKPaint? _pDrewPaint;
        private SKPaint? _lawnPaint;
        private SKPaint? _borderPaint;
        private SKPaint? _yellowPaint;
        private SKPaint? _bluePaint;
        private SKPaint? _redPaint;
        private SKPaint? _grayPaint;
        public void Init()
        {
            MainGraphics!.OriginalSize = new SKSize(66, 80); //change to what the original size is.
            _pDrewPaint = MainGraphics.GetStandardDrewPaint();
            _selectPaint = BaseDeckGraphicsCP.GetStandardSelectPaint();
            _lawnPaint = MiscHelpers.GetSolidPaint(SKColors.LawnGreen);
            _yellowPaint = MiscHelpers.GetSolidPaint(SKColors.Yellow);
            _bluePaint = MiscHelpers.GetSolidPaint(SKColors.Blue);
            _redPaint = MiscHelpers.GetSolidPaint(SKColors.Red);
            _grayPaint = MiscHelpers.GetSolidPaint(SKColors.Gray);
            _borderPaint = MiscHelpers.GetStrokePaint(SKColors.Black, 1);
        }
        public void DrawImage(SKCanvas canvas, SKRect rect_Card)
        {
            if (Sorry == EnumSorry.OnBoard)
            {
                DrawOnBoardCards(canvas);
                DrawHighLights(canvas, rect_Card);
                return;
            }
            if (Category == EnumCategory.Regular)
            {
                DrawPawn(canvas, rect_Card);
                DrawHighLights(canvas, rect_Card);
                return;
            }
            if (Category == EnumCategory.Sorry)
            {
                DrawSorry(canvas);
                DrawHighLights(canvas, rect_Card);
                return;
            }
            CustomBasicList<string> thisList;
            switch (Category)
            {
                case EnumCategory.Play2:
                    {
                        thisList = new CustomBasicList<string>() { "Play 2", "numbers", "to help", "you reach", "21!" };
                        break;
                    }

                case EnumCategory.Slide:
                    {
                        thisList = new CustomBasicList<string>() { Value.ToString(), "", "SLIDE" };
                        break;
                    }

                case EnumCategory.Switch:
                    {
                        thisList = new CustomBasicList<string>() { "Switch", "", "Direction" };
                        break;
                    }

                case EnumCategory.Take2:
                    {
                        thisList = new CustomBasicList<string>() { "Take 2", "cards to", "add to", "your", "Hand!" };
                        break;
                    }

                default:
                    {
                        throw new BasicBlankException("Not Supported");
                    }
            }
            DrawMisc(canvas, thisList);
            DrawHighLights(canvas, rect_Card);
        }
        private void DrawMisc(SKCanvas canvas, CustomBasicList<string> thisList)
        {
            SKRect firstRect;
            SKRect secondRect;
            SKRect thirdRect = default;
            var tempRect = MainGraphics!.GetActualRectangle(4, 4, 57, 73);
            canvas.DrawRoundRect(tempRect, 3, 3, _yellowPaint);
            if (thisList.Count == 3)
            {
                if (Category == EnumCategory.Slide)
                {
                    firstRect = MainGraphics.GetActualRectangle(3, 15, 60, 30);
                    secondRect = MainGraphics.GetActualRectangle(3, 40, 60, 30);
                }
                else
                {
                    firstRect = MainGraphics.GetActualRectangle(3, 3, 60, 26);
                    secondRect = MainGraphics.GetActualRectangle(3, 29, 60, 26);
                    thirdRect = MainGraphics.GetActualRectangle(3, 55, 60, 26);
                }
            }
            else
            {
                if (Category == EnumCategory.Slide)
                    throw new Exception("Slides can't have 5");
                firstRect = MainGraphics.GetActualRectangle(3, 3, 60, 15);
                secondRect = MainGraphics.GetActualRectangle(3, 18, 60, 15);
                thirdRect = MainGraphics.GetActualRectangle(3, 33, 60, 15);
            }
            var fontSize = firstRect.Height * 0.5f; // can always be adjusted as needed
            if (thisList.Count == 5)
                fontSize = firstRect.Height * 0.7f;
            var textPaint = MiscHelpers.GetTextPaint(SKColors.DarkGreen, fontSize);
            if (Category == EnumCategory.Slide)
            {
                fontSize = firstRect.Height * 0.62f;
                textPaint = MiscHelpers.GetTextPaint(SKColors.White, fontSize);

                canvas.DrawBorderText(thisList.First(), TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, _borderPaint!, firstRect);
                canvas.DrawBorderText(thisList.Last(), TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, _borderPaint!, secondRect);
            }
            else
            {
                canvas.DrawCustomText(thisList.First(), TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, firstRect, out _);
                if (!string.IsNullOrEmpty(thisList[1]))
                    canvas.DrawCustomText(thisList[1], TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, secondRect, out _);
                canvas.DrawCustomText(thisList[2], TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, thirdRect, out _);
            }
            if (thisList.Count == 3)
                return;
            firstRect = MainGraphics.GetActualRectangle(3, 48, 60, 15);
            secondRect = MainGraphics.GetActualRectangle(3, 63, 60, 15);
            canvas.DrawCustomText(thisList[3], TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, firstRect, out _);
            canvas.DrawCustomText(thisList[4], TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, secondRect, out _);
        }
        private void DrawSorry(SKCanvas canvas)
        {
            var tempRect = MainGraphics!.GetActualRectangle(4, 4, 57, 73);
            SKRect firstRect;
            SKRect secondRect;
            SKRect thirdRect;
            if (Sorry == EnumSorry.Dont)
            {
                canvas.DrawRoundRect(tempRect, 3, 3, _bluePaint);
                firstRect = MainGraphics.GetActualRectangle(3, 3, 60, 26);
                secondRect = MainGraphics.GetActualRectangle(3, 29, 60, 26);
                thirdRect = MainGraphics.GetActualRectangle(3, 55, 60, 26);
                var fontSize = firstRect.Height * 0.6f;
                var tempPaint = MiscHelpers.GetTextPaint(SKColors.White, fontSize);
                tempPaint.FakeBoldText = true;
                canvas.DrawCustomText("Don't", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, tempPaint, firstRect, out _);
                canvas.DrawCustomText("Be", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, tempPaint, secondRect, out _);
                canvas.DrawCustomText("Sorry", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, tempPaint, thirdRect, out _);
                return;
            }
            CustomBasicList<string> otherList;
            if (Sorry == EnumSorry.At21)
            {
                canvas.DrawRoundRect(tempRect, 3, 3, _redPaint);
                otherList = new CustomBasicList<string>() { "Play When", "Opponent", "Reaches 21!" };
            }
            else if (Sorry == EnumSorry.Regular)
            {
                canvas.DrawRoundRect(tempRect, 3, 3, _grayPaint);
                otherList = new CustomBasicList<string>() { "Flip Another", "Player Back", "To Start" };
            }
            else
            {
                throw new Exception("Not Supported");
            }
            SKRect fourthRect;
            firstRect = MainGraphics.GetActualRectangle(3, 3, 60, 21); // this one will allow a little more
            secondRect = MainGraphics.GetActualRectangle(3, 27, 60, 17);
            thirdRect = MainGraphics.GetActualRectangle(3, 44, 60, 17);
            fourthRect = MainGraphics.GetActualRectangle(3, 61, 60, 17);
            var firstFont = firstRect.Height * 0.8f;
            var secondFont = secondRect.Height * 0.55f;
            var firstPaint = MiscHelpers.GetTextPaint(SKColors.Black, firstFont);
            var secondPaint = MiscHelpers.GetTextPaint(SKColors.White, secondFont);
            canvas.DrawCustomText("Sorry!", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, firstPaint, firstRect, out _);
            canvas.DrawCustomText(otherList.First(), TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, secondPaint, secondRect, out _);
            canvas.DrawCustomText(otherList[1], TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, secondPaint, thirdRect, out _);
            canvas.DrawCustomText(otherList[2], TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, secondPaint, fourthRect, out _);
        }
        private void DrawOnBoardCards(SKCanvas canvas)
        {
            var tempRect = MainGraphics!.GetActualRectangle(5, 5, 55, 70);
            canvas.DrawRoundRect(tempRect, 3, 3, _lawnPaint);
            PawnPiecesCP<EnumColorChoices> thisPawn = new PawnPiecesCP<EnumColorChoices>();
            thisPawn.NeedsToClear = false;
            thisPawn.Location = MainGraphics.GetActualPoint(new SKPoint(6, 6));
            var tempSize = MainGraphics.GetActualSize(48, 48);
            thisPawn.ActualHeight = tempSize.Height;
            thisPawn.ActualWidth = tempSize.Width;
            thisPawn.MainColor = Color;
            thisPawn.DrawImage(canvas);
            string text;
            if (Category == EnumCategory.Home)
                text = "Home";
            else if (Category == EnumCategory.Start)
                text = "Start";
            else
                throw new BasicBlankException("Must be home or start");
            var thisRect = MainGraphics.GetActualRectangle(2, 50, 60, 28);
            var fontSize = thisRect.Height * .65f;
            var thisPaint = MiscHelpers.GetTextPaint(SKColors.Black, fontSize);
            thisPaint.FakeBoldText = true;
            canvas.DrawCustomText(text, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, thisPaint, thisRect, out _);
        }
        private void DrawPawn(SKCanvas canvas, SKRect rect_Card)
        {
            var tempRect = MainGraphics!.GetActualRectangle(4, 4, 57, 73);
            canvas.DrawRoundRect(tempRect, 3, 3, _lawnPaint);
            PawnPiecesCP<EnumColorChoices> thisPawn = new PawnPiecesCP<EnumColorChoices>();
            thisPawn.NeedsToClear = false;
            thisPawn.Location = MainGraphics.GetActualPoint(new SKPoint(6, 10));
            var tempSize = MainGraphics.GetActualSize(50, 50);
            thisPawn.ActualHeight = tempSize.Height;
            thisPawn.ActualWidth = tempSize.Width;
            thisPawn.MainColor = cs.Black;
            thisPawn.DrawImage(canvas);
            var fontSize = rect_Card.Height * .5f;
            var thisPaint = MiscHelpers.GetTextPaint(SKColors.White, fontSize);
            thisPaint.FakeBoldText = true;
            canvas.DrawBorderText(Value.ToString(), TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, thisPaint, _borderPaint!, rect_Card);
        }
    }
}