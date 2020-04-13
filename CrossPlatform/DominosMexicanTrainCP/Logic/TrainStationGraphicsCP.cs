using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.GameGraphicsCP.BasicGameBoards;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using DominosMexicanTrainCP.Data;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace DominosMexicanTrainCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class TrainStationGraphicsCP : BaseGameBoardCP
    {
        public struct PrivateTrain
        {
            public bool IsRotated;
            public bool IsOpposite;
            public bool IsBottom; // the positioning for the train (if any)
                                  // the bottom isopposite is false
                                  // top means isopposite is true because it goes from bottom to top
            public SKRect DominoArea;
            public SKRect TrainArea;
        }
        public Dictionary<int, PrivateTrain> PrivateList = new Dictionary<int, PrivateTrain>();
        //private readonly DominosMexicanTrainMainGameClass _mainGame;
        private readonly GlobalClass _thisGlobal;

        public TrainStationGraphicsCP(IGamePackageResolver mainContainer,GlobalClass global
            ) : base(mainContainer)
        {
            _thisGlobal = global;
        }
        public override string TagUsed => "";
        protected override SKSize OriginalSize { get; set; } = new SKSize(502, 502); // can adjust as needed
        protected override bool CanStartPaint()
        {
            return _thisGlobal.TrainStation1 != null;
        }
        internal CustomBasicList<SKRect> GetClickableRectangle(PrivateTrain thisPrivate)
        {
            // will return 2 rectangles.
            var firstRect = thisPrivate.DominoArea;
            var secondRect = thisPrivate.TrainArea;
            SKSize thisSize;
            thisSize = GetActualSize(firstRect.Width, firstRect.Height);
            var ThisPoint = GetActualPoint(firstRect.Location);
            CustomBasicList<SKRect> tempList = new CustomBasicList<SKRect>
            {
                SKRect.Create(ThisPoint, thisSize)
            };
            thisSize = GetActualSize(secondRect.Width, secondRect.Height);
            ThisPoint = GetActualPoint(secondRect.Location);
            tempList.Add(SKRect.Create(ThisPoint, thisSize));
            return tempList;
        }
        protected override async Task ClickProcessAsync(SKPoint thisPoint)
        {
            if (GlobalClass.GetTrainClicked == null)
            {
                throw new BasicBlankException("GetTrainClicked Not Populated.  Rethink");
            }
            if (GlobalClass.TrainClickedAsync == null)
            {
                throw new BasicBlankException("TrainClickedAsync Not Populated.  Rethink");
            }
            int index = GlobalClass.GetTrainClicked(thisPoint);
            if (index == 0)
                return;
            await GlobalClass.TrainClickedAsync(index);
        }
        protected override void CreateSpaces() { } //this had nothing
        private SKPaint? _fillPaint;
        private SKPaint? _borderPaint;
        private SKBitmap? _publicBit;
        private SKBitmap? _playerBit;
        private SKPaint? _bitPaint;
        private SKPaint? _redPaint;
        protected override void SetUpPaints()
        {
            Assembly thisA = Assembly.GetAssembly(GetType());
            _fillPaint = MiscHelpers.GetSolidPaint(SKColors.LightGray);
            _borderPaint = MiscHelpers.GetStrokePaint(SKColors.White, 3); // try 3
            _publicBit = ImageExtensions.GetSkBitmap(thisA, "publictrain.png");
            _playerBit = ImageExtensions.GetSkBitmap(thisA, "playertrain.png"); // for testing, will be this one.
            _bitPaint = MiscHelpers.GetBitmapPaint();
            _redPaint = MiscHelpers.GetSolidPaint(SKColors.Red);
        }
        private float FindCenter(float currentLocation, bool doubles)
        {
            var tempSize = GetActualSize(95, 31);
            var widths = tempSize.Width;
            var imageSize = tempSize.Height;
            float adds;
            adds = widths - imageSize;
            adds /= 2;
            if (doubles == true)
                adds = 0;
            return currentLocation + adds;
        }
        private float FindTopLeft(float position, bool doubles)
        {
            if (doubles == false)
                return position;
            var tempSize = GetActualSize(95, 31);
            return position + tempSize.Width - tempSize.Height;
        }
        private float GetWidths()
        {
            var tempSize = GetActualSize(95, 31);
            return tempSize.Width;
        }
        protected override void DrawBoard(SKCanvas canvas)
        {
            canvas.Clear();
            var bounds = GetBounds();
            canvas.DrawRect(bounds, _borderPaint);
            var tempSize = GetActualSize(200, 200);
            var tempPoint = GetActualPoint(new SKPoint(150, 155));
            var thisRect = SKRect.Create(tempPoint, tempSize);
            canvas.DrawRect(thisRect, _fillPaint);
            if (PrivateList.Count == 0)
                return;
            var tempDomino = _thisGlobal.TrainStation1!.CenterDomino;
            var thisCenter = _thisGlobal.GetDominoPiece(tempDomino);
            thisCenter.MainGraphics!.Location = GetActualPoint(new SKPoint(203, 239));
            thisCenter.MainGraphics.DrawImage(canvas);
            int x = 0;
            tempSize = GetActualSize(45, 28);
            foreach (var thisTrain in _thisGlobal.TrainStation1.TrainList.Values)
            {
                x++;
                var thisPrivate = PrivateList[thisTrain.Index];
                var bottomPoint = GetActualPoint(new SKPoint(0, 28));
                var thisPoint = GetActualPoint(thisPrivate.TrainArea.Location);
                if (thisPrivate.IsBottom == false && thisPrivate.IsRotated == false)
                    thisPoint.Y += 5;
                else if (thisPrivate.IsBottom)
                    thisPoint.Y += tempSize.Width - bottomPoint.Y;
                var tempRect = SKRect.Create(thisPoint, tempSize);
                if (x == _thisGlobal.TrainStation1.Self)
                    canvas.DrawRect(tempRect, _redPaint);
                if (thisTrain.TrainUp || x == _thisGlobal.TrainStation1.Satisfy)
                {
                    if (thisTrain.IsPublic)
                        canvas.DrawBitmap(_publicBit, tempRect, _bitPaint);
                    else
                        canvas.DrawBitmap(_playerBit, tempRect, _bitPaint);
                }
                int y = 0;
                thisTrain.DominoList.ForEach(thisDomino =>
                {
                    y++;
                    bool isDoubles = thisDomino.CurrentFirst == thisDomino.CurrentSecond;
                    var finalDomino = _thisGlobal.GetDominoPiece(thisDomino);
                    finalDomino.MainGraphics!.DrawImage(canvas);
                });
                if (x == _thisGlobal.TrainStation1.TrainList.Count && thisTrain.TrainUp == false)
                    throw new BasicBlankException("The public train must always be up.  Find out what happened");
            }
            if (_thisGlobal.Animates!.AnimationGoing)
            {
                _thisGlobal.MovingDomino!.MainGraphics!.Location = _thisGlobal.Animates.CurrentLocation;
                _thisGlobal.MovingDomino.MainGraphics.DrawImage(canvas);
            }
        }
        public SKPoint DominoLocationNeeded(int index, int firstNumber, int secondNumber) // probably needs to recalculate that when it goes.
        {
            // this will be the location where the domino piece will be placed (used for animation)
            // when doing the animation; then the old location will be point 0,0
            // all animation will be done for the 
            // index will be the index of the train one
            TrainInfo thisTrain;
            thisTrain = _thisGlobal.TrainStation1!.TrainList[index];
            bool doubles;
            if (firstNumber == secondNumber)
                doubles = true;
            else
                doubles = false;
            return DominoLocationNeeded(thisTrain, doubles);
        }
        private SKPoint DominoLocationNeeded(TrainInfo thisTrain, bool isDoubles)
        {
            PrivateTrain thisPrivate;
            thisPrivate = PrivateList[thisTrain.Index];
            int nums;
            if ((thisTrain.DominoList.Count == 0) & (isDoubles == false))
                nums = 1;
            else if ((thisTrain.DominoList.Count == 1) & (isDoubles == true))
                nums = 1;
            else
                nums = 2;
            return DominoLocationNeeded(thisPrivate, nums, isDoubles);
        }
        private SKPoint DominoLocationNeeded(PrivateTrain thisPrivate, int whichOne, bool doubles)
        {
            var widths = GetWidths();
            SKRect thisArea;
            var thisSize = GetActualSize(thisPrivate.DominoArea.Size.Width, thisPrivate.DominoArea.Size.Height);
            var thisPoint = GetActualPoint(thisPrivate.DominoArea.Location);
            thisArea = SKRect.Create(thisPoint, thisSize);
            if (thisPrivate.IsRotated == true)
            {
                if (whichOne == 1)
                {
                    if (thisPrivate.IsOpposite == true)
                        return new SKPoint(FindCenter(thisArea.Location.X, doubles), thisArea.Bottom - widths);
                    return new SKPoint(FindCenter(thisArea.Location.X, doubles), FindTopLeft(thisArea.Top, doubles));
                }
                if (thisPrivate.IsOpposite == true)
                    return new SKPoint(FindCenter(thisArea.Location.X, doubles), thisArea.Location.Y);
                return new SKPoint(FindCenter(thisArea.Location.X, doubles), FindTopLeft(thisArea.Bottom - widths, doubles));
            }
            if (whichOne == 1)
            {
                if (thisPrivate.IsOpposite == true)
                    return new SKPoint(thisArea.Right - widths, FindCenter(thisArea.Location.Y, doubles));
                return new SKPoint(FindTopLeft(thisArea.Location.X, doubles), FindCenter(thisArea.Location.Y, doubles));
            }
            if (thisPrivate.IsOpposite == true)
                return new SKPoint(thisArea.Location.X, FindCenter(thisArea.Location.Y, doubles));
            return new SKPoint(FindTopLeft(thisArea.Right - widths, doubles), FindCenter(thisArea.Location.Y, doubles));
        }
        public void FirstLoad()
        {
            if (PrivateList.Count > 0)
                throw new Exception("Cannot load the private list again because there are already items on the list");
            PrivateTrain thisPrivate;
            int x;
            for (x = 1; x <= 8; x++)
            {
                thisPrivate = new PrivateTrain();
                if (x == 1)
                {
                    thisPrivate.DominoArea = SKRect.Create(150, 12, 95, 143);
                    thisPrivate.TrainArea = SKRect.Create(175, 155, 70, 28);
                    thisPrivate.IsRotated = true; // domino
                    thisPrivate.IsOpposite = true;
                }
                else if (x == 2)
                {
                    thisPrivate.DominoArea = SKRect.Create(255, 12, 95, 143);
                    thisPrivate.TrainArea = SKRect.Create(280, 155, 70, 28);
                    thisPrivate.IsRotated = true; // domino
                    thisPrivate.IsOpposite = true;
                }
                else if (x == 3)
                {
                    thisPrivate.DominoArea = SKRect.Create(350, 155, 143, 95);
                    // this piece will need to be rotated for the train piece (well see)
                    thisPrivate.TrainArea = SKRect.Create(305, 184, 45, 50);
                    thisPrivate.IsRotated = false;
                    thisPrivate.IsOpposite = false;
                }
                else if (x == 4)
                {
                    thisPrivate.DominoArea = SKRect.Create(350, 260, 143, 95);
                    thisPrivate.TrainArea = SKRect.Create(305, 276, 45, 50);
                    thisPrivate.IsBottom = true;
                    thisPrivate.IsRotated = false;
                    thisPrivate.IsOpposite = false;
                }
                else if (x == 5)
                {
                    thisPrivate.DominoArea = SKRect.Create(255, 355, 95, 143);
                    thisPrivate.TrainArea = SKRect.Create(280, 327, 70, 28);
                    thisPrivate.IsRotated = true;
                    thisPrivate.IsOpposite = false;
                }
                else if (x == 6)
                {
                    thisPrivate.DominoArea = SKRect.Create(150, 355, 95, 143);
                    thisPrivate.TrainArea = SKRect.Create(175, 327, 70, 28);
                    thisPrivate.IsRotated = true;
                    thisPrivate.IsOpposite = false;
                }
                else if (x == 7)
                {
                    thisPrivate.DominoArea = SKRect.Create(7, 260, 143, 95);
                    thisPrivate.TrainArea = SKRect.Create(150, 276, 45, 50);
                    thisPrivate.IsBottom = true;
                    thisPrivate.IsRotated = false;
                    thisPrivate.IsOpposite = true;
                }
                else if (x == 8)
                {
                    thisPrivate.DominoArea = SKRect.Create(7, 155, 143, 95);
                    thisPrivate.TrainArea = SKRect.Create(150, 184, 45, 50);
                    thisPrivate.IsRotated = false;
                    thisPrivate.IsOpposite = true;
                }
                PrivateList.Add(x, thisPrivate);
            }
        }
    }
}
