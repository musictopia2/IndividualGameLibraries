using BaseMahjongTilesCP;
using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.CommonInterfaces;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using MahJongSolitaireCP.Data;
using MahJongSolitaireCP.EventModels;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace MahJongSolitaireCP.Logic
{
    [SingletonGame]
    public class MahJongSolitaireMainGameClass : IAggregatorContainer
    {

        //maybe we should not even both with possible wins.


        //private bool _possibleWins = true;
        //private readonly ISaveSinglePlayerClass _thisState;
        private readonly MahJongSolitaireModGlobal _customGlobal;
        private readonly MahJongSolitaireSaveInfo _saveRoot;
        public MahJongSolitaireGameBoardCP GameBoard1; //just in case.

        private readonly BaseMahjongGlobals _baseGlobal;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deck">0 to hide</param>
        public void ShowSelectedItem(int deck) //0 means to hide it
        {
            Aggregator.Publish(new TileChosenEventModel { Deck = deck });
        }

        public MahJongSolitaireMainGameClass(
            IEventAggregator aggregator,
            MahJongSolitaireModGlobal global,
            MahJongSolitaireGameBoardCP board,
            BaseMahjongGlobals baseGlobal,
            MahJongSolitaireSaveInfo saveRoot
            )
        {
            //_thisState = thisState;
            Aggregator = aggregator;
            _customGlobal = global;
            //_saveRoot = container.ReplaceObject<MahJongSolitaireSaveInfo>(); //can't create new one.  because if doing that, then anything that needs it won't have it.
            GameBoard1 = board; //hopefully no problems with circular references (?)
            _baseGlobal = baseGlobal;
            _saveRoot = saveRoot;
        }

        //private bool _opened;
        //internal bool _gameGoing;

        public IEventAggregator Aggregator { get; }

        internal void Publish()
        {
            Aggregator.Publish(this);
        }

        public Task NewGameAsync(Action<int> finalProcess)
        {
            _customGlobal.TileList.ShuffleObjects();
            _baseGlobal.CanShowTiles = true;
            LoadBoard();
            GameBoard1.PositionTiles();
            Publish();
            finalProcess.Invoke(0); //because you have 0 tiles left for sure
            ShowSelectedItem(0); //for now.
            return Task.CompletedTask;
        }

        private void LoadBoard()
        {
            int x = 0;
            _saveRoot!.BoardList.ForEach(thisBoard =>
            {
                int UpTo;
                if (thisBoard.BoardCategory == BoardInfo.EnumBoardCategory.Regular)
                    UpTo = thisBoard.HowManyColumns;
                else if (thisBoard.BoardCategory == BoardInfo.EnumBoardCategory.FarRight)
                    UpTo = 2;
                else
                    UpTo = 1;
                UpTo.Times(y =>
                {
                    AddCardToBoard(thisBoard, x);
                    x++;
                });
            });
        }
        public bool IsValidMove()
        {
            MahjongSolitaireTileInfo firstTile = _customGlobal.TileList.GetSpecificItem(_saveRoot!.FirstSelected);
            MahjongSolitaireTileInfo secondTile = _customGlobal.TileList.GetSpecificItem(_customGlobal.SecondSelected);
            if (firstTile.WhatNumber != BasicMahjongTile.EnumNumberType.IsNoNumber && secondTile.WhatNumber != BasicMahjongTile.EnumNumberType.IsNoNumber)
                return firstTile.NumberUsed == secondTile.NumberUsed && firstTile.WhatNumber == secondTile.WhatNumber;
            if (firstTile.WhatBonus != BasicMahjongTile.EnumBonusType.IsNoBonus && secondTile.WhatBonus != BasicMahjongTile.EnumBonusType.IsNoBonus)
                return firstTile.WhatBonus == secondTile.WhatBonus;
            if (firstTile.WhatColor != BasicMahjongTile.EnumColorType.IsNoColor && secondTile.WhatColor != BasicMahjongTile.EnumColorType.IsNoColor)
                return firstTile.WhatColor == secondTile.WhatColor;
            if (firstTile.WhatDirection != BasicMahjongTile.EnumDirectionType.IsNoDirection && secondTile.WhatDirection != BasicMahjongTile.EnumDirectionType.IsNoDirection)
                return firstTile.WhatDirection == secondTile.WhatDirection;
            return false;
        }
        public async Task PairSelectedManuallyAsync(Action updateTiles)
        {
            ShowSelectedItem(0); //needs to happen no matter what.
            if (IsValidMove() == false)
            {
                GameBoard1!.UnselectTiles();
                //await MainSave.SaveSimpleSinglePlayerGameAsync(_saveRoot!);
                return;
            }
            await GameBoard1!.ProcessPairAsync(false);
            updateTiles.Invoke();
            if (GameBoard1.IsGameOver() == true)
            {
                await ShowWinAsync();
                return;
            }
            //await MainSave.SaveSimpleSinglePlayerGameAsync(SaveRoot!);
        }
        private void AddCardToBoard(BoardInfo thisBoard, int whichOne)
        {
            MahjongSolitaireTileInfo thisTile = _customGlobal.TileList.GetIndexedTile(whichOne);
            thisBoard.TileList.Add(thisTile);
            thisTile.IsEnabled = false;
        }



        //private async Task RestoreGameAsync()
        //{
        //    _saveRoot = await _thisState.RetrieveSinglePlayerGameAsync<MahJongSolitaireSaveInfo>();

        //}
        public async Task ShowWinAsync()
        {
            //_gameGoing = false;
            await UIPlatform.ShowMessageAsync("You Win");
            //ThisMod.NewGameVisible = true;
            //await _thisState.DeleteSinglePlayerGameAsync();
            //send message to show win.
            await this.SendGameOverAsync();

        }
    }
}
