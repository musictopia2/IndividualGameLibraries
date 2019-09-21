using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.RummyClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace DummyRummyCP
{
    [SingletonGame]
    public class DummyRummyMainGameClass : CardGameClass<RegularRummyCard, DummyRummyPlayerItem, DummyRummySaveInfo>, IMiscDataNM, IStartNewGame
    {
        public DummyRummyMainGameClass(IGamePackageResolver container) : base(container) { }
        private RummyProcesses<EnumSuitList, EnumColorList, RegularRummyCard>? _rummys;
        private DummyRummyViewModel? _thisMod;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            _thisMod = MainContainer.Resolve<DummyRummyViewModel>();
        }
        public override Task PopulateSaveRootAsync()
        {
            SaveRoot!.SetList = _thisMod!.MainSets!.SavedSets();
            DummyRummyPlayerItem self = PlayerList!.GetSelf();
            self.AdditionalCards = _thisMod.TempSets!.ListAllObjects();
            return base.PopulateSaveRootAsync();
        }
        public override Task FinishGetSavedAsync()
        {
            _thisMod!.MainSets!.ClearBoard();
            LoadControls();
            int x = SaveRoot!.SetList.Count; //the first time, actually load manually.
            x.Times(items =>
            {
                DummySet thisSet = new DummySet(_thisMod);
                _thisMod.MainSets.CreateNewSet(thisSet);
            });
            PlayerList!.ForEach(thisPlayer =>
            {
                if (thisPlayer.AdditionalCards.Count > 0)
                {
                    thisPlayer.MainHandList.AddRange(thisPlayer.AdditionalCards); //later sorts anyways.
                    thisPlayer.AdditionalCards.Clear(); //i think.
                }
            });
            SingleInfo = PlayerList.GetSelf(); //hopefully won't cause problems.
            SortCards(); //has to be this way this time.
            ThisE.Subscribe(SingleInfo); //i think
            _thisMod.MainSets.LoadSets(SaveRoot.SetList);
            SaveRoot.LoadMod(_thisMod);
            return base.FinishGetSavedAsync();
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            _rummys = new RummyProcesses<EnumSuitList, EnumColorList, RegularRummyCard>();
            _rummys.HasSecond = false;
            _rummys.HasWild = false;
            _rummys.NeedMatch = true;
            _rummys.LowNumber = 1;
            _rummys.HighNumber = 13;
            IsLoaded = true; //i think needs to be here.
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            if (SaveRoot!.UpTo == 0)
                SaveRoot.UpTo = 3;
            else
                SaveRoot.UpTo++;
            SaveRoot.LoadMod(_thisMod!); //if it happens more than once, its okay.
            ResetCurrentPoints();
            SaveRoot.PlayerWentOut = 0;
            SaveRoot.PointsObtained = 0;
            AlreadyDrew = false; //i think i forgot this part.
            SaveRoot.SetsCreated = false;
            return base.StartSetUpAsync(isBeginning);
        }
        protected override Task LastPartOfSetUpBeforeBindingsAsync()
        {
            if (IsLoaded == false)
            {
                LoadControls();
                SingleInfo = PlayerList!.GetSelf();
                ThisE.Subscribe(SingleInfo);
            }
            _thisMod!.MainSets!.ClearBoard();
            SaveRoot!.SetList.Clear(); //i think this too.
            _thisMod.TempSets!.ClearBoard(); //i think this happens before sending data to other players.
            return base.LastPartOfSetUpBeforeBindingsAsync();
        }
        internal int CardsToPassOut
        {
            get
            {
                return SaveRoot!.UpTo; //i think.
            }
        }
        private void RemoveCard(int Deck)
        {
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Computer)
                return; //i think computer player would have already removed their card.
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
            {
                if (_thisMod!.TempSets!.HasObject(Deck))
                {
                    _thisMod.TempSets.RemoveObject(Deck);
                    return;
                }
            }
            SingleInfo.MainHandList.RemoveObjectByDeck(Deck);
        }
        private DeckRegularDict<RegularRummyCard> PlayerHand()
        {
            var output = SingleInfo!.MainHandList.ToRegularDeckDict();
            if (SingleInfo.PlayerCategory != EnumPlayerCategory.Self)
                return output;
            output.AddRange(_thisMod!.TempSets!.ListAllObjects());
            return output;
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "finishedsets":
                    await CreateSetsAsync(content);
                    await FinishedSetsAsync();
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
                var thisCol = await thisFirst.GetObjectsFromDataAsync(SingleInfo!.MainHandList);
                CreateSet(thisCol);
            }
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            SaveRoot!.SetsCreated = false;
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task EndTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            UnselectCards();
            if (SaveRoot!.PlayerWentOut > 0 && SaveRoot.PlayerWentOut != WhoTurn)
            {
                var tempList = PlayerHand();
                CalculatePoints(tempList);
                _thisMod!.TempSets!.ClearBoard(); //somehow this is how it has to be.
            }
            _thisMod!.CommandContainer!.ManuelFinish = true; //because it could be somebody else's turn.
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            if (WhoTurn == SaveRoot.PlayerWentOut)
            {
                await EndRoundAsync();
                return;
            }
            await StartNewTurnAsync();
        }
        private void CalculatePoints(IDeckDict<RegularRummyCard> thisCol)
        {
            thisCol.ForEach(thisCard =>
            {
                if (thisCard.Value >= EnumCardValueList.Ten)
                    SaveRoot!.PointsObtained += 10;
                else
                    SaveRoot!.PointsObtained += (int)thisCard.Value;
            });
        }
        private bool HasEnough
        {
            get
            {
                int cardsLeft = _thisMod!.Deck1!.CardsLeft();
                return cardsLeft >= PlayerList.Count(); //has to show whether there is enough for all players to draw again.
            }
        }
        private int PointsEarned => SaveRoot!.PointsObtained + 25;
        private void UnselectCards()
        {
            if (SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
                return;
            _thisMod!.PlayerHand1!.EndTurn();
            _thisMod.TempSets!.EndTurn();
        }
        public IDeckDict<RegularRummyCard> WhatSet(int whichOne)
        {
            return _thisMod!.TempSets!.ObjectList(whichOne);
        }
        public override async Task DiscardAsync(RegularRummyCard thisCard)
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            RemoveCard(thisCard.Deck);
            await AnimatePlayAsync(thisCard);
            UnselectCards();
            if (SaveRoot!.PlayerWentOut == 0 && SingleInfo.ObjectCount == 0)
                SaveRoot.PlayerWentOut = WhoTurn;
            if (SaveRoot.PlayerWentOut > 0)
            {
                await EndTurnAsync();
                return;
            }
            if (HasEnough == false)
            {
                await WillEndRoundAsync();
                return;
            }
            await EndTurnAsync();
        }
        private async Task WillEndRoundAsync()
        {
            ResetCurrentPoints();
            await FinishEndAsync();
        }
        private async Task FinishEndAsync()
        {
            if (SaveRoot!.UpTo == 13)
            {
                SingleInfo = PlayerList.OrderByDescending(items => items.TotalScore).First();
                await ShowWinAsync();
                return;
            }
            this.RoundOverNext();
        }
        public override async Task EndRoundAsync()
        {
            ResetCurrentPoints();
            int pointss = PointsEarned;
            var thisPlayer = PlayerList![SaveRoot!.PlayerWentOut];
            thisPlayer.CurrentScore = pointss;
            thisPlayer.TotalScore += pointss;
            await FinishEndAsync();
        }
        public async Task FinishedSetsAsync()
        {
            SaveRoot!.SetsCreated = true;
            await ContinueTurnAsync(); //i think this simple.
        }
        public void CreateSet(IDeckDict<RegularRummyCard> thisCol)
        {
            DummySet thisSet = new DummySet(_thisMod!);
            thisCol.UnhighlightObjects();
            thisSet.HandList.ReplaceRange(thisCol);
            _thisMod!.MainSets!.CreateNewSet(thisSet);
        }
        private void ResetCurrentPoints()
        {
            PlayerList!.ForEach(thisPlayer => thisPlayer.CurrentScore = 0);
        }
        public Task ResetAsync()
        {
            PlayerList!.ForEach(thisPlayer => thisPlayer.TotalScore = 0);
            ResetCurrentPoints(); //just in case.
            SaveRoot!.UpTo = 0;
            return Task.CompletedTask;
        }
        private bool HasSet(IDeckDict<RegularRummyCard> thisCol)
        {
            if (thisCol.Count < 3)
                return false; //has to have at least 3 cards for a set.
            if (thisCol.DistinctCount(items => items.Value) == 1)
                return true;
            return _rummys!.IsNewRummy(thisCol, thisCol.Count, RummyProcesses<EnumSuitList, EnumColorList, RegularRummyCard>.EnumRummyType.Runs);
        }
        public bool CanProcessDiscard(out bool PickUp, out int Index, out int Deck, out string Message)
        {
            Message = "";
            Index = 0;
            Deck = 0;
            if (AlreadyDrew == false && PreviousCard == 0)
                PickUp = true;
            else
                PickUp = false;
            if (PickUp == true)
            {

                if (_thisMod!.PlayerHand1!.HowManySelectedObjects > 0 || _thisMod.TempSets!.HowManySelectedObjects > 0)
                {
                    Message = "There is at least one card selected.  Unselect all the cards before picking up from discard";
                    return false;
                }
                return true;
            }
            var thisCol = _thisMod!.PlayerHand1!.ListSelectedObjects();
            var otherCol = _thisMod.TempSets!.ListSelectedObjects();
            if (thisCol.Count == 0 && otherCol.Count == 0)
            {
                Message = "Sorry, you must select a card to discard";
                return false;
            }
            if (thisCol.Count + otherCol.Count > 1)
            {
                Message = "Sorry, you can only select one card to discard";
                return false;
            }

            if (thisCol.Count == 0)
            {
                Index = _thisMod.TempSets.PileForSelectedObject;
                Deck = _thisMod.TempSets.DeckForSelectedObjected(Index);
            }
            else
                Deck = _thisMod.PlayerHand1.ObjectSelected();
            var thisCard = DeckList!.GetSpecificItem(Deck);

            if (thisCard.Deck == PreviousCard && SingleInfo!.ObjectCount > 1)
            {
                Message = "Sorry, you cannot discard the one that was picked up since there is more than one card to choose from";
                return false;
            }
            return true;
        }
        public CustomBasicList<TempInfo> ListValidSets()
        {
            CustomBasicList<TempInfo> output = new CustomBasicList<TempInfo>();
            DeckRegularDict<RegularRummyCard> thisCollection;
            IDeckDict<RegularRummyCard> tempCollection; //hopefully still works (?)
            TempInfo thisTemp;
            for (int x = 1; x <= 6; x++)
            {
                tempCollection = WhatSet(x);
                thisCollection = new DeckRegularDict<RegularRummyCard>();
                if (tempCollection.Count > 0)
                    thisCollection.AddRange(tempCollection);
                if (thisCollection.Count > 0)
                {
                    bool rets = HasSet(thisCollection);
                    if (rets == true)
                    {
                        thisTemp = new TempInfo();
                        thisTemp.SetNumber = x;
                        thisTemp.CardList = thisCollection;
                        output.Add(thisTemp);
                    }
                }
            }
            return output;
        }
        public async Task<bool> CanLaterLayDownAsync()
        {
            if (SingleInfo!.MainHandList.Count == 0)
            {
                await _thisMod!.ShowGameMessageAsync("Must have one card to discard");
                return false;
            }
            return true;
        }
        public bool HasInitialSet()
        {
            if (SingleInfo!.MainHandList.Count != 1)
                return false; //because must have one card to discard no matter what.
            IDeckDict<RegularRummyCard> tempCollection; //hopefully still works (?)
            DeckRegularDict<RegularRummyCard> thisCollection;
            for (int x = 1; x <= 6; x++)
            {
                tempCollection = WhatSet(x);
                thisCollection = new DeckRegularDict<RegularRummyCard>();
                if (tempCollection.Count > 0)
                    thisCollection.AddRange(tempCollection);
                if (thisCollection.Count > 0)
                {
                    bool rets = HasSet(thisCollection);
                    if (rets == false)
                        return false;
                }
            }
            return true;
        }
    }
}