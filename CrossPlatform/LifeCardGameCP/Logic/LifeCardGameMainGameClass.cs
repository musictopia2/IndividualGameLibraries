using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.Extensions; //most likely will be used.
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceMessages;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using LifeCardGameCP.Cards;
using LifeCardGameCP.Data;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.

namespace LifeCardGameCP.Logic
{
    [SingletonGame]
    public class LifeCardGameMainGameClass : CardGameClass<LifeCardGameCardInformation, LifeCardGamePlayerItem, LifeCardGameSaveInfo>, IMiscDataNM
    {


        private readonly LifeCardGameVMData _model;
        private readonly CommandContainer _command; //most of the time, needs this.  if not needed, take out.
        private readonly LifeCardGameGameContainer _gameContainer; //if we don't need it, take it out.



        public LifeCardGameMainGameClass(IGamePackageResolver mainContainer,
            IEventAggregator aggregator,
            BasicData basicData,
            TestOptions test,
            LifeCardGameVMData currentMod,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            ICardInfo<LifeCardGameCardInformation> cardInfo,
            CommandContainer command,
            LifeCardGameGameContainer gameContainer)
            : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer)
        {
            _model = currentMod;
            _command = command;
            _gameContainer = gameContainer;
            _gameContainer.ChosePlayerAsync = ChosePlayerAsync;
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
        private int _otherPlayerDraws;
        public override async Task FinishGetSavedAsync()
        {
            CreateLifeStories(); //even for autoresume needs to be done now.  if autoresume, can later resume it.
            await PlayerList!.ForEachAsync(async thisPlayer =>
            {
                var thisList = await js.DeserializeObjectAsync<DeckObservableDict<LifeCardGameCardInformation>>(thisPlayer.LifeString);
                thisPlayer.LifeStory!.HandList = new DeckObservableDict<LifeCardGameCardInformation>(thisList);
            });
            await PlayerList.RepositionCardsAsync(this, _gameContainer, _model); //i think.
            await base.FinishGetSavedAsync();
        }
        private void CreateLifeStories()
        {
            PlayerList!.ForEach(thisPlayer => _gameContainer.CreateLifeStoryPile(_model, thisPlayer));
        }
        public override async Task PopulateSaveRootAsync()
        {
            await PlayerList!.ForEachAsync(async thisPlayer =>
            {
                thisPlayer.LifeString = await js.SerializeObjectAsync(thisPlayer.LifeStory!.HandList);
            });

            await base.PopulateSaveRootAsync();
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            if (isBeginning)
                CreateLifeStories(); //hopefully doing here is fine (not sure).
            else
            {
                PlayerList!.ForEach(thisPlayer => thisPlayer.LifeStory!.HandList.Clear());
            }
            SaveRoot!.ImmediatelyStartTurn = true;
            SaveRoot.YearList.Clear();
            PlayerList!.ForEach(thisPlayer => thisPlayer.Points = 0);
            return base.StartSetUpAsync(isBeginning);
        }
        protected override Task LastPartOfSetUpBeforeBindingsAsync()
        {
            if (PlayerList!.HasYears())
                throw new BasicBlankException("Players cannot have years");
            _model!.Pile1!.ClearCards();
            _model.CurrentPile!.ClearCards(); //just in case.
            SaveRoot!.CurrentCard = 0; //just in case here too.
            var newList = _gameContainer.YearCards();
            _model.Deck1!.ShuffleInExtraCards(newList);
            var tempList = _model.Deck1.DeckList().ToRegularDeckDict();
            if (tempList.Count(items => items.CanBeInPlayerHandToBeginWith == false) == 0)
                throw new BasicBlankException("There was no years passed in the entire deck.  Really rethink");
            if (tempList.Count(items => items.CanBeInPlayerHandToBeginWith == false) != 8)
                throw new BasicBlankException("Must have 6 years passed in the deck.  Rethink"); //if worst comes to worst, i have to put back in manually.

            return base.LastPartOfSetUpBeforeBindingsAsync();
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            LifeCardGameCardInformation thisCard;
            switch (status)
            {
                case "turnbacktime":
                    DeckRegularDict<LifeCardGameCardInformation> thisList = await content.GetNewObjectListFromDeckListAsync(_gameContainer.DeckList!); //hopefully this is fine too.
                    await FinishedTurningBackTimeAsync(thisList);
                    return;
                case "playcard":
                    thisCard = SingleInfo!.MainHandList.GetSpecificItem(int.Parse(content));
                    await PlayCardAsync(thisCard);
                    return;
                case "choseplayer":
                    await ChosePlayerAsync(int.Parse(content));
                    return;
                case "cardchosen":
                    thisCard = _gameContainer.DeckList!.GetSpecificItem(int.Parse(content));
                    await ChoseSingleCardAsync(thisCard);
                    return;
                case "cardstraded":
                    var thisTrade = await js.DeserializeObjectAsync<TradeCard>(content);
                    var yourCard = _gameContainer.DeckList!.GetSpecificItem(thisTrade.YourCard);
                    var opponentCard = _gameContainer.DeckList.GetSpecificItem(thisTrade.OtherCard);
                    await TradeCardsAsync(yourCard, opponentCard);
                    return;
                case "lifeswap":
                    var thisSwap = await js.DeserializeObjectAsync<Swap>(content);
                    await FinishLifeSwapAsync(thisSwap.Player, thisSwap.YourCards, thisSwap.OtherCards);
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            _model!.CurrentPile!.ClearCards();
            SingleInfo = PlayerList!.GetWhoPlayer();
            await PlayerList.RepositionCardsAsync(this, _gameContainer, _model);
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task EndTurnAsync()
        {
            _command.ManuelFinish = true; //because it could be somebody else's turn.
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync(true);
            await StartNewTurnAsync();
        }
        protected override void GetPlayerToContinueTurn()
        {
            if (OtherTurn > 0)
                SingleInfo = PlayerList!.GetOtherPlayer();
            else
                base.GetPlayerToContinueTurn();
        }
        public override async Task DiscardAsync(LifeCardGameCardInformation ThisCard)
        {
            if (SingleInfo!.MainHandList.Count == 5)
                SingleInfo.MainHandList.RemoveObjectByDeck(ThisCard.Deck);
            await AnimatePlayAsync(ThisCard);
            await DrawAtLastAsync();
        }
        private async Task DrawAtLastAsync()
        {
            LifeCardGamePlayerItem thisPlayer;
            if (_otherPlayerDraws > 0)
                thisPlayer = PlayerList![_otherPlayerDraws];
            else
            {
                SingleInfo = PlayerList!.GetWhoPlayer();
                thisPlayer = SingleInfo;
            }
            await CheckDeckAsync();
            SingleInfo!.Points = SingleInfo.LifeStory!.TotalPoints();
            OtherTurn = 0;
            _model!.CurrentPile!.ClearCards();
            if (SaveRoot!.YearList.Count == 6)
            {
                SingleInfo = PlayerList.OrderByDescending(items => items.Points).First();
                await ShowWinAsync();
                return;
            }
            thisPlayer.MainHandList.Add(_model.Deck1!.DrawCard());
            if (thisPlayer.MainHandList.Count != 5)
                throw new BasicBlankException("Must have 5 cards in hand after drawing");
            if (thisPlayer.PlayerCategory == EnumPlayerCategory.Self)
                thisPlayer.MainHandList.Sort(); //i think this should be fine.
            await EndTurnAsync();
        }
        private async Task CheckDeckAsync()
        {
            int oldDeck = 0;
            do
            {
                var nextCard = _model!.Deck1!.RevealCard();
                if (nextCard.Points > 0)
                    return;
                nextCard = _model.Deck1.DrawCard();
                if (nextCard.Points > 0)
                    throw new BasicBlankException("Can't have 0 points");
                if (oldDeck == nextCard.Deck)
                    throw new BasicBlankException("A card is repeating");
                oldDeck = nextCard.Deck;
                SaveRoot!.YearList.Add(nextCard.Deck);
                await UIPlatform.ShowMessageAsync($"Time's Flying.  Total years is {SaveRoot.YearsPassed()}.  Therefore; another card is being drawn.");
                if (SaveRoot.YearList.Count == 6)
                    return;
            } while (true);
        }
        private async Task FinishedTurningBackTimeAsync(IEnumerableDeck<LifeCardGameCardInformation> thisList) //i think we need this parameter now.
        {
            _model!.Deck1!.OriginalList(thisList);
            _model.CurrentPile!.ClearCards();
            await PlayerList!.RepositionCardsAsync(this, _gameContainer, _model);
            await DrawAtLastAsync();
        }
        private async Task TurnBackTimeAsync()
        {
            await UIPlatform.ShowMessageAsync("Turn back time.  Therefore; the deck is being reshuffled");
            var thisCard = _gameContainer.DeckList!.GetSpecificItem(SaveRoot!.YearList.Last());
            SaveRoot.YearList.RemoveLastItem(); //i think.
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.OtherHuman)
            {
                Check!.IsEnabled = true;
                return;
            }
            var thisList = _model.Deck1!.DeckList();
            thisList.Add(thisCard);
            thisList.ShuffleList();
            if (BasicData!.MultiPlayer)
                await Network!.SendAllAsync("turnbacktime", thisList.Select(items => items.Deck).ToCustomBasicList());
            await FinishedTurningBackTimeAsync(thisList);
        }
        private void RecalculatePoints(LifeCardGamePlayerItem otherPlayer)
        {
            var thisCard = _model!.CurrentPile!.GetCardInfo();
            if (thisCard.OpponentKeepsCard)
                otherPlayer.LifeStory!.AddCard(thisCard);
            otherPlayer.Points = otherPlayer.LifeStory!.TotalPoints();
        }
        private async Task ShowOtherPlayerAsync(LifeCardGamePlayerItem otherPlayer)
        {
            _gameContainer!.PlayerChosen = otherPlayer.Id;
            _gameContainer.CardChosen = null;
            await PlayerList!.RepositionCardsAsync(this, _gameContainer, _model);
            if (Test!.NoAnimations == false)
                await Delay!.DelaySeconds(.75);
            _gameContainer.PlayerChosen = 0;
            await PlayerList!.RepositionCardsAsync(this, _gameContainer, _model);
        }
        private async Task DiscardPassportAsync(LifeCardGamePlayerItem otherPlayer)
        {
            var thisCard = otherPlayer.LifeStory!.HandList.Single(items => items.SpecialCategory == EnumSpecialCardCategory.Passport);
            otherPlayer.LifeStory.RemoveCard(thisCard);
            RecalculatePoints(otherPlayer);
            await AnimatePlayAsync(thisCard); //i think it needs to go to the discard pile.  old version had that bug.
            await DrawAtLastAsync();
        }
        private async Task TakePaydayAsync(LifeCardGamePlayerItem otherPlayer)
        {
            var thisCard = otherPlayer.LifeStory!.HandList.First(items => items.IsPayday() == true);
            otherPlayer.LifeStory.RemoveCard(thisCard);
            RecalculatePoints(otherPlayer);
            SingleInfo!.LifeStory!.AddCard(thisCard);
            SingleInfo.Points = SingleInfo.LifeStory.TotalPoints();
            await DrawAtLastAsync();
        }
        private async Task DiscardPaydayAsync(LifeCardGamePlayerItem otherPlayer)
        {
            var thisCard = otherPlayer.LifeStory!.HandList.First(items => items.IsPayday() == true);
            otherPlayer.LifeStory.RemoveCard(thisCard);
            RecalculatePoints(otherPlayer);
            await AnimatePlayAsync(thisCard);
            await DrawAtLastAsync();
        }
        private async Task TakeCardAsync(LifeCardGameCardInformation thisCard)
        {
            var tempPlayer = _gameContainer!.PlayerWithCard(thisCard);
            await ShowOtherCardAsync(tempPlayer, thisCard);
            tempPlayer.LifeStory!.RemoveCard(thisCard);
            RecalculatePoints(tempPlayer);
            SingleInfo!.LifeStory!.AddCard(thisCard);
            SingleInfo.Points = SingleInfo.LifeStory.TotalPoints();
            await DrawAtLastAsync();
        }
        private async Task ShowOtherCardAsync(LifeCardGamePlayerItem tempPlayer, LifeCardGameCardInformation thisCard)
        {
            _gameContainer!.PlayerChosen = tempPlayer.Id;
            thisCard.IsSelected = false;
            _gameContainer.CardChosen = thisCard;
            await PlayerList!.RepositionCardsAsync(this, _gameContainer, _model);
            if (Test!.NoAnimations == false)
                await Delay!.DelaySeconds(.75);
            _gameContainer.PlayerChosen = 0;
            _gameContainer.CardChosen = null;
            await PlayerList!.RepositionCardsAsync(this, _gameContainer, _model);
        }
        private async Task DonateToCharityAsync(LifeCardGameCardInformation thisCard)
        {
            var tempPlayer = _gameContainer!.PlayerWithCard(thisCard);
            await ShowOtherCardAsync(tempPlayer, thisCard);
            tempPlayer.LifeStory!.RemoveCard(thisCard);
            RecalculatePoints(tempPlayer);
            await AnimatePlayAsync(thisCard);
            await DrawAtLastAsync();
        }
        private async Task FinishLifeSwapAsync(int otherPlayer, CustomBasicList<int> yourList, CustomBasicList<int> otherList)
        {
            var tempPlayer = PlayerList![otherPlayer];
            var opponentCardList = otherList.Select(items => _gameContainer.DeckList!.GetSpecificItem(items)).ToRegularDeckDict();
            var yourCardList = yourList.Select(items => _gameContainer.DeckList!.GetSpecificItem(items)).ToRegularDeckDict();
            yourList.ForEach(index => SingleInfo!.MainHandList.RemoveObjectByDeck(index));
            otherList.ForEach(index => tempPlayer.MainHandList.RemoveObjectByDeck(index));
            SingleInfo!.MainHandList.AddRange(opponentCardList);
            tempPlayer.MainHandList.AddRange(yourCardList);
            if (tempPlayer.MainHandList.Count != 5)
                throw new BasicBlankException("Other player must have 5 cards in hand");
            if (SingleInfo.MainHandList.Count != 4)
                throw new BasicBlankException("Your hand must have 4 cards in hand");
            if (tempPlayer.PlayerCategory == EnumPlayerCategory.Self)
                tempPlayer.MainHandList.Sort();
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                SingleInfo.MainHandList.Sort();
            await DrawAtLastAsync();
        }
        private async Task SwapEntireHandAsync(LifeCardGamePlayerItem otherPlayer)
        {
            var yourHand = SingleInfo!.MainHandList.ToRegularDeckDict();
            var opponentHand = otherPlayer.MainHandList.ToRegularDeckDict();
            SingleInfo.MainHandList.ReplaceRange(opponentHand);
            otherPlayer.MainHandList.ReplaceRange(yourHand);
            _otherPlayerDraws = otherPlayer.Id;
            otherPlayer.LifeStory!.AddCard(_model!.CurrentPile!.GetCardInfo());
            if (otherPlayer.PlayerCategory == EnumPlayerCategory.Self)
                otherPlayer.MainHandList.Sort();
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                SingleInfo.MainHandList.Sort();
            await DrawAtLastAsync();
        }
        private async Task LifeSwapAsync(LifeCardGamePlayerItem otherPlayer)
        {
            await ShowOtherPlayerAsync(otherPlayer);
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.OtherHuman)
            {
                Check!.IsEnabled = true;
                return;
            }
            var yourRandomList = SingleInfo.MainHandList.GetRandomList(false, 2);
            var opponentRandomList = otherPlayer.MainHandList.GetRandomList(false, 2);
            Swap thisS = new Swap();
            thisS.Player = otherPlayer.Id;
            thisS.YourCards = yourRandomList.Select(items => items.Deck).ToCustomBasicList();
            thisS.OtherCards = opponentRandomList.Select(items => items.Deck).ToCustomBasicList();
            if (BasicData!.MultiPlayer)
                await Network!.SendAllAsync("lifeswap", thisS);
            await FinishLifeSwapAsync(otherPlayer.Id, thisS.YourCards, thisS.OtherCards);
        }
        public async Task ChosePlayerAsync(int player)
        {
            if (_model!.CurrentPile!.PileEmpty())
                throw new BasicBlankException("There is no card in current pile");
            var thisCard = _model.CurrentPile.GetCardInfo();
            OtherTurn = player;
            _gameContainer!.PlayerChosen = OtherTurn;
            var otherPlayer = PlayerList!.GetOtherPlayer();
            await PlayerList.RepositionCardsAsync(this, _gameContainer, _model);
            if (Test!.NoAnimations == false)
                await Delay!.DelaySeconds(.5);
            _gameContainer.PlayerChosen = 0;
            await PlayerList.RepositionCardsAsync(this, _gameContainer, _model);
            if (thisCard.Action == EnumAction.MidlifeCrisis)
            {
                await SwapEntireHandAsync(otherPlayer);
                return;
            }
            if (thisCard.Action == EnumAction.LifeSwap)
            {
                await LifeSwapAsync(otherPlayer);
                return;
            }
            if (thisCard.Action == EnumAction.LostPassport)
            {
                await DiscardPassportAsync(otherPlayer);
                return;
            }
            if (thisCard.Action == EnumAction.IMTheBoss)
            {
                await TakePaydayAsync(otherPlayer);
                return;
            }
            if (thisCard.Action == EnumAction.YoureFired)
            {
                await DiscardPaydayAsync(otherPlayer);
                return;
            }
            await ContinueTurnAsync();
        }
        public async Task ChoseSingleCardAsync(LifeCardGameCardInformation thisCard)
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            if (_model!.CurrentPile!.PileEmpty())
                throw new BasicBlankException("There is no card in current pile");
            var otherCard = _model.CurrentPile.GetCardInfo();
            switch (otherCard.Action)
            {
                case EnumAction.AdoptBaby:
                case EnumAction.LongLostRelative:
                case EnumAction.Lawsuit:
                case EnumAction.YourStory:
                case EnumAction.SecondChance:
                    await TakeCardAsync(thisCard);
                    return;
                case EnumAction.DonateToCharity:
                    await DonateToCharityAsync(thisCard);
                    break;
                default:
                    throw new BasicBlankException("Don't know what to do about choose single card now");
            }
        }
        public async Task TradeCardsAsync(LifeCardGameCardInformation yourCard, LifeCardGameCardInformation opponentCard)
        {
            if (_model!.CurrentPile!.PileEmpty())
                throw new BasicBlankException("There is no card in current pile");
            var thisCard = _model.CurrentPile.GetCardInfo();
            if (thisCard.Action == EnumAction.None)
                throw new BasicBlankException("Must have action in order to trade cards");
            if (SingleInfo!.LifeStory!.HandList.ObjectExist(yourCard.Deck) == false)
                throw new BasicBlankException("Don't have the requested card in your life story");
            if (thisCard.Action == EnumAction.CareerSwap || thisCard.Action == EnumAction.MovingHouse || thisCard.Action == EnumAction.MixUpAtVets)
            {
                _gameContainer!.TradeList!.Clear();
                _gameContainer.TradeList.Add(yourCard);
                _gameContainer.TradeList.Add(opponentCard);
                _gameContainer.PlayerChosen = 0;
                await PlayerList!.RepositionCardsAsync(this, _gameContainer, _model);
                if (Test!.NoAnimations == false)
                    await Delay!.DelaySeconds(1);
                var otherPlayer = _gameContainer.PlayerWithCard(opponentCard);
                otherPlayer.LifeStory!.RemoveCard(opponentCard);
                otherPlayer.LifeStory.AddCard(yourCard);
                SingleInfo.LifeStory.RemoveCard(yourCard);
                SingleInfo.LifeStory.AddCard(opponentCard);
                _gameContainer.TradeList.Clear();
                RecalculatePoints(otherPlayer);
                SingleInfo.Points = SingleInfo.LifeStory.TotalPoints();
                await DrawAtLastAsync();
                return;
            }
            throw new BasicBlankException("Should not be trading cards");
        }
        public async Task PlayCardAsync(LifeCardGameCardInformation thisCard)
        {
            if (OtherTurn > 0)
                throw new BasicBlankException("Can't play a card when its otherturn.  Try creating a routine to process the otherturn");
            if (thisCard.Points == 0)
                throw new BasicBlankException("Must have at least 5 points");
            SingleInfo!.MainHandList.RemoveObjectByDeck(thisCard.Deck);
            _model!.CurrentPile!.AddCard(thisCard);
            if (thisCard.OpponentKeepsCard == false)
            {
                SingleInfo.LifeStory!.AddCard(thisCard);
                _gameContainer!.PlayerChosen = WhoTurn;
                _gameContainer.CardChosen = thisCard;
                await PlayerList!.RepositionCardsAsync(this, _gameContainer, _model);
                SingleInfo.Points = SingleInfo.LifeStory.TotalPoints();
                if (Test!.NoAnimations == false)
                    await Delay!.DelaySeconds(1);
                _gameContainer.PlayerChosen = 0;
                _gameContainer.CardChosen = null;
                await PlayerList!.RepositionCardsAsync(this, _gameContainer, _model);
                if (thisCard.Action == EnumAction.TurnBackTime)
                {
                    await TurnBackTimeAsync();
                    return;
                }
                if (thisCard.Action == EnumAction.LifeSwap)
                {
                    await ContinueTurnAsync();
                    return;
                }
                _model.CurrentPile.ClearCards();
                await PlayerList!.RepositionCardsAsync(this, _gameContainer, _model);
                await DrawAtLastAsync();
                return;
            }
            if (thisCard.Action == EnumAction.None)
                throw new BasicBlankException("Must have an action card currently if an opponent is keeping this card");
            await PlayerList!.RepositionCardsAsync(this, _gameContainer, _model);
            await ContinueTurnAsync();
        }
        public bool CanPlayCard(LifeCardGameCardInformation thisCard)
        {
            if (thisCard.OnlyOneAllowed())
                return !SingleInfo!.LifeStory!.HandList.Any(items => items.SpecialCategory == thisCard.SpecialCategory);
            if (thisCard.Requirement != EnumSpecialCardCategory.None && thisCard.Action != EnumAction.MovingHouse)
                return SingleInfo!.LifeStory!.HandList.Any(items => items.SpecialCategory == thisCard.Requirement);
            if (thisCard.IsPayday())
            {
                var careers = SingleInfo!.LifeStory!.HandList.Count(items => items.SwitchCategory == EnumSwitchCategory.Career);
                var pays = SingleInfo.LifeStory.HandList.Count(items => items.IsPayday());
                var maxs = careers * 3;
                var actual = pays + 1;
                return actual <= maxs;
            }
            if (thisCard.Action == EnumAction.TurnBackTime)
                return SaveRoot!.YearList.Count > 0;
            if (thisCard.Action == EnumAction.None || thisCard.Action == EnumAction.LifeSwap || thisCard.Action == EnumAction.MidlifeCrisis)
                return true;
            var opponentList = PlayerList!.OpponentStory();
            if (thisCard.Action == EnumAction.AdoptBaby)
                return opponentList.Any(items => items.SwitchCategory == EnumSwitchCategory.Baby);
            if (thisCard.Action == EnumAction.CareerSwap)
            {
                if (SingleInfo!.LifeStory!.HandList.Any(items => items.SwitchCategory == EnumSwitchCategory.Career) == false)
                    return false; //because you have no careers to swap with.
                return opponentList.Any(items => items.SwitchCategory == EnumSwitchCategory.Career);
            }
            if (thisCard.Action == EnumAction.DonateToCharity)
                return opponentList.Any(items => items.Category == EnumFirstCardCategory.Wealth && items.Points > 5 && items.SpecialCategory != EnumSpecialCardCategory.Passport);
            if (thisCard.Action == EnumAction.IMTheBoss || thisCard.Action == EnumAction.YoureFired)
                return opponentList.Any(items => items.IsPayday());
            if (thisCard.Action == EnumAction.Lawsuit)
                return opponentList.Any(items => items.Points >= 30 && items.SpecialCategory != EnumSpecialCardCategory.Marriage);
            if (thisCard.Action == EnumAction.LongLostRelative)
                return opponentList.Any(items => items.Category == EnumFirstCardCategory.Family && items.SpecialCategory != EnumSpecialCardCategory.Marriage && items.Points > 5);
            if (thisCard.Action == EnumAction.LostPassport)
                return opponentList.Any(items => items.SpecialCategory == EnumSpecialCardCategory.Passport);
            if (thisCard.Action == EnumAction.MixUpAtVets)
            {
                if (SingleInfo!.LifeStory!.HandList.Any(items => items.SwitchCategory == EnumSwitchCategory.Pet) == false)
                    return false;
                return opponentList.Any(items => items.SwitchCategory == EnumSwitchCategory.Pet);
            }
            if (thisCard.Action == EnumAction.MovingHouse)
            {
                if (SingleInfo!.LifeStory!.HandList.Any(items => items.SpecialCategory == EnumSpecialCardCategory.House) == false)
                    return false;
                return opponentList.Any(items => items.SpecialCategory == EnumSpecialCardCategory.House);
            }
            if (thisCard.Action == EnumAction.SecondChance)
                return opponentList.Any(items => items.Points >= 10 && items.Points <= 30 && items.SpecialCategory != EnumSpecialCardCategory.Passport);
            if (thisCard.Action == EnumAction.YourStory)
                return opponentList.Any(items => items.Category == EnumFirstCardCategory.Adventure && items.Points != 5);
            throw new BasicBlankException("Don't know if it can play the card or not?");
        }
    }
}
