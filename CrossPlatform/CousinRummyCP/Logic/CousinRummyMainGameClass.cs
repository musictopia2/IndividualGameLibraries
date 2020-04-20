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
using CousinRummyCP.Data;
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
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceModels;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.MiscHelpers;
using BasicGameFrameworkLibrary.SpecializedGameTypes.RummyClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
namespace CousinRummyCP.Logic
{
    [SingletonGame]
    public class CousinRummyMainGameClass : CardGameClass<RegularRummyCard, CousinRummyPlayerItem, CousinRummySaveInfo>, IMiscDataNM, IStartNewGame
    {
        

        private readonly CousinRummyVMData _model;
        private readonly CommandContainer _command; //most of the time, needs this.  if not needed, take out.
        private readonly CousinRummyGameContainer _gameContainer; //if we don't need it, take it out.
        internal CustomBasicList<SetList>? SetsList { get; set; }
        private readonly RummyProcesses<EnumSuitList, EnumColorList, RegularRummyCard> _rummys;
        public CousinRummyMainGameClass(IGamePackageResolver mainContainer,
            IEventAggregator aggregator,
            BasicData basicData,
            TestOptions test,
            CousinRummyVMData currentMod,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            ICardInfo<RegularRummyCard> cardInfo,
            CommandContainer command,
            CousinRummyGameContainer gameContainer)
            : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer)
        {
            _model = currentMod;
            _command = command;
            _gameContainer = gameContainer;
            _gameContainer.ModifyCards = ModifyCards;
            _rummys = new RummyProcesses<EnumSuitList, EnumColorList, RegularRummyCard>();
        }
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

        //private RegularRummyCard ManuallyChooseCard
        //{
        //    get
        //    {
        //        var tempList = _gameContainer.DeckList.Where(items => items.IsObjectWild == false).ToRegularDeckDict();
        //        do
        //        {
        //            var thisCard = tempList.GetRandomItem();
        //            if (_model!.Deck1!.CardExists(thisCard.Deck))
        //                return thisCard;
        //        } while (true);
        //    }
        //}
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
            _model!.MainSets!.ClearBoard();
            LoadControls();
            int x = SaveRoot!.SetList.Count; //the first time, actually load manually.
            x.Times(Items =>
            {
                PhaseSet ThisSet = new PhaseSet(_gameContainer);
                _model.MainSets.CreateNewSet(ThisSet);
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
            _model.MainSets.LoadSets(SaveRoot.SetList);
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
            }
            _model!.MainSets!.ClearBoard();
            SaveRoot!.SetList.Clear(); //i think this too.
            //var tempCard = ManuallyChooseCard;
            //_model.Deck1!.ManuallyRemoveSpecificCard(tempCard);
            //_model.Pile1!.AddCard(tempCard);
            return base.LastPartOfSetUpBeforeBindingsAsync();
        }
        public override Task PopulateSaveRootAsync()
        {
            SaveRoot!.SetList = _model!.MainSets!.SavedSets();
            CousinRummyPlayerItem Self = PlayerList!.GetSelf();
            Self.AdditionalCards = _model.TempSets!.ListAllObjects();
            return base.PopulateSaveRootAsync();
        }
        private void RemoveCard(int deck)
        {
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Computer)
                return; //i think computer player would have already removed their card.
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
            {
                if (_model!.TempSets!.HasObject(deck))
                {
                    _model.TempSets.RemoveObject(deck);
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
                _model!.OtherLabel = SingleInfo.NickName;
            }
            else
            {
                SingleInfo = PlayerList!.GetWhoPlayer();
                _model!.OtherLabel = "None";
            }
            _model.PhaseData = SetsList![SaveRoot!.Round - 1].Description;
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
            switch (status) //can't do switch because we don't know what the cases are ahead of time.
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
            SingleInfo.MainHandList.UnhighlightObjects(); //i think this is best.
            UnselectCards();
            _command.ManuelFinish = true; //because it could be somebody else's turn.
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
        }
        private void UnselectCards()
        {
            if (SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
                return;
            _model!.PlayerHand1!.EndTurn();
            _model!.TempSets!.EndTurn();
        }
        public IDeckDict<RegularRummyCard> WhatSet(int whichOne)
        {
            return _model!.TempSets!.ObjectList(whichOne);
        }
        public override async Task EndRoundAsync()
        {
            UnselectCards();
            SingleInfo = PlayerList!.GetSelf();
            var TempCol = _model!.TempSets!.ListObjectsRemoved();
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
            await this.RoundOverNextAsync();
        }
        private int CalculatePoints(CousinRummyPlayerItem thisPlayer)
        {
            int plusPoints = _model!.MainSets!.SetList.Sum(items => items.PointsReceived(thisPlayer.Id));
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
            for (int x = 1; x <= _model!.TempSets!.HowManySets; x++)
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
                                    _model.TempSets.ClearBoard(x);
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
                            _model.TempSets.ClearBoard(x);
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
            for (int x = 1; x <= _model!.TempSets!.HowManySets; x++)
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
            if (BasicData.IsXamarinForms)
            {
                await Task.Delay(20);
            }
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
            PhaseSet thisSet = new PhaseSet(_gameContainer);
            thisSet.CreateSet(thisTemp.CardList);
            _model!.MainSets!.CreateNewSet(thisSet);
        }
        public async Task AddToSetAsync(int ourSet, int deck)
        {
            PhaseSet thisSet = _model!.MainSets!.GetIndividualSet(ourSet);
            var thisCard = _gameContainer.DeckList!.GetSpecificItem(deck);
            thisSet.AddCard(thisCard);
            RemoveCard(deck);
            await LayDownOtherSetsAsync();
        }
        public bool CanAddToSet(PhaseSet thisSet, out RegularRummyCard? whatCard, out string message)
        {
            message = "";
            whatCard = null;
            int howManySelected = _model!.PlayerHand1!.HowManySelectedObjects + _model.TempSets!.HowManySelectedObjects;
            if (howManySelected == 0)
            {
                if (BasicData!.IsXamarinForms == false)
                    message = "There are no cards selected";
                return false;
            }
            if (howManySelected > 1)
            {
                if (BasicData!.IsXamarinForms == false)
                    message = "Sorry, only one card can be selected";
                return false;
            }
            int decks;
            if (_model.TempSets.HowManySelectedObjects == 1)
            {
                int piles = _model.TempSets.PileForSelectedObject;
                decks = _model.TempSets.DeckForSelectedObjected(piles);
            }
            else
                decks = _model.PlayerHand1.ObjectSelected();
            whatCard = _gameContainer.DeckList!.GetSpecificItem(decks);
            if (thisSet.CanExpand(whatCard) == false)
            {
                message = "Sorry, this card cannot be used to expand the set"; //i guess this would be okay no matter what.
                return false;
            }
            return true;
        }
    }
}
