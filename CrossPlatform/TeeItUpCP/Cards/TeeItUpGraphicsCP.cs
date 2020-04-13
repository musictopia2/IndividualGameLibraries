using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System.Linq;
namespace TeeItUpCP.Cards
{
    public class TeeItUpGraphicsCP : ObservableObject, IDeckGraphicsCP
    {
        public BaseDeckGraphicsCP? MainGraphics { get; set; }
        private int _points = -6; //so blank cards don't get drawn.

        public int Points
        {
            get { return _points; }
            set
            {
                if (SetProperty(ref _points, value))
                {
                    MainGraphics!.PaintUI?.DoInvalidate();
                }

            }
        }
        private bool _isMulligan;

        public bool IsMulligan
        {
            get { return _isMulligan; }
            set
            {
                if (SetProperty(ref _isMulligan, value))
                {
                    MainGraphics!.PaintUI?.DoInvalidate();
                }

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
                    MainGraphics!.PaintUI?.DoInvalidate();
                }

            }
        }
        public bool NeedsToDrawBacks => true; //if you don't need to draw backs, then set false.
        public bool CanStartDrawing()
        {
            if (MainGraphics!.IsUnknown == true)
                return true; //default to true.  change to what you need to start drawing.
            return Points != -6;
        }
        public void DrawDefaultRectangles(SKCanvas dc, SKRect rect_Card, SKPaint thisPaint)
        {
            MainGraphics!.DrawDefaultRectangles(dc, rect_Card, thisPaint);
        }
        public void DrawBorders(SKCanvas dc, SKRect rect_Card)
        {
            SKPaint thisPaint = MiscHelpers.GetStrokePaint(SKColors.Black, 3); //i think 3 is enough.
            MainGraphics!.DrawDefaultRectangles(dc, rect_Card, thisPaint);
            if (MainGraphics.IsSelected == true)
                MainGraphics.DrawDefaultRectangles(dc, rect_Card, _selectPaint!);
            else if (Drew == true)
                MainGraphics.DrawDefaultRectangles(dc, rect_Card, _pDrewPaint!);
        }
        public void DrawBacks(SKCanvas Canvas, SKRect rect_Card)
        {
            SKRect firstRect;
            SKRect secondRect;
            SKRect thirdRect;
            firstRect = MainGraphics!.GetActualRectangle(2, 2, 74, 33);
            secondRect = MainGraphics.GetActualRectangle(2, 35, 74, 33);
            thirdRect = MainGraphics.GetActualRectangle(2, 68, 74, 33);
            var fontSize = firstRect.Height * 0.6f; // can be adjusted
            var textPaint = MiscHelpers.GetTextPaint(SKColors.White, fontSize);
            Canvas.DrawBorderText("Tee", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, _blackBorder!, firstRect);
            Canvas.DrawBorderText("It", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, _blackBorder!, secondRect);
            Canvas.DrawBorderText("Up", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, _blackBorder!, thirdRect);
        }
        public SKColor GetFillColor()
        {
            if (MainGraphics!.IsUnknown == true)
                return SKColors.Green; //this is the color of the back card if we have nothing else to it.
            return SKColors.White;
        }
        public SKRect GetDrawingRectangle()
        {
            return MainGraphics!.GetDrawingRectangle();
        }
        private SKPaint? _selectPaint;
        private SKPaint? _pDrewPaint;
        private SKPaint? _blackBorder;
        private SKPaint? _greenPaint;
        public void Init()
        {
            MainGraphics!.OriginalSize = new SKSize(107, 135); //change to what the original size is.
            _pDrewPaint = MainGraphics.GetStandardDrewPaint();
            _selectPaint = BaseDeckGraphicsCP.GetStandardSelectPaint();
            _blackBorder = MiscHelpers.GetStrokePaint(SKColors.Black, 1);
            _greenPaint = MiscHelpers.GetSolidPaint(SKColors.Green);
        }
        private CustomBasicList<string> GetSingleList(string firstText)
        {
            return new CustomBasicList<string>() { firstText };
        }
        private CustomBasicList<string> GetPairList(string firstText, string secondText)
        {
            return new CustomBasicList<string>() { firstText, secondText };
        }
        private CustomBasicList<string> GetTextList()
        {
            if (IsMulligan == true)
                return GetSingleList("Mulligan");
            if (Points == -5)
                return GetPairList("Hole", "In One");
            if (Points == -3)
                return new CustomBasicList<string>() { "Albatross" };
            if (Points == -2)
                return new CustomBasicList<string>() { "Eagle" };
            if (Points == -1)
                return GetSingleList("Birdie");
            if (Points == 0)
                return GetSingleList("Par");
            if (Points == 1)
                return GetSingleList("Bogey");
            if (Points == 2)
                return GetPairList("Double", "Bogey");
            if (Points == 3)
                return GetPairList("Triple", "Bogey");
            if (Points == 4)
                return GetPairList("Out Of", "Bounds");
            if (Points == 5)
                return GetPairList("Water", "Hazard");
            if (Points == 6)
                return GetPairList("Sand", "Trap");
            if (Points == 7)
                return GetSingleList("In Rough");// i think this is single as well
            if (Points == 8)
                return GetPairList("Lost", "Ball");
            if (Points == 9)
                return GetSingleList("In Ravine");// before it was single.  if that is not correct, can fix.  can even base on what screen is used as well (?)
            throw new BasicBlankException("Nothing Found.  If Wrong, Then Fix");
        }
        public void DrawImage(SKCanvas canvas, SKRect rect_Card)
        {
            SKRect topRect;
            topRect = MainGraphics!.GetActualRectangle(13, 13, 50, 50);
            canvas.DrawOval(topRect, _greenPaint);
            if (IsMulligan == false)
            {
                var firstFonts = topRect.Height * 0.8f;
                var firstTextPaint = MiscHelpers.GetTextPaint(SKColors.White, firstFonts);
                canvas.DrawBorderText(Points.ToString(), TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, firstTextPaint, _blackBorder!, topRect);
            }
            SKRect fullBottom;
            fullBottom = MainGraphics.GetActualRectangle(4, 67, 68, 35);
            var firstBottom = MainGraphics.GetActualRectangle(4, 67, 68, 17);
            var secondBottom = MainGraphics.GetActualRectangle(4, 84, 68, 17);
            var thisList = GetTextList();
            var fontSize = fullBottom.Height * 0.41f;
            SKPaint thisPaint;
            if (IsMulligan == true)
                thisPaint = MiscHelpers.GetTextPaint(SKColors.Red, fontSize);
            else
                thisPaint = MiscHelpers.GetTextPaint(SKColors.Green, fontSize);
            if (thisList.Count == 1)
            {
                canvas.DrawCustomText(thisList.Single(), TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, thisPaint, fullBottom, out _);
            }
            else if (thisList.Count == 2)
            {
                canvas.DrawCustomText(thisList.First(), TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, thisPaint, firstBottom, out _);
                canvas.DrawCustomText(thisList.Last(), TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, thisPaint, secondBottom, out _);
            }
            else
            {
                throw new BasicBlankException("Rethink");
            }
        }
    }
}
