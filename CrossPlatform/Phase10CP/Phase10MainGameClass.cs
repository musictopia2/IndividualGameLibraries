using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.ColorCards;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFramework.SpecializedGameTypes.RummyClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace Phase10CP
{
    [SingletonGame]
    public class Phase10MainGameClass : CardGameClass<Phase10CardInformation, Phase10PlayerItem, Phase10SaveInfo>
        , IMiscDataNM, IStartNewGame, IMissTurnClass<Phase10PlayerItem>
    {
        public Phase10MainGameClass(IGamePackageResolver container) : base(container) { }

        private Phase10ViewModel? _thisMod;
        private RummyProcesses<EnumColorTypes, EnumColorTypes, Phase10CardInformation>? _rummys;
        private CustomBasicList<PhaseList>? _phaseInfo;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            _thisMod = MainContainer.Resolve<Phase10ViewModel>();
        }

        public override async Task FinishGetSavedAsync()
        {
            _thisMod!.MainSets!.ClearBoard(); //i think because its all gone.
            LoadControls();
            int x = SaveRoot!.SetList.Count; //the first time, actually load manually.
            x.Times(Items =>
            {
                PhaseSet thisSet = new PhaseSet(_thisMod);
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
            SingleInfo.MainHandList.Sort(); //has to always sort no matter what now.
            ThisE.Subscribe(SingleInfo); //i think
            _thisMod.MainSets.LoadSets(SaveRoot.SetList); //i think clear board too.
            if (SaveRoot.ImmediatelyStartTurn == false)
                PrepStartTurn(); //i think
            await base.FinishGetSavedAsync();
        }
        protected override void PrepStartTurn()
        {
            base.PrepStartTurn();
            _thisMod!.CurrentPhase = _phaseInfo![SingleInfo!.Phase - 1].Description; //0 based.
        }
        public override async Task PopulateSaveRootAsync()
        {
            SaveRoot!.SetList = _thisMod!.MainSets!.SavedSets();
            Phase10PlayerItem Self = PlayerList!.GetSelf();
            Self.AdditionalCards = _thisMod.TempSets!.ListAllObjects();
            await base.PopulateSaveRootAsync(); //you need this too.  otherwise, don't get enough details.
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
            _rummys = new RummyProcesses<EnumColorTypes, EnumColorTypes, Phase10CardInformation>();
            _rummys.HasSecond = false;
            _rummys.HasWild = true;
            _rummys.LowNumber = 1;
            _rummys.HighNumber = 12;
            _rummys.NeedMatch = false;
            PhaseList thisPhase = new PhaseList();
            thisPhase.Description = "2 Sets of 3";
            SetInfo newSets;
            _phaseInfo = new CustomBasicList<PhaseList>();
            2.Times(x =>
            {
                newSets = new SetInfo();
                newSets.HowMany = 3;
                newSets.SetType = EnumWhatSets.Kinds;
                thisPhase.PhaseSets.Add(newSets);
            });
            _phaseInfo.Add(thisPhase);
            thisPhase = new PhaseList();
            thisPhase.Description = "1 Set of 3, 1 Run of 4";
            2.Times(x =>
            {
                newSets = new SetInfo();
                if (x == 2)
                {
                    newSets.HowMany = 3;
                    newSets.SetType = EnumWhatSets.Kinds;
                }
                else
                {
                    newSets.HowMany = 4;
                    newSets.SetType = EnumWhatSets.Runs;
                }
                thisPhase.PhaseSets.Add(newSets);
            });
            _phaseInfo.Add(thisPhase);
            thisPhase = new PhaseList();
            thisPhase.Description = "1 Set of 4, 1 Run of 4";
            2.Times(x =>
            {
                newSets = new SetInfo();
                newSets.HowMany = 4;
                if (x == 2)
                    newSets.SetType = EnumWhatSets.Kinds;
                else
                    newSets.SetType = EnumWhatSets.Runs;
                thisPhase.PhaseSets.Add(newSets);
            });
            _phaseInfo.Add(thisPhase);
            for (int x = 7; x <= 9; x++)
            {
                thisPhase = new PhaseList();
                newSets = new SetInfo();
                thisPhase.Description = "1 Run of " + x;
                newSets.SetType = EnumWhatSets.Runs;
                newSets.HowMany = x;
                thisPhase.PhaseSets.Add(newSets);
                _phaseInfo.Add(thisPhase);
            }
            thisPhase = new PhaseList();
            thisPhase.Description = "2 Sets Of 4";
            for (int x = 1; x <= 2; x++)
            {
                newSets = new SetInfo();
                newSets.HowMany = 4;
                newSets.SetType = EnumWhatSets.Kinds;
                thisPhase.PhaseSets.Add(newSets);
            }
            _phaseInfo.Add(thisPhase);
            thisPhase = new PhaseList();
            thisPhase.Description = "7 Cards Of 1 Color";
            newSets = new SetInfo();
            newSets.HowMany = 7;
            newSets.SetType = EnumWhatSets.Colors;
            thisPhase.PhaseSets.Add(newSets);
            _phaseInfo.Add(thisPhase);
            int Y;
            for (int x = 2; x <= 3; x++)
            {
                thisPhase = new PhaseList();
                thisPhase.Description = "1 Set Of 5, 1 Set Of " + x;
                for (Y = 1; Y <= 2; Y++)
                {
                    newSets = new SetInfo();
                    newSets.SetType = EnumWhatSets.Kinds;
                    if (Y == 2)
                        newSets.HowMany = x;
                    else
                        newSets.HowMany = 5;
                    thisPhase.PhaseSets.Add(newSets);
                }
                _phaseInfo.Add(thisPhase);
            }
        }
        protected override async Task ComputerTurnAsync()
        {
            //if there is nothing, then just won't do anything.
            await Task.CompletedTask;
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            SaveRoot!.ImmediatelyStartTurn = true; //needed so it can do the extra stuff needed.  prepstart is not good enough.
            PlayerList!.ForEach(ThisPlayer =>
            {
                ThisPlayer.MissNextTurn = false;
                ThisPlayer.Completed = false;
            });
            return base.StartSetUpAsync(isBeginning);
        }
        protected override async Task LastPartOfSetUpBeforeBindingsAsync()
        {
            if (IsLoaded == false)
            {
                LoadControls();
                SingleInfo = PlayerList!.GetSelf();
                ThisE.Subscribe(SingleInfo);
            }
            _thisMod!.MainSets!.ClearBoard();
            SaveRoot!.IsTie = false;
            SaveRoot.Skips = false; //i think
            SaveRoot.SetList.Clear(); //i think this too.
            _thisMod.TempSets!.ClearBoard(); //i think this happens before sending data to other players.
            Phase10CardInformation thisCard = _thisMod.Pile1!.GetCardInfo();
            if (thisCard.CardCategory == EnumCardCategory.Skip)
                WhoTurn = await PlayerList!.CalculateWhoTurnAsync(true);
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "phasecompleted":
                    await CreateSetsAsync(content);
                    await ProcessCompletedPhaseAsync();
                    break;
                case "playerskipped":
                    await _thisMod!.ShowGameMessageAsync("Needs To Think About Skipping Player"); //we never did anything for skipping player.
                    break;
                case "expandrummy":
                    SendExpandedSet Expands = await js.DeserializeObjectAsync<SendExpandedSet>(content);
                    await ExpandHumanRummyAsync(Expands.Number, Expands.Deck, Expands.Position);
                    break;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer(); //just in case.
            await base.StartNewTurnAsync();
            _thisMod!.CurrentPhase = _phaseInfo![SingleInfo.Phase - 1].Description; //0 based.
            SaveRoot!.CompletedPhase = SingleInfo.Completed;
            SaveRoot.Skips = false; //maybe not needed anymore (?)
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task EndTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            if (SingleInfo.ObjectCount == 9)
                throw new BasicBlankException("After ending turn, a player should never have only 9 cards left");
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                UnselectCards();
            _thisMod!.CommandContainer!.ManuelFinish = true; //because it could be somebody else's turn.
            WhoTurn = await PlayerList.CalculateWhoTurnAsync(true); //has to be true because if 2 players, then you get a second turn.
            await StartNewTurnAsync(); //i think.
        }
        public override async Task EndRoundAsync() //uno worked without the using original class so this should be fine too
        {
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                UnselectCards();
            SingleInfo = PlayerList!.GetSelf();
            var tempCol = _thisMod!.TempSets!.ListObjectsRemoved();
            SingleInfo.MainHandList.AddRange(tempCol);
            SingleInfo.MainHandList.Sort();
            int scores;
            PlayerList.ForEach(tempPlayer =>
            {
                scores = tempPlayer.MainHandList.TotalPoints();
                tempPlayer.TotalScore += scores;
                if (tempPlayer.Completed == true)
                    tempPlayer.Phase++;
            });
            if (CanEndGame == false)
            {
                this.RoundOverNext();
                return;
            }
            GetWinPlayer();
            if (SingleInfo == null)
            {
                this.RoundOverNext();
                if (ThisData!.MultiPlayer == false || ThisData.Client == false)
                    _thisMod.Status = "There was a tie.  Therefore another round for the players who tied is needed to determine the winner";
                return;
            }
            await ShowWinAsync();
        }
        protected override Task AfterDrawingAsync()
        {
            if (SingleInfo!.ObjectCount == 10)
                throw new BasicBlankException("You can never have only 10 cards after drawing");
            return base.AfterDrawingAsync();
        }
        protected override Task AfterPickupFromDiscardAsync()
        {
            if (SingleInfo!.ObjectCount == 10)
                throw new BasicBlankException("You can never have only 10 cards after drawing");
            return base.AfterPickupFromDiscardAsync();
        }
        private async Task CreateSetsAsync(string message)
        {
            var firstTemp = await js.DeserializeObjectAsync<CustomBasicList<string>>(message);
            foreach (var thisFirst in firstTemp)
            {
                var thisSend = await js.DeserializeObjectAsync<SendNewSet>(thisFirst);
                var thisCol = await thisSend.CardListData.GetObjectsFromDataAsync(SingleInfo!.MainHandList);
                TempInfo thisTemp = new TempInfo();
                thisTemp.CardList = thisCol;
                thisTemp.WhatSet = thisSend.WhatSet;
                CreateNewSet(thisTemp);
            }
        }

        Task IStartNewGame.ResetAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.Phase = 1;
                thisPlayer.TotalScore = 0;
            });
            return Task.CompletedTask;
        }

        public override Task ContinueTurnAsync()
        {
            if (SingleInfo!.ObjectCount == 9)
                throw new BasicBlankException("You should never have only 9 cards");
            return base.ContinueTurnAsync();
        }
        private void RemoveCard(int deck)
        {
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Computer)
                throw new BasicBlankException("Computer should have never gone on this game.");
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
        public bool IsCardSelected()
        {
            if (SingleInfo!.MainHandList.HowManySelectedItems() > 0)
                return true;
            return _thisMod!.TempSets!.HowManySelectedObjects > 0;
        }
        public IDeckDict<Phase10CardInformation> WhatSet(int whichOne)
        {
            return _thisMod!.TempSets!.ObjectList(whichOne);
        }
        private void UnselectCards()
        {
            if (SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
                return;
            _thisMod!.PlayerHand1!.EndTurn();
            _thisMod.TempSets!.EndTurn();
        }
        private bool CanEndGame
        {
            get
            {
                if (SaveRoot!.IsTie == true)
                    return true;
                return PlayerList.Any(items => items.Phase == 11);
            }
        }
        private CustomBasicList<string> PossibleSkipList
        {
            get
            {
                return PlayerList!.Where(Items => Items.MissNextTurn == false && Items.Id != WhoTurn).Select(Items => Items.NickName).ToCustomBasicList();
            }
        }
        public override async Task DiscardAsync(Phase10CardInformation thisCard)
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            RemoveCard(thisCard.Deck);
            await AnimatePlayAsync(thisCard);
            if (SingleInfo.ObjectCount == 0) //going to try to trust the object count.
            {
                await EndRoundAsync();
                return;
            }
            if (thisCard.CardCategory == EnumCardCategory.Skip)
                await ChooseSkipAsync();
            else
                await EndTurnAsync();
        }
        private async Task ChooseSkipAsync()
        {
            var ThisList = PossibleSkipList;
            if (ThisList.Count == 0)
            {
                await EndTurnAsync();
                return;
            }
            if (ThisList.Count() == 1)
            {
                await SkipPlayerAsync(ThisList.Single());
                return;
            }
            SaveRoot!.Skips = true; //means you have to choose someone to skip
            _thisMod!.CommandContainer!.ManuelFinish = true;
            await _thisMod.ShowGameMessageAsync("Needs To Think About Skipping Player");
        }
        public async Task SkipPlayerAsync(string nickName)
        {
            PlayerList![nickName].MissNextTurn = true;
            await EndTurnAsync();
        }
        private void GetWinPlayer()
        {
            CustomBasicList<Phase10PlayerItem> firstList = PlayerList.Where(items => items.Phase == 11).OrderBy(items => items.TotalScore).ToCustomBasicList();
            if (firstList.Count == 0)
                throw new BasicBlankException("Game Should Not Be Over");
            if (firstList.Count == 1)
            {
                SingleInfo = firstList.Single();
                return;
            }
            if (firstList.First().TotalScore < firstList[1].TotalScore)
            {
                SingleInfo = firstList.First();
                return;
            }
            PlayerList!.ForEach(Items => Items.InGame = false);
            int totalScore = firstList.First().TotalScore;
            firstList.KeepConditionalItems(Items => Items.TotalScore == totalScore);
            if (firstList.Count <= 1)
                throw new BasicBlankException("Should not be tie");
            firstList.ForEach(Items => Items.InGame = true);
            SingleInfo = null; //so it won't show the end of the game after all.
        }
        Task IMissTurnClass<Phase10PlayerItem>.PlayerMissTurnAsync(Phase10PlayerItem player)
        {
            return Task.CompletedTask; //this does not do anything for missing next turn.
        }
        private void ResetSuccess()
        {
            _phaseInfo!.ForEach(thisPhase =>
            {
                thisPhase.PhaseSets.ForEach(thisSet => thisSet.DidSucceed = false);
            });
        }
        public bool DidCompletePhase(out int howMany)
        {
            int phase = SingleInfo!.Phase;
            howMany = 0;
            PhaseList thisPhase = _phaseInfo![phase - 1]; //because 0 based.
            DeckRegularDict<Phase10CardInformation> thisCollection;
            IDeckDict<Phase10CardInformation> tempCollection; //hopefully still works (?)
            Phase10CardInformation thisCard;
            for (int x = 1; x <= _thisMod!.TempSets!.HowManySets; x++)
            {
                tempCollection = WhatSet(x);
                thisCollection = new DeckRegularDict<Phase10CardInformation>();
                if (tempCollection.Count > 0)
                    thisCollection.AddRange(tempCollection);
                if (thisCollection.Count > 0)
                {
                    foreach (var newSet in thisPhase.PhaseSets)
                    {
                        if (newSet.DidSucceed == false)
                        {
                            newSet.DidSucceed = newSet.SetType switch
                            {
                                EnumWhatSets.Colors => _rummys!.IsNewRummy(thisCollection, newSet.HowMany, RummyProcesses<EnumColorTypes, EnumColorTypes, Phase10CardInformation>.EnumRummyType.Colors),
                                EnumWhatSets.Kinds => _rummys!.IsNewRummy(thisCollection, newSet.HowMany, RummyProcesses<EnumColorTypes, EnumColorTypes, Phase10CardInformation>.EnumRummyType.Sets),
                                EnumWhatSets.Runs => _rummys!.IsNewRummy(thisCollection, newSet.HowMany, RummyProcesses<EnumColorTypes, EnumColorTypes, Phase10CardInformation>.EnumRummyType.Runs),
                                _ => throw new BasicBlankException("Not Supported"),
                            };
                            if (newSet.DidSucceed == true)
                            {
                                thisCard = thisCollection.First();
                                howMany += thisCollection.Count();
                                break;
                            }
                        }
                    }
                }
            }
            if (thisPhase.PhaseSets.Any(Items => Items.DidSucceed == false))
            {
                howMany = 0; //because you failed so does not matter anyway obviously.
                ResetSuccess();
                return false;
            }
            ResetSuccess();
            return true;
        }
        public CustomBasicList<TempInfo> ListValidSets()
        {
            CustomBasicList<TempInfo> output = new CustomBasicList<TempInfo>();
            int phase = SingleInfo!.Phase;
            PhaseList thisPhase = _phaseInfo![phase - 1]; //because 0 based.
            DeckRegularDict<Phase10CardInformation> thisCollection;
            IDeckDict<Phase10CardInformation> tempCollection; //hopefully still works (?)
            TempInfo thisTemp;
            for (int x = 1; x <= _thisMod!.TempSets!.HowManySets; x++)
            {
                tempCollection = WhatSet(x);
                thisCollection = new DeckRegularDict<Phase10CardInformation>();
                if (tempCollection.Count > 0)
                    thisCollection.AddRange(tempCollection);
                if (thisCollection.Count > 0)
                    foreach (var newSet in thisPhase.PhaseSets)
                    {
                        if (newSet.DidSucceed == false)
                        {
                            newSet.DidSucceed = newSet.SetType switch
                            {
                                EnumWhatSets.Colors => _rummys!.IsNewRummy(thisCollection, newSet.HowMany, RummyProcesses<EnumColorTypes, EnumColorTypes, Phase10CardInformation>.EnumRummyType.Colors),
                                EnumWhatSets.Kinds => _rummys!.IsNewRummy(thisCollection, newSet.HowMany, RummyProcesses<EnumColorTypes, EnumColorTypes, Phase10CardInformation>.EnumRummyType.Sets),
                                EnumWhatSets.Runs => _rummys!.IsNewRummy(thisCollection, newSet.HowMany, RummyProcesses<EnumColorTypes, EnumColorTypes, Phase10CardInformation>.EnumRummyType.Runs),
                                _ => throw new BasicBlankException("Not Supported"),
                            };
                            if (newSet.DidSucceed == true)
                            {
                                thisTemp = new TempInfo();
                                thisTemp.CardList = thisCollection;
                                thisTemp.WhatSet = newSet.SetType;
                                if (newSet.SetType == EnumWhatSets.Runs)
                                {
                                    thisTemp.FirstNumber = _rummys!.FirstUsed;
                                    thisTemp.SecondNumber = _rummys!.FirstUsed + thisCollection.Count - 1;
                                }
                                output.Add(thisTemp);
                                _thisMod.TempSets.ClearBoard(x);
                                break;
                            }
                        }
                        
                        
                    }
                if (output.Count == thisPhase.PhaseSets.Count)
                    break; //try this too.
            }
            ResetSuccess();
            return output;
        }
        public async Task ProcessCompletedPhaseAsync()
        {
            SaveRoot!.CompletedPhase = true;
            SingleInfo!.Completed = true;
            await ContinueTurnAsync(); //decided to not update hand.  hopefully this works.
        }
        public void CreateNewSet(TempInfo thisTemp)
        {
            PhaseSet thisSet = new PhaseSet(_thisMod!);
            thisSet.CreateSet(thisTemp.CardList, thisTemp.WhatSet);
            _thisMod!.MainSets!.CreateNewSet(thisSet); //decided to be all one time.  that was best.
        }
        public async Task ExpandHumanRummyAsync(int phaseSet, int deck, int position)
        {
            PhaseSet thisPhase = _thisMod!.MainSets!.GetIndividualSet(phaseSet);
            Phase10CardInformation thisCard = DeckList!.GetSpecificItem(deck); //i think.
            thisPhase.AddCard(thisCard, position);
            RemoveCard(deck);
            await ContinueTurnAsync();
        }
        public bool CanHumanExpand(PhaseSet thisPhase, ref int position, out Phase10CardInformation? whatCard, out string message)
        {
            message = "";
            whatCard = null;//defaults to null.
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
            if (_thisMod.TempSets.TotalObjects + SingleInfo!.MainHandList.Count == 1)
            {
                if (ThisData!.IsXamarinForms == false)
                    message = "Sorry, there must be a card left to discard";
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
            int Nums = thisPhase.PositionToPlay(whatCard, position);
            if (Nums == 0)
            {
                position = 0;
                whatCard = null;
                if (ThisData!.IsXamarinForms == false)
                    message = "Sorry, this card cannot be used to expand the set";
                return false;
            }
            position = Nums;
            return true;
        }
        public bool CanProcessDiscard(out bool pickUp, out int index, out int deck, out string message)
        {
            index = -1; //defaults to -1
            message = "";
            deck = 0; //has to populate defaults first.
            if (AlreadyDrew == false && PreviousCard == 0)
                pickUp = true;
            else
                pickUp = false;
            Phase10CardInformation thisCard;
            if (pickUp == true)
            {
                if (IsCardSelected() == true)
                {
                    message = "There is at least one card selected.  Unselect all the cards before picking up from discard";
                    return false;
                }
                thisCard = _thisMod!.Pile1!.GetCardInfo();
                if (thisCard.CardCategory == EnumCardCategory.Skip)
                {
                    message = "Sorry, cannot pickup a skip";
                    return false;
                }
                return true;
            }
            var SelectList = SingleInfo!.MainHandList.GetSelectedItems();
            int Counts = SelectList.Count + _thisMod!.TempSets!.HowManySelectedObjects;
            if (Counts > 1)
            {
                message = "Sorry, you can only select one card to discard";
                return false;
            }
            if (Counts == 0)
            {
                message = "Sorry, you must select a card to discard";
                return false;
            }
            if (SelectList.Count == 1)
            {
                index = 0;
                deck = _thisMod.PlayerHand1!.ObjectSelected();
            }
            else
            {
                index = _thisMod.TempSets.PileForSelectedObject;
                deck = _thisMod.TempSets.DeckForSelectedObjected(index);
            }
            thisCard = DeckList!.GetSpecificItem(deck);
            if (thisCard.Deck == PreviousCard && SingleInfo.MainHandList.Count + _thisMod.TempSets.TotalObjects > 1)
            {
                deck = 0;
                index = -1;
                message = "Sorry, you cannot discard the one that was picked up since there is more than one card to choose from";
                return false;
            }
            return true;
        }
    }
}