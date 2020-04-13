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
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using FourSuitRummyCP.Data;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.

namespace FourSuitRummyCP.Logic
{
    [SingletonGame]
    public class FourSuitRummyMainGameClass : CardGameClass<RegularRummyCard, FourSuitRummyPlayerItem, FourSuitRummySaveInfo>, IMiscDataNM, IStartNewGame
    {


        private readonly FourSuitRummyVMData _model;
        private readonly CommandContainer _command; //most of the time, needs this.  if not needed, take out.
        private readonly FourSuitRummyGameContainer _gameContainer; //if we don't need it, take it out.

        public FourSuitRummyMainGameClass(IGamePackageResolver mainContainer,
            IEventAggregator aggregator,
            BasicData basicData,
            TestOptions test,
            FourSuitRummyVMData currentMod,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            ICardInfo<RegularRummyCard> cardInfo,
            CommandContainer command,
            FourSuitRummyGameContainer gameContainer)
            : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer)
        {
            _model = currentMod;
            _command = command;
            _gameContainer = gameContainer;

        }
        public override Task PopulateSaveRootAsync()
        {
            FourSuitRummyPlayerItem self = PlayerList!.GetSelf();
            self.AdditionalCards = _model.TempSets!.ListAllObjects();
            PlayerList.ForEach(thisPlayer =>
            {
                thisPlayer.SetList = thisPlayer.MainSets!.SavedSets();
            });

            return base.PopulateSaveRootAsync();
        }

