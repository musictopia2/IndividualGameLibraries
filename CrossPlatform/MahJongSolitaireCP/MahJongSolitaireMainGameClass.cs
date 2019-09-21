using BaseMahjongTilesCP;
using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace MahJongSolitaireCP
{
    [SingletonGame]
    public class MahJongSolitaireMainGameClass
    {
        private bool _possibleWins = true;
        public IGamePackageResolver MainContainer { get; set; }
        public MahJongSolitaireGameBoardCP? GameBoard1; //just in case.
        //private MahjongShuffler MainList;
        public readonly ISaveSinglePlayerClass MainSave;
        private bool _didLoad;
        public MahJongSolitaireSaveInfo? SaveRoot;
        public BaseMahjongGlobals? ThisGlobal;
        private readonly EventAggregator _thisE;
        //private bool IsXamarinForms;
        public readonly MahJongSolitaireModGlobal CustomGlobal;
        private readonly ISimpleUI _thisMessage;
        public MahJongSolitaireMainGameClass(IGamePackageResolver container)
        {
            MainContainer = container;
            MainSave = MainContainer.Resolve<ISaveSinglePlayerClass>();
            _thisE = MainContainer.Resolve<EventAggregator>();
            CustomGlobal = MainContainer.Resolve<MahJongSolitaireModGlobal>();
            _thisMessage = MainContainer.Resolve<ISimpleUI>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deck">0 to hide</param>
        public void ShowSelectedItem(int deck) //0 means to hide it
        {
            _thisE.Publish(new TileChosenEventModel { Deck = deck });
        }
        public async Task NewGameAsync(Action<int> finalProcess)
        {
            bool autoResume = false;
            if (_didLoad == false)
            {
                if (await MainSave.CanOpenSavedSinglePlayerGameAsync() == true)
                {
                    SaveRoot = await MainSave.RetrieveSinglePlayerGameAsync<MahJongSolitaireSaveInfo>();
                    MainContainer.ReplaceObject(SaveRoot); //try this way.
                    autoResume = true;
                }
                else
                {
                    SaveRoot = MainContainer.Resolve<MahJongSolitaireSaveInfo>();
                }
                GameBoard1 = MainContainer.Resolve<MahJongSolitaireGameBoardCP>();
                ThisGlobal = MainContainer.Resolve<BaseMahjongGlobals>();
            }
            else
            {
                GameBoard1!.ClearBoard();
                CustomGlobal.TileList.ClearObjects(); //maybe this was needed too.
                _thisE.Publish(new StartNewGameEventModel());
            }

            if (autoResume == true)
            {
                ThisGlobal!.CanShowTiles = true;
                int CardCount = 0;
                SaveRoot!.BoardList.ForEach(Items =>
                {
                    CardCount += Items.TileList.Count;
                    CustomGlobal.TileList.AddRelinkedTiles(Items.TileList);
                });
                finalProcess.Invoke(144 - CardCount);
                _thisE.Publish(this); //i think
                _didLoad = true;
                if (SaveRoot.FirstSelected > 0)
                    ShowSelectedItem(SaveRoot.FirstSelected);
                else if (CustomGlobal.SecondSelected > 0)
                    ShowSelectedItem(CustomGlobal.SecondSelected);
                return;
            }

            _possibleWins = false; //for now.  so i can get more hints on the problems.
            if (_possibleWins == true)
            {
                ThisGlobal!.CanShowTiles = false;
                await GetWinnableListAsync();
                GameBoard1.ClearBoard();
                CustomGlobal.TileList.ForEach(Items =>
                {
                    Items.IsEnabled = false;
                    Items.Visible = true;
                });
                //CustomGlobal.TileList.ForEach(Items =>
                //{
                //    Items.IsNotifying = true;
                //});
            }
            else
                CustomGlobal.TileList.ShuffleObjects(); //i think
            ThisGlobal!.CanShowTiles = true;
            LoadBoard();
            GameBoard1.PositionTiles();
            _thisE.Publish(this);
            finalProcess.Invoke(0); //because you have 0 tiles left for sure
            ShowSelectedItem(0); //for now.
            await MainSave.SaveSimpleSinglePlayerGameAsync(SaveRoot!);
            _didLoad = true;
        }
        internal void Publish()
        {
            _thisE.Publish(this);
        }
        private void LoadBoard()
        {
            int x = 0;
            SaveRoot!.BoardList.ForEach(thisBoard =>
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
            MahjongSolitaireTileInfo firstTile = CustomGlobal.TileList.GetSpecificItem(SaveRoot!.FirstSelected);
            MahjongSolitaireTileInfo secondTile = CustomGlobal.TileList.GetSpecificItem(CustomGlobal.SecondSelected);
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
                await MainSave.SaveSimpleSinglePlayerGameAsync(SaveRoot!);
                return;
            }
            await GameBoard1!.ProcessPairAsync(false);
            updateTiles.Invoke();
            if (GameBoard1.IsGameOver() == true)
            {
                await _thisMessage.ShowMessageBox("You Win");
                SaveRoot!.PreviousList.Clear();
                await MainSave.DeleteSinglePlayerGameAsync();
                return;
            }
            await MainSave.SaveSimpleSinglePlayerGameAsync(SaveRoot!);
        }
        private void AddCardToBoard(BoardInfo thisBoard, int whichOne)
        {
            MahjongSolitaireTileInfo thisTile = CustomGlobal.TileList.GetIndexedTile(whichOne);
            thisBoard.TileList.Add(thisTile);
            thisTile.IsEnabled = false;
        }

        private async Task GetWinnableListAsync()
        {
            ThisGlobal!.CanShowTiles = false;
            int tempGone;
            await Task.Run(async () =>
            {
                do
                {
                    tempGone = 0;
                    CustomGlobal.TileList.ShuffleObjects();
                    GameBoard1!.ClearBoard();
                    LoadBoard();
                    GameBoard1.CheckForValidTiles();
                    int i1;
                    int i2;
                    bool bln_Paired = false;
                    while (true)
                    {
                        bln_Paired = false;
                        i1 = 1;
                        DeckRegularDict<MahjongSolitaireTileInfo> OurList = GameBoard1.ValidList.ToRegularDeckDict();
                        foreach (var tile1 in OurList)
                        {
                            i2 = 1;
                            foreach (var tile2 in OurList)
                            {
                                if (!(tile1 == tile2))
                                {
                                    SaveRoot!.FirstSelected = tile1.Deck;
                                    CustomGlobal.SecondSelected = tile2.Deck;
                                    if (IsValidMove() == true)
                                    {
                                        bln_Paired = true;
                                        await GameBoard1.ProcessPairAsync(true);
                                        tempGone += 2;
                                        if (GameBoard1.IsGameOver())
                                            return; //okay because no lambdas
                                    }
                                }
                                i2++;
                                if (i1 == GameBoard1.ValidList.Count & i2 == GameBoard1.ValidList.Count)
                                {
                                    if (tempGone == 144)
                                        return;
                                    break;
                                }
                            }
                            if ((bln_Paired))
                                break;
                            i1 += 1;
                        }
                    }
                } while (true);
            });
        }
    }
}