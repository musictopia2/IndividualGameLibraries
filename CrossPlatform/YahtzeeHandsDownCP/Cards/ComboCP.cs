using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System;
using System.Linq;

namespace YahtzeeHandsDownCP.Cards
{
    public class ComboCP : ObservableObject, IDeckGraphicsCP
    {
        public ComboCardInfo? ThisCombo;
        public BaseDeckGraphicsCP? MainGraphics { get; set; }
        private bool _drew;
        public bool Drew
        {
            get { return _drew; }
            set
            {
                if (SetProperty(ref _drew, value))
                {
                    MainGraphics!.PaintUI!.DoInvalidate();
                }
            }
        }
        public bool NeedsToDrawBacks => false; //if you don't need to draw backs, then set false.
        public bool CanStartDrawing()
        {
            return ThisCombo != null;
        }
        public void DrawDefaultRectangles(SKCanvas dc, SKRect rect_Card, SKPaint thisPaint)
        {
            MainGraphics!.DrawDefaultRectangles(dc, rect_Card, thisPaint);
        }
        public void DrawBorders(SKCanvas dc, SKRect rect_Card)
        {
            SKPaint thisPaint = MiscHelpers.GetStrokePaint(SKColors.Black, 3); //i think 3 is enough.
            MainGraphics!.DrawDefaultRectangles(dc, rect_Card, thisPaint);
        }
        public void DrawBacks(SKCanvas Canvas, SKRect rect_Card) { }
        public SKColor GetFillColor()
        {
            if (MainGraphics!.IsUnknown == true)
                return SKColors.Red; //this is the color of the back card if we have nothing else to it.
            return MainGraphics.GetFillColor();
        }
        public SKRect GetDrawingRectangle()
        {
            return MainGraphics!.GetDrawingRectangle();
        }
        private SKPaint? _whitePaint;
        private SKPaint? _yellowPaint;
        private SKPaint? _redPaint;
        private SKPaint? _thickBorder;
        public void Init()
        {
            MainGraphics!.OriginalSize = new SKSize(230, 130); //change to what the original size is.
            _whitePaint = MiscHelpers.GetSolidPaint(SKColors.White);
            _yellowPaint = MiscHelpers.GetSolidPaint(SKColors.Yellow);
            _redPaint = MiscHelpers.GetSolidPaint(SKColors.Red);
            _thickBorder = MiscHelpers.GetStrokePaint(SKColors.Black, 2);
        }
        public void DrawImage(SKCanvas canvas, SKRect rect_Card)
        {
            var tempRect = MainGraphics!.GetActualRectangle(3, 3, 224, 124);
            canvas.DrawRect(tempRect, _redPaint);
            var firstRect = MainGraphics.GetActualRectangle(4, 4, 50, 50);
            canvas.DrawRect(firstRect, _whitePaint);
            var fontSize = MainGraphics.GetFontSize(14); // can adjust as needed
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Black, fontSize);
            canvas.DrawCustomText(ThisCombo!.Points.ToString() + " PTS.", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, firstRect, out _);
            var SecondRect = MainGraphics.GetActualRectangle(56, 4, 170, 50);
            var temps = MainGraphics.GetFontSize(3);
            canvas.DrawRoundRect(SecondRect, temps, temps, _yellowPaint);
            canvas.DrawRoundRect(SecondRect, temps, temps, _thickBorder);
            firstRect = MainGraphics.GetActualRectangle(56, 4, 170, 16);
            fontSize = MainGraphics.GetFontSize(16);
            textPaint = MiscHelpers.GetTextPaint(SKColors.Black, fontSize);
            textPaint.FakeBoldText = true;
            canvas.DrawCustomText(ThisCombo.FirstDescription, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, firstRect, out _);
            string firstText;
            string secondText;
            var tempList = ThisCombo.SecondDescription.Split("|").ToCustomBasicList();
            if (tempList.Count != 2)
                throw new Exception("Needs 2 lines of text");
            firstText = tempList.First();
            secondText = tempList.Last();
            firstRect = MainGraphics.GetActualRectangle(56, 20, 170, 16);
            textPaint = MiscHelpers.GetTextPaint(SKColors.Black, fontSize);
            SecondRect = MainGraphics.GetActualRectangle(56, 36, 170, 16);
            canvas.DrawCustomText(firstText, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, firstRect, out _);
            canvas.DrawCustomText(secondText, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, SecondRect, out _);
            float diffs;
            diffs = 44; // i think
            var finalSize = MainGraphics.GetFontSize(68);
            float lefts;
            lefts = 5;
            var tops = 58;
            var otherSize = MainGraphics.GetFontSize(42);
            foreach (var thisItem in ThisCombo.SampleList)
            {
                YahtzeeHandsDownGraphicsCP thisCard = new YahtzeeHandsDownGraphicsCP();
                thisCard.MainGraphics = new BaseDeckGraphicsCP(); //think.
                thisCard.MainGraphics.ThisGraphics = thisCard;
                thisCard.MainGraphics.NeedsToClear = false;
                thisCard.Init();//hopefully this too.
                thisCard.MainGraphics.ActualWidth = otherSize;
                thisCard.MainGraphics.ActualHeight = finalSize;
                thisCard.MainGraphics.Location = MainGraphics.GetActualPoint(new SKPoint(lefts, tops));
                thisCard.Color = thisItem.Color;
                thisCard.IsWild = thisItem.IsWild;
                thisCard.FirstValue = thisItem.FirstValue;
                thisCard.SecondValue = thisItem.SecondValue;
                thisCard.MainGraphics.DrawImage(canvas);
                lefts += diffs;
            }
        }
    }
}
