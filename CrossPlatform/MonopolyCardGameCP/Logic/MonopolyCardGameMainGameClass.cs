using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.Extensions; //most likely will be used.
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceMessages;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.NetworkingClasses.Extensions;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using MonopolyCardGameCP.Cards;
using MonopolyCardGameCP.Data;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.

namespace MonopolyCardGameCP.Logic
{
    [SingletonGame]
    public class MonopolyCardGameMainGameClass : CardGameClass<MonopolyCardGameCardInformation, MonopolyCardGamePlayerItem, MonopolyCardGameSaveInfo>, IMiscDataNM, IStartNewGame
    {


        private readonly MonopolyCardGameVMData _model;
        private readonly CommandContainer _command; //most of the time, needs this.  if not needed, take out.
        private readonly MonopolyCardGameGameContainer _gameContainer; //if we don't need it, take it out.

        public MonopolyCardGameMainGameClass(IGamePackageResolver mainContainer,
            IEventAggregator aggregator,
            BasicData basicData,
            TestOptions test,
            MonopolyCardGameVMData currentMod,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            ICardInfo<MonopolyCardGameCardInformation> cardInfo,
            CommandContainer command,
            MonopolyCardGameGameContainer gameContainer)
            : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer)
        {
            _model = currentMod;
            _command = command;
            _gameContainer = gameContainer;
            _gameContainer.ProcessTrade = ProcessTrade;
        }
        private bool _doContinue;

