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
using BasicGameFrameworkLibrary.CommonInterfaces;
using TileRummyCP.Data;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceMessages;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.Extensions; //most likely will be used.
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using BasicGameFrameworkLibrary.SpecializedGameTypes.RummyClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;

namespace TileRummyCP.Logic
{
    [SingletonGame]
    public class TileRummyMainGameClass : BasicGameClass<TileRummyPlayerItem, TileRummySaveInfo>, IMiscDataNM
    {
        public TileRummyMainGameClass(IGamePackageResolver resolver,
            IEventAggregator aggregator,
            BasicData basic,
            TestOptions test,
            TileRummyVMData model,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            CommandContainer command,
            BasicGameContainer<TileRummyPlayerItem, TileRummySaveInfo> gameContainer
            ) : base(resolver, aggregator, basic, test, model, state,delay, command, gameContainer)
        {
            _model = model;
            _command = command;
            _rummys = new RummyProcesses<EnumColorType, EnumColorType, TileInfo>(); //decided to create new one here.  hopefully i don't regret this
        }

        private readonly TileRummyVMData _model;
        private readonly CommandContainer _command;
        internal RummyProcesses<EnumColorType, EnumColorType, TileInfo> _rummys;

        private void PassOutTiles()
        {
            PlayerList!.ForEach(thisPlayer => thisPlayer.MainHandList.ReplaceRange(_model!.Pool1!.FirstDraw()));
            PlayerList.ForEach(thisPlayer => thisPlayer.HookUpHand()); //i think this was needed.
            SingleInfo = PlayerList.GetSelf();
            _model!.PlayerHand1!.HandList = SingleInfo.MainHandList; //forgot to hook up here.
            Aggregator.Subscribe(SingleInfo);
            SingleInfo.MainHandList.Sort(); //i think.
            _model.Pool1!.PopulateTotals();
        }

