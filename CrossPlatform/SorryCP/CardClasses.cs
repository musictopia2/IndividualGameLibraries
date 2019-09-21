using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace SorryCP
{
    public class DeckCount : IDeckCount
    {
        int IDeckCount.GetDeckCount()
        {
            return 45;
        }
    }
    public class CardInfo : SimpleDeckObject, IDeckObject
    {
        public int Value { get; set; }
        public bool Sorry { get; set; }
        public bool CanTakeFromStart { get; set; }
        public bool AnotherTurn { get; set; }
        public bool SplitMove { get; set; }
        public bool Trade { get; set; }
        public int SpacesBackwards { get; set; }
        public int SpacesForward { get; set; }
        public string Details { get; set; } = "";
        public CardInfo()
        {
            DefaultSize = new SKSize(55, 72); //well see.
        }
        public void Populate(int chosen)
        {
            Deck = chosen;
            if (chosen <= 0)
                throw new BasicBlankException("Cannot be 0 or below");
            if (chosen < 6)
            {
                Value = 1;
                Details = "Move from start or move one forward one space";
                CanTakeFromStart = true;
            }
            else if (chosen < 10)
            {
                Details = "Move from start or move forward 2 spaces.  Draw Again.";
                Value = 2;
                CanTakeFromStart = true;
                AnotherTurn = true;
            }
            else if (chosen < 14)
            {
                Details = "Move forward three spaces.";
                Value = 3;
            }
            else if (chosen < 18)
            {
                Details = "Move backward four.";
                Value = 4;
                SpacesBackwards = 4;
            }
            else if (chosen < 22)
            {
                Details = "Move forward five spaces.";
                Value = 5;
            }
            else if (chosen < 26)
            {
                Details = "Move one forward seven spaces or split move between 2 pawns the seven spaces.";
                Value = 7;
                SplitMove = true;
            }
            else if (chosen < 30)
            {
                Details = "Move forward eight spaces.";
                Value = 8;
            }
            else if (chosen < 34)
            {
                Details = "Move forward ten spaces or move backward one space.";
                Value = 10;
                SpacesBackwards = 1;
            }
            else if (chosen < 38)
            {
                Details = "Move forward eleven spaces or change places with an opponent.";
                Value = 11;
                Trade = true;
            }
            else if (chosen < 42)
            {
                Details = "Move forward twelve spaces.";
                Value = 12;
            }
            else if (chosen < 46)
            {
                Details = "Move from start and swith places with an opponent who bump back to start.";
                Value = 13;
                Sorry = true;
            }
            else
            {
                throw new BasicBlankException("Cannot find sorry card.  Rethink");
            }

            if (Value < 13 && Value != 4)
                SpacesForward = Value;
        }
        public void Reset() { }
    }
    public class CardGraphicsCP : ObservableObject, IDeckGraphicsCP
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

                }

            }
        }
        public bool NeedsToDrawBacks => false; //if you don't need to draw backs, then set false.
        private SorryMainGameClass? _mainGame;
        public bool CanStartDrawing()
        {
            if (_mainGame == null)
                _mainGame = Resolve<SorryMainGameClass>(); //this means has to forgo the unit testing
            return _mainGame.SaveRoot!.DidDraw;
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
        public void DrawBacks(SKCanvas canvas, SKRect rect_Card) { }
        public SKColor GetFillColor()
        {
            if (MainGraphics!.IsUnknown == true)
                return SKColors.White; //this is the color of the back card if we have nothing else to it.
            return MainGraphics.GetFillColor();
        }
        public SKRect GetDrawingRectangle()
        {
            return MainGraphics!.GetDrawingRectangle();
        }
        public void Init()
        {
            MainGraphics = new BaseDeckGraphicsCP(); //hopefully this simple (not sure)
            MainGraphics.ThisGraphics = this;
            MainGraphics.OriginalSize = new SKSize(55, 72); //change to what the original size is.
            MainGraphics.NeedsToClear = false; //because something else will draw this.
        }
        public void DrawImage(SKCanvas canvas, SKRect rect_Card)
        {
            if (_mainGame == null)
                _mainGame = Resolve<SorryMainGameClass>(); //this means has to forgo the unit testing
            string realText;
            string value = _mainGame.SaveRoot!.CurrentCard!.Value.ToString();
            if (value == "13")
                realText = "S";
            else
                realText = value;
            float fontSize;
            if (int.Parse(value) < 10 || realText == "S")
                fontSize = rect_Card.Width * 1.8f;
            else
                fontSize = rect_Card.Width * .9f;
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Black, fontSize);
            canvas.DrawCustomText(realText, TextExtensions.EnumLayoutOptions.Start, TextExtensions.EnumLayoutOptions.Center, textPaint, rect_Card, out _);
        }
    }
}