        public override Task FinishGetSavedAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.MainSets = new MainSets(thisPlayer, _gameContainer);
                int x = thisPlayer.SetList.Count; //the first time, actually load manually.
                x.Times(items =>
                {
                    SetInfo thisSet = new SetInfo(_command);
                    thisPlayer.MainSets.CreateNewSet(thisSet);
                });
                thisPlayer.MainSets.LoadSets(thisPlayer.SetList); //hopefully this works (?)
                if (thisPlayer.AdditionalCards.Count > 0)
                {
                    thisPlayer.MainHandList.AddRange(thisPlayer.AdditionalCards); //later sorts anyways.
                    thisPlayer.AdditionalCards.Clear(); //i think.
                }
            });
            SingleInfo = PlayerList.GetSelf(); //hopefully won't cause problems.
            SortCards(); //has to be this way this time.
            Aggregator.Subscribe(SingleInfo); //i think
            IsLoaded = true; //try this too.
            return base.FinishGetSavedAsync();
        }
        protected override async Task ComputerTurnAsync()
        {
            //if there is nothing, then just won't do anything.
            await Task.CompletedTask;
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            SaveRoot!.TimesReshuffled = 0;
            if (PlayerList.First().MainSets == null)
            {
                PlayerList!.ForEach(thisPlayer =>
                {
                    thisPlayer.MainSets = new MainSets(thisPlayer, _gameContainer);
                });
            }
            return base.StartSetUpAsync(isBeginning);
        }
        protected override Task LastPartOfSetUpBeforeBindingsAsync()
        {
            SingleInfo = PlayerList!.GetSelf();
            Aggregator.Subscribe(SingleInfo); //because it gets cleared out each round.
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.MainSets!.ClearBoard(); //this too.
                thisPlayer.SetList.Clear(); //i think this too
                thisPlayer.AdditionalCards.Clear(); //try this too.
            }); //tempsets has to be cleared later.
            return base.LastPartOfSetUpBeforeBindingsAsync();
        }

        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status) //can't do switch because we don't know what the cases are ahead of time.
            {
                case "finishedsets":
                    await CreateSetsAsync(content);
                    await ContinueTurnAsync();
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task EndTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.MainHandList.UnhighlightObjects(); //i think this is best.
            if (CanReshuffle == false)
            {
                await EndRoundAsync();
                return;
            }
            _command.ManuelFinish = true; //because it could be somebody else's turn.
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        private async Task CreateSetsAsync(string message)
        {
            var firstTemp = await js.DeserializeObjectAsync<CustomBasicList<string>>(message);
            SingleInfo = PlayerList!.GetWhoPlayer(); //trying this as well.
            foreach (var thisFirst in firstTemp)
            {
                CustomBasicList<int> thisCol = await js.DeserializeObjectAsync<CustomBasicList<int>>(thisFirst);
                thisCol.ForEach(deck =>
                {
                    CustomBasicList<int> otherList = SingleInfo.MainHandList.GetDeckListFromObjectList();
                    string thisstr = "";
                    otherList.ForEach(thisItem =>
                    {
                        thisstr += thisItem + ",";
                    });
                    if (SingleInfo.MainHandList.ObjectExist(deck) == false)
                        throw new BasicBlankException($"Deck of {deck} does not exist.  Player Is {SingleInfo.NickName}. ids are {thisstr} ");
                });
                var ThisCol = await thisFirst.GetObjectsFromDataAsync(SingleInfo.MainHandList);
                AddSet(ThisCol);
            }
        }
        public bool CanProcessDiscard(out bool pickUp, out int index, out int deck, out string message)
        {
            message = "";
            index = 0;
            deck = 0;
            if (_gameContainer.AlreadyDrew == false && _gameContainer.PreviousCard == 0)
                pickUp = true;
            else
                pickUp = false;
            if (pickUp == true)
            {
                if (_model!.PlayerHand1!.HowManySelectedObjects > 0 || _model.TempSets!.HowManySelectedObjects > 0)
                {
                    message = "There is at least one card selected.  Unselect all the cards before picking up from discard";
                    return false;
                }
                return true;
            }
            var thisCol = _model!.PlayerHand1!.ListSelectedObjects();
            var otherCol = _model.TempSets!.ListSelectedObjects();
            if (thisCol.Count == 0 && otherCol.Count == 0)
            {
                message = "Sorry, you must select a card to discard";
                return false;
            }
            if (thisCol.Count + otherCol.Count > 1)
            {
                message = "Sorry, you can only select one card to discard";
                return false;
            }
            if (thisCol.Count == 0)
            {
                index = _model.TempSets.PileForSelectedObject;
                deck = _model.TempSets.DeckForSelectedObjected(index);
            }
            else
                deck = _model.PlayerHand1.ObjectSelected();
            var thisCard = _gameContainer.DeckList!.GetSpecificItem(deck);
            if (thisCard.Deck == _gameContainer.PreviousCard && SingleInfo!.ObjectCount > 1)
            {
                message = "Sorry, you cannot discard the one that was picked up since there is more than one card to choose from";
                return false;
            }
            return true;
        }
        private void UnselectCards()
        {
            if (SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
                return;
            _model!.PlayerHand1!.EndTurn();
            _model.TempSets!.EndTurn();
        }
        public override async Task DiscardAsync(RegularRummyCard thisCard)
        {
            RemoveCard(thisCard.Deck);
            await AnimatePlayAsync(thisCard);
            UnselectCards();
            if (SingleInfo!.ObjectCount == 0 || Test!.EndRoundEarly) //going to try to trust the object count.
            {
                await EndRoundAsync();
                return;
            }
            await EndTurnAsync();
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
            if (SingleInfo.MainHandList.ObjectExist(deck) == false)
                throw new BasicBlankException($"{deck} did not exist  count was {SingleInfo.MainHandList.Count} and the name of the player was {SingleInfo.NickName}");
            SingleInfo.MainHandList.RemoveObjectByDeck(deck);
        }
        public bool CanReshuffle => SaveRoot!.TimesReshuffled < 2;
        public override async Task EndRoundAsync()
        {
            CalculateScore();
            _model!.TempSets!.ClearBoard(); //try here too.
            if (PlayerList.Any(items => items.TotalScore >= 1000))
            {
                SingleInfo = PlayerList.OrderByDescending(items => items.TotalScore).First();
                await ShowWinAsync();
                return;
            }
            await this.RoundOverNextAsync(); //well see.
        }
        private void CalculateScore()
        {
            int seconds;
            int firsts;
            int scores;
            if (SingleInfo!.ObjectCount == 0)
            {
                if (WhoTurn == 1)
                    seconds = ScoreHand(2);
                else
                    seconds = ScoreHand(1);
                firsts = SingleInfo.MainSets!.CalculateScore;
                scores = firsts + seconds;
                SingleInfo.CurrentScore = scores;
                SingleInfo.TotalScore += scores;
                int newTurn;
                if (WhoTurn == 1)
                    newTurn = 2;
                else
                    newTurn = 1;
                var tempPlayer = PlayerList![newTurn];
                firsts = tempPlayer.MainSets!.CalculateScore;
                tempPlayer.CurrentScore = firsts;
                tempPlayer.TotalScore += firsts;
                return;
            }
            PlayerList!.ForEach(thisPlayer =>
            {
                firsts = thisPlayer.MainSets!.CalculateScore;
                thisPlayer.CurrentScore = firsts;
                thisPlayer.TotalScore += firsts;
            });
        }
        private DeckRegularDict<RegularRummyCard> PlayerHand()
        {
            var output = SingleInfo!.MainHandList.ToRegularDeckDict();
            if (SingleInfo.PlayerCategory != EnumPlayerCategory.Self)
                return output;
            output.AddRange(_model!.TempSets!.ListAllObjects());
            output.ForEach(thisCard =>
            {
                if (thisCard.Value == EnumCardValueList.HighAce)
                    thisCard.Points = 15;
                else if (thisCard.Value >= EnumCardValueList.Ten)
                    thisCard.Points = 10;
                else
                    thisCard.Points = (int)thisCard.Value;
            });
            return output;
        }
        private int ScoreHand(int player)
        {
            int whos = WhoTurn;
            WhoTurn = player;
            SingleInfo = PlayerList!.GetWhoPlayer();
            var hands = PlayerHand();
            int points = hands.Sum(items => items.Points);
            WhoTurn = whos;
            SingleInfo = PlayerList.GetWhoPlayer();
            return points;
        }
        Task IStartNewGame.ResetAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.TotalScore = 0;
                thisPlayer.CurrentScore = 0;
            });
            return Task.CompletedTask;
        }
        public void AddSet(IDeckDict<RegularRummyCard> thisCol)
        {
            SingleInfo!.MainSets!.AddNewSet(thisCol); //no need to update hand anymore.  hopefully i am right by assuming.
        }
        private bool HasRun(IDeckDict<RegularRummyCard> thisCol)
        {
            if (thisCol.Count != 3)
                return false; //must have 3 cards period to consider for run.
            if (thisCol.DistinctCount(items => items.Suit) != 1)
                return false; //must be same suit each time.
            DeckRegularDict<RegularRummyCard> aceList = thisCol.Where(items => items.Value == EnumCardValueList.LowAce || items.Value == EnumCardValueList.HighAce).ToRegularDeckDict();
            DeckRegularDict<RegularRummyCard> tempCol = thisCol.Where(items => items.Value != EnumCardValueList.LowAce
            && items.Value != EnumCardValueList.HighAce).OrderBy(items => items.Value).ToRegularDeckDict();
            int x = 0;
            int previousNumber = 0;
            int currentNumber;
            int diffs;
            foreach (var thisCard in tempCol)
            {
                x++;
                if (x == 1)
                {
                    previousNumber = (int)thisCard.Value;
                    currentNumber = (int)thisCard.Value;
                }
                else
                {
                    currentNumber = (int)thisCard.Value;
                    diffs = currentNumber - previousNumber - 1;
                    if (diffs > 0)
                    {
                        if (aceList.Count < diffs)
                            return false;
                        for (int y = 1; y <= diffs; y++)
                        {
                            aceList.RemoveFirstItem();
                        }
                    }
                    previousNumber = currentNumber;
                }
            }
            return true;
        }
        public CustomBasicList<int> SetList()
        {
            CustomBasicList<int> output = new CustomBasicList<int>();
            DeckRegularDict<RegularRummyCard> thisCollection;
            IDeckDict<RegularRummyCard> tempCollection; //hopefully still works (?)
            for (int x = 1; x <= _model!.TempSets!.HowManySets; x++)
            {
                tempCollection = _model.TempSets.ObjectList(x);
                thisCollection = new DeckRegularDict<RegularRummyCard>();

                if (tempCollection.Count > 0)
                    thisCollection.AddRange(tempCollection);
                if (HasRun(thisCollection))
                    output.Add(x);
            }
            if (Test!.DoubleCheck == true && output.Count > 1)
                throw new BasicBlankException("can't have more than one for now");
            return output;
        }

    }
}