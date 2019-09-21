using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers;
using SkiaSharp;
using SkiaSharpGeneralLibrary.Interfaces;
using SkiaSharpGeneralLibrary.SKExtensions;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using static SkiaSharpGeneralLibrary.SKExtensions.TextExtensions;
namespace RummyDiceCP
{
    public class RummyDiceGraphicsCP : ObservableObject
    {
        public IRepaintControl PaintUI;

        private EnumColorType _Color;

        public EnumColorType Color
        {
            get { return _Color; }
            set
            {
                if (SetProperty(ref _Color, value))
                {
                    //can decide what to do when property changes
                    PopulateFill();
                    PaintUI.DoInvalidate();
                }

            }
        }
        private bool _IsSelected;

        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                if (SetProperty(ref _IsSelected, value))
                {
                    PaintUI.DoInvalidate();
                }

            }
        }

        private string _Value = "";

        public string Value
        {
            get { return _Value; }
            set
            {
                if (SetProperty(ref _Value, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private bool _Visible = true;

        public bool Visible
        {
            get { return _Visible; }
            set
            {
                if (SetProperty(ref _Visible, value))
                {
                    //can decide what to do when property changes
                }

            }
        }


        public float ActualWidthHeight { get; set; }
        public float MinimumWidthHeight { get; set; }
        public RummyDiceGraphicsCP(IRepaintControl paintUI)
        {
            PaintUI = paintUI; //to make sure i don't forget.
            Load();

        }

        private SKPaint? _selectPaint; //i may have to rethink (not sure).
        private SKPaint? _fillPaint;
        private SKPaint? _borderPaint;
        private SKPaint? _textBorder;

        private bool CanStartDrawing()
        {
            return Color != EnumColorType.None;
        }

        private void PopulateFill()
        {
            _fillPaint = Color switch
            {
                EnumColorType.Blue => MiscHelpers.GetSolidPaint(SKColors.Blue),
                EnumColorType.Green => MiscHelpers.GetSolidPaint(SKColors.Green),
                EnumColorType.Red => MiscHelpers.GetSolidPaint(SKColors.Red),
                EnumColorType.Yellow => MiscHelpers.GetSolidPaint(SKColors.Yellow),
                _ => throw new BasicBlankException("Not Found"),
            };
        }
        private float RoundedRadius { get; set; } = 8;
        private void Load()
        {
            _selectPaint = BaseDeckGraphicsCP.GetDarkerSelectPaint(); //this time, need the darker ones.
            BasicData thisData = Resolve<BasicData>();
            if (thisData.IsXamarinForms == true)
                _borderPaint = MiscHelpers.GetStrokePaint(SKColors.White, 3);
            else
                _borderPaint = MiscHelpers.GetStrokePaint(SKColors.White, 4);
            _textBorder = MiscHelpers.GetStrokePaint(SKColors.Black, 2);
            MiscHelpers.DefaultFont = "Tahoma";
        }
        public void DrawDice(SKCanvas dc)
        {
            if (Visible == false)
                return; //just in case.
            if (CanStartDrawing() == false)
                return;
            dc.Clear();
            SKRect rect_Card;
            rect_Card = SKRect.Create(0, 0, ActualWidthHeight, ActualWidthHeight); //i think
            dc.DrawRoundRect(rect_Card, RoundedRadius, RoundedRadius, _fillPaint);
            if (IsSelected == true)
                dc.DrawRoundRect(rect_Card, RoundedRadius, RoundedRadius, _selectPaint);
            dc.DrawRoundRect(rect_Card, RoundedRadius, RoundedRadius, _borderPaint);
            var fontSize = rect_Card.Height * .5f;
            SKPaint textPaint = MiscHelpers.GetTextPaint(SKColors.White, fontSize);
            dc.DrawBorderText(Value, EnumLayoutOptions.Center, EnumLayoutOptions.Center, textPaint, _textBorder, rect_Card);
        }
    }
}