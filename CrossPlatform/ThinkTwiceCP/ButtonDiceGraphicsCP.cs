using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using CommonBasicStandardLibraries.MVVMHelpers;
using SkiaSharp;
using SkiaSharpGeneralLibrary.Interfaces;
using SkiaSharpGeneralLibrary.SKExtensions;
namespace ThinkTwiceCP
{
    public class ButtonDiceGraphicsCP : ObservableObject
    {

        private bool _WillHold;

        public bool WillHold
        {
            get { return _WillHold; }
            set
            {
                if (SetProperty(ref _WillHold, value))
                    PaintUI?.DoInvalidate();
            }
        }

        private string _Text = "";

        public string Text
        {
            get { return _Text; }
            set
            {
                if (SetProperty(ref _Text, value))
                    PaintUI?.DoInvalidate();
            }
        }
        public IRepaintControl? PaintUI;
        public float ActualWidthHeight { get; set; }
        public float MinimumWidthHeight { get; set; }
        private readonly SKPaint _selectPaint; //i may have to rethink (not sure).
        private readonly SKPaint _blackBorder;
        private readonly SKPaint _fillPaint;
        public float RoundedRadius { get; set; } = 8;
        public ButtonDiceGraphicsCP()
        {
            _blackBorder = MiscHelpers.GetStrokePaint(SKColors.Black, 1);
            _fillPaint = MiscHelpers.GetSolidPaint(SKColors.White);
            _selectPaint = BaseDeckGraphicsCP.GetStandardSelectPaint();
        }
        public void DrawDice(SKCanvas dc)
        {
            dc.Clear(); //always needs to clear for this one.
            if (Text == "-1" || Text == "")
                return; //because can't be -1.
            SKRect rect_Card;
            rect_Card = SKRect.Create(0, 0, ActualWidthHeight, ActualWidthHeight); //i think
            dc.DrawRoundRect(rect_Card, RoundedRadius, RoundedRadius, _fillPaint);
            if (WillHold == true)
                dc.DrawRoundRect(rect_Card, RoundedRadius, RoundedRadius, _selectPaint);
            SKPaint thisPaint;
            float fontSize = rect_Card.Height * 0.9f;
            thisPaint = MiscHelpers.GetTextPaint(SKColors.Blue, fontSize);
            thisPaint.FakeBoldText = WillHold;
            dc.DrawBorderText(Text, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, thisPaint, _blackBorder, rect_Card);
        }
    }
}