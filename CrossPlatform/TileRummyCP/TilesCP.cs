using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DIContainers;
using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using BasicGameFramework.GameGraphicsCP.MiscClasses;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace TileRummyCP
{
    public class TileCountClass : IDeckCount
    {
        int IDeckCount.GetDeckCount()
        {
            return 106;
        }
    }
    public class TileInfo : SimpleDeckObject, IDeckObject, IRummmyObject<EnumColorType, EnumColorType>, IComparable<TileInfo>, ILocationDeck
    {
        private SKPoint _Location;
        public SKPoint Location
        {
            get { return _Location; }
            set
            {
                if (SetProperty(ref _Location, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public TileInfo()
        {
            DefaultSize = new SKSize(60, 40);
        }
        private bool _IsJoker;
        public bool IsJoker
        {
            get { return _IsJoker; }
            set
            {
                if (SetProperty(ref _IsJoker, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public int Points { get; set; }
        private EnumDrawType _WhatDraw = EnumDrawType.IsNone;
        public EnumDrawType WhatDraw
        {
            get { return _WhatDraw; }
            set
            {
                if (SetProperty(ref _WhatDraw, value))
                {
                    //can decide what to do when property changes
                    Drew = value != EnumDrawType.IsNone;
                }
            }
        }
        private EnumColorType _Color;
        public EnumColorType Color
        {
            get { return _Color; }
            set
            {
                if (SetProperty(ref _Color, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _Number;
        public int Number
        {
            get { return _Number; }
            set
            {
                if (SetProperty(ref _Number, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        int IRummmyObject<EnumColorType, EnumColorType>.GetSecondNumber => Number;
        int ISimpleValueObject<int>.ReadMainValue => Number;
        bool IWildObject.IsObjectWild => IsJoker;
        bool IIgnoreObject.IsObjectIgnored => false;
        EnumColorType ISuitObject<EnumColorType>.GetSuit => Color;
        EnumColorType IColorObject<EnumColorType>.GetColor => Color;
        public void Populate(int chosen)
        {
            int x;
            int y;
            int z;
            int q = 0;
            for (x = 1; x <= 4; x++)
            {
                for (y = 1; y <= 13; y++)
                {
                    for (z = 1; z <= 2; z++)
                    {
                        q += 1;
                        if (q == chosen)
                        {
                            Deck = chosen;
                            if (x == 1)
                                Color = EnumColorType.Black;
                            else if (x == 2)
                                Color = EnumColorType.Blue;
                            else if (x == 3)
                                Color = EnumColorType.Orange;
                            else if (x == 4)
                                Color = EnumColorType.Red;
                            Number = y;
                            Points = y;
                            IsJoker = false;
                            return;
                        }
                    }
                }
            }
            for (x = 105; x <= 106; x++)
            {
                if (chosen == x)
                {
                    Deck = chosen;
                    IsJoker = true;
                    Number = 20;
                    Points = 25;
                    if (x == 105)
                        Color = EnumColorType.Black;
                    else
                        Color = EnumColorType.Red;
                    return;
                }
            }
            throw new BasicBlankException("Cannot find the deck " + Deck);
        }
        public void Reset()
        {
            WhatDraw = EnumDrawType.IsNone;
        }
        int IComparable<TileInfo>.CompareTo(TileInfo other)
        {
            if (Number != other.Number)
                return Number.CompareTo(other.Number);
            return Color.CompareTo(other.Color);
        }
    }
    public class TileShuffler : IDeckShuffler<TileInfo>, IScatterList<TileInfo>,
         IAdvancedDIContainer, ISimpleList<TileInfo>, IListShuffler<TileInfo>
    {
        private readonly BasicObjectShuffler<TileInfo> ThisShuffle;
        private readonly DeckRegularDict<TileInfo> ObjectList = new DeckRegularDict<TileInfo>();
        public int Count => ObjectList.Count;
        public bool NeedsToRedo { get => ThisShuffle.NeedsToRedo; set => ThisShuffle.NeedsToRedo = value; }
        public IGamePackageResolver? MainContainer { get => ThisShuffle.MainContainer; set => ThisShuffle.MainContainer = value; }
        public TileShuffler()
        {
            ThisShuffle = new BasicObjectShuffler<TileInfo>(ObjectList); //i think.
            NeedsToRedo = true; //for this one, must be redo.
        }
        public Task<DeckObservableDict<TileInfo>> GetListFromJsonAsync(string jsonData)
        {
            return ThisShuffle.GetListFromJsonAsync(jsonData);
        }
        public void ClearObjects()
        {
            ThisShuffle.ClearObjects();
        }
        public void OrderedObjects()
        {
            ThisShuffle.OrderedObjects();
        }
        public void ShuffleObjects()
        {
            ThisShuffle.ShuffleObjects();
        }
        public void ReshuffleFirstObjects(IDeckDict<TileInfo> thisList, int startAt, int endAt)
        {
            ThisShuffle.ReshuffleFirstObjects(thisList, startAt, endAt);
        }
        public TileInfo GetSpecificItem(int deck)
        {
            return ThisShuffle.GetSpecificItem(deck);
        }
        public bool ObjectExist(int deck)
        {
            return ThisShuffle.ObjectExist(deck);
        }
        public int GetDeckCount()
        {
            return ThisShuffle.GetDeckCount();
        }
        public Task ForEachAsync(BasicDataFunctions.ActionAsync<TileInfo> action)
        {
            return ObjectList.ForEachAsync(action);
        }
        public void ForEach(Action<TileInfo> action)
        {
            ObjectList.ForEach(action);
        }
        public void ForConditionalItems(Predicate<TileInfo> match, Action<TileInfo> action)
        {
            ObjectList.ForConditionalItems(match, action);
        }
        public Task ForConditionalItemsAsync(Predicate<TileInfo> match, BasicDataFunctions.ActionAsync<TileInfo> action)
        {
            return ObjectList.ForConditionalItemsAsync(match, action);
        }
        public bool Exists(Predicate<TileInfo> match)
        {
            return ObjectList.Exists(match);
        }
        public bool Contains(TileInfo item)
        {
            return ObjectList.Contains(item);
        }
        public TileInfo Find(Predicate<TileInfo> match)
        {
            return ObjectList.Find(match);
        }
        public TileInfo FindOnlyOne(Predicate<TileInfo> match)
        {
            return ObjectList.FindOnlyOne(match);
        }
        public ICustomBasicList<TileInfo> FindAll(Predicate<TileInfo> match)
        {
            return ObjectList.FindAll(match);
        }
        public int FindFirstIndex(Predicate<TileInfo> match)
        {
            return ObjectList.FindFirstIndex(match);
        }
        public int FindFirstIndex(int startIndex, Predicate<TileInfo> match)
        {
            return ObjectList.FindFirstIndex(startIndex, match);
        }
        public int FindFirstIndex(int startIndex, int count, Predicate<TileInfo> match)
        {
            return ObjectList.FindFirstIndex(startIndex, count, match);
        }
        public TileInfo FindLast(Predicate<TileInfo> match)
        {
            return ObjectList.FindLast(match);
        }
        public int FindLastIndex(Predicate<TileInfo> match)
        {
            return ObjectList.FindLastIndex(match);
        }
        public int FindLastIndex(int startIndex, Predicate<TileInfo> match)
        {
            return ObjectList.FindLastIndex(startIndex, match);
        }
        public int FindLastIndex(int startIndex, int count, Predicate<TileInfo> match)
        {
            return ObjectList.FindLastIndex(startIndex, count, match);
        }
        public int HowMany(Predicate<TileInfo> match)
        {
            return ObjectList.HowMany(match);
        }
        public int IndexOf(TileInfo value)
        {
            return ObjectList.IndexOf(value);
        }
        public int IndexOf(TileInfo value, int Index)
        {
            return ObjectList.IndexOf(value, Index);
        }
        public int IndexOf(TileInfo value, int Index, int Count)
        {
            return ObjectList.IndexOf(value, Index, Count);
        }
        public bool TrueForAll(Predicate<TileInfo> match)
        {
            return ObjectList.TrueForAll(match);
        }
        public IEnumerator<TileInfo> GetEnumerator()
        {
            return ObjectList.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ObjectList.GetEnumerator();
        }

        public void RelinkObject(int oldDeck, TileInfo newObject)
        {
            ThisShuffle.RelinkObject(oldDeck, newObject);
        }
    }
    public class TileCP : ObservableObject, IDeckGraphicsCP
    {
        public BaseDeckGraphicsCP? MainGraphics { get; set; }
        private EnumColorType _Color = EnumColorType.None;
        public EnumColorType Color
        {
            get
            {
                return _Color;
            }

            set
            {
                if (SetProperty(ref _Color, value) == true)
                {
                    TempColor = GetColor();
                    MainGraphics?.PaintUI?.DoInvalidate();
                }
            }
        }
        private SKColor GetColor()
        {
            switch (Color)
            {
                case EnumColorType.Black:
                    {
                        return SKColors.Black;
                    }

                case EnumColorType.Blue:
                    {
                        return SKColors.Blue;
                    }

                case EnumColorType.Orange:
                    {
                        return SKColors.Orange;
                    }

                case EnumColorType.Red:
                    {
                        return SKColors.Red;
                    }

                default:
                    {
                        throw new BasicBlankException("Color not found for none or other");
                    }
            }
        }
        private SKColor TempColor;
        private int _Number;
        public int Number
        {
            get
            {
                return _Number;
            }

            set
            {
                if (SetProperty(ref _Number, value) == true)
                    MainGraphics?.PaintUI?.DoInvalidate();
            }
        }
        private bool _IsJoker;
        public bool IsJoker
        {
            get
            {
                return _IsJoker;
            }

            set
            {
                if (SetProperty(ref _IsJoker, value) == true)
                    MainGraphics?.PaintUI?.DoInvalidate();
            }
        }
        private EnumDrawType _WhatDraw;
        public EnumDrawType WhatDraw
        {
            get
            {
                return _WhatDraw;
            }

            set
            {
                if (SetProperty(ref _WhatDraw, value) == true)
                {
                    MainGraphics?.PaintUI?.DoInvalidate();
                }
            }
        }
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
        public bool NeedsToDrawBacks => true; //if you don't need to draw backs, then set false.
        public bool CanStartDrawing()
        {
            if (MainGraphics!.IsUnknown)
                return true;
            return Color != EnumColorType.None;
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
            else if (WhatDraw == EnumDrawType.FromSet)
                MainGraphics.DrawDefaultRectangles(dc, rect_Card, _altDrewPaint!);
            else if (Drew == true)
                MainGraphics.DrawDefaultRectangles(dc, rect_Card, _pDrewPaint!);
        }
        public void DrawBacks(SKCanvas canvas, SKRect rect_Card) { }
        public SKColor GetFillColor()
        {
            return MainGraphics!.GetFillColor();
        }
        public SKRect GetDrawingRectangle()
        {
            return MainGraphics!.GetDrawingRectangle();
        }
        private SKPaint? _selectPaint;
        private SKPaint? _pDrewPaint;
        private SKPaint? _altDrewPaint;
        private SKPaint? _blackPaint;
        private SKPaint? _blackBorder;
        public void Init()
        {
            MainGraphics!.OriginalSize = new SKSize(60, 40); //change to what the original size is.
            _pDrewPaint = MainGraphics.GetStandardDrewPaint();
            _selectPaint = BaseDeckGraphicsCP.GetStandardSelectPaint();
            SKColor thisColor = SKColors.Purple; //we can change as needed.
            SKColor otherColor = new SKColor(thisColor.Red, thisColor.Green, thisColor.Blue, 40);
            _altDrewPaint = MiscHelpers.GetSolidPaint(otherColor);
            _blackPaint = MiscHelpers.GetSolidPaint(SKColors.Black);
            _blackBorder = MiscHelpers.GetStrokePaint(SKColors.Black, 2);
        }
        public void DrawImage(SKCanvas canvas, SKRect rect_Card)
        {
            if (IsJoker == true)
            {
                var thisPen = MiscHelpers.GetStrokePaint(TempColor, 2);
                var tempRect = MainGraphics!.GetActualRectangle(SKRect.Create(16, 6, 28, 28));
                canvas.DrawSmiley(tempRect, null!, thisPen, _blackPaint!);
                return;
            }
            var FontSize = rect_Card.Height * 0.9f; // can be adjusted obviously
            var ThisPaint = MiscHelpers.GetTextPaint(TempColor, FontSize);
            canvas.DrawBorderText(Number.ToString(), TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, ThisPaint, _blackBorder, rect_Card);
        }
    }
}