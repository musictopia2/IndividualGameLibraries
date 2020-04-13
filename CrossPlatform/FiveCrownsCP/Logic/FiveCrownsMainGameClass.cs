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
using BasicGameFrameworkLibrary.SpecializedGameTypes.RummyClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using FiveCrownsCP.Cards;
using FiveCrownsCP.Data;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.

namespace FiveCrownsCP.Logic
{
    [SingletonGame]
    public class FiveCrownsMainGameClass : CardGameClass<FiveCrownsCardInformation, FiveCrownsPlayerItem, FiveCrownsSaveInfo>, IMiscDataNM, IStartNewGame
    {


        private readonly FiveCrownsVMData _model;
        private readonly CommandContainer _command; //most of the time, needs this.  if not needed, take out.
        private readonly FiveCrownsGameContainer _gameContainer; //if we don't need it, take it out.

        private readonly RummyProcesses<EnumSuitList, EnumColorList, FiveCrownsCardInformation> _rummys;

        public FiveCrownsMainGameClass(IGamePackageResolver mainContainer,
            IEventAggregator aggregator,
            BasicData basicData,
            TestOptions test,
            FiveCrownsVMData currentMod,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            ICardInfo<FiveCrownsCardInformation> cardInfo,
            CommandContainer command,
            FiveCrownsGameContainer gameContainer,
            FiveCrownsDelegates delegates
            )
            : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer)
        {
            _model = currentMod;
            _command = command;
            _gameContainer = gameContainer;
            _rummys = new RummyProcesses<EnumSuitList, EnumColorList, FiveCrownsCardInformation>();
            delegates.CardsToPassOut = (() => CardsToPassOut);
        }
        internal int CardsToPassOut
        {
            get
            {
                return SaveRoot!.UpTo; //i think.
            }
        }
        public override Task PopulateSaveRootAsync()
        {
            SaveRoot!.SetList = _model!.MainSets!.SavedSets();
            FiveCrownsPlayerItem self = PlayerList!.GetSelf();
            self.AdditionalCards = _model.TempSets!.ListAllObjects();
            return base.PopulateSaveRootAsync();
        }
        public override Task FinishGetSavedAsync()
        {
            _model!.MainSets!.ClearBoard();
            LoadControls();
            int x = SaveRoot!.SetList.Count; //the first time, actually load manually.
            x.Times(items =>
            {
                PhaseSet thisSet = new PhaseSet(_command);
                _model.MainSets.CreateNewSet(thisSet);
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
            _model.MainSets.LoadSets(SaveRoot.SetList);
            SaveRoot.LoadMod(_model);
            return base.FinishGetSavedAsync();
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            _rummys.HasSecond = false;
            _rummys.HasWild = true;
            _rummys.NeedMatch = true;
            _rummys.LowNumber = 3;
            _rummys.HighNumber = 13;
            IsLoaded = true; //i think needs to be here.
        }

        protected override Task StartSetUpAsync(bool isBeginning)
        {
            if (SaveRoot!.UpTo == 0)
                SaveRoot.UpTo = 3;
            else
                SaveRoot.UpTo++;
            SaveRoot.LoadMod(_model!); //if it happens more than once, its okay.
            ResetCurrentPoints();
            SaveRoot.PlayerWentOut = 0;
            _gameContainer.AlreadyDrew = false; //i think i forgot this part.
            SaveRoot.SetsCreated = false;
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
            return base.LastPartOfSetUpBeforeBindingsAsync();
        }
        private void RemoveCard(int Deck)
        {
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Computer)
                return; //i think computer player would have already removed their card.
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
            {
                if (_model!.TempSets!.HasObject(Deck))
                {
                    _model.TempSets.RemoveObject(Deck);
                    return;
                }
            }
            SingleInfo.MainHandList.RemoveObjectByDeck(Deck);
        }
        private DeckRegularDict<FiveCrownsCardInformation> PlayerHand()
        {
            var output = SingleInfo!.MainHandList.ToRegularDeckDict();
            if (SingleInfo.PlayerCategory != EnumPlayerCategory.Self)
                return output;
            output.AddRange(_model!.TempSets!.ListAllObjects());
            return output;
        }

        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status) //can't do switch because we don't know what the cases are ahead of time.
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
                int pointss = CalculatePoints(tempList);
                SingleInfo.CurrentScore = pointss;
                SingleInfo.TotalScore += pointss;
                _model!.TempSets!.ClearBoard(); //somehow this is how it has to be.
            }
            _command.ManuelFinish = true; //because it could be somebody else's turn.
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            if (WhoTurn == SaveRoot.PlayerWentOut || Test!.EndRoundEarly)
            {
                await EndRoundAsync();
                return;
            }
            await StartNewTurnAsync();
        }

        public void ModifyCards(IDeckDict<FiveCrownsCardInformation> thisCol)
        {
            thisCol.ForEach(thisCard =>
            {
                if ((int)thisCard.CardValue == SaveRoot!.UpTo)
                    thisCard.Points = 20;
                else if (thisCard.CardValue == EnumCardValueList.Joker)
                    thisCard.Points = 50;
                else
                    thisCard.Points = (int)thisCard.CardValue;
            });
        }
        private int CalculatePoints(IDeckDict<FiveCrownsCardInformation> thisCol)
        {
            if (thisCol.Count == 0)
                return 0;
            ModifyCards(thisCol);
            return thisCol.Sum(items => items.Points);
        }
        private void UnselectCards()
        {
            if (SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
                return;
            _model!.PlayerHand1!.EndTurn();
            _model.TempSets!.EndTurn();
        }
        public IDeckDict<FiveCrownsCardInformation> WhatSet(int whichOne)
        {
            return _model!.TempSets!.ObjectList(whichOne);
        }
        public override async Task DiscardAsync(FiveCrownsCardInformation thisCard)
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            RemoveCard(thisCard.Deck);
            await AnimatePlayAsync(thisCard);
            UnselectCards();
            if (SaveRoot!.PlayerWentOut == 0 && SingleInfo.ObjectCount == 0)
            {
                SaveRoot.PlayerWentOut = WhoTurn;
                SingleInfo.CurrentScore = 0;
            }
            await EndTurnAsync(); //i think this simple this time.
        }
        private async Task FinishEndAsync()
        {
            if (SaveRoot!.UpTo == 13)
            {
                SingleInfo = PlayerList.OrderBy(items => items.TotalScore).First();
                await ShowWinAsync();
                return;
            }
            await this.RoundOverNextAsync();
        }
        public override async Task EndRoundAsync()
        {
            await FinishEndAsync();
        }
        public async Task FinishedSetsAsync()
        {
            SaveRoot!.SetsCreated = true;
            await ContinueTurnAsync(); //i think this simple.
        }
        public void CreateSet(IDeckDict<FiveCrownsCardInformation> thisCol)
        {
            PhaseSet thisSet = new PhaseSet(_command!);
            thisCol.UnhighlightObjects();
            thisSet.HandList.ReplaceRange(thisCol);
            _model!.MainSets!.CreateNewSet(thisSet);
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
        private bool HasSet(IDeckDict<FiveCrownsCardInformation> thisCol)
        {
            if (thisCol.Count < 3)
                return false; //has to have at least 3 cards for a set.
            if (_rummys!.IsNewRummy(thisCol, thisCol.Count, RummyProcesses<EnumSuitList, EnumColorList, FiveCrownsCardInformation>.EnumRummyType.Sets))
                return true;
            if (_rummys.IsNewRummy(thisCol, thisCol.Count, RummyProcesses<EnumSuitList, EnumColorList, FiveCrownsCardInformation>.EnumRummyType.Runs))
                return true;
            return false;
        }
        public bool CanProcessDiscard(out bool PickUp, out int Index, out int Deck, out string Message)
        {
            Message = "";
            Index = 0;
            Deck = 0;
            if (_gameContainer.AlreadyDrew == false && _gameContainer.PreviousCard == 0)
                PickUp = true;
            else
                PickUp = false;
            if (PickUp == true)
            {

                if (_model!.PlayerHand1!.HowManySelectedObjects > 0 || _model.TempSets!.HowManySelectedObjects > 0)
                {
                    Message = "There is at least one card selected.  Unselect all the cards before picking up from discard";
                    return false;
                }
                return true;
            }
            var thisCol = _model!.PlayerHand1!.ListSelectedObjects();
            var otherCol = _model.TempSets!.ListSelectedObjects();
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
                Index = _model.TempSets.PileForSelectedObject;
                Deck = _model.TempSets.DeckForSelectedObjected(Index);
            }
            else
                Deck = _model.PlayerHand1.ObjectSelected();
            var thisCard = _gameContainer.DeckList!.GetSpecificItem(Deck);

            if (thisCard.Deck == _gameContainer.PreviousCard && SingleInfo!.ObjectCount > 1)
            {
                Message = "Sorry, you cannot discard the one that was picked up since there is more than one card to choose from";
                return false;
            }
            return true;
        }
        public CustomBasicList<TempInfo> ListValidSets()
        {
            CustomBasicList<TempInfo> output = new CustomBasicList<TempInfo>();
            DeckRegularDict<FiveCrownsCardInformation> thisCollection;
            IDeckDict<FiveCrownsCardInformation> tempCollection; //hopefully still works (?)
            TempInfo thisTemp;
            for (int x = 1; x <= 6; x++)
            {
                tempCollection = WhatSet(x);
                thisCollection = new DeckRegularDict<FiveCrownsCardInformation>();
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
                await UIPlatform.ShowMessageAsync("Must have one card to discard");
                return false;
            }
            return true;
        }
        public bool HasInitialSet()
        {
            if (SingleInfo!.MainHandList.Count != 1)
                return false; //because must have one card to discard no matter what.
            IDeckDict<FiveCrownsCardInformation> tempCollection; //hopefully still works (?)
            DeckRegularDict<FiveCrownsCardInformation> thisCollection;
            for (int x = 1; x <= 6; x++)
            {
                tempCollection = WhatSet(x);
                thisCollection = new DeckRegularDict<FiveCrownsCardInformation>();
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
