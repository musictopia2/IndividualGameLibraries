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
using System;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace ChinazoCP
{
    [SingletonGame]
    public class ChinazoMainGameClass : CardGameClass<ChinazoCard, ChinazoPlayerItem, ChinazoSaveInfo>, IMiscDataNM, IStartNewGame
    {
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
        internal int CardsToPassOut
        {
            get
            {
                if (SaveRoot!.Round < 4)
                    return 8;
                if (SaveRoot.Round <= 7)
                    return 11;
                return 13;
            }
        }
        internal CustomBasicList<SetList>? SetsList;
        private RummyProcesses<EnumSuitList, EnumColorList, ChinazoCard>? _rummys;
        public ChinazoMainGameClass(IGamePackageResolver container) : base(container) { }

        private ChinazoViewModel? _thisMod;
        public void ModifyCards(CustomBasicList<ChinazoCard> thisList)
        {
            thisList.ForEach(thisCard =>
            {
                if (thisCard.Value == EnumCardValueList.Joker)
                    thisCard.Points = 50;
                else if (thisCard.Value == EnumCardValueList.LowAce || thisCard.Value == EnumCardValueList.HighAce)
                    thisCard.Points = 20;
                else if (thisCard.Value >= EnumCardValueList.Nine)
                    thisCard.Points = 10;
                else
                    thisCard.Points = (int)thisCard.Value;
            });
        }
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            _thisMod = MainContainer.Resolve<ChinazoViewModel>();
        }
        public override Task FinishGetSavedAsync()
        {
            _thisMod!.MainSets!.ClearBoard();
            LoadControls();
            int x = SaveRoot!.SetList.Count; //the first time, actually load manually.
            x.Times(items =>
            {
                PhaseSet thisSet = new PhaseSet(_thisMod!);
                _thisMod!.MainSets.CreateNewSet(thisSet);
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
            _thisMod!.MainSets.LoadSets(SaveRoot.SetList);
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
            _rummys = new RummyProcesses<EnumSuitList, EnumColorList, ChinazoCard>();
            _rummys.HasSecond = true;
            _rummys.HasWild = true;
            _rummys.LowNumber = 1;
            _rummys.HighNumber = 14;
            SetsList = new CustomBasicList<SetList>();
            SetList FirstSet = new SetList(); //for now, just keep the names since i copied/pasted.
            FirstSet.Description = "1 Set of 3, 1 Run of 3";
            FillInSets(FirstSet, 1, RummyProcesses<EnumSuitList, EnumColorList, ChinazoCard>.EnumRummyType.Sets);
            FillInSets(FirstSet, 1, RummyProcesses<EnumSuitList, EnumColorList, ChinazoCard>.EnumRummyType.Runs);
            SetsList.Add(FirstSet);
            FirstSet = new SetList();
            FirstSet.Description = "2 Sets of 3";
            FillInSets(FirstSet, 2, RummyProcesses<EnumSuitList, EnumColorList, ChinazoCard>.EnumRummyType.Sets);
            SetsList.Add(FirstSet);
            FirstSet = new SetList();
            FirstSet.Description = "2 Runs of 3";
            FillInSets(FirstSet, 2, RummyProcesses<EnumSuitList, EnumColorList, ChinazoCard>.EnumRummyType.Runs);
            SetsList.Add(FirstSet);
            FirstSet = new SetList();
            FirstSet.Description = "2 Sets of 3, 1 Run Of 3";
            FillInSets(FirstSet, 2, RummyProcesses<EnumSuitList, EnumColorList, ChinazoCard>.EnumRummyType.Sets);
            FillInSets(FirstSet, 1, RummyProcesses<EnumSuitList, EnumColorList, ChinazoCard>.EnumRummyType.Runs);
            SetsList.Add(FirstSet);
            FirstSet = new SetList();
            FirstSet.Description = "1 Set Of 3, 2 Runs of 3";
            FillInSets(FirstSet, 1, RummyProcesses<EnumSuitList, EnumColorList, ChinazoCard>.EnumRummyType.Sets);
            FillInSets(FirstSet, 2, RummyProcesses<EnumSuitList, EnumColorList, ChinazoCard>.EnumRummyType.Runs);
            SetsList.Add(FirstSet);
            FirstSet = new SetList();
            FirstSet.Description = "3 Sets Of 3";
            FillInSets(FirstSet, 3, RummyProcesses<EnumSuitList, EnumColorList, ChinazoCard>.EnumRummyType.Sets);
            SetsList.Add(FirstSet);
            FirstSet = new SetList();
            FirstSet.Description = "3 Runs Of 3";
            FillInSets(FirstSet, 3, RummyProcesses<EnumSuitList, EnumColorList, ChinazoCard>.EnumRummyType.Runs);
            SetsList.Add(FirstSet);
            FirstSet = new SetList();
            FirstSet.Description = "3 Sets of 3, 1 Run Of 3";
            FillInSets(FirstSet, 3, RummyProcesses<EnumSuitList, EnumColorList, ChinazoCard>.EnumRummyType.Sets);
            FillInSets(FirstSet, 1, RummyProcesses<EnumSuitList, EnumColorList, ChinazoCard>.EnumRummyType.Runs);
            SetsList.Add(FirstSet);
            FirstSet = new SetList();
            FirstSet.Description = "1 Set Of 3, 3 Runs of 3";
            FillInSets(FirstSet, 1, RummyProcesses<EnumSuitList, EnumColorList, ChinazoCard>.EnumRummyType.Sets);
            FillInSets(FirstSet, 3, RummyProcesses<EnumSuitList, EnumColorList, ChinazoCard>.EnumRummyType.Runs);
            SetsList.Add(FirstSet);
            FirstSet = new SetList();
            FirstSet.Description = "4 Sets Of 3";
            FillInSets(FirstSet, 4, RummyProcesses<EnumSuitList, EnumColorList, ChinazoCard>.EnumRummyType.Sets);
            SetsList.Add(FirstSet);
            FirstSet = new SetList();
            FirstSet.Description = "4 Runs Of 3";
            FillInSets(FirstSet, 4, RummyProcesses<EnumSuitList, EnumColorList, ChinazoCard>.EnumRummyType.Runs);
            SetsList.Add(FirstSet);
            if (SetsList.Count != 11)
                throw new Exception("Must have 11 sets created");
        }
        private void FillInSets(SetList firstSet, int howMany, RummyProcesses<EnumSuitList, EnumColorList, ChinazoCard>.EnumRummyType whatSet)
        {
            int x;
            var loopTo = howMany;
            for (x = 1; x <= loopTo; x++)
                firstSet.PhaseSets.Add(new SetInfo() { WhatSet = whatSet, HowMany = 3 });
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
        public override Task PopulateSaveRootAsync()
        {
            SaveRoot!.SetList = _thisMod!.MainSets!.SavedSets();
            ChinazoPlayerItem self = PlayerList!.GetSelf();
            self.AdditionalCards = _thisMod.TempSets!.ListAllObjects();
            return base.PopulateSaveRootAsync();
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            SaveRoot!.Round++;
            if (ThisTest!.DoubleCheck == true && SaveRoot.Round < 3)
                SaveRoot.Round = 3;
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.LaidDown = false;
            });
            SaveRoot.ImmediatelyStartTurn = true;
            SaveRoot.HadChinazo = false;
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
                case "expandrummy":
                    SendExpandedSet thiss = await js.DeserializeObjectAsync<SendExpandedSet>(content);
                    await AddToSetAsync(thiss.Number, thiss.Deck, thiss.Position);
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            OtherTurn = WhoTurn;
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
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
            _thisMod.TempSets!.EndTurn();
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
        public override async Task DiscardAsync(ChinazoCard ThisCard)
        {
            RemoveCard(ThisCard.Deck);
            await AnimatePlayAsync(ThisCard);
            if (SingleInfo!.ObjectCount == 0) //going to try to trust the object count.
            {
                await EndRoundAsync();
                return;
            }
            await EndTurnAsync();
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
            if (SingleInfo!.LaidDown == true)
            {
                await PassAsync(); //they have to pass automatically because they have no tokens left or they laid down. or you discarded.
                return;
            }
            await ContinueTurnAsync();
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
            if (OtherTurn == 0)
            {
                await base.AfterDrawingAsync();
                return;
            }
            if (WhoTurn != OtherTurn)
            {
                OtherTurn = 0;
                PlayerDraws = WhoTurn;
                await DrawAsync();
                return;
            }
            OtherTurn = 0; //try this too.
            await base.AfterDrawingAsync();
        }
        private async Task CreateSetsAsync(string Message)
        {
            var firstTemp = await js.DeserializeObjectAsync<CustomBasicList<string>>(Message);
            foreach (var thisFirst in firstTemp)
            {
                var thisSend = await js.DeserializeObjectAsync<SendNewSet>(thisFirst);
                var thisCol = await thisSend.CardListData.GetObjectsFromDataAsync(SingleInfo!.MainHandList!);
                TempInfo thisTemp = new TempInfo();
                thisTemp.CardList = thisCol;
                thisTemp.WhatSet = thisSend.WhatSet;
                thisTemp.UseSecond = thisSend.UseSecond;
                CreateNewSet(thisTemp);
            }
        }
        private int CalculatePoints(ChinazoPlayerItem thisPlayer)
        {
            int points = thisPlayer.MainHandList.Sum(items => items.Points);
            if (SaveRoot!.HadChinazo == true)
                points *= 2;
            return points;
        }

        public override async Task EndRoundAsync()
        {
            UnselectCards();
            SingleInfo = PlayerList!.GetSelf();
            var tempCol = _thisMod!.TempSets!.ListObjectsRemoved();
            SingleInfo.MainHandList.AddRange(tempCol);
            SortCards();
            PlayerList.ForEach(tempPlayer =>
            {
                ModifyCards(tempPlayer.MainHandList); //most likely the old version could had a bug.
                int points = CalculatePoints(tempPlayer);
                tempPlayer.CurrentScore = points;
                tempPlayer.TotalScore += points;
            });
            if (SaveRoot!.Round == 11)
            {
                SingleInfo = PlayerList.OrderBy(items => items.TotalScore).First();
                await ShowWinAsync();
                return;
            }
            this.RoundOverNext();
        }
        public async Task LayDownOtherSetsAsync()
        {
            _thisMod!.MainSets!.SetList.ForEach(thisSet =>
            {
                thisSet.CheckList();
            });
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
            bool didHave = PlayerList.Any(items => items.LaidDown == true);
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.LaidDown = true;
            if (didHave == false)
            {
                int manys = SingleInfo.ObjectCount;
                if (manys <= 1)
                {
                    SaveRoot!.HadChinazo = true;
                    if (SingleInfo.PlayerCategory != EnumPlayerCategory.Self)
                        await _thisMod!.ShowGameMessageAsync($"{SingleInfo.NickName} had a chinzao.  Therefore; all other players will be getting double points");
                }
            }
            await LayDownOtherSetsAsync();
        }
        public void CreateNewSet(TempInfo thisTemp)
        {
            PhaseSet thisSet = new PhaseSet(_thisMod!);
            thisSet.CreateSet(thisTemp.CardList, thisTemp.WhatSet, thisTemp.UseSecond);
            _thisMod!.MainSets!.CreateNewSet(thisSet);
        }
        public async Task AddToSetAsync(int ourSet, int deck, int position)
        {
            PhaseSet thisSet = _thisMod!.MainSets!.GetIndividualSet(ourSet);
            var thisCard = DeckList!.GetSpecificItem(deck);
            thisSet.AddCard(thisCard, position);
            RemoveCard(deck);
            await LayDownOtherSetsAsync();
        }
        public bool CanAddToSet(PhaseSet thisSet, out ChinazoCard? whatCard, int section, out string message)
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
            int position = thisSet.PositionToPlay(whatCard, section);
            if (position == 0)
            {
                message = "Sorry, this card cannot be used to expand the set"; //i guess this would be okay no matter what.
                return false;
            }
            return true;
        }
        public IDeckDict<ChinazoCard> WhatSet(int whichOne)
        {
            return _thisMod!.TempSets!.ObjectList(whichOne);
        }
        public CustomBasicList<TempInfo> ListValidSets()
        {
            CustomBasicList<TempInfo> output = new CustomBasicList<TempInfo>();
            SetList thisSet = SetsList![SaveRoot!.Round - 1];
            DeckRegularDict<ChinazoCard> thisCollection;
            IDeckDict<ChinazoCard> tempCollection; //hopefully still works (?)
            TempInfo thisTemp;
            for (int x = 1; x <= _thisMod!.TempSets!.HowManySets; x++)
            {
                tempCollection = WhatSet(x);
                thisCollection = new DeckRegularDict<ChinazoCard>();

                if (tempCollection.Count > 0)
                    thisCollection.AddRange(tempCollection);
                if (thisCollection.Count(items => items.IsObjectWild == false) >= 2)
                {

                    foreach (var newSet in thisSet.PhaseSets)
                    {
                        if (newSet.DidSucceed == false)
                        {
                            newSet.DidSucceed = _rummys!.IsNewRummy(thisCollection, newSet.HowMany, newSet.WhatSet);
                            if (newSet.DidSucceed == true)
                            {
                                thisTemp = new TempInfo();
                                thisTemp.CardList = thisCollection;
                                thisTemp.WhatSet = newSet.WhatSet; //this was needed too now.
                                thisTemp.UseSecond = _rummys.UseSecond; // because the second one may not use it after all.
                                output.Add(thisTemp);
                                _thisMod.TempSets.ClearBoard(x);
                                break;
                            }
                        }
                    }
                }
            }
            ResetSuccess();
            return output;
        }
        public bool CanLayDownInitialSets()
        {
            SetList thisSet = SetsList![SaveRoot!.Round - 1];
            DeckRegularDict<ChinazoCard> thisCollection;
            IDeckDict<ChinazoCard> tempCollection; //hopefully still works (?)
            for (int x = 1; x <= _thisMod!.TempSets!.HowManySets; x++)
            {
                tempCollection = WhatSet(x);
                thisCollection = new DeckRegularDict<ChinazoCard>();

                if (tempCollection.Count > 0)
                    thisCollection.AddRange(tempCollection);
                if (thisCollection.Count(items => items.IsObjectWild == false) >= 2)
                {
                    foreach (var newSet in thisSet.PhaseSets)
                    {
                        if (newSet.DidSucceed == false)
                        {
                            newSet.DidSucceed = _rummys!.IsNewRummy(thisCollection, newSet.HowMany, newSet.WhatSet);
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

        Task IStartNewGame.ResetAsync()
        {
            SaveRoot!.Round = 0;
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.CurrentScore = 0;
                thisPlayer.TotalScore = 0;
            });
            return Task.CompletedTask;
        }
    }
}