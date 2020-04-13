using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicEventModels;
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
using BasicGameFrameworkLibrary.SpecializedGameTypes.TrickClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using Pinochle2PlayerCP.Cards;
using Pinochle2PlayerCP.Data;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace Pinochle2PlayerCP.Logic
{
    [SingletonGame]
    public class Pinochle2PlayerMainGameClass
        : TrickGameClass<EnumSuitList, Pinochle2PlayerCardInformation, Pinochle2PlayerPlayerItem, Pinochle2PlayerSaveInfo>
        , IMiscDataNM, IStartNewGame
    {


        private readonly Pinochle2PlayerVMData _model;
        private readonly CommandContainer _command; //most of the time, needs this.  if not needed, take out.
        private readonly Pinochle2PlayerGameContainer _gameContainer; //if we don't need it, take it out.
        private readonly ITrickPlay _trickPlay;
        private readonly IAdvancedTrickProcesses _aTrick;

        public Pinochle2PlayerMainGameClass(IGamePackageResolver mainContainer,
            IEventAggregator aggregator,
            BasicData basicData,
            TestOptions test,
            Pinochle2PlayerVMData currentMod,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            ICardInfo<Pinochle2PlayerCardInformation> cardInfo,
            CommandContainer command,
            Pinochle2PlayerGameContainer gameContainer,
            ITrickData trickData,
            ITrickPlay trickPlay,
            IAdvancedTrickProcesses aTrick
            )
            : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, trickData, trickPlay)
        {
            _model = currentMod;
            _command = command;
            _gameContainer = gameContainer;
            _trickPlay = trickPlay;
            _aTrick = aTrick;
        }


        public override async Task FinishGetSavedAsync()
        {
            LoadControls();
            LoadVM();
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
            Aggregator.Subscribe(SingleInfo); //i think
            if (SaveRoot!.StartMessage != "")
                await UIPlatform.ShowMessageAsync(SaveRoot.StartMessage);
            await base.FinishGetSavedAsync();
            _aTrick!.LoadGame();
        }
        protected override async Task LastPartOfSetUpBeforeBindingsAsync()
        {
            if (IsLoaded == false)
            {
                SingleInfo = PlayerList!.GetSelf();
                Aggregator.Subscribe(SingleInfo);
                SingleInfo = PlayerList.GetWhoPlayer(); //i think.
            }
            LoadControls();
            LoadVM();
            SaveRoot!.MeldList.Clear();
            SaveRoot.CardList.Clear();
            SaveRoot.TrumpSuit = _model.Pile1!.GetCardInfo().Suit;
            _aTrick!.ClearBoard(); //try this too.
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.TricksWon = 0;
                thisPlayer.CurrentScore = 0;
            });
            var thisCard = _model.Pile1.GetCardInfo();
            if (thisCard.Value == EnumCardValueList.Nine)
            {
                int player = WhoStarts;
                if (player == 1)
                    player = 2;
                else
                    player = 1;
                MeldClass thisMeld = new MeldClass();
                thisMeld.Player = player;
                thisMeld.ClassAValue = EnumClassA.Dix;
                SaveRoot.MeldList.Add(thisMeld);
                var tempPlayer = PlayerList[player];
                SaveRoot.StartMessage = $"{tempPlayer.NickName} gets an exta 10 points because the discard to begin with is a dix and is not going first";
                CalculateScore(player);
                await UIPlatform.ShowMessageAsync(SaveRoot.StartMessage);
            }
            await base.LastPartOfSetUpBeforeBindingsAsync();
        }
        public override Task PopulateSaveRootAsync()
        {
            Pinochle2PlayerPlayerItem self = PlayerList!.GetSelf();
            self.AdditionalCards = _model!.TempSets!.ListAllObjects();
            return base.PopulateSaveRootAsync();
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;

            IsLoaded = true; //i think needs to be here.
        }
        protected override async Task ComputerTurnAsync()
        {
            if (Test!.NoAnimations == false)
                await Delay!.DelaySeconds(.75);
            if (SaveRoot!.ChooseToMeld)
            {
                await EndTurnAsync(); //because the computer player will never meld cards.
                return;
            }
            var tempList = SingleInfo!.MainHandList.Where(items => IsValidMove(items.Deck)).ToRegularDeckDict();
            await PlayCardAsync(tempList.GetRandomItem().Deck);
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            SaveRoot!.ChooseToMeld = false;
            return base.StartSetUpAsync(isBeginning);
        }

        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status) //can't do switch because we don't know what the cases are ahead of time.
            {
                case "meld":
                    var tempList = await content.GetSavedIntegerListAsync();
                    await MeldAsync(tempList);
                    return;
                case "exchangediscard":
                    await ExchangeDiscardAsync(int.Parse(content));
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        protected override int PossibleOtherSelected(int firstChosen, out string message)
        {
            message = ""; //this represents the message that will be a messagebox.
            int others;
            int tempCounts;
            others = _model!.YourMelds!.ObjectSelected();
            tempCounts = _model.TempSets!.HowManySelectedObjects;
            if (tempCounts == 0 && others == 0)
                return firstChosen;
            if (others > 0 && firstChosen > 0)
            {
                message = "You can choose from the melds pile or from hand but not both";
                return 0; //try this way.
            }
            if (others > 0 && tempCounts > 0 || firstChosen > 0 && tempCounts > 0)
            {
                message = "You can choose a card from hand or tempsets but not both";
                return 0;
            }
            if (tempCounts > 1)
            {
                message = "Can only play one card at a time from the tempsets for the trick";
                return 0;
            }
            int decks = firstChosen;
            if (decks == 0 && others > 0)
                return others;
            if (decks == 0 && tempCounts > 0)
                return tempCounts;
            return decks;
        }
        public override async Task EndTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.MainHandList.UnhighlightObjects(); //i think this is best.
            _command.ManuelFinish = true; //because it could be somebody else's turn.
            _model.TempSets!.EndTurn();
            var thisCard = _model.Deck1!.DrawCard();
            thisCard.Drew = true;
            SingleInfo.MainHandList.Add(thisCard); //after clearing rest.
            int newTurn;
            if (WhoTurn == 1)
                newTurn = 2;
            else
                newTurn = 1;
            var tempPlayer = PlayerList[newTurn];
            if (_model.Deck1.IsEndOfDeck())
            {
                thisCard = _model.Pile1!.GetCardInfo();
                _model.Pile1.ClearCards();
            }
            else
                thisCard = _model.Deck1.DrawCard();
            thisCard.Drew = true;
            tempPlayer.MainHandList.Add(thisCard);
            if (_model.Deck1.IsEndOfDeck())
                MeldsBackInHand();
            SingleInfo = PlayerList.GetSelf();
            SortCards();
            SingleInfo = PlayerList.GetWhoPlayer();
            await StartNewTrickAsync(); //i guess this is how its done.
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            SaveRoot!.ChooseToMeld = false;
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task ContinueTrickAsync()
        {
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            await StartNewTurnAsync(); //i think.
        }
        private int WhoWonTrick(DeckObservableDict<Pinochle2PlayerCardInformation> thisCol)
        {
            var leadCard = thisCol.First();
            var thisCard = thisCol.Last();
            if (thisCard.Suit == SaveRoot!.TrumpSuit && leadCard.Suit != SaveRoot.TrumpSuit)
                return WhoTurn;
            if (leadCard.Suit == SaveRoot.TrumpSuit && thisCard.Suit != SaveRoot.TrumpSuit)
                return leadCard.Player;
            if (thisCard.Suit == leadCard.Suit)
            {
                if (thisCard.PinochleCardValue > leadCard.PinochleCardValue)
                    return WhoTurn;
            }
            return leadCard.Player;
        }
        private void CalculateScore(int player)
        {
            var firstPoints = SaveRoot!.CardList.Where(items => items.Player == player).Sum(items => items.Points);
            var secondPoints = SaveRoot.MeldList.Where(items => items.Player == player).Sum(items => (int)items.ClassAValue + (int)items.ClassBValue + (int)items.ClassCValue);
            var thisPlayer = PlayerList![player];
            thisPlayer.CurrentScore = firstPoints + secondPoints;
        }
        private int CardsLeft()
        {
            var thisPlayer = PlayerList!.GetWhoPlayer();
            if (_model!.Pile1!.PileEmpty() == true)
                return thisPlayer.ObjectCount;
            int counts = SaveRoot!.MeldList.Where(items => items.Player == WhoTurn).Select(items => items.CardList.Count).ToCustomBasicList().Count;
            return counts = thisPlayer.ObjectCount;
        }
        public override async Task EndTrickAsync()
        {
            var trickList = SaveRoot!.TrickList;
            int wins = WhoWonTrick(trickList);
            Pinochle2PlayerPlayerItem thisPlayer = PlayerList![wins];
            thisPlayer.TricksWon++;
            await _aTrick!.AnimateWinAsync(wins);
            trickList.ForEach(card =>
            {
                card.Points = card.PinochleCardValue;
                card.Player = wins;
            });

            SaveRoot.CardList.AddRange(trickList);
            CalculateScore(wins);
            WhoTurn = wins; //most of the time, whoever wins leads again.
            SingleInfo = PlayerList.GetWhoPlayer();
            int lefts = CardsLeft();
            if (lefts == 0)
            {
                if (_model!.Pile1!.PileEmpty() == false)
                    throw new BasicBlankException("Never went to end");
                SingleInfo.CurrentScore += 10;
                await UIPlatform.ShowMessageAsync($"{SingleInfo.NickName} gets an extra 10 points for winning the last trick");
                await EndRoundAsync();
                return;
            }
            SingleInfo = PlayerList.GetSelf();
            _model!.PlayerHand1!.EndTurn();
            SortCards();
            SingleInfo = PlayerList.GetWhoPlayer();
            if (_model.Pile1!.PileEmpty() == false)
            {
                SaveRoot.ChooseToMeld = true;
                this.ShowTurn();
                await ContinueTurnAsync();
                return;
            }
            await StartNewTrickAsync();
        }
        private async Task StartNewTrickAsync()
        {
            _aTrick!.ClearBoard();
            _command!.ManuelFinish = true; //because it could be somebody else's turn.
            await StartNewTurnAsync(); //hopefully this simple.
        }
        public override Task ContinueTurnAsync()
        {
            SaveRoot!.StartMessage = "";
            ReloadMeldFrames();
            return base.ContinueTurnAsync();
        }
        protected override async Task PlayCardAsync(int deck)
        {
            bool hadCard = false;
            if (SingleInfo!.MainHandList.ObjectExist(deck))
            {
                SingleInfo.MainHandList.RemoveObjectByDeck(deck);
                hadCard = true;
            }
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
            {
                if (_model!.TempSets!.HasObject(deck))
                {
                    _model.TempSets.RemoveObject(deck);
                    hadCard = true;
                }
            }
            if (hadCard == false)
            {
                var thisCard = _gameContainer.DeckList!.GetSpecificItem(deck);
                var thisMeld = GetMeldFromCard(thisCard);

                SaveRoot!.MeldList.ForEach(tempMeld =>
                {
                    tempMeld.CardList.RemoveAllOnly(items => items == deck);
                });
            }
            await _trickPlay!.PlayCardAsync(deck);
        }
        public override bool IsValidMove(int deck)
        {
            if (_model!.Pile1!.PileEmpty())
                return true; //if there is nothing in the pile, you can play anything no matter what
            return base.IsValidMove(deck);
        }
        public MeldClass GetMeldFromCard(Pinochle2PlayerCardInformation thisCard) => SaveRoot!.MeldList.First(thisMeld => thisMeld.Player == WhoTurn && thisMeld.CardList.Any(items => thisCard.Deck == items));
        public override bool CanEnableTrickAreas => !SaveRoot!.ChooseToMeld;
        Task IStartNewGame.ResetAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.CurrentScore = 0;
                thisPlayer.TotalScore = 0;
            });
            return Task.CompletedTask;
        }
        private DeckRegularDict<Pinochle2PlayerCardInformation> GetPlayerMeldList(int player)
        {
            var thisList = SaveRoot!.MeldList.Where(items => items.Player == player).Select(items => items.CardList).ToCustomBasicList();
            DeckRegularDict<Pinochle2PlayerCardInformation> output = new DeckRegularDict<Pinochle2PlayerCardInformation>();
            thisList.ForEach(firstTemp =>
            {
                var tempList = firstTemp.GetNewObjectListFromDeckList(_gameContainer.DeckList!);
                tempList.ForEach(thisCard =>
                {
                    if (output.ObjectExist(thisCard.Deck) == false)
                        output.Add(thisCard);
                });
            });
            return output;
        }
        private void MeldsBackInHand()
        {
            2.Times(x =>
            {
                var tempPlayer = PlayerList![x];
                tempPlayer.MainHandList.AddRange(GetPlayerMeldList(x));
                if (tempPlayer.PlayerCategory == EnumPlayerCategory.Self)
                {
                    var thisList = _model!.TempSets!.ListObjectsRemoved();
                    tempPlayer.MainHandList.AddRange(thisList);
                }
                if (tempPlayer.MainHandList.Count != 12)
                    throw new BasicBlankException("Must have 12 cards in hand at end");
            });
        }
        private void ReloadMeldFrames()
        {
            _model!.YourMelds!.HandList.Clear();
            _model.OpponentMelds!.HandList.Clear();
            if (_model.Pile1!.PileEmpty())
            {
                _model.YourMelds.Visible = false; //i think
                _model.OpponentMelds.Visible = false;
                return;
            }
            _model.YourMelds.Visible = true;
            _model.OpponentMelds.Visible = true;
            int myID = PlayerList!.GetSelf().Id;
            2.Times(x =>
            {
                var thisList = GetPlayerMeldList(x);
                thisList.ForEach(thisCard =>
                {
                    thisCard.IsUnknown = false;
                    thisCard.Visible = true;
                });
                if (myID == 1 && x == 1 || myID == 2 && x == 2)
                {
                    if (_model.YourMelds.IgnoreMaxRules == false)
                        throw new BasicBlankException("Failed To Ignore Max Rules");
                    _model.YourMelds.PopulateObjects(thisList);
                }
                else
                    _model.OpponentMelds.PopulateObjects(thisList);
            });
        }
        public async Task ExchangeDiscardAsync(int deck)
        {
            var thisCard = _model!.Pile1!.GetCardInfo();
            _model.Pile1.RemoveFromPile();
            await Aggregator.AnimatePickUpDiscardAsync(thisCard);
            var newCard = _gameContainer.DeckList!.GetSpecificItem(deck);
            thisCard.Drew = true;
            SingleInfo!.MainHandList.Add(thisCard);
            MeldClass thisMeld = new MeldClass();
            thisMeld.ClassAValue = EnumClassA.Dix;
            thisMeld.Player = WhoTurn;
            SaveRoot!.MeldList.Add(thisMeld);
            CalculateScore(WhoTurn);
            if (SingleInfo.MainHandList.ObjectExist(deck))
                SingleInfo.MainHandList.RemoveObjectByDeck(deck);
            else
                _model.TempSets!.RemoveObject(deck);
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                SortCards();
            await AnimatePlayAsync(newCard);
            await ContinueTurnAsync();
        }
        private bool DidWinGame
        {
            get
            {
                if (PlayerList.Count() != 2)
                    throw new BasicBlankException("Must have 2 players only");
                if (PlayerList.First().TotalScore == PlayerList.Last().TotalScore)
                    return false;
                SingleInfo = PlayerList.OrderByDescending(items => items.TotalScore).First();
                return true;
            }
        }
        public override async Task EndRoundAsync()
        {
            PlayerList!.ForEach(thisPlayer => thisPlayer.TotalScore += thisPlayer.CurrentScore);
            if (PlayerList.Any(items => items.TotalScore >= SaveRoot!.GameOverAt))
            {
                if (DidWinGame == false)
                    SaveRoot!.GameOverAt += 200;
                else
                {
                    await this.ShowWinAsync();
                    return;
                }
            }
            await this.RoundOverNextAsync();
        }
        private bool HasNewMeld(MeldClass oldMeld, MeldClass newMeld)
        {
            if (oldMeld.ClassAValue != EnumClassA.None && newMeld.ClassAValue != EnumClassA.None)
                return false;
            if (oldMeld.ClassBValue != EnumClassB.None && newMeld.ClassBValue != EnumClassB.None)
                return false;
            if (oldMeld.ClassCValue != EnumClassC.None && newMeld.ClassCValue != EnumClassC.None)
                return false;
            return true;
        }
        private void FirstProcessMeld(CustomBasicList<int> thisList)
        {
            var thisMeld = SaveRoot!.MeldList.Where(items => items.Player == WhoTurn && items.CardList.Any(fins => thisList.Contains(fins))).FirstOrDefault();
            var temps = thisList.GetNewObjectListFromDeckList(_gameContainer.DeckList!);
            if (thisMeld == null)
            {
                temps.ForEach(thisCard =>
                {
                    thisCard.Drew = true;
                    thisCard.IsSelected = false;
                });
                thisMeld = GetMeldFromList(temps);
                thisMeld.Player = WhoTurn;
                thisMeld.CardList = thisList;
                SaveRoot.MeldList.Add(thisMeld);
                return; //i think
            }
            var tempMeld = GetMeldFromList(temps);
            var nextList = thisList.Where(items => thisMeld.CardList.Any(fins => fins == items) == false).ToCustomBasicList();
            var firstTemp = SaveRoot.MeldList.ToCustomBasicList();
            firstTemp.RemoveSpecificItem(thisMeld);
            nextList.RemoveAllOnly(items =>
            {
                return firstTemp.Any(fins =>
                {
                    return fins.CardList.Any(z => z == items);
                });
            });
            if (thisMeld.ClassAValue != EnumClassA.None && tempMeld.ClassAValue != EnumClassA.None && tempMeld.ClassAValue > thisMeld.ClassAValue)
                thisMeld.ClassAValue = tempMeld.ClassAValue; // maybe this was missing.
            else if (thisMeld.ClassBValue != EnumClassB.None && tempMeld.ClassBValue != EnumClassB.None && tempMeld.ClassBValue > thisMeld.ClassBValue)
                thisMeld.ClassBValue = tempMeld.ClassBValue;
            else if (thisMeld.ClassCValue != EnumClassC.None && tempMeld.ClassCValue != EnumClassC.None && tempMeld.ClassCValue > thisMeld.ClassCValue)
                thisMeld.ClassCValue = tempMeld.ClassCValue;
            else if (HasNewMeld(thisMeld, tempMeld) == true)
            {
                tempMeld.Player = WhoTurn;
                tempMeld.CardList = thisList;
                SaveRoot.MeldList.Add(tempMeld);
                return;
            }
            thisMeld.CardList.AddRange(nextList); //hopefully this works.
        }
        public MeldClass GetMeldFromList(IDeckDict<Pinochle2PlayerCardInformation> thisList)
        {
            MeldClass defaultMeld = new MeldClass();
            defaultMeld.ClassAValue = EnumClassA.None;
            defaultMeld.ClassBValue = EnumClassB.None;
            defaultMeld.ClassCValue = EnumClassC.None;
            if (thisList.Count == 1 && thisList.Single().Value == EnumCardValueList.Nine && thisList.Single().Suit == SaveRoot!.TrumpSuit)
            {
                defaultMeld.ClassAValue = EnumClassA.Dix;
                return defaultMeld;
            }
            if (thisList.Any(items => items.Value == EnumCardValueList.Nine))
                return defaultMeld;
            int suitCount = thisList.GroupBy(items => items.Suit).Count();
            int numberCount = thisList.GroupBy(items => items.Value).Count();
            if (suitCount == 1 && numberCount == 5 && thisList.First().Suit == SaveRoot!.TrumpSuit)
            {
                defaultMeld.ClassAValue = EnumClassA.Flush;
                return defaultMeld;
            }
            if (suitCount == 4 && numberCount == 1 && thisList.Count == 4)
            {
                defaultMeld.ClassBValue = thisList.First().Value switch
                {
                    EnumCardValueList.Jack => EnumClassB.J,
                    EnumCardValueList.Queen => EnumClassB.Q,
                    EnumCardValueList.King => EnumClassB.K,
                    EnumCardValueList.HighAce => EnumClassB.A,
                    _ => EnumClassB.None,
                };
                return defaultMeld;
            }
            if (thisList.Count == 2)
            {
                if (thisList.All(items =>
                {
                    if (items.Value == EnumCardValueList.Jack && items.Suit == EnumSuitList.Diamonds)
                        return true;
                    if (items.Value == EnumCardValueList.Queen && items.Suit == EnumSuitList.Spades)
                        return true;
                    return false;
                }))
                {
                    defaultMeld.ClassCValue = EnumClassC.Pinochle;
                    return defaultMeld;
                }
                if (suitCount == 1 && numberCount == 2)
                {
                    if (thisList.All(items =>
                    {
                        if (items.Value == EnumCardValueList.Queen || items.Value == EnumCardValueList.King)
                            return true;
                        return false;
                    }))
                    {
                        if (thisList.First().Suit == SaveRoot!.TrumpSuit)
                            defaultMeld.ClassAValue = EnumClassA.RoyalMarriage;
                        else
                            defaultMeld.ClassAValue = EnumClassA.Marriage;
                        return defaultMeld;
                    }
                }
                defaultMeld.ClassAValue = EnumClassA.None;
            }
            return defaultMeld;
        }
        public async Task MeldAsync(CustomBasicList<int> thisList)
        {
            FirstProcessMeld(thisList);
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
            {
                thisList.ForEach(thisCard =>
                {
                    if (_model!.TempSets!.HasObject(thisCard))
                        _model.TempSets.RemoveObject(thisCard);
                });
            }
            thisList.ForEach(thisCard =>
            {
                if (SingleInfo.MainHandList.ObjectExist(thisCard))
                    SingleInfo.MainHandList.RemoveObjectByDeck(thisCard);
            });
            CalculateScore(WhoTurn);
            ReloadMeldFrames();
            if (Test!.NoAnimations == false)
                await Delay!.DelaySeconds(.5);
            if (thisList.Count == 1)
            {
                await UIPlatform.ShowMessageAsync("Since the dix was played; can make one more meld");
                await ContinueTurnAsync();
                return;
            }
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                _model!.YourMelds!.EndTurn();
            else
                _model!.OpponentMelds!.EndTurn();
            CalculateScore(WhoTurn); //just in case.
            await EndTurnAsync();
        }
    }
}
