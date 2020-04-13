using BaseMahjongTilesCP;
using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using MahJongSolitaireCP.Data;
using SkiaSharp;
using System;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace MahJongSolitaireCP.Logic
{
    [SingletonGame]
    public class MahJongSolitaireGameBoardCP : ObservableObject //i think
    {
        private readonly MahJongSolitaireSaveInfo _saveRoot;
        private readonly BaseMahjongGlobals _customGlobal;
        private readonly MahJongSolitaireModGlobal _mainGlobal;
        private readonly SKSize _sizeUsed;
        private readonly BasicData _data;
        public CustomBasicList<BoardInfo> GetPriorityBoards() //go ahead and use the old fashioned sql like since i already did it.
        {
            return (from items in _saveRoot.BoardList
                    orderby items.Floor, items.BoardCategory, items.RowStart
                    select items).ToCustomBasicList();
        }

        public MahJongSolitaireGameBoardCP(MahJongSolitaireSaveInfo saveRoot, BaseMahjongGlobals customGlobal
            , MahJongSolitaireModGlobal mainGlobal, IProportionImage thisP, BasicData data)
        {
            _saveRoot = saveRoot; //if this causes too many problems, has to resolve later.
            _customGlobal = customGlobal;
            _mainGlobal = mainGlobal;
            _data = data;
            _sizeUsed = MahjongSolitaireTileInfo.GetDefaultSize().GetSizeUsed(thisP.Proportion);
            if (saveRoot.BoardList.Count > 0)
            {
                throw new BasicBlankException("The saveroot should have cleared out every game.  Rethink");
            }
            FirstLoad();
        }
        private void FirstLoad()
        {
            // this will help on positioning.
            BoardInfo thisBoard;
            thisBoard = new BoardInfo();
            thisBoard.Floor = 1;
            thisBoard.BoardCategory = BoardInfo.EnumBoardCategory.FarLeft;
            _saveRoot.BoardList.Add(thisBoard);
            thisBoard = new BoardInfo();
            thisBoard.Floor = 1;
            thisBoard.BoardCategory = BoardInfo.EnumBoardCategory.FarRight; // always holding 2 tiles.
            _saveRoot.BoardList.Add(thisBoard);
            thisBoard = new BoardInfo();
            thisBoard.Floor = 1;
            thisBoard.RowStart = 0; // should be 0 based.
            thisBoard.ColumnStart = 1; // first column is for holding the last one.
            thisBoard.HowManyColumns = 12;
            _saveRoot.BoardList.Add(thisBoard);
            thisBoard = new BoardInfo();
            thisBoard.Floor = 1;
            thisBoard.RowStart = 1;
            thisBoard.ColumnStart = 3;
            thisBoard.HowManyColumns = 8;
            _saveRoot.BoardList.Add(thisBoard);
            thisBoard = new BoardInfo();
            thisBoard.Floor = 1;
            thisBoard.RowStart = 2;
            thisBoard.ColumnStart = 2;
            thisBoard.HowManyColumns = 10;
            _saveRoot.BoardList.Add(thisBoard);
            int x;
            for (x = 3; x <= 4; x++)
            {
                thisBoard = new BoardInfo();
                thisBoard.Floor = 1;
                thisBoard.RowStart = x;
                thisBoard.ColumnStart = 1;
                thisBoard.HowManyColumns = 12;
                _saveRoot.BoardList.Add(thisBoard);
            }
            thisBoard = new BoardInfo();
            thisBoard.Floor = 1;
            thisBoard.RowStart = 5;
            thisBoard.ColumnStart = 2;
            thisBoard.HowManyColumns = 10;
            _saveRoot.BoardList.Add(thisBoard);
            thisBoard = new BoardInfo();
            thisBoard.Floor = 1;
            thisBoard.RowStart = 6;
            thisBoard.ColumnStart = 3;
            thisBoard.HowManyColumns = 8;
            _saveRoot.BoardList.Add(thisBoard);
            thisBoard = new BoardInfo();
            thisBoard.Floor = 1;
            thisBoard.RowStart = 7;
            thisBoard.ColumnStart = 1;
            thisBoard.HowManyColumns = 12;
            _saveRoot.BoardList.Add(thisBoard);
            int manys;
            manys = 6;
            int row;
            int column;
            row = 1;
            column = 4;
            int tempRow;
            int y;
            for (x = 2; x <= 4; x++)
            {
                tempRow = row;
                var loopTo = manys;
                for (y = 1; y <= loopTo; y++)
                {
                    thisBoard = new BoardInfo();
                    thisBoard.RowStart = tempRow;
                    thisBoard.ColumnStart = column;
                    thisBoard.HowManyColumns = manys;
                    thisBoard.Floor = x;
                    _saveRoot.BoardList.Add(thisBoard);
                    tempRow += 1;
                }
                row += 1;
                column += 1;
                manys -= 2;
            }
            thisBoard = new BoardInfo();
            thisBoard.Floor = 5; // might as well
            thisBoard.BoardCategory = BoardInfo.EnumBoardCategory.VeryTop;
            _saveRoot.BoardList.Add(thisBoard);
        }
        public void ClearBoard()
        {
            _saveRoot.BoardList.ForEach(board =>
            {
                board.TileList.Clear();
                board.BackTaken = 0;
                board.FrontTaken = 0;
            });
        }

        public void PositionTiles()
        {
            float currentLeft;
            float currentTop;
            BoardInfo thisBoard;
            float adds;
            if (_data.IsXamarinForms)
                adds = 0;
            else
                adds = 10;
            var thisList = (from Items in _saveRoot.BoardList
                            where Items.BoardCategory == BoardInfo.EnumBoardCategory.Regular
                            select Items).ToCustomBasicList();
            foreach (var tempBoard in thisList)
            {
                currentLeft = tempBoard.ColumnStart * (_sizeUsed.Width + adds);
                if (currentLeft < 0)
                    currentLeft = 0;
                currentTop = tempBoard.RowStart * (_sizeUsed.Height + adds);
                if (currentTop < 0)
                    currentTop = 0;
                foreach (var ThisCard in tempBoard.TileList)
                {
                    ThisCard.Left = currentLeft;
                    ThisCard.Top = currentTop;
                    currentLeft += _sizeUsed.Width; // because 3d style (well see)
                }
            }
            thisBoard = (from Items in _saveRoot.BoardList
                         where Items.BoardCategory == BoardInfo.EnumBoardCategory.FarRight
                         select Items).Single();
            var otherBoard = (from Items in _saveRoot.BoardList
                              where Items.Floor == 1 && Items.RowStart == 3
                              select Items).Single();
            if (otherBoard.TileList.Count == 0)
                throw new Exception("No cards to give any hints");
            currentLeft = otherBoard.TileList.Last().Left + _sizeUsed.Width; // because 3d style
            currentTop = otherBoard.TileList.Last().Top + _sizeUsed.Height / 2;
            foreach (var thisCard in thisBoard.TileList)
            {
                thisCard.Left = currentLeft;
                thisCard.Top = currentTop;
                currentLeft += _sizeUsed.Width;
            }
            thisBoard = (from Items in _saveRoot.BoardList
                         where Items.BoardCategory == BoardInfo.EnumBoardCategory.FarLeft
                         select Items).Single();
            if (thisBoard.TileList.Count > 0)
            {
                thisBoard.TileList.Single().Left = 0;
                thisBoard.TileList.Single().Top = currentTop;
            }

            thisBoard = (from Items in _saveRoot.BoardList
                         where Items.BoardCategory == BoardInfo.EnumBoardCategory.VeryTop
                         select Items).Single();
            otherBoard = (from Items in _saveRoot.BoardList
                          where Items.Floor == 4 && Items.RowStart == 3
                          select Items).Single();
            if (otherBoard.TileList.Count == 0)
                throw new Exception("No cards to give any hints");
            currentLeft = otherBoard.TileList.First().Left + (_sizeUsed.Width / 2);
            currentTop = otherBoard.TileList.First().Top + (_sizeUsed.Height / 2);
            thisBoard.TileList.Single().Left = currentLeft;
            thisBoard.TileList.Single().Top = currentTop;
            CheckForValidTiles();
            Check3DTiles(); // this should be after checking for valid cards.
        }
        internal DeckRegularDict<MahjongSolitaireTileInfo> ValidList { get; set; } = new DeckRegularDict<MahjongSolitaireTileInfo>();

        private void UpdateTile(MahjongSolitaireTileInfo thisTile)
        {
            if (_customGlobal.CanShowTiles == true)
            {
                thisTile.IsEnabled = true;
                thisTile.Drew = true;
            }
            if (ValidList.ObjectExist(thisTile.Deck) == false)
                ValidList.Add(thisTile); // needs to be added to get hints even if not auto.
            //because it could go to more than one thing.
        }
        public void CheckForValidTiles()
        {
            ValidList = new DeckRegularDict<MahjongSolitaireTileInfo>();
            var firstBoard = (from items in _saveRoot.BoardList
                              where items.BoardCategory == BoardInfo.EnumBoardCategory.FarLeft
                              select items).Single();
            bool hasFarLeft;
            hasFarLeft = false;
            foreach (var thisCard in firstBoard.TileList)
            {
                UpdateTile(thisCard);
                thisCard.IsEnabled = true; // can always do this one no matter what
                hasFarLeft = true;
            }
            firstBoard = (from items in _saveRoot.BoardList
                          where items.BoardCategory == BoardInfo.EnumBoardCategory.VeryTop
                          select items).Single();
            bool hasVeryTop;
            hasVeryTop = false;
            foreach (var thisCard in firstBoard.TileList)
            {
                UpdateTile(thisCard);
                hasVeryTop = true;
            }
            bool hasFarRight;
            hasFarRight = false;
            firstBoard = (from items in _saveRoot.BoardList
                          where items.BoardCategory == BoardInfo.EnumBoardCategory.FarRight
                          select items).Single();
            if (firstBoard.TileList.Count > 0)
            {
                hasFarRight = true;
                UpdateTile(firstBoard.TileList.Last());
            }
            CustomBasicList<BoardInfo> tempList;
            if (hasVeryTop == false)
            {
                tempList = (from items in _saveRoot.BoardList
                            where items.Floor == 4
                            select items).ToCustomBasicList();
                foreach (var TempBoard in tempList)
                {
                    foreach (var ThisCard in TempBoard.TileList)
                        UpdateTile(ThisCard);
                }
            }
            tempList = (from items in _saveRoot.BoardList
                        where items.BoardCategory == BoardInfo.EnumBoardCategory.Regular && items.Floor == 1
                        select items).ToCustomBasicList();
            foreach (var thisBoard in tempList)
            {
                if (thisBoard.TileList.Count > 0)
                {
                    bool canUpdateLeftSoFar;
                    bool canUpdateRightSoFar;
                    canUpdateLeftSoFar = true;
                    canUpdateRightSoFar = true;
                    if (thisBoard.RowStart == 3 || thisBoard.RowStart == 4)
                    {
                        if (hasFarLeft == true)
                            canUpdateLeftSoFar = false;
                        if (hasFarRight == true)
                            canUpdateRightSoFar = false;
                    }
                    if (thisBoard.FrontTaken == 0)
                    {
                        if (canUpdateLeftSoFar == true)
                            UpdateTile(thisBoard.TileList.First());
                    }
                    if (thisBoard.BackTaken == 0)
                    {
                        if (canUpdateRightSoFar == true)
                            UpdateTile(thisBoard.TileList.Last());
                    }
                    if (thisBoard.FrontTaken == 0 && thisBoard.BackTaken == 0)
                        continue;
                    var tempBoard = (from items in _saveRoot.BoardList
                                     where items.Floor == thisBoard.Floor + 1 && items.RowStart == thisBoard.RowStart
                                     select items).SingleOrDefault();
                    if (tempBoard == null || tempBoard.TileList.Count == 0)
                    {
                        if (canUpdateLeftSoFar == true)
                            UpdateTile(thisBoard.TileList.First());
                        if (canUpdateRightSoFar == true)
                            UpdateTile(thisBoard.TileList.Last());
                        continue;
                    }
                    float realFirst;
                    float realSecond;
                    if (thisBoard.FrontTaken > 0)
                    {
                        realFirst = thisBoard.ColumnStart + thisBoard.FrontTaken;
                        realSecond = tempBoard.ColumnStart + tempBoard.FrontTaken;
                        if (realFirst < realSecond)
                        {
                            if (canUpdateLeftSoFar == true)
                                UpdateTile(thisBoard.TileList.First());
                        }
                    }
                    if (thisBoard.BackTaken > 0)
                    {
                        realFirst = thisBoard.ColumnStart + thisBoard.HowManyColumns - thisBoard.BackTaken;
                        realSecond = tempBoard.ColumnStart + tempBoard.HowManyColumns - tempBoard.BackTaken;
                        if (realFirst > realSecond)
                        {
                            if (canUpdateRightSoFar == true)
                                UpdateTile(thisBoard.TileList.Last());
                        }
                    }
                }
            }
            tempList = (from items in _saveRoot.BoardList
                        where items.Floor == 2 || items.Floor == 3
                        select items).ToCustomBasicList();
            foreach (var thisBoard in tempList)
            {
                if (thisBoard.TileList.Count > 0)
                {
                    if (thisBoard.FrontTaken == 0)
                        UpdateTile(thisBoard.TileList.First());
                    if (thisBoard.BackTaken == 0)
                        UpdateTile(thisBoard.TileList.Last());
                    if (thisBoard.FrontTaken > 0 || thisBoard.BackTaken > 0)
                    {
                        var tempBoard = (from items in _saveRoot.BoardList
                                         where items.Floor == thisBoard.Floor + 1 && items.RowStart == thisBoard.RowStart
                                         select items).SingleOrDefault();
                        if (tempBoard == null || tempBoard.TileList.Count == 0)
                        {
                            UpdateTile(thisBoard.TileList.First());
                            UpdateTile(thisBoard.TileList.Last());
                        }
                        else
                        {
                            float realFirst;
                            float realSecond;
                            if (thisBoard.FrontTaken > 0)
                            {
                                realFirst = thisBoard.ColumnStart + thisBoard.FrontTaken;
                                realSecond = tempBoard.ColumnStart + tempBoard.FrontTaken;
                                if (realFirst < realSecond)
                                    UpdateTile(thisBoard.TileList.First());
                            }
                            if (thisBoard.BackTaken > 0)
                            {
                                realFirst = thisBoard.ColumnStart + thisBoard.HowManyColumns - thisBoard.BackTaken;
                                realSecond = tempBoard.ColumnStart + tempBoard.HowManyColumns - tempBoard.BackTaken;
                                if (realFirst > realSecond)
                                    UpdateTile(thisBoard.TileList.Last());
                            }
                        }
                    }
                }
            }
        }

        public void UnselectTiles()
        {
            _mainGlobal.TileList.UnselectAllObjects();
            _saveRoot.FirstSelected = 0;
            _mainGlobal.SecondSelected = 0;
        }

        public bool IsGameOver()
        {
            return !(_saveRoot.BoardList.Any(items => items.TileList.Count > 0)); //try this way.
        }


        private void RemoveSpecificTile(MahjongSolitaireTileInfo thisTile)
        {
            // i have an idea (hopefully won't cause more problems).
            foreach (var thisBoard in _saveRoot.BoardList)
            {
                if (thisBoard.TileList.Count >= 2)
                {
                    if (thisBoard.TileList.First().Deck == thisTile.Deck)
                    {
                    }
                    if (thisBoard.TileList.First().Deck == thisTile.Deck)
                    {
                        thisBoard.FrontTaken += 1;
                        RemoveTile(thisBoard, thisBoard.TileList.First());
                        return;
                    }
                    if (thisBoard.TileList.Last().Deck == thisTile.Deck)
                    {
                        thisBoard.BackTaken += 1;
                        RemoveTile(thisBoard, thisBoard.TileList.Last());
                        return;
                    }
                }
                else if (thisBoard.TileList.Count == 1)
                {
                    if (thisBoard.TileList.Single().Deck == thisTile.Deck)
                    {
                        RemoveTile(thisBoard, thisBoard.TileList.Single());
                        return;
                    }
                }
            }
            if (_customGlobal.CanShowTiles == true)
                throw new BasicBlankException("No tile to remove");
        }

        private void RemoveTile(BoardInfo thisBoard, MahjongSolitaireTileInfo thisTile)
        {
            thisBoard.TileList.RemoveSpecificItem(thisTile);
            if (_customGlobal.CanShowTiles == true)
                thisTile.Visible = false;// making visible false means no subscriptions needed
        }

        private DeckRegularDict<MahjongSolitaireTileInfo> GetPreviousTiles()
        {
            BoardInfo firstBoard;
            BoardInfo secondBoard;
            DeckRegularDict<MahjongSolitaireTileInfo> output = new DeckRegularDict<MahjongSolitaireTileInfo>();
            for (int x = 0; x < _saveRoot.PreviousList.Count; x++)
            {
                firstBoard = _saveRoot.PreviousList[x];
                secondBoard = _saveRoot.BoardList[x];
                if (firstBoard.TileList.Count != secondBoard.TileList.Count)
                {
                    firstBoard.TileList.ForEach(tile =>
                    {
                        if (secondBoard.TileList.ObjectExist(tile.Deck) == false)
                        {
                            tile.IsSelected = false;
                            output.Add(tile);
                        }
                    });
                }
            }
            return output;
        }

        public void PopulateBoardFromUndo()
        {
            if (_saveRoot.PreviousList.Count != _saveRoot.BoardList.Count)
                throw new BasicBlankException("Count don't reconcile");
            var thisList = GetPreviousTiles();
            if (thisList.Count != 2)
                throw new BasicBlankException("Must have 2 cards only because must be one pair at a time");
            _saveRoot.BoardList = _saveRoot.PreviousList.ToCustomBasicList();
            _mainGlobal.TileList.ClearObjects();
            _saveRoot.BoardList.ForEach(TempBoard =>
            {
                _mainGlobal.TileList.AddRelinkedTiles(TempBoard.TileList);
                TempBoard.TileList.ForEach(TempTile =>
                {
                    TempTile.Drew = false;
                    TempTile.IsEnabled = false;
                });
            });
            CheckForValidTiles();
            Check3DTiles();
        }

        public async Task ProcessPairAsync(bool isAuto)
        {
            var firstTile = _mainGlobal.TileList.GetSpecificItem(_saveRoot.FirstSelected);
            var secondTile = _mainGlobal.TileList.GetSpecificItem(_mainGlobal.SecondSelected);
            if (isAuto == false)
            {
                firstTile.IsSelected = false;
                secondTile.IsSelected = false;
                await MahJongSolitaireStaticFunctions.SaveMoveAsync(_saveRoot);
            }
            RemoveSpecificTile(firstTile);
            RemoveSpecificTile(secondTile);
            _saveRoot.FirstSelected = 0;
            _mainGlobal.SecondSelected = 0;
            if (isAuto == false)
                _saveRoot.TilesGone += 2;
            ValidList.Clear(); //maybe i had to clear the list again (?)
            CheckForValidTiles();
            if (isAuto == false)
                Check3DTiles();
        }

        public void Check3DTiles()
        {
            foreach (var thisTemp in _saveRoot.BoardList)
            {
                foreach (var tempCard in thisTemp.TileList)
                {
                    tempCard.NeedsBottom = false;
                    tempCard.NeedsTop = false;
                    tempCard.NeedsRight = false;
                    tempCard.NeedsLeft = false;
                }
            };
            var thisList = _saveRoot.BoardList.ToCustomBasicList();
            var thisBoard = _saveRoot.BoardList.Where(Items => Items.BoardCategory == BoardInfo.EnumBoardCategory.VeryTop).Single();
            thisBoard.TileList.ForEach(Items =>
            {
                Items.NeedsBottom = true;
                Items.NeedsLeft = true;
                Items.NeedsRight = true;
                Items.NeedsTop = true;
            });
            thisList.RemoveSpecificItem(thisBoard);
            thisList.KeepConditionalItems(Items => Items.Floor == 2);
            foreach (var firsts in thisList)
            {
                if (firsts.TileList.Count > 0)
                {
                    firsts.TileList.First().NeedsLeft = true;
                    firsts.TileList.Last().NeedsRight = true;
                    foreach (var ThisCard in firsts.TileList)
                    {
                        if (firsts.RowStart == 1)
                            ThisCard.NeedsTop = true;
                        else if (firsts.RowStart == 6)
                            ThisCard.NeedsBottom = true;
                    }
                }
            }
            thisList = (from Items in _saveRoot.BoardList
                        where Items.Floor == 3
                        select Items).ToCustomBasicList();
            foreach (var firsts in thisList)
            {
                if (firsts.TileList.Count > 0)
                {
                    firsts.TileList.First().NeedsLeft = true;
                    firsts.TileList.Last().NeedsRight = true;
                    foreach (var thisCard in firsts.TileList)
                    {
                        if (firsts.RowStart == 2)
                            thisCard.NeedsTop = true;
                        else if (firsts.RowStart == 5)
                            thisCard.NeedsBottom = true;
                    }
                }
            }
            thisList = (from Items in _saveRoot.BoardList
                        where Items.Floor == 4
                        select Items).ToCustomBasicList();
            foreach (var firsts in thisList)
            {
                if (firsts.TileList.Count > 0)
                {
                    firsts.TileList.First().NeedsLeft = true;
                    firsts.TileList.Last().NeedsRight = true;
                    foreach (var ThisCard in firsts.TileList)
                    {
                        if (firsts.RowStart == 3)
                            ThisCard.NeedsTop = true;
                        else if (firsts.RowStart == 4)
                            ThisCard.NeedsBottom = true;
                    }
                }
            }

        }
    }
}