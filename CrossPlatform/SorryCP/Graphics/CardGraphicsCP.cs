using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using SorryCP.Data;
using System;
using System.Collections.Generic;
using System.Text;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace SorryCP.Graphics
{
    public class CardGraphicsCP : ObservableObject, IDeckGraphicsCP
    {
        public BaseDeckGraphicsCP? MainGraphics { get; set; }
        private bool _drew;
        public bool Drew
        {
            get { return _drew; }
            set
            {
                if (SetProperty(ref _drew, value))
                {

                }

            }
        }
        public bool NeedsToDrawBacks => false; //if you don't need to draw backs, then set false.
        private SorryGameContainer? _gameContainer;
        public bool CanStartDrawing()
        {
            if (_gameContainer == null)
            {
                _gameContainer = Resolve<SorryGameContainer>();
            }
            return _gameContainer.SaveRoot!.DidDraw;
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
            if (_gameContainer == null)
            {
                _gameContainer = Resolve<SorryGameContainer>();
            }
            string realText;
            string value = _gameContainer.SaveRoot!.CurrentCard!.Value.ToString();
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
            if (value == "0")
            {
                return;
            }
            canvas.DrawCustomText(realText, TextExtensions.EnumLayoutOptions.Start, TextExtensions.EnumLayoutOptions.Center, textPaint, rect_Card, out _);
        }
    }
}