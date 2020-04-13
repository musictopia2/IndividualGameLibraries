using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System;
using ThreeLetterFunCP.Data;
using ThreeLetterFunCP.Logic;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace ThreeLetterFunCP.GraphicsCP
{
    public class ThreeLetterFunCardGraphicsCP : ObservableObject, IDeckGraphicsCP
    {
        public static string TagUsed => "card"; //standard enough.
        public BaseDeckGraphicsCP? MainGraphics { get; set; }
        public bool Drew { get; set; } = false; //not relevent this time.  if i am wrong, rethink.
        public bool NeedsToDrawBacks => false;
        private CustomBasicList<char> _charList = new CustomBasicList<char>();

        public CustomBasicList<char> CharList
        {
            get { return _charList; }
            set
            {
                if (SetProperty(ref _charList, value))
                    MainGraphics?.PaintUI!.DoInvalidate();
            }
        }

        private CustomBasicList<TileInformation> _tiles = new CustomBasicList<TileInformation>();

        public CustomBasicList<TileInformation> Tiles
        {
            get { return _tiles; }
            set
            {
                if (SetProperty(ref _tiles, value))
                    MainGraphics?.PaintUI!.DoInvalidate();
            }
        }

        private int _hiddenValue;

        public int HiddenValue
        {
            get { return _hiddenValue; }
            set
            {
                if (SetProperty(ref _hiddenValue, value))
                    MainGraphics?.PaintUI!.DoInvalidate();

            }
        }
        public bool CanStartDrawing()
        {
            if (CharList.Count != 3)
                return false;
            return _mainGame!.SaveRoot!.Level != EnumLevel.None;
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
        public void DrawBacks(SKCanvas canvas, SKRect rect_Card)
        {
            throw new BasicBlankException("Should not draw backs");
        }
        public SKColor GetFillColor()
        {
            if (_mainGame!.SaveRoot!.Level == EnumLevel.Easy)
                return SKColors.LimeGreen;
            else if (_mainGame.SaveRoot.Level == EnumLevel.None)
                return SKColors.Transparent;
            else
                return SKColors.DarkOrange;
        }
        public SKRect GetDrawingRectangle()
        {
            return MainGraphics!.GetDrawingRectangle();
        }
        ThreeLetterFunMainGameClass? _mainGame;
        private SKPaint? _whitePaint;

        public void Init()
        {
            _mainGame = Resolve<ThreeLetterFunMainGameClass>(); //this can't be unit tested anyways.
            _whitePaint = MiscHelpers.GetSolidPaint(SKColors.White);
        }
        public EnumClickPosition GetClickLocation(SKPoint clickPoint)
        {
            var halfs = MainGraphics!.ActualWidth / 2;
            if (_mainGame!.SaveRoot!.Level == EnumLevel.Easy)
                return EnumClickPosition.Left;
            if (clickPoint.X <= halfs)
                return EnumClickPosition.Left;
            return EnumClickPosition.Right;
        }
        public void DrawImage(SKCanvas canvas, SKRect rect_Card)
        {
            if (CharList.Count != 3)
                throw new Exception("Must have 3 characters for use for drawing");
            CustomBasicList<int> leftList = new CustomBasicList<int>() { 3, 25, 47 };
            int x = default;
            foreach (var thisChar in CharList)
            {
                x += 1;
                var thisRect = MainGraphics!.GetActualRectangle(leftList[x - 1], 3, 19, 30);
                canvas.DrawRoundRect(thisRect, 4, 4, _whitePaint);
                if (VBCompat.AscW(thisChar) > 0)
                {
                    var thisText = thisChar.ToString().ToUpper();
                    var thisColor = thisText.GetColorOfLetter();
                    var fontSize = MainGraphics.GetFontSize(21);
                    var thisPaint = MiscHelpers.GetTextPaint(thisColor, fontSize);
                    canvas.DrawCustomText(thisText, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, thisPaint, thisRect, out _); // hopefully this simple
                }
            }
            var tempSize = MainGraphics!.GetActualSize(19, 30);
            foreach (var thisItem in Tiles)
            {
                TileCP thisTile = new TileCP();
                thisTile.MainGraphics = new BaseDeckGraphicsCP();
                thisTile.MainGraphics.ThisGraphics = thisTile;
                thisTile.MainGraphics.Location = MainGraphics.GetActualPoint(new SKPoint(leftList[thisItem.Index], 3));
                thisTile.MainGraphics.OriginalSize = new SKSize(19, 30);
                thisTile.MainGraphics.ActualHeight = (int)tempSize.Height;
                thisTile.MainGraphics.ActualWidth = (int)tempSize.Width;
                thisTile.MainGraphics.NeedsToClear = false;
                thisTile.Letter = thisItem.Letter; // i think
                thisTile.IsMoved = thisItem.IsMoved; // not sure (?)
                thisTile.MainGraphics.DrawImage(canvas);
            }
        }
    }
}