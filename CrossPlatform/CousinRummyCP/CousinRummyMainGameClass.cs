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
namespace CousinRummyCP
{
    [SingletonGame]
    public class CousinRummyMainGameClass : CardGameClass<RegularRummyCard, CousinRummyPlayerItem, CousinRummySaveInfo>, IMiscDataNM, IStartNewGame
    {
        internal CustomBasicList<SetList>? SetsList;
        private RummyProcesses<EnumSuitList, EnumColorList, RegularRummyCard>? _rummys;
        public int OtherTurn
        {
            get
            {
                return SaveRoot!.PlayOrder.OtherTurn;
            }
            set
            {
                SaveRoot!.PlayOrder.OtherTurn = value;
            }

        }
        public CousinRummyMainGameClass(IGamePackageResolver container) : base(container) { }
        private CousinRummyViewModel? _thisMod;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            _thisMod = MainContainer.Resolve<CousinRummyViewModel>();
        }
        private RegularRummyCard ManuallyChooseCard
        {
            get
            {
                var tempList = DeckList.Where(items => items.IsObjectWild == false).ToRegularDeckDict();
                do
                {
                    var thisCard = tempList.GetRandomItem();
                    if (_thisMod!.Deck1!.CardExists(thisCard.Deck))
                        return thisCard;
                } while (true);
            }
        }
        public void ModifyCards(CustomBasicList<RegularRummyCard> thisList)
        {
            thisList.ForEach(thisCard =>
            {
                if (thisCard.Value == EnumCardValueList.Two || thisCard.Value == EnumCardValueList.HighAce)
                {
                    thisCard.Points = 20;
                }
                else if (thisCard.Value == EnumCardValueList.Joker)
                    thisCard.Points = 50;
                else if (thisCard.Value >= EnumCardValueList.Nine)
                    thisCard.Points = 10;
                else
                    thisCard.Points = 5;
            });

        }
        public override Task FinishGetSavedAsync()
        {
            _thisMod!.MainSets!.ClearBoard();
            LoadControls();
            int x = SaveRoot!.SetList.Count; //the first time, actually load manually.
            x.Times(Items =>
            {
                PhaseSet ThisSet = new PhaseSet(_thisMod);
                _thisMod.MainSets.CreateNewSet(ThisSet);
            });
            PlayerList!.ForEach(ThisPlayer =>
            {
                if (ThisPlayer.AdditionalCards.Count > 0)
                {
                    ThisPlayer.MainHandList.AddRange(ThisPlayer.AdditionalCards); //later sorts anyways.
                    ThisPlayer.AdditionalCards.Clear(); //i think.
                }
            });
            SingleInfo = PlayerList.GetSelf(); //hopefully won't cause problems.
            SortCards(); //has to be this way this time.
            ThisE.Subscribe(SingleInfo); //i think
            _thisMod.MainSets.LoadSets(SaveRoot.SetList);
            return base.FinishGetSavedAsync();
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            CreateSets();
            IsLoaded = true; //i think needs to be here.
        }
        private void CreateSets()
        {
            _rummys = new RummyProcesses<EnumSuitList, EnumColorList, RegularRummyCard>();
            _rummys.HasSecond = false;
            _rummys.HasWild = true;
            _rummys.LowNumber = 3;
            _rummys.HighNumber = 14;
            SetsList = new CustomBasicList<SetList>();
            SetList firstSet = new SetList();
            firstSet.Description = "1 Set Of 3";
            AddSets(firstSet, true, 3);
            firstSet = new SetList();
            firstSet.Description = "2 Sets Of 3";
            AddSets(firstSet, false, 3);
            firstSet = new SetList();
            firstSet.Description = "1 Set Of 4";
            AddSets(firstSet, true, 4);
            firstSet = new SetList();
            firstSet.Description = "2 Sets Of 4";
            AddSets(firstSet, false, 4);
            firstSet = new SetList();
            firstSet.Description = "1 Set Of 5";
            AddSets(firstSet, true, 5);
            firstSet = new SetList();
            firstSet.Description = "2 Sets Of 5";
            AddSets(firstSet, false, 5);
            firstSet = new SetList();
            firstSet.Description = "1 Set Of 6";
            AddSets(firstSet, true, 6);
            firstSet = new SetList();
            firstSet.Description = "2 Sets Of 6";
            AddSets(firstSet, false, 6);
        }
        private void AddSets(SetList firstSet, bool oneOnly, int howMany)
        {
            firstSet.PhaseSets.Add(new SetInfo { HowMany = howMany });
            if (!oneOnly)
                firstSet.PhaseSets.Add(new SetInfo { HowMany = howMany });
            SetsList!.Add(firstSet);
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            SaveRoot!.Round++;
            PlayerList!.ForEach(thisPlayer =>
            {
                //
                if (isBeginning == true)
                    thisPlayer.TokensLeft = 10;
                thisPlayer.LaidDown = false;
            });
            SaveRoot.ImmediatelyStartTurn = true;
            SaveRoot.WhoDiscarded = 0;
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
            var tempCard = ManuallyChooseCard;
            _thisMod.Deck1!.ManuallyRemoveSpecificCard(tempCard);
            _thisMod.Pile1!.AddCard(tempCard);
            return base.LastPartOfSetUpBeforeBindingsAsync();
        }
        public override Task PopulateSaveRootAsync()
        {
            SaveRoot!.SetList = _thisMod!.MainSets!.SavedSets();
            CousinRummyPlayerItem Self = PlayerList!.GetSelf();
            Self.AdditionalCards = _thisMod.TempSets!.ListAllObjects();
            return base.PopulateSaveRootAsync();
        }
        private void RemoveCard(int deck)
        {
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Computer)
                return; //i think computer player would have already removed their card.
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
            {
                if (_thisMod!.TempSets!.HasObject(deck))
                {
                    _thisMod.TempSets.RemoveObject(deck);
                    return;
                }
            }
            SingleInfo.MainHandList.RemoveObjectByDeck(deck);
        }
        protected override async Task ComputerTurnAsync()
        {
            if (OtherTurn > 0)
            {
                await PassAsync();
                return;
            }
            await DiscardAsync(SingleInfo!.MainHandList.GetRandomItem(true).Deck); //hopefully is not forced to remove that card from hand (?)
        }
        public async Task PassAsync()
        {
            OtherTurn = await PlayerList!.CalculateOtherTurnAsync(false);
            if (OtherTurn == 0)
            {
                SingleInfo = PlayerList.GetWhoPlayer();
                await DrawAsync();
                return;
            }
            SingleInfo = PlayerList.GetOtherPlayer();
            await ProcessOtherTurnAsync();
        }
        private async Task ProcessOtherTurnAsync()
        {
            if (SingleInfo!.TokensLeft == 0 || SingleInfo.LaidDown == true || OtherTurn == SaveRoot!.WhoDiscarded)
            {
                await PassAsync(); //they have to pass automatically because they have no tokens left or they laid down. or you discarded.
                return;
            }
            await ContinueTurnAsync();
        }
        public override async Task ContinueTurnAsync()
        {
            if (OtherTurn > 0)
            {
                SingleInfo = PlayerList!.GetOtherPlayer();
                _thisMod!.OtherLabel = SingleInfo.NickName;
            }
            else
            {
                SingleInfo = PlayerList!.GetWhoPlayer();
                _thisMod!.OtherLabel = "None";
            }
            _thisMod.PhaseData = SetsList![SaveRoot!.Round - 1].Description;
            await base.ContinueTurnAsync();
        }
        protected override void GetPlayerToContinueTurn()
        {
            if (OtherTurn == 0)
            {
                base.GetPlayerToContinueTurn();
                return;
            }
            SingleInfo = PlayerList!.GetOtherPlayer();
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "laiddowninitial":
                    await CreateSetsAsync(content);
                    await LaidDownInitialSetsAsync();
                    return;
                case "pass":
                    await PassAsync();
                    return;
                case "laydownothers":
                    await CreateSetsAsync(content);
                    await LayDownOtherSetsAsync();
                    return;
                case "expandrummy":
                    SendExpandedSet thiss = await js.DeserializeObjectAsync<SendExpandedSet>(content);
                    await AddToSetAsync(thiss.Number, thiss.Deck);
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
                var thisCol = await thisFirst.GetObjectsFromDataAsync<RegularRummyCard>(SingleInfo!.MainHandList);
                TempInfo thisTemp = new TempInfo();
                thisTemp.CardList = thisCol;
                CreateNewSet(thisTemp);
            }
        }
        Task IStartNewGame.ResetAsync()
        {
            SaveRoot!.Round = 0; //i think
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.CurrentScore = 0;
                thisPlayer.TotalScore = 0;
                thisPlayer.TokensLeft = 10; //just in case.
            });
            return Task.CompletedTask;
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            OtherTurn = WhoTurn;
            await ProcessOtherTurnAsync();
        }
        public override async Task EndTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            UnselectCards();
            _thisMod!.CommandContainer!.ManuelFinish = true; //because it could be somebody else's turn.
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
        }
        private void UnselectCards()
        {
            if (SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
                return;
            _thisMod!.PlayerHand1!.EndTurn();
            _thisMod!.TempSets!.EndTurn();
        }
        public IDeckDict<RegularRummyCard> WhatSet(int whichOne)
        {
            return _thisMod!.TempSets!.ObjectList(whichOne);
        }
        public override async Task EndRoundAsync()
        {
            UnselectCards();
            SingleInfo = PlayerList!.GetSelf();
            var TempCol = _thisMod!.TempSets!.ListObjectsRemoved();
            SingleInfo.MainHandList.AddRange(TempCol);
            SortCards();
            PlayerList.ForEach(tempPlayer =>
            {
                ModifyCards(tempPlayer.MainHandList); //most likely the old version could had a bug.
                int points = CalculatePoints(tempPlayer);
                tempPlayer.CurrentScore = points;
                tempPlayer.TotalScore += points;
            });
            if (SaveRoot!.Round == 8)
            {
                SingleInfo = PlayerList.OrderByDescending(items => items.TotalScore).First();
                await ShowWinAsync();
                return;
            }
            this.RoundOverNext();
        }
        private int CalculatePoints(CousinRummyPlayerItem thisPlayer)
        {
            int plusPoints = _thisMod!.MainSets!.SetList.Sum(items => items.PointsReceived(thisPlayer.Id));
            int minusPoints = thisPlayer.MainHandList.Sum(items => items.Points);
            return plusPoints - minusPoints;
        }
        public override async Task DiscardAsync(RegularRummyCard thisCard)
        {
            RemoveCard(thisCard.Deck);
            await AnimatePlayAsync(thisCard);
            SaveRoot!.WhoDiscarded = WhoTurn;
            if (SingleInfo!.ObjectCount == 0) //going to try to trust the object count.
            {
                await EndRoundAsync();
                return;
            }
            await EndTurnAsync();
        }
        private void ResetSuccess()
        {
            SetsList!.ForEach(thisPhase =>
            {
                thisPhase.PhaseSets.ForEach(ThisSet => ThisSet.DidSucceed = false);
            });
        }
        protected override Task PlayerChosenForPickingUpFromDiscardAsync()
        {
            SingleInfo = PlayerList!.GetOtherPlayer();
            return Task.CompletedTask;
        }
        protected override async Task AfterPickupFromDiscardAsync()
        {
            PlayerDraws = OtherTurn;
            LeftToDraw = 3;
            await DrawAsync();
        }
        protected override async Task AfterDrawingAsync()
        {
            if (OtherTurn == 0)
            {
                await base.AfterDrawingAsync();
                return;
            }
            SingleInfo = PlayerList!.GetOtherPlayer();
            SingleInfo.TokensLeft--;
            if (OtherTurn != WhoTurn)
            {
                OtherTurn = 0;
                SingleInfo = PlayerList.GetWhoPlayer();
                LeftToDraw = 1;
                PlayerDraws = WhoTurn;
                await DrawAsync();
                return;
            }
            OtherTurn = 0;
            await base.AfterDrawingAsync();
        }

        public CustomBasicList<TempInfo> ListValidSets(bool needsInitial)
        {
            CustomBasicList<TempInfo> output = new CustomBasicList<TempInfo>();
            SetList thisSet = SetsList![SaveRoot!.Round - 1];

            DeckRegularDict<RegularRummyCard> thisCollection;
            IDeckDict<RegularRummyCard> tempCollection; //hopefully still works (?)
            TempInfo thisTemp;
            for (int x = 1; x <= _thisMod!.TempSets!.HowManySets; x++)
            {
                tempCollection = WhatSet(x);
                thisCollection = new DeckRegularDict<RegularRummyCard>();

                if (tempCollection.Count > 0)
                    thisCollection.AddRange(tempCollection);
                if (thisCollection.Count(items => items.IsObjectWild == false) >= 2)
                {
                    if (needsInitial == true)
                    {
                        foreach (var newSet in thisSet.PhaseSets)
                        {
                            if (newSet.DidSucceed == false)
                            {
                                newSet.DidSucceed = _rummys!.IsNewRummy(thisCollection, newSet.HowMany, RummyProcesses<EnumSuitList, EnumColorList, RegularRummyCard>.EnumRummyType.Sets);
                                if (newSet.DidSucceed == true)
                                {
                                    thisTemp = new TempInfo();
                                    thisTemp.CardList = thisCollection;
                                    output.Add(thisTemp);
                                    _thisMod.TempSets.ClearBoard(x);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (_rummys!.IsNewRummy(thisCollection, 3, RummyProcesses<EnumSuitList, EnumColorList, RegularRummyCard>.EnumRummyType.Sets))
                        {
                            thisTemp = new TempInfo();
                            thisTemp.CardList = thisCollection;
                            output.Add(thisTemp);
                            _thisMod.TempSets.ClearBoard(x);
                        }
                    }
                }
            }
            if (needsInitial == true)
                ResetSuccess();
            return output;
        }
        public bool CanLayDownInitialSets()
        {
            SetList thisSet = SetsList![SaveRoot!.Round - 1];
            DeckRegularDict<RegularRummyCard> thisCollection;
            IDeckDict<RegularRummyCard> tempCollection; //hopefully still works (?)
            for (int x = 1; x <= _thisMod!.TempSets!.HowManySets; x++)
            {
                tempCollection = WhatSet(x);
                thisCollection = new DeckRegularDict<RegularRummyCard>();

                if (tempCollection.Count > 0)
                    thisCollection.AddRange(tempCollection);
                if (thisCollection.Count(items => items.IsObjectWild == false) >= 2)
                {
                    foreach (var newSet in thisSet.PhaseSets)
                    {
                        if (newSet.DidSucceed == false)
                        {
                            newSet.DidSucceed = _rummys!.IsNewRummy(thisCollection, newSet.HowMany, RummyProcesses<EnumSuitList, EnumColorList, RegularRummyCard>.EnumRummyType.Sets);
                            if (newSet.DidSucceed == true)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            bool rets = thisSet.PhaseSets.All(items => items.DidSucceed == true);
            ResetSuccess();
            return rets;
        }
        public async Task LayDownOtherSetsAsync()
        {
            int manys = SingleInfo!.ObjectCount; //since i can trust it works properly.
            if (manys == 0)
            {
                await EndRoundAsync();
                return;
            }
            await ContinueTurnAsync();
        }
        public async Task LaidDownInitialSetsAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.LaidDown = true;
            await LayDownOtherSetsAsync();
        }
        public void CreateNewSet(TempInfo thisTemp)
        {
            PhaseSet thisSet = new PhaseSet(_thisMod!);
            thisSet.CreateSet(thisTemp.CardList);
            _thisMod!.MainSets!.CreateNewSet(thisSet);
        }
        public async Task AddToSetAsync(int ourSet, int deck)
        {
            PhaseSet thisSet = _thisMod!.MainSets!.GetIndividualSet(ourSet);
            var thisCard = DeckList!.GetSpecificItem(deck);
            thisSet.AddCard(thisCard);
            RemoveCard(deck);
            await LayDownOtherSetsAsync();
        }
        public bool CanAddToSet(PhaseSet thisSet, out RegularRummyCard? whatCard, out string message)
        {
            message = "";
            whatCard = null;
            int howManySelected = _thisMod!.PlayerHand1!.HowManySelectedObjects + _thisMod.TempSets!.HowManySelectedObjects;
            if (howManySelected == 0)
            {
                if (ThisData!.IsXamarinForms == false)
                    message = "There are no cards selected";
                return false;
            }
            if (howManySelected > 1)
            {
                if (ThisData!.IsXamarinForms == false)
                    message = "Sorry, only one card can be selected";
                return false;
            }
            int decks;
            if (_thisMod.TempSets.HowManySelectedObjects == 1)
            {
                int piles = _thisMod.TempSets.PileForSelectedObject;
                decks = _thisMod.TempSets.DeckForSelectedObjected(piles);
            }
            else
                decks = _thisMod.PlayerHand1.ObjectSelected();
            whatCard = DeckList!.GetSpecificItem(decks);
            if (thisSet.CanExpand(whatCard) == false)
            {
                message = "Sorry, this card cannot be used to expand the set"; //i guess this would be okay no matter what.
                return false;
            }
            return true;
        }
    }
}