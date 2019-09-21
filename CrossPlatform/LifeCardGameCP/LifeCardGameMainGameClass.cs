using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.

namespace LifeCardGameCP
{
    [SingletonGame]
    public class LifeCardGameMainGameClass : CardGameClass<LifeCardGameCardInformation, LifeCardGamePlayerItem, LifeCardGameSaveInfo>, IMiscDataNM
    {
        public LifeCardGameMainGameClass(IGamePackageResolver container) : base(container) { }
        internal LifeCardGameViewModel? ThisMod;
        private int _otherPlayerDraws;
        internal GlobalClass? ThisGlobal;
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
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            ThisMod = MainContainer.Resolve<LifeCardGameViewModel>();
            ThisGlobal = MainContainer.Resolve<GlobalClass>();
        }
        public override async Task FinishGetSavedAsync()
        {
            CreateLifeStories(); //even for autoresume needs to be done now.  if autoresume, can later resume it.
            await PlayerList!.ForEachAsync(async thisPlayer =>
            {
                var thisList = await js.DeserializeObjectAsync<DeckObservableDict<LifeCardGameCardInformation>>(thisPlayer.LifeString);
                thisPlayer.LifeStory!.HandList = new DeckObservableDict<LifeCardGameCardInformation>(thisList);
            });
            PlayerList.RepositionCards(this); //i think.
            await base.FinishGetSavedAsync();
        }
        private void CreateLifeStories()
        {
            PlayerList!.ForEach(thisPlayer => ThisGlobal!.CreateLifeStoryPile(ThisMod!, thisPlayer));
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
        internal DeckRegularDict<LifeCardGameCardInformation> YearCards() => DeckList.Where(items => items.CanBeInPlayerHandToBeginWith == false).ToRegularDeckDict();
        protected override Task LastPartOfSetUpBeforeBindingsAsync()
        {
            if (PlayerList!.HasYears())
                throw new BasicBlankException("Players cannot have years");
            ThisMod!.Pile1!.ClearCards();
            ThisMod.CurrentPile!.ClearCards(); //just in case.
            SaveRoot!.CurrentCard = 0; //just in case here too.
            var newList = YearCards();
            ThisMod.Deck1!.ShuffleInExtraCards(newList);
            var tempList = ThisMod.Deck1.DeckList().ToRegularDeckDict();
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
                    DeckRegularDict<LifeCardGameCardInformation> thisList = await content.GetNewObjectListFromDeckListAsync(DeckList!); //hopefully this is fine too.
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
                    thisCard = DeckList!.GetSpecificItem(int.Parse(content));
                    await ChoseSingleCardAsync(thisCard);
                    return;
                case "cardstraded":
                    var thisTrade = await js.DeserializeObjectAsync<TradeCard>(content);
                    var yourCard = DeckList!.GetSpecificItem(thisTrade.YourCard);
                    var opponentCard = DeckList.GetSpecificItem(thisTrade.OtherCard);
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
            ThisMod!.CurrentPile!.ClearCards();
            SingleInfo = PlayerList!.GetWhoPlayer();
            PlayerList.RepositionCards(this);
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task EndTurnAsync()
        {
            ThisMod!.CommandContainer!.ManuelFinish = true; //because it could be somebody else's turn.
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
            ThisMod!.CurrentPile!.ClearCards();
            if (SaveRoot!.YearList.Count == 6)
            {
                SingleInfo = PlayerList.OrderByDescending(items => items.Points).First();
                await ShowWinAsync();
                return;
            }
            thisPlayer.MainHandList.Add(ThisMod.Deck1!.DrawCard());
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
                var nextCard = ThisMod!.Deck1!.RevealCard();
                if (nextCard.Points > 0)
                    return;
                nextCard = ThisMod.Deck1.DrawCard();
                if (nextCard.Points > 0)
                    throw new BasicBlankException("Can't have 0 points");
                if (oldDeck == nextCard.Deck)
                    throw new BasicBlankException("A card is repeating");
                oldDeck = nextCard.Deck;
                SaveRoot!.YearList.Add(nextCard.Deck);
                await ThisMod.ShowGameMessageAsync($"Time's Flying.  Total years is {SaveRoot.YearsPassed()}.  Therefore; another card is being drawn.");
                if (SaveRoot.YearList.Count == 6)
                    return;
            } while (true);
        }
        private async Task FinishedTurningBackTimeAsync(IEnumerableDeck<LifeCardGameCardInformation> thisList) //i think we need this parameter now.
        {
            ThisMod!.Deck1!.OriginalList(thisList);
            ThisMod.CurrentPile!.ClearCards();
            PlayerList!.RepositionCards(this);
            await DrawAtLastAsync();
        }
        private async Task TurnBackTimeAsync()
        {
            await ThisMod!.ShowGameMessageAsync("Turn back time.  Therefore; the deck is being reshuffled");
            var thisCard = DeckList!.GetSpecificItem(SaveRoot!.YearList.Last());
            SaveRoot.YearList.RemoveLastItem(); //i think.
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.OtherHuman)
            {
                ThisCheck!.IsEnabled = true;
                return;
            }
            var thisList = ThisMod.Deck1!.DeckList();
            thisList.Add(thisCard);
            thisList.ShuffleList();
            if (ThisData!.MultiPlayer)
                await ThisNet!.SendAllAsync("turnbacktime", thisList.Select(items => items.Deck).ToCustomBasicList());
            await FinishedTurningBackTimeAsync(thisList);
        }
        private void RecalculatePoints(LifeCardGamePlayerItem otherPlayer)
        {
            var thisCard = ThisMod!.CurrentPile!.GetCardInfo();
            if (thisCard.OpponentKeepsCard)
                otherPlayer.LifeStory!.AddCard(thisCard);
            otherPlayer.Points = otherPlayer.LifeStory!.TotalPoints();
        }
        private async Task ShowOtherPlayerAsync(LifeCardGamePlayerItem otherPlayer)
        {
            ThisGlobal!.PlayerChosen = otherPlayer.Id;
            ThisGlobal.CardChosen = null;
            PlayerList!.RepositionCards(this);
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(.75);
            ThisGlobal.PlayerChosen = 0;
            PlayerList!.RepositionCards(this);
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
            var tempPlayer = ThisGlobal!.PlayerWithCard(thisCard);
            await ShowOtherCardAsync(tempPlayer, thisCard);
            tempPlayer.LifeStory!.RemoveCard(thisCard);
            RecalculatePoints(tempPlayer);
            SingleInfo!.LifeStory!.AddCard(thisCard);
            SingleInfo.Points = SingleInfo.LifeStory.TotalPoints();
            await DrawAtLastAsync();
        }
        private async Task ShowOtherCardAsync(LifeCardGamePlayerItem tempPlayer, LifeCardGameCardInformation thisCard)
        {
            ThisGlobal!.PlayerChosen = tempPlayer.Id;
            thisCard.IsSelected = false;
            ThisGlobal.CardChosen = thisCard;
            PlayerList!.RepositionCards(this);
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(.75);
            ThisGlobal.PlayerChosen = 0;
            ThisGlobal.CardChosen = null;
            PlayerList!.RepositionCards(this);
        }
        private async Task DonateToCharityAsync(LifeCardGameCardInformation thisCard)
        {
            var tempPlayer = ThisGlobal!.PlayerWithCard(thisCard);
            await ShowOtherCardAsync(tempPlayer, thisCard);
            tempPlayer.LifeStory!.RemoveCard(thisCard);
            RecalculatePoints(tempPlayer);
            await AnimatePlayAsync(thisCard);
            await DrawAtLastAsync();
        }
        private async Task FinishLifeSwapAsync(int otherPlayer, CustomBasicList<int> yourList, CustomBasicList<int> otherList)
        {
            var tempPlayer = PlayerList![otherPlayer];
            var opponentCardList = otherList.Select(items => DeckList!.GetSpecificItem(items)).ToRegularDeckDict();
            var yourCardList = yourList.Select(items => DeckList!.GetSpecificItem(items)).ToRegularDeckDict();
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
            otherPlayer.LifeStory!.AddCard(ThisMod!.CurrentPile!.GetCardInfo());
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
                ThisCheck!.IsEnabled = true;
                return;
            }
            var yourRandomList = SingleInfo.MainHandList.GetRandomList(false, 2);
            var opponentRandomList = otherPlayer.MainHandList.GetRandomList(false, 2);
            Swap thisS = new Swap();
            thisS.Player = otherPlayer.Id;
            thisS.YourCards = yourRandomList.Select(items => items.Deck).ToCustomBasicList();
            thisS.OtherCards = opponentRandomList.Select(items => items.Deck).ToCustomBasicList();
            if (ThisData!.MultiPlayer)
                await ThisNet!.SendAllAsync("lifeswap", thisS);
            await FinishLifeSwapAsync(otherPlayer.Id, thisS.YourCards, thisS.OtherCards);
        }
        public async Task ChosePlayerAsync(int player)
        {
            if (ThisMod!.CurrentPile!.PileEmpty())
                throw new BasicBlankException("There is no card in current pile");
            var thisCard = ThisMod.CurrentPile.GetCardInfo();
            OtherTurn = player;
            ThisGlobal!.PlayerChosen = OtherTurn;
            var otherPlayer = PlayerList!.GetOtherPlayer();
            PlayerList.RepositionCards(this);
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(.5);
            ThisGlobal.PlayerChosen = 0;
            PlayerList.RepositionCards(this);
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
            if (ThisMod!.CurrentPile!.PileEmpty())
                throw new BasicBlankException("There is no card in current pile");
            var otherCard = ThisMod.CurrentPile.GetCardInfo();
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
            if (ThisMod!.CurrentPile!.PileEmpty())
                throw new BasicBlankException("There is no card in current pile");
            var thisCard = ThisMod.CurrentPile.GetCardInfo();
            if (thisCard.Action == EnumAction.None)
                throw new BasicBlankException("Must have action in order to trade cards");
            if (SingleInfo!.LifeStory!.HandList.ObjectExist(yourCard.Deck) == false)
                throw new BasicBlankException("Don't have the requested card in your life story");
            if (thisCard.Action == EnumAction.CareerSwap || thisCard.Action == EnumAction.MovingHouse || thisCard.Action == EnumAction.MixUpAtVets)
            {
                ThisGlobal!.TradeList!.Clear();
                ThisGlobal.TradeList.Add(yourCard);
                ThisGlobal.TradeList.Add(opponentCard);
                ThisGlobal.PlayerChosen = 0;
                PlayerList!.RepositionCards(this);
                if (ThisTest!.NoAnimations == false)
                    await Delay!.DelaySeconds(1);
                var otherPlayer = ThisGlobal.PlayerWithCard(opponentCard);
                otherPlayer.LifeStory!.RemoveCard(opponentCard);
                otherPlayer.LifeStory.AddCard(yourCard);
                SingleInfo.LifeStory.RemoveCard(yourCard);
                SingleInfo.LifeStory.AddCard(opponentCard);
                ThisGlobal.TradeList.Clear();
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
            ThisMod!.CurrentPile!.AddCard(thisCard);
            if (thisCard.OpponentKeepsCard == false)
            {
                SingleInfo.LifeStory!.AddCard(thisCard);
                ThisGlobal!.PlayerChosen = WhoTurn;
                ThisGlobal.CardChosen = thisCard;
                PlayerList!.RepositionCards(this);
                SingleInfo.Points = SingleInfo.LifeStory.TotalPoints();
                if (ThisTest!.NoAnimations == false)
                    await Delay!.DelaySeconds(1);
                ThisGlobal.PlayerChosen = 0;
                ThisGlobal.CardChosen = null;
                PlayerList!.RepositionCards(this);
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
                ThisMod.CurrentPile.ClearCards();
                PlayerList!.RepositionCards(this);
                await DrawAtLastAsync();
                return;
            }
            if (thisCard.Action == EnumAction.None)
                throw new BasicBlankException("Must have an action card currently if an opponent is keeping this card");
            PlayerList!.RepositionCards(this);
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