using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using BasicGameFramework.GameGraphicsCP.MiscClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System.Reflection;
namespace MilkRunCP
{
    public class MilkRunGraphicsCP : ObservableObject, IDeckGraphicsCP
    {
        public BaseDeckGraphicsCP? MainGraphics { get; set; }
        private bool _Drew;
        public bool Drew
        {
            get { return _Drew; }
            set
            {
                if (SetProperty(ref _Drew, value))
                {
                    MainGraphics?.PaintUI?.DoInvalidate();
                }

            }
        }
        private EnumMilkType _MilkType;
        public EnumMilkType MilkType
        {
            get { return _MilkType; }
            set
            {
                if (SetProperty(ref _MilkType, value))
                {
                    MainGraphics?.PaintUI?.DoInvalidate();
                }

            }
        }
        private EnumCardCategory _CardCategory;
        public EnumCardCategory CardCategory
        {
            get { return _CardCategory; }
            set
            {
                if (SetProperty(ref _CardCategory, value))
                {
                    MainGraphics?.PaintUI?.DoInvalidate();
                }
            }
        }
        private int _Points;
        public int Points
        {
            get { return _Points; }
            set
            {
                if (SetProperty(ref _Points, value))
                {
                    MainGraphics?.PaintUI?.DoInvalidate();
                }
            }
        }
        public bool NeedsToDrawBacks => true; //if you don't need to draw backs, then set false.
        public bool CanStartDrawing()
        {
            if (MainGraphics!.IsUnknown == true)
                return true; //default to true.  change to what you need to start drawing.
            return CardCategory != EnumCardCategory.None && MilkType != EnumMilkType.None;
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
        public void DrawBacks(SKCanvas canvas, SKRect rect_Card)
        {
            canvas.DrawRect(rect_Card, _backPaint);
            SKRect firstRect;
            firstRect = SKRect.Create(8, 8, rect_Card.Width - 6, rect_Card.Height / 2.1f);
            var secondRect = SKRect.Create(8, rect_Card.Height / 2, rect_Card.Width - 6, rect_Card.Height / 2.1f);
            canvas.DrawRect(firstRect, _redPaint);
            canvas.DrawRect(secondRect, _chocolatePaint);
            CustomBasicList<SKRect> thisList = new CustomBasicList<SKRect>();
            float fontSize = rect_Card.Height / 3.2f;
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Aqua, fontSize);
            thisList.Add(firstRect);
            thisList.Add(secondRect);
            int x = 0;
            string thisText;
            foreach (var thisRect in thisList)
            {
                x += 1;
                switch (x)
                {
                    case 1:
                        {
                            thisText = "Milk";
                            break;
                        }

                    case 2:
                        {
                            thisText = "Run";
                            break;
                        }

                    default:
                        {
                            throw new BasicBlankException("Only 2 lines");
                        }
                }
                canvas.DrawBorderText(thisText, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, _textBorder, thisRect);
            }
        }
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
        private Assembly? _thisAssembly;
        private SKPaint? _selectPaint;
        private SKPaint? _pDrewPaint;
        private SKBitmap? _chocolateImage;
        private SKBitmap? _strawberryImage;
        private SKPaint? _redPaint;
        private SKPaint? _chocolatePaint;
        private SKPaint? _limePaint;
        private SKPaint? _textBorder;
        private SKPaint? _backPaint;
        private SKPaint? _blackPaint;
        private void GetImages()
        {
            _chocolateImage = ImageExtensions.GetSkBitmap(_thisAssembly, "chocolate.png");
            _strawberryImage = ImageExtensions.GetSkBitmap(_thisAssembly, "strawberry.png");
        }
        public void Init()
        {
            MainGraphics!.OriginalSize = new SKSize(55, 72); //change to what the original size is.
            _thisAssembly = Assembly.GetAssembly(this.GetType());
            GetImages();
            _pDrewPaint = MainGraphics.GetStandardDrewPaint();
            _selectPaint = BaseDeckGraphicsCP.GetStandardSelectPaint();
            _redPaint = MiscHelpers.GetSolidPaint(SKColors.Red);
            _limePaint = MiscHelpers.GetSolidPaint(SKColors.Lime);
            _textBorder = MiscHelpers.GetStrokePaint(SKColors.Black, 2);
            _chocolatePaint = MiscHelpers.GetSolidPaint(SKColors.Chocolate);
            _backPaint = MiscHelpers.GetSolidPaint(SKColors.Aqua);
            _blackPaint = MiscHelpers.GetSolidPaint(SKColors.Black);
        }
        public void DrawImage(SKCanvas canvas, SKRect rect_Card)
        {
            SKRect firstRect;
            SKRect secondRect;
            firstRect = SKRect.Create(2, 2, rect_Card.Width - 6, rect_Card.Height / 2.1f);
            secondRect = SKRect.Create(rect_Card.Width / 4, rect_Card.Height / 2, rect_Card.Width / 2, rect_Card.Height / 2.2f);
            var circleRect = SKRect.Create(2, rect_Card.Height / 2, rect_Card.Width - 4, rect_Card.Height / 2.2f);
            switch (MilkType)
            {
                case EnumMilkType.Strawberry:
                    canvas.DrawBitmap(_strawberryImage, firstRect);
                    break;
                case EnumMilkType.Chocolate:
                    canvas.DrawBitmap(_chocolateImage, firstRect);
                    break;
                default:
                    throw new BasicBlankException("Must be strawberry or chocolate");
            }
            string thisText = "";
            if (CardCategory != EnumCardCategory.Joker)
            {
                if (CardCategory == EnumCardCategory.Go)
                    thisText = "Go";
                else if (CardCategory == EnumCardCategory.Stop)
                    thisText = "Stop";
                else
                    thisText = Points.ToString();
            }
            switch (CardCategory)
            {
                case EnumCardCategory.Joker:
                    var newRect = MainGraphics!.GetActualRectangle(13, 36, 30, 30);
                    if (MilkType == EnumMilkType.Chocolate)
                        canvas.DrawSmiley(newRect, _chocolatePaint!, _textBorder!, _blackPaint!);
                    else if (MilkType == EnumMilkType.Strawberry)
                        canvas.DrawSmiley(newRect, _redPaint!, _textBorder!, _blackPaint!);
                    return;
                case EnumCardCategory.Go:
                    canvas.DrawOval(circleRect, _limePaint);
                    break;
                case EnumCardCategory.Stop:
                    canvas.DrawOval(circleRect, _redPaint);
                    break;
                default:
                    break;
            }
            if (thisText == "")
                throw new BasicBlankException("No text found");
            var fontSize = secondRect.Height * 1.2f;
            if (thisText == "Stop")
                fontSize = secondRect.Height * .7f; //otherwise parts get cut off.
            var textPaint = MiscHelpers.GetTextPaint(SKColors.White, fontSize);
            canvas.DrawBorderText(thisText, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, _textBorder, secondRect);
        }
    }
}