        public override Task FinishGetSavedAsync()
        {
            _model!.MainSets1!.ClearBoard();
            LoadControls();
            _model.Pool1!.SavedGame(SaveRoot!.PoolData!);
            int x = SaveRoot.SetList.Count; //the first time, actually load manually.
            x.Times(y =>
            {
                TileSet set = new TileSet(_command, _rummys);
                _model.MainSets1.CreateNewSet(set);
            });
            PlayerList!.ForEach(ThisPlayer =>
            {
                if (ThisPlayer.AdditionalTileList.Count > 0)
                {
                    ThisPlayer.MainHandList.AddRange(ThisPlayer.AdditionalTileList); //later sorts anyways.
                    ThisPlayer.AdditionalTileList.Clear(); //i think.
                }
            });
            PlayerList.ForEach(thisPlayer => thisPlayer.HookUpHand()); //i think this was needed.
            SingleInfo = PlayerList.GetSelf();
            _model.PlayerHand1!.HandList = SingleInfo.MainHandList; //forgot to hook up here.
            SingleInfo.MainHandList.Sort(); //i think.
            _model.Pool1.PopulateTotals();
            Aggregator.Subscribe(SingleInfo); //i think
            _model.MainSets1.LoadSets(SaveRoot.SetList);
            return Task.CompletedTask;
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            _rummys.HasSecond = false;
            _rummys.HasWild = true;
            _rummys.LowNumber = 1;
            _rummys.HighNumber = 13;
            _rummys.NeedMatch = true;
            IsLoaded = true; //i think needs to be here.
        }
        public override async Task ContinueTurnAsync()
        {
            await base.ContinueTurnAsync();
            _model.PlayerHand1.ReportCanExecuteChange();
            _model.TempSets.ReportCanExecuteChange();
        }
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            LoadControls();
            if (FinishUpAsync == null)
            {
                throw new BasicBlankException("The loader never set the finish up code.  Rethink");
            }
            SaveRoot!.FirstPlayedLast = 0;
            SaveRoot.ImmediatelyStartTurn = true;
            _model.MainSets1!.ClearBoard();
            _model.TempSets!.ClearBoard();
            SaveRoot.TilesFromField.Clear();
            _model.Pool1!.PopulatePool(); //i think.
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.InitCompleted = false;
                thisPlayer.Score = 0;
            });
            PassOutTiles();
            await FinishUpAsync(isBeginning);
        }
        public override async Task PopulateSaveRootAsync()
        {
            SaveRoot!.SetList = _model.MainSets1!.SavedSets();
            TileRummyPlayerItem self = PlayerList!.GetSelf();
            self.AdditionalTileList = _model.TempSets!.ListAllObjects();
            SaveRoot.PoolData = _model.Pool1!.SavedData();
            SaveRoot.SetList = _model.MainSets1.SavedSets();
            await base.PopulateSaveRootAsync();
        }
        public override async Task StartNewTurnAsync()
        {
            PrepStartTurn(); //anything else is below.
            _model!.Pool1!.NewTurn();
            SaveRoot!.TilesFromField.Clear();
            SaveRoot.BeginningList = _model.MainSets1!.SavedSets();
            var firsts = PlayerHand();
            SaveRoot.YourTiles = firsts.GetDeckListFromObjectList();
            SaveRoot.DidExpand = false;
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        private DeckRegularDict<TileInfo> PlayerHand()
        {
            return PlayerHand(SingleInfo!);
        }
        private DeckRegularDict<TileInfo> PlayerHand(TileRummyPlayerItem thisPlayer)
        {
            var output = thisPlayer.MainHandList.ToRegularDeckDict();
            if (thisPlayer.PlayerCategory != EnumPlayerCategory.Self)
                return output;
            output.AddRange(_model.TempSets!.ListAllObjects());
            return output;
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "laiddowninitial":
                    await CreateSetsAsync(content);
                    await InitialCompletedAsync();
                    return;
                case "createnewset":
                    var thisItem = await js.DeserializeObjectAsync<SendCreateSet>(content);
                    var thisCol = thisItem.CardList.GetObjectsFromList(SingleInfo!.MainHandList);
                    var thisTemp = new TempInfo();
                    thisTemp.CardList = thisCol;
                    thisTemp.WhatSet = thisItem.WhatSet;
                    await CreateNewSetAsync(thisTemp, false);
                    return;
                case "undomove":
                    await UndoMoveAsync();
                    return;
                case "finished":
                    await FinishedAsync(bool.Parse(content)); //hopefully okay.
                    return;
                case "drewtile":
                    var thisSend1 = await js.DeserializeObjectAsync<SendDraw>(content);
                    var thisTile = _model!.Pool1!.GetTile(thisSend1.Deck);
                    await DrawTileAsync(thisTile, thisSend1.FromEnd);
                    return;
                case "removeentireset":
                    await RemoveEntireSetAsync(int.Parse(content));
                    return;
                case "removeonefromset":
                    var thisSend2 = await js.DeserializeObjectAsync<SendSet>(content);
                    await RemoveTileFromSetAsync(thisSend2.Index, thisSend2.Tile);
                    return;
                case "addtoset":
                    var thisSend3 = await js.DeserializeObjectAsync<SendSet>(content);
                    var thisTile2 = _model!.Pool1!.GetTile(thisSend3.Tile);
                    thisTile2.IsUnknown = false;
                    SingleInfo!.MainHandList.RemoveObjectByDeck(thisSend3.Tile);
                    await AddToSetAsync(thisSend3.Index, thisTile2, thisSend3.Position);
                    return;
                case "endcustom":
                    var thisEnd = await js.DeserializeObjectAsync<SendCustom>(content);
                    await EndTurnAsync(thisEnd.DidPlay, thisEnd.ValidSets);
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }

        private async Task CreateSetsAsync(string message)
        {
            var firstTemp = await js.DeserializeObjectAsync<CustomBasicList<string>>(message);
            foreach (var thisFirst in firstTemp)
            {
                var thisSend = await js.DeserializeObjectAsync<SendCreateSet>(thisFirst);
                var thisCol = thisSend.CardList.GetObjectsFromList(SingleInfo!.MainHandList);
                TempInfo thisTemp = new TempInfo();
                thisTemp.CardList = thisCol;
                thisTemp.WhatSet = thisSend.WhatSet;
                await CreateNewSetAsync(thisTemp, true);
            }
        }
        public override async Task EndTurnAsync()
        {
            await FinishedAsync(false);
        }
        public async Task EndTurnAsync(bool didPlay, bool validSets)
        {
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
            {
                _model!.TempSets!.EndTurn();
                _model.PlayerHand1!.EndTurn();
                if (BasicData.IsXamarinForms)
                {
                    await Task.Delay(700); //try 30 to start with.
                }
                //await UIPlatform.ShowMessageAsync("Ending Turn");
            }
            if (validSets == false)
            {
                if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                    await UIPlatform.ShowMessageAsync("Failed to rearrange the sets.  Therefore, you have to draw 3 tiles if any are left");
                await ResetAsync();
            }
            _model!.MainSets1!.EndTurn();
            if (BasicData!.MultiPlayer)
            {
                if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                {
                    await Network!.SendAllAsync("finished", didPlay);
                }
                else
                {
                    Check!.IsEnabled = true;
                    return;
                }
            }
            await FinishedAsync(didPlay);
        }
        private async Task<bool> CanEndGameAsync(bool didPlay)
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            var thisList = PlayerHand();
            if (thisList.Count == 0)
                return true;
            if (didPlay == true || _model!.Pool1!.HasDrawn())
            {
                SaveRoot!.FirstPlayedLast = 0;
                return false;
            }
            if (SaveRoot!.FirstPlayedLast == 0)
            {
                SaveRoot.FirstPlayedLast = WhoTurn;
                return false;
            }
            int newTurn = await PlayerList.CalculateWhoTurnAsync(true);
            return newTurn == SaveRoot.FirstPlayedLast;
        }
        private async Task GameOverAsync()
        {
            UpdatePoints();
            WinProcesses();
            await ShowWinAsync(); //i think it should be set to the proper one (?)
        }
        private void UpdatePoints()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                var thisCol = PlayerHand(thisPlayer);
                thisPlayer.Score = thisCol.Sum(items => items.Points);
            });
        }
        public async Task FinishedAsync(bool didPlay)
        {
            if (await CanEndGameAsync(didPlay))
            {
                await GameOverAsync();
                return;
            }
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
        }
        private void WinProcesses()
        {
            if (PlayerList.Any(items => items.ObjectCount == 0))
            {
                SingleInfo = PlayerList.Single(items => items.ObjectCount == 0);
                return;
            }
            int lefts;
            int minPoints = 500;
            int tilesLeft = 1111;
            int whoWon = 0;
            int currents;
            CustomBasicList<int> tieList = new CustomBasicList<int>();
            PlayerList!.ForEach(thisPlayer =>
            {
                lefts = thisPlayer.ObjectCount;
                currents = thisPlayer.Score;
                if (currents < minPoints)
                {
                    tilesLeft = 1111;
                    whoWon = thisPlayer.Id;
                }
                else if (currents == minPoints && lefts < tilesLeft)
                {
                    tilesLeft = lefts;
                    tieList = new CustomBasicList<int>
                    {
                        thisPlayer.Id
                    };
                    whoWon = thisPlayer.Id;
                }
                else if (currents == minPoints && lefts == tilesLeft)
                {
                    tieList.Add(thisPlayer.Id);
                }
            });
            if (tieList.Count <= 1)
            {
                WhoTurn = whoWon;
                SingleInfo = PlayerList.GetWhoPlayer();
                return;
            }
            whoWon = tieList.GetRandomItem();
            WhoTurn = whoWon;
            SingleInfo = PlayerList.GetWhoPlayer();
        }
        public IDeckDict<TileInfo> WhatSet(int WhichOne)
        {
            return _model!.TempSets!.ObjectList(WhichOne);
        }
        public async Task UndoMoveAsync()
        {
            ResetField();
            SaveRoot!.DidExpand = false;
            await ContinueTurnAsync();
        }
        private void ResetField()
        {
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                _model!.TempSets!.ClearBoard(); //i think
            DeckRegularDict<TileInfo> tempList = new DeckRegularDict<TileInfo>();
            SaveRoot!.YourTiles.ForEach(thisIndex =>
            {
                var thisTile = _model!.Pool1!.GetTile(thisIndex);
                tempList.Add(thisTile);
            });
            SingleInfo.MainHandList.ReplaceRange(tempList);
            SaveRoot.TilesFromField.Clear();
            int x = SaveRoot.BeginningList.Count; //the first time, actually load manually.
            _model!.MainSets1!.SetList.Clear(); //try this way.
            x.Times(y =>
            {
                TileSet set = new TileSet(_command, _rummys);
                _model.MainSets1.CreateNewSet(set);
            });
            _model.MainSets1.LoadSets(SaveRoot.BeginningList);
            _model.MainSets1.RedoSets(); //i think.
            SingleInfo.MainHandList.ForEach(thisCard => thisCard.WhatDraw = EnumDrawType.IsNone);
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                SingleInfo.MainHandList.Sort();
        }
        private async Task ResetAsync()
        {
            ResetField();
            await SaveStateAsync(); //just in case.
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.OtherHuman)
            {
                Check!.IsEnabled = true;
                return;
            }
            for (int x = 1; x <= 3; x++)
            {
                if (_model!.Pool1!.HasTiles() == false)
                    break;
                var thisTile = _model.Pool1.DrawTile();
                if (BasicData!.MultiPlayer)
                {
                    SendDraw thisSend = new SendDraw();
                    thisSend.Deck = thisTile.Deck;
                    thisSend.FromEnd = true;
                    await Network!.SendAllAsync("drewtile", thisSend);
                }
                await DrawTileAsync(thisTile, true);
            }
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                SingleInfo.MainHandList.Sort();
        }
        public DeckRegularDict<TileInfo> GetSelectedList()
        {
            DeckRegularDict<TileInfo> output = new DeckRegularDict<TileInfo>();
            var thisList = _model!.PlayerHand1!.ListSelectedObjects();
            output.AddRange(thisList);
            var newList = _model.TempSets!.ListSelectedObjects();
            output.AddRange(newList);
            return output;
        }
        public async Task DrawTileAsync(TileInfo thisTile, bool fromEnd)
        {
            thisTile.WhatDraw = EnumDrawType.FromHand;
            _model!.Pool1!.RemoveTile(thisTile);
            _model.Pool1!.PopulateTotals();
            SingleInfo!.MainHandList.Add(thisTile);
            if (fromEnd == false)
                SaveRoot!.YourTiles.Add(thisTile.Deck);
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self && fromEnd == false)
                SingleInfo.MainHandList.Sort();
            if (fromEnd == false || SingleInfo.PlayerCategory == EnumPlayerCategory.OtherHuman)
            {
                await ContinueTurnAsync();
            }
        }
        public bool ValidPlay()
        {
            if (SaveRoot!.TilesFromField.Count > 0)
                return false;
            var setList = _model!.MainSets1!.SetList;
            return !setList.Any(items => items.IsAcceptableSet() == false);
        }
        public async Task InitialCompletedAsync()
        {
            SingleInfo!.InitCompleted = true;
            await ContinueTurnAsync();
        }
        public async Task CreateNewSetAsync(TempInfo thisTemp, bool isInit)
        {
            if (thisTemp.CardList.Count == 0)
                throw new BasicBlankException("There must be at least one tile in order to create a new set");
            if (thisTemp.WhatSet == EnumWhatSets.Runs)
            {
                thisTemp.CardList.Sort(); //hopefully that will work (?)
                var firstTile = thisTemp.CardList.First();
                var lastTile = thisTemp.CardList.Last();
                if (firstTile.IsJoker)
                    firstTile.Number = thisTemp.FirstNumber;
                if (lastTile.IsJoker)
                    lastTile.Number = thisTemp.SecondNumber;
            }
            thisTemp.CardList.ForEach(thisTile =>
            {
                if (thisTile.WhatDraw != EnumDrawType.FromSet)
                    thisTile.WhatDraw = EnumDrawType.FromHand;
                thisTile.IsSelected = false;
                thisTile.Drew = true;
            });
            TileSet thisSet = new TileSet(_command, _rummys);
            thisSet.CreateSet(thisTemp.CardList, thisTemp.WhatSet);
            _model!.MainSets1!.CreateNewSet(thisSet);
            if (isInit == false)
            {
                SaveRoot!.DidExpand = true;
                await ContinueTurnAsync();
            }
        }
        public bool IsKinds(DeckRegularDict<TileInfo> thisList)
        {
            if (thisList.Count > 4)
                return false;
            var temps = thisList.Where(items => items.IsJoker == false).ToRegularDeckDict();
            var count = temps.DistinctCount(items => items.Color);
            if (count != temps.Count)
                return false; //because has to have different colors.
            return temps.DistinctCount(items => items.Number) == 1; //if its one, then its okay.  otherwise, its not.
        }
        public bool HasValidSet(DeckRegularDict<TileInfo> thisCol, out int firstNumber, out int secondNumber)
        {
            firstNumber = -1;
            secondNumber = -1; //until proven.
            if (thisCol.Count < 3)
                return false;
            if (thisCol.Count(items => items.IsJoker == true) > 1)
                return false; //can only use one joker for each set.
            if (thisCol.Count > 2)
            {
                var newCol = thisCol.ToRegularDeckDict();
                if (_rummys!.IsNewRummy(newCol, newCol.Count, RummyProcesses<EnumColorType, EnumColorType, TileInfo>.EnumRummyType.Runs))
                {
                    firstNumber = _rummys.FirstUsed;
                    secondNumber = _rummys.FirstUsed + thisCol.Count - 1;
                    return true;
                }
            }
            return IsKinds(thisCol);
        }
        public bool HasInitialSets(out CustomBasicList<TempInfo> newList)
        {
            IDeckDict<TileInfo> tempCollection; //hopefully still works (?)
            DeckRegularDict<TileInfo> thisCollection;
            newList = new CustomBasicList<TempInfo>();
            int totalPoints = 0;
            for (int x = 1; x <= 4; x++)
            {
                tempCollection = WhatSet(x);
                thisCollection = new DeckRegularDict<TileInfo>();
                if (tempCollection.Count > 0)
                    thisCollection.AddRange(tempCollection);
                if (HasValidSet(thisCollection, out int firsts, out int seconds))
                {
                    TempInfo thisTemp = new TempInfo();
                    thisTemp.CardList = thisCollection;
                    totalPoints += thisTemp.CardList.Sum(items => items.Points);
                    thisTemp.TempSet = x;
                    if (firsts == -1)
                    {
                        thisTemp.WhatSet = EnumWhatSets.Kinds;
                    }
                    else
                    {
                        thisTemp.WhatSet = EnumWhatSets.Runs;
                        thisTemp.FirstNumber = firsts;
                        thisTemp.SecondNumber = seconds;
                    }
                    newList.Add(thisTemp);
                }
            }
            return totalPoints >= 30; //brenden said the total points has to be 30 or over for the initial sets.
        }
        public async Task AddToSetAsync(int index, TileInfo thisTile, int position)
        {
            var thisSet = _model!.MainSets1!.GetIndividualSet(index);
            if (thisTile.WhatDraw != EnumDrawType.FromSet)
                thisTile.WhatDraw = EnumDrawType.FromHand;
            thisTile.IsSelected = false;
            SaveRoot!.DidExpand = true;
            thisSet.AddTile(thisTile, position);
            await ContinueTurnAsync();
        }
        public async Task RemoveTileFromSetAsync(int index, int deck)
        {
            SaveRoot!.TilesFromField.Add(deck);
            TileSet thisSet = _model!.MainSets1!.GetIndividualSet(index);
            SaveRoot.DidExpand = true;
            var thisTile = _model.Pool1!.GetTile(deck);
            thisTile.WhatDraw = EnumDrawType.FromSet;
            if (thisTile.IsJoker)
                thisTile.Number = 20;
            thisSet.HandList.RemoveObjectByDeck(deck);
            SingleInfo!.MainHandList.Add(thisTile);
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                SingleInfo.MainHandList.Sort();
            await ContinueTurnAsync();
        }
        public async Task RemoveEntireSetAsync(int index)
        {
            TileSet thisSet = _model!.MainSets1!.GetIndividualSet(index); //looks like 0 based bug again  try a different method that is not 0 based.
            var tempList = new DeckRegularDict<TileInfo>();
            thisSet.HandList.ForEach(thisTile =>
            {
                thisTile.WhatDraw = EnumDrawType.FromSet;
                SaveRoot!.TilesFromField.Add(thisTile.Deck);
                if (thisTile.IsJoker)
                    thisTile.Number = 20;
                tempList.Add(thisTile);
            });
            _model.MainSets1.RemoveSet(index - 1); //i think 0 based bug again.
            SingleInfo!.MainHandList.AddRange(tempList);
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                SingleInfo.MainHandList.Sort();
            await ContinueTurnAsync();
        }

    }
}