        public override async Task FinishGetSavedAsync()
        {
            LoadTradePiles();
            _doContinue = true;
            await PlayerList!.ForEachAsync(async thisPlayer =>
            {
                var thisList = await js.DeserializeObjectAsync<DeckObservableDict<MonopolyCardGameCardInformation>>(thisPlayer.TradeString);
                thisPlayer.TradePile!.HandList = new DeckObservableDict<MonopolyCardGameCardInformation>(thisList);
            });
            await base.FinishGetSavedAsync();
        }
        protected override async Task ComputerTurnAsync()
        {
            //if there is nothing, then just won't do anything.
            await Task.CompletedTask;
        }
        public override async Task PopulateSaveRootAsync()
        {
            await PlayerList!.ForEachAsync(async thisPlayer =>
            {
                thisPlayer.TradeString = await js.SerializeObjectAsync(thisPlayer.TradePile!.HandList);
            });
            await base.PopulateSaveRootAsync();
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            if (isBeginning)
                LoadTradePiles();
            SaveRoot!.ImmediatelyStartTurn = true;
            SaveRoot.GameStatus = EnumWhatStatus.DrawOrTrade;
            _doContinue = true;
            _model!.AdditionalInfo1!.Clear();
            return base.StartSetUpAsync(isBeginning);
        }
        protected override async Task LastPartOfSetUpBeforeBindingsAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.TradePile!.ClearBoard(_model!.Deck1!.DrawCard());
            });
            await base.LastPartOfSetUpBeforeBindingsAsync();
        }

        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status) //can't do switch because we don't know what the cases are ahead of time.
            {
                case "goout":
                    SingleInfo = PlayerList!.GetWhoPlayer();
                    await ProcessGoingOutAsync();
                    Check!.IsEnabled = true;
                    return; //i did have to wait for other players at this point.
                case "trade1":
                    var thisCol = await content.GetObjectsFromDataAsync(SingleInfo!.MainHandList); //i think.
                    thisCol.ForEach(thisCard => SingleInfo.TradePile!.AddCard(thisCard.Deck));
                    SingleInfo.TradePile!.ScrollToBottom();
                    Check!.IsEnabled = true; //wait for more
                    return;
                case "trade2":
                    SendTrade thisSend = await js.DeserializeObjectAsync<SendTrade>(content);
                    var tempPlayer = PlayerList![thisSend.Player];
                    TradePile newTrade = tempPlayer.TradePile!;
                    var tempCollection = thisSend.CardList.GetNewObjectListFromDeckList(_gameContainer.DeckList!).ToRegularDeckDict();
                    ProcessTrade(newTrade, tempCollection, SingleInfo!.TradePile!);
                    newTrade.ScrollToBottom();
                    await ContinueTurnAsync(); //maybe this was missing.
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        Task IStartNewGame.ResetAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.TotalMoney = 0;
                thisPlayer.PreviousMoney = 0;
            });
            return Task.CompletedTask;
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            if (SaveRoot!.GameStatus != EnumWhatStatus.LookOnly)
            {
                if (SingleInfo!.MainHandList.Count > 10)
                    SaveRoot.GameStatus = EnumWhatStatus.Either;
                else
                    SaveRoot.GameStatus = EnumWhatStatus.DrawOrTrade;
            }
            else
            {
                await ProcessEndAsync();
                if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Computer)
                {
                    await EndTurnAsync();
                    return;
                }
            }
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task EndRoundAsync()
        {
            if (PlayerList.Any(items => items.TotalMoney >= 10000))
            {
                SingleInfo = PlayerList.OrderByDescending(items => items.TotalMoney).First();
                await ShowWinAsync();
                return;
            }
            await this.RoundOverNextAsync();
        }
        public override async Task EndTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            if (SingleInfo.CanSendMessage(BasicData!))
                await Network!.SendEndTurnAsync();
            _command.ManuelFinish = true; //because it could be somebody else's turn.
            if (SingleInfo.PlayerCategory != EnumPlayerCategory.Self)
                SingleInfo.MainHandList.UnhighlightObjects();
            else if (_gameContainer.AlreadyDrew)
                _model.PlayerHand1!.EndTurn();
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                _model.AdditionalInfo1!.Clear();
            PlayerList.ForEach(thisPlayer => thisPlayer.TradePile!.EndTurn());
            if (SaveRoot!.GameStatus == EnumWhatStatus.LookOnly)
            {
                var previousOne = SaveRoot.WhoWentOut - 1;
                if (previousOne == 0)
                    previousOne = PlayerList.Count();
                if (previousOne == WhoTurn)
                {
                    await EndRoundAsync();
                    return;
                }
            }
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
        }
        #region "Advanced Processes"
        private async Task DrawUpTo5Async()
        {
            LeftToDraw = 5;
            _doContinue = false;
            await DrawAsync();
        }
        public async Task ProcessGoingOutAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            SaveRoot!.WhoWentOut = WhoTurn;
            _model!.Status = SingleInfo.NickName + " went out.  Finishing the round";
            PlayerList.ForEach(thisPlayer =>
            {
                thisPlayer.TradePile!.RemoveCards();
            });
            await DrawUpTo5Async();
        }
        protected override bool ShowNewCardDrawn(MonopolyCardGamePlayerItem TempPlayer)
        {
            if (_doContinue == false)
                return true;
            return base.ShowNewCardDrawn(TempPlayer);
        }
        protected override async Task AfterDrawingAsync()
        {
            if (_doContinue)
            {
                SaveRoot!.GameStatus = EnumWhatStatus.Discard;
                await base.AfterDrawingAsync();
                return;
            }
            var newScore = CalculateScore(WhoTurn, true, out DeckRegularDict<MonopolyCardGameCardInformation> newGroup);
            SaveRoot!.GameStatus = EnumWhatStatus.LookOnly;
            await FinalProcessAsync(newGroup, newScore);
        }
        public void ProcessTrade(TradePile newTrade, DeckRegularDict<MonopolyCardGameCardInformation> oldCollection, TradePile yourTrade)
        {
            newTrade.GetNumberOfItems(oldCollection.Count);
            SingleInfo = PlayerList![newTrade.GetPlayerIndex];
            yourTrade.GetNumberOfItems(oldCollection.Count);
            yourTrade.ScrollToBottom();
            newTrade.ScrollToBottom();
            SingleInfo = PlayerList.GetWhoPlayer();
            SaveRoot!.GameStatus = EnumWhatStatus.Discard; //hopefully this simple (?)
        }
        internal void CreateTradePile(MonopolyCardGamePlayerItem tempPlayer)
        {
            tempPlayer.TradePile = new TradePile(_gameContainer, _model, tempPlayer.Id); //hopefully this simple (?)
        }
        private void LoadTradePiles()
        {
            var tempList = PlayerList!.GetAllPlayersStartingWithSelf();
            tempList.ForEach(thisPlayer => CreateTradePile(thisPlayer));
        }
        private async Task ProcessEndAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            var newScore = CalculateScore(WhoTurn, false, out DeckRegularDict<MonopolyCardGameCardInformation> newGroup);
            await FinalProcessAsync(newGroup, newScore);
        }
        private async Task FinalProcessAsync(DeckRegularDict<MonopolyCardGameCardInformation> newGroup, decimal newScore)
        {
            var thisTrade = SingleInfo!.TradePile;
            thisTrade!.RemoveCards();
            newGroup.ForEach(thisCard =>
            {
                if (SingleInfo.MainHandList.ObjectExist(thisCard.Deck))
                {
                    SingleInfo.MainHandList.RemoveObjectByDeck(thisCard.Deck);
                    thisTrade.AddCard(thisCard.Deck);
                }
            });
            thisTrade.ScrollToBottom();
            if (Test!.NoAnimations == false)
                await Delay!.DelaySeconds(.5);
            SingleInfo.PreviousMoney = newScore;
            SingleInfo.TotalMoney += newScore;
        }
        public bool CanGoOut(DeckRegularDict<MonopolyCardGameCardInformation> whatGroup)
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            var tempCol = whatGroup.ToRegularDeckDict();
            var groupList = tempCol.GroupBy(items => items.Group).ToCustomBasicList();
            bool hasRailRoad = whatGroup.Any(items => items.WhatCard == EnumCardType.IsRailRoad);
            bool hasUtilities = whatGroup.Any(items => items.WhatCard == EnumCardType.IsUtilities);
            int numWilds = whatGroup.Count(items => items.WhatCard == EnumCardType.IsChance);
            if (numWilds < 2 && hasRailRoad == false && hasUtilities == false && groupList.Count == 0)
                return false; //cannot go out because do not have any properties, no railroads, no utilities, and not enough wilds
            var temps = tempCol.Where(items => (int)items.WhatCard > 3 && (int)items.WhatCard < 7).ToRegularDeckDict();
            tempCol.RemoveGivenList(temps, NotifyCollectionChangedAction.Remove);
            CustomBasicList<int> setList = new CustomBasicList<int>();
            DeckRegularDict<MonopolyCardGameCardInformation> monCol;
            foreach (var thisGroup in groupList)
            {
                if (thisGroup.Key > 0)
                {
                    monCol = MonopolyCol(tempCol, thisGroup.Key, EnumCardType.IsProperty);
                    if (monCol.Count == 0)
                        return false;
                    setList.Add(thisGroup.Key);
                }
            }
            if (hasRailRoad)
                monCol = MonopolyCol(tempCol, 0, EnumCardType.IsRailRoad);
            if (hasUtilities)
                monCol = MonopolyCol(tempCol, 0, EnumCardType.IsUtilities);
            setList.ForEach(thisSet =>
            {
                HouseCollection(tempCol); //to filter further
            });
            tempCol.RemoveAllOnly(items => items.WhatCard == EnumCardType.IsChance);
            return tempCol.Count == 0;
        }
        private DeckRegularDict<MonopolyCardGameCardInformation> HouseCollection(DeckRegularDict<MonopolyCardGameCardInformation> whatCol)
        {
            DeckRegularDict<MonopolyCardGameCardInformation> output = new DeckRegularDict<MonopolyCardGameCardInformation>();
            if (whatCol.Any(items => items.WhatCard == EnumCardType.IsChance || items.WhatCard == EnumCardType.IsHouse) == false)
                return output; //because there are no houses or chance which is wild.
            MonopolyCardGameCardInformation thisCard;
            for (int x = 1; x <= 4; x++)
            {
                if (whatCol.Any(items => items.HouseNum == x) == false)
                {
                    if (!whatCol.Any(items => items.WhatCard == EnumCardType.IsChance))
                        return output;
                    thisCard = whatCol.First(items => items.WhatCard == EnumCardType.IsChance);
                }
                else
                    thisCard = whatCol.First(items => items.HouseNum == x);
                whatCol.RemoveSpecificItem(thisCard);
                output.Add(thisCard);
            }
            if (whatCol.Any(items => items.WhatCard == EnumCardType.IsHotel) == true)
                thisCard = whatCol.First(items => items.WhatCard == EnumCardType.IsHotel);
            else if (!whatCol.Any(items => items.WhatCard == EnumCardType.IsChance))
                return output;
            else
                thisCard = whatCol.First(items => items.WhatCard == EnumCardType.IsChance);
            whatCol.RemoveSpecificItem(thisCard);
            output.Add(thisCard);
            return output;
        }
        private DeckRegularDict<MonopolyCardGameCardInformation> MonopolyCol(DeckRegularDict<MonopolyCardGameCardInformation> whatCol, int whatGroup, EnumCardType whatType)
        {
            DeckRegularDict<MonopolyCardGameCardInformation> output = new DeckRegularDict<MonopolyCardGameCardInformation>();
            int numWilds = whatCol.Count(items => items.WhatCard == EnumCardType.IsChance);
            int howMany = whatCol.Count(items => items.WhatCard == whatType);
            if (howMany == 0 && whatGroup == 0)
                return output;
            var temps = whatCol.Where(items => items.WhatCard == whatType).ToRegularDeckDict();
            if (whatType == EnumCardType.IsUtilities)
            {
                if (howMany > 2)
                    throw new BasicBlankException("Can't have more than 2 utilities");
                if (howMany == 2 || howMany == 1 && numWilds > 0)
                {
                    output.AddRange(temps);
                    whatCol.RemoveGivenList(temps, NotifyCollectionChangedAction.Remove); //i think
                    if (howMany == 2)
                        return output;
                }
                if (numWilds == 0)
                    return output;
                var utilCard = whatCol.First(items => items.WhatCard == EnumCardType.IsChance);
                whatCol.RemoveSpecificItem(utilCard);
                output.Add(utilCard);
                return output;
            }
            if (whatType == EnumCardType.IsRailRoad)
            {
                if (howMany > 4)
                    throw new BasicBlankException("Can't have more than 4 railroads");
                if (howMany > 1 || howMany == 1 && numWilds > 0)
                {
                    if (numWilds == 0 && howMany == 1)
                        return output;
                    output.AddRange(temps);
                    whatCol.RemoveGivenList(temps, NotifyCollectionChangedAction.Remove); //i think
                }
                if (numWilds > 0) //can only use one wild for rail roads
                {
                    var tempCard = whatCol.First(items => items.WhatCard == EnumCardType.IsChance);
                    whatCol.RemoveSpecificItem(tempCard);
                    output.Add(tempCard);
                }
                return output;
            }
            temps = whatCol.Where(Items => Items.Group == whatGroup).ToRegularDeckDict();
            howMany = temps.Count;
            if (howMany == 0)
                return output; //i think we need at least one to even use a chance.
            int numberNeeded;
            if (whatGroup == 1 || whatGroup == 8)
                numberNeeded = 2;
            else
                numberNeeded = 3;
            if (howMany > numberNeeded)
                throw new BasicBlankException("Can't have more than the needed number");
            if (howMany == numberNeeded)
            {
                output.AddRange(temps);
                whatCol.RemoveGivenList(temps, NotifyCollectionChangedAction.Remove);
                return output;
            }
            if (numWilds + howMany < numberNeeded)
                return output;
            int wildsNeeded = numberNeeded - howMany;
            output.AddRange(temps);
            whatCol.RemoveGivenList(temps, NotifyCollectionChangedAction.Remove);
            temps = whatCol.Where(Items => Items.WhatCard == EnumCardType.IsChance).Take(wildsNeeded).ToRegularDeckDict();
            output.AddRange(temps);
            whatCol.RemoveGivenList(temps, NotifyCollectionChangedAction.Remove);
            return output;
        }
        private DeckRegularDict<MonopolyCardGameCardInformation> MonopolyColWild(DeckRegularDict<MonopolyCardGameCardInformation> whatCol, int whatGroup, EnumCardType whatType, bool useWild)
        {
            DeckRegularDict<MonopolyCardGameCardInformation> output = new DeckRegularDict<MonopolyCardGameCardInformation>();
            int numWilds = whatCol.Count(items => items.WhatCard == EnumCardType.IsChance);
            int howMany = whatCol.Count(items => items.WhatCard == whatType);
            if (howMany == 0 && whatGroup == 0)
                return output;
            var temps = whatCol.Where(items => items.WhatCard == whatType).ToRegularDeckDict();
            if (whatType == EnumCardType.IsUtilities)
            {
                if (howMany > 2)
                    throw new BasicBlankException("Can't have more than 2 utilities");
                if (howMany == 2 || howMany == 1 && numWilds > 0 && useWild == true)
                {
                    output.AddRange(temps);
                    whatCol.RemoveGivenList(temps, NotifyCollectionChangedAction.Remove); //i think
                    if (howMany == 2)
                        return output;
                }
                var utilCard = whatCol.First(items => items.WhatCard == EnumCardType.IsChance);
                whatCol.RemoveSpecificItem(utilCard);
                output.Add(utilCard);
                return output;
            }
            if (whatType == EnumCardType.IsRailRoad)
            {
                if (howMany > 4)
                    throw new BasicBlankException("Can't have more than 4 railroads");
                if (howMany > 1 || howMany == 1 && numWilds > 0 && useWild == true)
                {
                    output.AddRange(temps);
                    whatCol.RemoveGivenList(temps, NotifyCollectionChangedAction.Remove); //i think
                    if (numWilds == 0)
                        return output;
                }
                if (numWilds > 0 && useWild) //can only use one wild for rail roads
                {
                    var tempCard = whatCol.First(items => items.WhatCard == EnumCardType.IsChance);
                    whatCol.RemoveSpecificItem(tempCard);
                    output.Add(tempCard);
                }
                return output;
            }
            temps = whatCol.Where(Items => Items.Group == whatGroup).ToRegularDeckDict();
            howMany = temps.Count;
            if (howMany == 0)
                return output; //i think we need at least one to even use a chance.
            int numberNeeded;
            if (whatGroup == 1 || whatGroup == 8)
                numberNeeded = 2;
            else
                numberNeeded = 3;
            if (howMany > numberNeeded)
                throw new BasicBlankException("Can't have more than the needed number");
            if (howMany == numberNeeded)
            {
                output.AddRange(temps);
                whatCol.RemoveGivenList(temps, NotifyCollectionChangedAction.Remove);
                return output;
            }
            if (useWild == false)
                return output;
            if (numWilds + howMany < numberNeeded)
                return output;
            int wildsNeeded = numberNeeded - howMany;
            output.AddRange(temps);
            whatCol.RemoveGivenList(temps, NotifyCollectionChangedAction.Remove);
            temps = whatCol.Where(Items => Items.WhatCard == EnumCardType.IsChance).Take(wildsNeeded).ToRegularDeckDict();
            output.AddRange(temps);
            whatCol.RemoveGivenList(temps, NotifyCollectionChangedAction.Remove);
            return output;
        }
        private DeckRegularDict<MonopolyCardGameCardInformation> HouseCollectionWild(DeckRegularDict<MonopolyCardGameCardInformation> whatCol, bool makeWildHotel, bool useWildHouse)
        {
            DeckRegularDict<MonopolyCardGameCardInformation> output = new DeckRegularDict<MonopolyCardGameCardInformation>();
            if (whatCol.Any(items => items.WhatCard == EnumCardType.IsChance || items.WhatCard == EnumCardType.IsHouse) == false)
                return output; //because there are no houses or chance which is wild.
            MonopolyCardGameCardInformation thisCard;
            for (int x = 1; x <= 4; x++)
            {
                if (whatCol.Any(items => items.HouseNum == x) == false)
                {
                    if (!whatCol.Any(items => items.WhatCard == EnumCardType.IsChance) || useWildHouse == false)
                        return output;
                    thisCard = whatCol.First(items => items.WhatCard == EnumCardType.IsChance);
                }
                else
                    thisCard = whatCol.First(items => items.HouseNum == x);
                whatCol.RemoveSpecificItem(thisCard);
                output.Add(thisCard);
            }
            if (whatCol.Any(items => items.WhatCard == EnumCardType.IsHotel) == true)
                thisCard = whatCol.First(items => items.WhatCard == EnumCardType.IsHotel);
            else if (!whatCol.Any(items => items.WhatCard == EnumCardType.IsChance) || makeWildHotel == false)
                return output;
            else
                thisCard = whatCol.First(items => items.WhatCard == EnumCardType.IsChance);
            whatCol.RemoveSpecificItem(thisCard);
            output.Add(thisCard);
            return output;
        }
        private ListInfo PlaceTokens(CustomBasicList<ListInfo> whatGroup)
        {
            ListInfo output;
            decimal mostss = 0;
            output = new ListInfo();
            whatGroup.ForEach(thisList =>
            {
                decimal currentMoneys = CalculateMoneyFromGroup(thisList, 0);
                if (currentMoneys > mostss)
                {
                    output = thisList;
                    mostss = currentMoneys;
                }
            });
            return output;
        }
        private decimal CalculateMoneyFromGroup(ListInfo thisList, int numTokens) //looks like no ref needed because value does not change anyways
        {
            int output;
            if (thisList.WhatCard == EnumCardType.IsRailRoad)
            {
                if (thisList.RailRoads == 2)
                    output = 250;
                else if (thisList.RailRoads == 3)
                    output = 500;
                else
                    output = 1000;
            }
            else if (thisList.WhatCard == EnumCardType.IsUtilities)
                output = 500;
            else
            {
                int bases;
                if (thisList.Group == 8)
                    bases = 400;
                else if (thisList.Group == 7)
                    bases = 350;
                else if (thisList.Group == 6)
                    bases = 300;
                else if (thisList.Group == 5)
                    bases = 250;
                else if (thisList.Group == 4)
                    bases = 200;
                else if (thisList.Group == 3)
                    bases = 150;
                else if (thisList.Group == 2)
                    bases = 100;
                else if (thisList.Group == 1)
                    bases = 50;
                else
                    throw new BasicBlankException("Must be between 1 and 8");
                if (thisList.NumberOfHouses > 0)
                {
                    bases *= (thisList.NumberOfHouses + 1);
                    if (thisList.HasHotel)
                        bases += 500;
                }
                output = bases;
            }
            if (numTokens == 0)
                return output;
            output *= (numTokens + 1);
            return output;
        }
        private int MostMrs()
        {
            int mostss = 0;
            int output = 0;
            PlayerList!.ForEach(thisPlayer =>
            {
                var tempList = thisPlayer.MainHandList.ToRegularDeckDict();
                int manys = tempList.Count(items => items.WhatCard == EnumCardType.IsMr);
                if (manys == mostss)
                    output = 0;
                else if (manys > mostss)
                {
                    output = thisPlayer.Id;
                    mostss = manys;
                }

            });
            return output;
        }
        private decimal CalculateScore(int player, bool wentOut, out DeckRegularDict<MonopolyCardGameCardInformation> newGroup)
        {
            newGroup = new DeckRegularDict<MonopolyCardGameCardInformation>();
            SingleInfo = PlayerList![player];
            var tempCol = SingleInfo.MainHandList.ToRegularDeckDict();
            if (wentOut == false && tempCol.Any(items => items.WhatCard == EnumCardType.IsChance))
                return 0;//if you did not go out, you get 0 for having a chance in your hand.
            int hadMostMrs = MostMrs(); //i think even if a player did not go out, they could get the points for the most mr monopolies.
            decimal tempScore = 0;
            if (hadMostMrs == player)
            {
                tempScore += 1000;
                newGroup.AddRange(tempCol.Where(items => items.WhatCard == EnumCardType.IsMr));
            }
            DeckRegularDict<MonopolyCardGameCardInformation> thisCol = new DeckRegularDict<MonopolyCardGameCardInformation>();
            thisCol = tempCol.Where(items => items.WhatCard == EnumCardType.IsGo).ToRegularDeckDict();
            tempScore += (thisCol.Count * 200);
            newGroup.AddRange(thisCol);
            thisCol = tempCol.Where(items => items.WhatCard == EnumCardType.IsToken).ToRegularDeckDict();
            var tokenList = thisCol.ToRegularDeckDict();
            int tokens = thisCol.Count;
            ListInfo thisList;
            ListInfo places;
            CustomBasicList<ListInfo> listMons = new CustomBasicList<ListInfo>();
            DeckRegularDict<MonopolyCardGameCardInformation> mons;
            DeckRegularDict<MonopolyCardGameCardInformation> hou;
            var possList = tempCol.Where(items => items.Group > 0).GroupOrderDescending(items => items.Group).ToCustomBasicList();
            if (tempCol.Any(items => items.WhatCard == EnumCardType.IsChance) == false)
            {//processes for when there is no chance (easiest).
                mons = MonopolyCol(tempCol, 0, EnumCardType.IsRailRoad);
                if (mons.Count > 0)
                {
                    newGroup.AddRange(mons);
                    thisList = new ListInfo();
                    thisList.RailRoads = mons.Count;
                    thisList.WhatCard = EnumCardType.IsRailRoad;
                    thisList.ID = listMons.Count + 1;
                    listMons.Add(thisList);
                }
                mons = MonopolyCol(tempCol, 0, EnumCardType.IsUtilities);
                if (mons.Count > 0)
                {
                    newGroup.AddRange(mons);
                    thisList = new ListInfo();
                    thisList.WhatCard = EnumCardType.IsUtilities;
                    thisList.ID = listMons.Count + 1;
                    listMons.Add(thisList);
                }
                DeckRegularDict<MonopolyCardGameCardInformation> prList = new DeckRegularDict<MonopolyCardGameCardInformation>();
                possList.ForConditionalItems(items => items.Key > 0, thisPoss =>
                {
                    mons = MonopolyCol(tempCol, thisPoss.Key, EnumCardType.IsProperty);
                    if (mons.Count > 0)
                    {
                        prList.AddRange(mons);
                        thisList = new ListInfo();
                        thisList.Group = thisPoss.Key;
                        hou = HouseCollection(tempCol);
                        if (hou.Count > 0)
                            prList.AddRange(hou);
                        if (hou.Count == 5)
                        {
                            thisList.HasHotel = true;
                            thisList.NumberOfHouses = hou.Count - 1;
                        }
                        else
                        {
                            thisList.HasHotel = false;
                            thisList.NumberOfHouses = hou.Count;
                        }
                        thisList.ID = listMons.Count + 1;
                        listMons.Add(thisList);
                    }
                });
                newGroup.AddRange(prList);
                if (listMons.Count == 0)
                    return tempScore;
                newGroup.AddRange(tokenList);
                places = PlaceTokens(listMons);
                decimal temps;
                listMons.ForEach(thisItem =>
                {
                    if (thisItem.ID == places.ID)
                        temps = CalculateMoneyFromGroup(thisItem, tokens);
                    else
                        temps = CalculateMoneyFromGroup(thisItem, 0);
                    tempScore += temps;
                    if (thisItem.WhatCard == EnumCardType.IsUtilities && temps == 0)
                        throw new BasicBlankException("Utilities cannot worth 0 points");
                });
                return tempScore;
            }
            // Here is the new logic for the wildcards.  Given this table:
            // Number of houses
            // Group  Value   1       2       3       4       Hotel
            // 8      400     800     1200    1600    2000    2500
            // 7      350     700     1050    1400    1750    2250
            // 6      300     600     900     1200    1500    2000
            // 5      250     500     750     1000    1250    1750
            // 4      200     400     600     800     1000    1500
            // 3      150     300     450     600     750     1250
            // 2      100     200     300     400     500     1000
            // 1      50      100     150     200     250     750

            // 2 RR   250
            // 2 util 500
            // 3 RR   500
            // 4 RR   1000

            // Now the logic is as follows:  it is always better to be a token than an extra house because
            // for any value in the table, unless you have 2 or more token cards.  We set a variable
            // to only create wild houses when we have 2 or more token cards
            // The best way to see this is to assume that you have 1 token and 1 chance card and try the
            // values in the table to see how that affects the final score.
            // An example:  You have group 5 with 3 houses.  This is worth $1000.  A token card means you have
            // $2000 in value.  Now, with a wild card, you can either add a house, making it $1250 or $2500 with
            // token, or add an aditional token, making it $1000+$1000+$1000 or $3000, which is the better choice.
            // This holds true for the entire table, where being a token is always a break even or better move, with
            // the only exception being that if your group is 1, 2, or 3, and you have 4 houses, you would be better
            // off being a wild hotel card than a token card.  This is because the $500 value of the hotel is so
            // much greater than the base property value in the table.

            // Sometimes it is better to be a railroad or a utility though.   If the value given by a railroad or a utility
            // is greater than the value in the chart where you are then you should do this.  This means for two railroads
            // worth $250 you should always take it if you are group 3 with no houses, group 2 with 1 or no houses, or
            // group 1 with 0, 1, 2, or 3 houses.  The logic is similar for 3 railroads and 2 utilities, or 4 railroads,
            // which you can see in the case select code below.

            // First, we need to know how many of everything we have.  Not all these values a used, but they are taken
            // in case we discover a case where we need to use them
            int monoCount, monoWildCount, rrCount, utilCount, houseCount, hotelCount, wildCount, tokenCount, houseWildCount;
            DeckRegularDict<MonopolyCardGameCardInformation> propList = new DeckRegularDict<MonopolyCardGameCardInformation>();
            DeckRegularDict<MonopolyCardGameCardInformation> propCheckList;
            DeckRegularDict<MonopolyCardGameCardInformation> houseList;
            DeckRegularDict<MonopolyCardGameCardInformation> houseCheckList;
            CustomBasicList<int> searchPos = new CustomBasicList<int>();
            rrCount = 0;
            utilCount = 0;
            houseCount = 0;
            hotelCount = 0;
            wildCount = 0;
            tokenCount = 0;
            houseWildCount = 0;
            monoWildCount = 0;
            monoCount = 0;
            int i;
            thisCol = tempCol.Where(items => items.WhatCard == EnumCardType.IsProperty).ToRegularDeckDict();
            propList.AddRange(thisCol);
            possList = tempCol.Where(items => items.Group > 0).GroupOrderAscending(items => items.Group).ToCustomBasicList();
            possList.ForEach(thisPoss =>
            {
                propCheckList = MonopolyCol(propList, thisPoss.Key, EnumCardType.IsProperty);
                if (propCheckList.Count > 0)
                    monoCount += 1;
            });
            thisCol = tempCol.Where(items => items.WhatCard == EnumCardType.IsProperty).ToRegularDeckDict();
            propList.ReplaceRange(thisCol);
            thisCol = tempCol.Where(items => items.WhatCard == EnumCardType.IsChance).ToRegularDeckDict();
            propList.AddRange(thisCol);
            possList = SingleInfo.MainHandList.Where(items => items.Group > 0).GroupOrderAscending(items => items.Group).ToCustomBasicList();
            int numCards;
            foreach (var thisItem in possList)
            {
                propCheckList = MonopolyCol(propList, thisItem.Key, EnumCardType.IsProperty);
                if (propCheckList.Count > 0)
                {
                    monoWildCount++;
                    numCards = 0;
                    if (thisItem.Key != 0)
                    {
                        var loopTo1 = propCheckList.Count;
                        for (i = 1; i <= loopTo1; i++)
                        {
                            if (propCheckList[i - 1].WhatCard != EnumCardType.IsChance)
                                numCards++;
                        }
                        searchPos.Add(thisItem.Key);
                        searchPos.Add(propCheckList.Count - numCards);
                    }
                }
            };

            tokenCount = tempCol.Count(items => items.WhatCard == EnumCardType.IsToken);
            rrCount = tempCol.Count(items => items.WhatCard == EnumCardType.IsRailRoad);
            utilCount = tempCol.Count(items => items.WhatCard == EnumCardType.IsUtilities);
            hotelCount = tempCol.Count(items => items.WhatCard == EnumCardType.IsHotel);
            wildCount = tempCol.Count(items => items.WhatCard == EnumCardType.IsChance);
            thisCol = tempCol.Where(items => items.WhatCard == EnumCardType.IsHouse).ToRegularDeckDict();
            houseList = new DeckRegularDict<MonopolyCardGameCardInformation>();
            houseList.AddRange(thisCol);
            houseCheckList = HouseCollection(houseList);
            thisCol = tempCol.Where(items => items.WhatCard == EnumCardType.IsHouse || items.WhatCard == EnumCardType.IsChance).ToRegularDeckDict();
            houseList.ReplaceRange(thisCol);
            if (houseList.Count > 0)
            {
                houseCheckList = HouseCollection(houseList);
                houseWildCount = houseCheckList.Count;
            }
            int monGroup;
            bool wildRail = false;
            bool wildUtil = false;
            bool wildHouse = false;
            bool wildHotel = false;
            bool wildProp = false;
            bool largeProp = false;
            if (monoWildCount > monoCount) // This loop is to check all the conditions and see what is best to make wild
                wildProp = true;
            possList = tempCol.Where(items => items.Group > 0).GroupOrderDescending(items => items.Group).ToCustomBasicList();
            possList.ForEach(thisItem =>
            {
                monGroup = thisItem.Key;
                switch (monGroup)// Here is the logic based on the table above
                {
                    case 8:
                    case 7:
                        {
                            if ((rrCount == 2) & (houseCount == 0))
                                wildRail = true;
                            if ((rrCount == 3) & (houseCount < 2))
                                wildRail = true;
                            if (((houseCount >= 2) & (rrCount == 3)) | ((houseCount > 0) & (rrCount == 2)))
                                largeProp = true;
                            if ((utilCount == 1) & (houseCount == 0))
                                wildUtil = true;
                            break;
                        }

                    case 6:
                    case 5:
                        {
                            if ((rrCount == 2) & (houseCount == 0))
                                wildRail = true;
                            if ((rrCount == 3) & (houseCount < 3))
                                wildRail = true;
                            if (((houseCount >= 3) & (rrCount == 3)) | ((houseCount > 0) & (rrCount == 2)))
                                largeProp = true;
                            if ((utilCount == 1) & (houseCount == 0))
                                wildUtil = true;
                            break;
                        }

                    case 4:
                        {
                            if ((rrCount == 2) & (houseCount < 2))
                                wildRail = true;
                            if ((rrCount == 3) & (houseCount < 4))
                                wildRail = true;
                            if (((houseCount >= 4) & (rrCount == 3)) | ((houseCount >= 2) & (rrCount == 2)))
                                largeProp = true;
                            if ((utilCount == 1) & (houseCount < 2))
                                wildUtil = true;
                            if (houseCount == 4)
                                wildHotel = true;
                            break;
                        }

                    case 3:
                        {
                            if ((rrCount == 1) & (houseCount == 0))
                                wildRail = true;
                            if ((rrCount == 2) & (houseCount < 3))
                                wildRail = true;
                            if ((rrCount == 3) & (houseCount < 5))
                                wildRail = true;
                            if (((houseCount >= 5) & (rrCount == 3)) | ((houseCount >= 3) & (rrCount == 2)) | ((houseCount == 0) & (rrCount == 1)))
                                largeProp = true;
                            if ((utilCount == 1) & (houseCount < 3))
                                wildUtil = true;
                            if (houseCount == 4)
                                wildHotel = true;
                            break;
                        }

                    case 2:
                        {
                            if ((rrCount == 1) & (houseCount < 2))
                                wildRail = true;
                            if ((rrCount == 2) & (houseCount < 4))
                                wildRail = true;
                            if ((rrCount == 3) & (houseCount < 5))
                                wildRail = true;
                            if (((houseCount >= 5) & (rrCount == 3)) | ((houseCount >= 4) & (rrCount == 2)) | ((houseCount >= 2) & (rrCount == 1)))
                                largeProp = true;
                            if ((utilCount == 1) & (houseCount < 4))
                                wildUtil = true;
                            if (houseCount == 4)
                                wildHotel = true;
                            break;
                        }

                    case 1:
                        {
                            if ((rrCount == 1) & (houseCount < 4))
                                wildRail = true;
                            if ((rrCount == 2) & (houseCount < 5))
                                wildRail = true;
                            if ((rrCount == 3) & (houseCount < 5))
                                wildRail = true;
                            if (((houseCount >= 5) & (rrCount == 3)) | ((houseCount >= 5) & (rrCount == 2)) | ((houseCount >= 4) & (rrCount == 1)))
                                largeProp = true;
                            if ((utilCount == 1) & (houseCount < 5))
                                wildUtil = true;
                            if (houseCount == 4)
                                wildHotel = true;
                            break;
                        }
                }
            });
            // now we've completed our first pass and can start using the wild cards
            // if we need a wild railroad card, use it first
            if (largeProp)
            {
                wildRail = false;
                wildUtil = false;
            }
            if ((wildRail))
            {
                mons = MonopolyCol(tempCol, 0, EnumCardType.IsRailRoad);
                if (mons.Count > 0)
                {
                    newGroup.AddRange(mons);
                    thisList = new ListInfo();
                    thisList.RailRoads = mons.Count;
                    thisList.WhatCard = EnumCardType.IsRailRoad;
                    thisList.ID = listMons.Count + 1;
                    listMons.Add(thisList);
                }
            }
            // if we need a wild utility card, use it next
            if ((wildUtil))
            {
                mons = MonopolyCol(tempCol, 0, EnumCardType.IsUtilities);
                if (mons.Count > 0)
                {
                    newGroup.AddRange(mons);
                    thisList = new ListInfo();
                    thisList.WhatCard = EnumCardType.IsUtilities;
                    thisList.ID = listMons.Count + 1;
                    listMons.Add(thisList);
                }
            }

            var loopTo2 = searchPos.Count;
            for (var x = 1; x <= loopTo2; x += 2)
            {
                mons = MonopolyColWild(tempCol, searchPos[x - 1], EnumCardType.IsProperty, wildProp);
                if (mons.Count > 0)
                {
                    foreach (var ThisCard in mons)
                        newGroup.Add(ThisCard);
                    thisList = new ListInfo();
                    thisList.Group = searchPos[x - 1]; // because 0 based
                    if (thisList.Group == 0)
                        throw new BasicBlankException("The group cannot be 0 for properties");
                    // call new function that will handle wild houses or wild hotels
                    hou = HouseCollectionWild(tempCol, wildHotel, wildHouse);
                    if (hou.Count == 5)
                    {
                        thisList.HasHotel = true;
                        thisList.NumberOfHouses = 4;
                    }
                    else
                    {
                        thisList.HasHotel = false;
                        thisList.NumberOfHouses = hou.Count;
                    }
                    thisList.ID = listMons.Count + 1;
                    if (hou.Count > 0)
                    {
                        foreach (var ThisCard in hou)
                            newGroup.Add(ThisCard);

                    }
                    listMons.Add(thisList);
                }
            }
            // okay, at this point, any wild cards left over will be tokens
            if (listMons.Count == 0)
            {
                return tempScore;
            }
            foreach (var ThisCard in tokenList)
                newGroup.Add(ThisCard);

            places = PlaceTokens(listMons);
            thisCol = tempCol.Where(items => items.WhatCard == EnumCardType.IsChance).ToRegularDeckDict();
            int manys = thisCol.Count;
            newGroup.AddRange(thisCol);
            foreach (var TempCard in thisCol)
                tempCol.RemoveSpecificItem(TempCard);
            // if we have utilities or railroads without wild cards, we need to add them in the score now
            // if we already did them with wild cards, we won't find any left
            mons = MonopolyCol(tempCol, 0, EnumCardType.IsUtilities);
            if (mons.Count > 0)
            {
                newGroup.AddRange(mons);
                thisList = new ListInfo();
                thisList.WhatCard = EnumCardType.IsUtilities;
                thisList.ID = listMons.Count + 1;
                listMons.Add(thisList);
            }
            mons = MonopolyCol(tempCol, 0, EnumCardType.IsRailRoad);
            if (mons.Count > 0)
            {
                newGroup.AddRange(mons);
                thisList = new ListInfo();
                thisList.WhatCard = EnumCardType.IsRailRoad;
                thisList.ID = listMons.Count + 1;
                listMons.Add(thisList);
            }
            tokens += manys;
            foreach (var ThisList in listMons)
            {
                if (ThisList.ID == places.ID)
                    tempScore += CalculateMoneyFromGroup(ThisList, tokens);
                else
                {
                    var argNumTokens = 0;
                    tempScore += CalculateMoneyFromGroup(ThisList, argNumTokens);
                }
            }
            return tempScore;
        }
        #endregion

    }
}
