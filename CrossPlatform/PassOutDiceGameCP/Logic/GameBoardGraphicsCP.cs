using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.GameGraphicsCP.BasicGameBoards;
using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using SkiaSharp;
using PassOutDiceGameCP.Data;
using SkiaSharpGeneralLibrary.SKExtensions;
using System.Collections.Generic;
using BasicGameFrameworkLibrary.GameGraphicsCP.Dice;
//i think this is the most common things i like to do
namespace PassOutDiceGameCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class GameBoardGraphicsCP : BaseGameBoardCP<CheckerPiecesCP>
    {
        private readonly PassOutDiceGameGameContainer _gameContainer;
        public GameBoardGraphicsCP(PassOutDiceGameGameContainer gameContainer) : base(gameContainer.Resolver)
        {
            _gameContainer = gameContainer;
        }
        public override string TagUsed => "";
        protected override SKSize OriginalSize { get; set; } = new SKSize(100, 539); // can adjust as needed
        protected override bool CanStartPaint()
        {
            return true;
        }
        protected override async Task ClickProcessAsync(SKPoint thisPoint)
        {
            if (_gameContainer.Command.IsExecuting)
            {
                return;
            }
            if (_gameContainer.MakeMoveAsync == null)
            {
                throw new BasicBlankException("Nobody is handling make move.  Rethink");
            }
            var thisList = _gameContainer.SpaceList!.Values.Where(items => items.IsEnabled == true && items.Player == 0).ToCustomBasicList();
            foreach (var thisNumber in thisList)
            {
                if (MiscHelpers.DidClickRectangle(thisNumber.Bounds, thisPoint) == true)
                {
                    await _gameContainer.ProcessCustomCommandAsync(_gameContainer.MakeMoveAsync, _gameContainer.SpaceList.GetKey(thisNumber));
                    return;
                }
            }
        }
        internal void CreateSpaceList()
        {
            _gameContainer.SpaceList = new Dictionary<int, SpaceInfo>();
            int x;
            int y;
            int z = 0;
            for (x = 1; x <= 11; x++)
            {
                for (y = 1; y <= 2; y++)
                {
                    z += 1;
                    var thisSpace = new SpaceInfo();
                    if (z == 1)
                        thisSpace.IsEnabled = false;
                    else
                        thisSpace.IsEnabled = true;
                    _gameContainer.SpaceList.Add(z, thisSpace);
                }
            }
        }
        protected override void CreateSpaces()
        {
            int x;
            int y;
            int z = 0;
            var SpaceSize = GetActualSize(45, 45);
            int lefts;
            SpaceInfo thisSpace;
            int tops = 0;
            for (x = 1; x <= 11; x++)
            {
                lefts = 8;
                for (y = 1; y <= 2; y++)
                {
                    z += 1;
                    thisSpace = _gameContainer.SpaceList![z];
                    thisSpace.Bounds = SKRect.Create(lefts, tops, SpaceSize.Width, SpaceSize.Height);
                    if (z == 1)
                        thisSpace.IsEnabled = false;
                    else
                        thisSpace.IsEnabled = true;
                    lefts += (int)SpaceSize.Width;
                }
                tops += (int)SpaceSize.Height;
            }
            FirstLoad();
        }
        private void FirstLoad()
        {
            SpaceInfo thisSpace;
            int x;
            var loopTo = _gameContainer.SpaceList!.Count;
            for (x = 2; x <= loopTo; x++)
            {
                thisSpace = _gameContainer.SpaceList[x];
                if (x == 2)
                {
                    thisSpace.FirstValue = 6;
                    thisSpace.SecondValue = 2;
                }
                else if (x == 3)
                {
                    thisSpace.FirstValue = 5;
                    thisSpace.SecondValue = 1;
                }
                else if (x == 4)
                {
                    thisSpace.FirstValue = 6;
                    thisSpace.SecondValue = 4;
                }
                else if (x == 5)
                {
                    thisSpace.FirstValue = 6;
                    thisSpace.SecondValue = 3;
                }
                else if (x == 6)
                {
                    thisSpace.FirstValue = 1;
                    thisSpace.SecondValue = 3;
                }
                else if (x == 7)
                {
                    thisSpace.FirstValue = 3;
                    thisSpace.SecondValue = 3;
                }
                else if (x == 8)
                {
                    thisSpace.FirstValue = 5;
                    thisSpace.SecondValue = 2;
                }
                else if (x == 9)
                {
                    thisSpace.FirstValue = 6;
                    thisSpace.SecondValue = 2;
                }
                else if (x == 10)
                {
                    thisSpace.FirstValue = 2;
                    thisSpace.SecondValue = 2;
                }
                else if (x == 11)
                {
                    thisSpace.FirstValue = 4;
                    thisSpace.SecondValue = 3;
                }
                else if (x == 12)
                {
                    thisSpace.FirstValue = 1;
                    thisSpace.SecondValue = 3;
                }
                else if (x == 13)
                {
                    thisSpace.FirstValue = 5;
                    thisSpace.SecondValue = 6;
                }
                else if (x == 14)
                {
                    thisSpace.FirstValue = 2;
                    thisSpace.SecondValue = 4;
                }
                else if (x == 15)
                {
                    thisSpace.FirstValue = 1;
                    thisSpace.SecondValue = 1;
                }
                else if (x == 16)
                {
                    thisSpace.FirstValue = 5;
                    thisSpace.SecondValue = 4;
                }
                else if (x == 17)
                {
                    thisSpace.FirstValue = 4;
                    thisSpace.SecondValue = 4;
                }
                else if (x == 18)
                {
                    thisSpace.FirstValue = 5;
                    thisSpace.SecondValue = 5;
                }
                else if (x == 19)
                {
                    thisSpace.FirstValue = 6;
                    thisSpace.SecondValue = 1;
                }
                else if (x == 20)
                {
                    thisSpace.FirstValue = 5;
                    thisSpace.SecondValue = 3;
                }
                else if (x == 21)
                {
                    thisSpace.FirstValue = 6;
                    thisSpace.SecondValue = 6;
                }
                else if (x == 22)
                {
                    thisSpace.FirstValue = 1;
                    thisSpace.SecondValue = 4;
                }
            }
        }
        protected override void DrawBoard(SKCanvas canvas)
        {
            canvas.Clear();
            var bounds = GetBounds();
            canvas.DrawRect(bounds, _backgroundPaint);
            canvas.DrawRect(bounds, _outLinePaint);
            int x = default;
            foreach (var thisSpace in _gameContainer.SpaceList!.Values)
            {
                x += 1;
                SKRect tempRect;
                tempRect = SKRect.Create(thisSpace.Bounds.Location.X + 3, thisSpace.Bounds.Location.Y + 3, thisSpace.Bounds.Width - 6, thisSpace.Bounds.Height - 6);
                canvas.DrawRect(thisSpace.Bounds, _blackBorder);
                if (thisSpace.IsEnabled == false)
                {
                    var firstRect = SKRect.Create(thisSpace.Bounds.Location.X, thisSpace.Bounds.Location.Y, thisSpace.Bounds.Width, thisSpace.Bounds.Height / 2);
                    var fontSize = firstRect.Height * 0.8f;
                    var textPaint = MiscHelpers.GetTextPaint(SKColors.White, fontSize);
                    canvas.DrawCustomText("Pass", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, firstRect, out _);
                    firstRect = SKRect.Create(thisSpace.Bounds.Location.X, thisSpace.Bounds.Location.Y + (thisSpace.Bounds.Height / 2), thisSpace.Bounds.Width, thisSpace.Bounds.Height / 2);
                    canvas.DrawCustomText("Out", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, firstRect, out _);
                }
                else
                {
                    if (x == _gameContainer.SaveRoot!.PreviousSpace)
                        canvas.DrawRect(thisSpace.Bounds, _hightlightPaint);
                    StandardDiceGraphicsCP thisDice;
                    thisDice = new StandardDiceGraphicsCP();
                    thisDice.NeedsToClear = false; // misleading.
                    thisDice.Location = new SKPoint(thisSpace.Bounds.Location.X + 3, thisSpace.Bounds.Location.Y + 3);
                    var thisSize = GetActualSize(20, 20);
                    thisDice.ActualWidthHeight = thisSize.Height; //try this way.
                    thisDice.Value = thisSpace.FirstValue;
                    thisDice.UseSmallerBorders();
                    thisDice.DrawDice(canvas);
                    thisDice = new StandardDiceGraphicsCP();
                    thisDice.NeedsToClear = false;
                    int diffs;
                    diffs = (int)thisSize.Height + 5;
                    thisDice.Location = new SKPoint(thisSpace.Bounds.Location.X + diffs, thisSpace.Bounds.Location.Y + diffs);
                    thisDice.ActualWidthHeight = thisSize.Height;
                    thisDice.Value = thisSpace.SecondValue;
                    thisDice.UseSmallerBorders();
                    thisDice.DrawDice(canvas);
                    if (thisSpace.Player > 0)
                    {
                        var thisPiece = GetGamePiece(thisSpace.Color.ToColor(), thisSpace.Bounds.Location);
                        thisPiece.ActualHeight = thisSpace.Bounds.Height;
                        thisPiece.ActualWidth = thisSpace.Bounds.Width;
                        thisPiece.DrawImage(canvas);
                    }
                }
            }
        }
        private SKPaint? _backgroundPaint;
        private SKPaint? _outLinePaint;
        private SKPaint? _hightlightPaint;
        private SKPaint? _blackBorder;
        protected override void SetUpPaints()
        {
            _backgroundPaint = MiscHelpers.GetSolidPaint(SKColors.Maroon);
            _outLinePaint = MiscHelpers.GetStrokePaint(SKColors.Black, 7);
            _hightlightPaint = MiscHelpers.GetSolidPaint(SKColors.Yellow);
            _blackBorder = MiscHelpers.GetStrokePaint(SKColors.Black, 1);
        }
    }
}