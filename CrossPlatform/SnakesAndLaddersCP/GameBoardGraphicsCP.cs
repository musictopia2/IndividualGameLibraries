using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.MVVMHelpers;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System.Collections.Generic;
using System.Reflection;
namespace SnakesAndLaddersCP
{
    public class GameBoardGraphicsCP : ObservableObject
    {
        private class TempSpace
        {
            public SKRect Bounds { get; set; }
            public SKColor Fill { get; set; }
        }
        private readonly Dictionary<int, TempSpace> _spaceList = new Dictionary<int, TempSpace>(); //we decided this time to redo the structure of this game.
        public int HeightWidth { get; set; } = 50;
        private readonly SKPaint _bitPaint;
        private readonly SKPaint _borderPaint;
        private bool _alreadyRan;
        public void LoadBoard() // so at some point, this will run.
        {
            if (_alreadyRan == true)
                return;
            var bounds = SKRect.Create(0, 0, HeightWidth * 10, HeightWidth * 10);
            int int_RowCount;
            for (int_RowCount = 1; int_RowCount <= 10; int_RowCount++)
            {
                int int_ColCount;
                for (int_ColCount = 1; int_ColCount <= 10; int_ColCount++)
                {
                    TempSpace thisExp = new TempSpace();
                    int int_Count;
                    if ((int_RowCount % 2) == 0)
                        int_Count = (100 - (((int_RowCount - 1) * 10) + (11 - int_ColCount))) + 1;
                    else
                        int_Count = (100 - (((int_RowCount - 1) * 10) + (int_ColCount))) + 1;
                    // *** If it's an even row, number it backwards
                    if (int_Count == 100)
                        thisExp.Fill = SKColors.DarkRed;
                    else if ((int_Count % 2) == 0)
                        thisExp.Fill = SKColors.Gold;
                    else
                        thisExp.Fill = SKColors.DodgerBlue;
                    thisExp.Bounds = SKRect.Create(bounds.Location.X + ((bounds.Width * (float)(int_ColCount - 1)) / (float)10), bounds.Location.Y + ((bounds.Height * (float)(int_RowCount - 1)) / (float)10), ((bounds.Width * (float)int_ColCount) / (float)10) - ((bounds.Width * (float)(int_ColCount - 1)) / (float)10), ((bounds.Height * (float)int_RowCount) / (float)10) - ((bounds.Height * (float)(int_RowCount - 1)) / (float)10));
                    _spaceList.Add(int_Count, thisExp);
                }
            }
            _alreadyRan = true;
        }
        public int SpaceClicked(float x, float y)
        {
            foreach (var thisItem in _spaceList.Values)
            {
                bool rets = default;
                rets = MiscHelpers.DidClickRectangle(thisItem.Bounds, (int)x, (int)y);
                if (rets == true)
                    return _spaceList.GetKey(thisItem);
            }
            return 0;
        }
        public SKRect GetBounds(int space)
        {
            return _spaceList[space].Bounds;
        } // this simple.
        public GameBoardGraphicsCP()
        {
            _bitPaint = MiscHelpers.GetBitmapPaint();
            _borderPaint = MiscHelpers.GetStrokePaint(SKColors.Black, 1);
            _thisAssembly = Assembly.GetAssembly(this.GetType());
        }
        private readonly Assembly _thisAssembly;
        public void PaintBoard(SKCanvas canvas)
        {
            if (_spaceList.Count == 0)
                return;
            canvas.Clear();
            foreach (var ThisItem in _spaceList.Values)
                DrawSpace(canvas, ThisItem);
            var thisBit = ImageExtensions.GetSkBitmap(_thisAssembly, "snakeladder.png");
            var thisRect = SKRect.Create(0, 0, HeightWidth * 10, HeightWidth * 10);
            canvas.DrawBitmap(thisBit, thisRect, _bitPaint);
        }
        private void DrawSpace(SKCanvas canvas, TempSpace thisItem)
        {
            var thisPaint = MiscHelpers.GetSolidPaint(thisItem.Fill);
            var ThisRect = thisItem.Bounds;
            canvas.DrawRect(ThisRect, _borderPaint);
            canvas.DrawRect(ThisRect, thisPaint);
            SKPaint textPaint;
            int number;
            number = _spaceList.GetKey(thisItem);
            if (number == 100)
                textPaint = MiscHelpers.GetTextPaint(SKColors.Yellow, (thisItem.Bounds.Height * 4) / 9, "Comic Sans MS");
            else if ((number % 10) == 0)
                textPaint = MiscHelpers.GetTextPaint(SKColors.Red, (thisItem.Bounds.Height * 21) / 36, "Comic Sans MS");
            else
                textPaint = MiscHelpers.GetTextPaint(SKColors.Red, (thisItem.Bounds.Height * 5) / 9, "Comic Sans MS");
            textPaint.FakeBoldText = true;
            canvas.DrawCustomText(number.ToString(), TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, thisItem.Bounds, out _);
        }
    }
}