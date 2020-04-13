using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using BasicGameFrameworkLibrary.GameGraphicsCP.Dice;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;

namespace SequenceDiceCP.Logic
{
    public class MiscGraphicsCP : BaseGraphicsCP
    {

        private bool _wasPrevious;
        public bool WasPrevious
        {
            get
            {
                return _wasPrevious;
            }

            set
            {
                if (SetProperty(ref _wasPrevious, value) == true)
                    // code to run
                    PaintUI?.DoInvalidate();
            }
        }

        private int _number;
        public int Number
        {
            get
            {
                return _number;
            }

            set
            {
                if (SetProperty(ref _number, value) == true)
                    // code to run
                    PaintUI?.DoInvalidate();
            }
        }

        private readonly SKPaint _yellowPaint;
        private readonly SKPaint _borderPaint;
        private readonly SKPaint _text1Paint;
        private readonly SKPaint _whitePaint;
        private readonly SKPaint _grayPaint;

        public MiscGraphicsCP()
        {
            MainColor = cs.Transparent;
            _yellowPaint = MiscHelpers.GetSolidPaint(SKColors.Yellow);
            _borderPaint = MiscHelpers.GetStrokePaint(SKColors.Black, 2);
            _text1Paint = MiscHelpers.GetStrokePaint(SKColors.Black, 1);
            _whitePaint = MiscHelpers.GetSolidPaint(SKColors.White);
            _grayPaint = MiscHelpers.GetSolidPaint(SKColors.Gray);
        }
        public override void DrawImage(SKCanvas dc)
        {
            if (ActualHeight == 1)
            {
                return;
            }
            // anything that needs to be drawn for each space will be here.  if nothing is done, then will do nothing.
            if (Number == 0)
                return;// because no number associated with it.  must have a number
            if (Number == 10 || Number == 11)
                throw new BasicBlankException("Can't have 10 or 11 for number");
            if (Number < 1 || Number > 12)
                throw new BasicBlankException("Number has to be between 2 and 12 but no 10 or 11");
            // can be transparent and previous one (if removing space)
            if (ActualHeight != ActualWidth)
                throw new BasicBlankException("Must be perfect squares for this game");
            if (ActualWidth == 0 || ActualHeight == 0)
                throw new BasicBlankException("Can't be 0 for the actual width or height");
            var thisRect = GetMainRect();
            var newRect = SKRect.Create(3, 3, thisRect.Width - 6, thisRect.Height - 6);
            if (Number != 2 && Number != 12)
            {
                if (WasPrevious == true)
                    dc.DrawRect(thisRect, _yellowPaint);
                else
                    dc.DrawRect(thisRect, _whitePaint);
                SKPaint otherTextPaint;
                if (MainColor.Equals(cs.Transparent) == false)
                {
                    // this means do a circle
                    dc.DrawOval(newRect, _borderPaint);
                    dc.DrawOval(newRect, MainPaint); // i think
                    otherTextPaint = MiscHelpers.GetTextPaint(SKColors.Aqua, newRect.Height * 1.2f);
                }
                else
                {
                    otherTextPaint = MiscHelpers.GetTextPaint(SKColors.Red, newRect.Height * 1.2f);
                }
                dc.DrawBorderText(Number.ToString(), TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, otherTextPaint, _text1Paint, newRect);
                dc.DrawRect(thisRect, _borderPaint);
                return;
            }
            dc.DrawRect(thisRect, _grayPaint);
            StandardDiceGraphicsCP firstDice = new StandardDiceGraphicsCP();
            {
                var withBlock = firstDice;
                withBlock.Location = new SKPoint(newRect.Location.X, newRect.Location.Y);
                withBlock.NeedsToClear = false;
                withBlock.ActualWidthHeight = newRect.Height / 2.1f;
                withBlock.Value = Number / 2;
                withBlock.UseSmallerBorders();
                if (WasPrevious == true)
                    withBlock.FillColor = cs.Yellow;
            }

            StandardDiceGraphicsCP secondDice = new StandardDiceGraphicsCP();
            {
                var withBlock1 = secondDice;
                withBlock1.Location = new SKPoint(firstDice.Location.X + 3 + firstDice.ActualWidthHeight, firstDice.Location.Y + 3 + firstDice.ActualWidthHeight);
                withBlock1.NeedsToClear = false;
                withBlock1.ActualWidthHeight = newRect.Height / 2.1f;
                withBlock1.Value = Number / 2;
                withBlock1.UseSmallerBorders();
                if (WasPrevious == true)
                    withBlock1.FillColor = cs.Yellow;
            }
            firstDice.DrawDice(dc);
            secondDice.DrawDice(dc);
            if (MainColor.Equals(cs.Transparent) == false)
            {
                // this means do a circle
                dc.DrawOval(newRect, _borderPaint);
                dc.DrawOval(newRect, MainPaint); // i think
                SKPaint otherTextPaint;
                if (Number == 2)
                    otherTextPaint = MiscHelpers.GetTextPaint(SKColors.Aqua, newRect.Height * 1.2f);
                else
                    otherTextPaint = MiscHelpers.GetTextPaint(SKColors.Aqua, newRect.Height * 0.7f);
                dc.DrawBorderText(Number.ToString(), TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, otherTextPaint, _text1Paint, newRect);
                dc.DrawRect(thisRect, _borderPaint);
                return;
            }
            dc.DrawRect(thisRect, _borderPaint);
        }
    }
}