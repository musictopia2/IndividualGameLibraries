using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace SixtySix2PlayerCP
{
    [SingletonGame]
    public class SixtySix2PlayerMainGameClass : TrickGameClass<EnumSuitList, SixtySix2PlayerCardInformation,
        SixtySix2PlayerPlayerItem, SixtySix2PlayerSaveInfo>, IMiscDataNM, IStartNewGame
    {
        private IAdvancedTrickProcesses? _aTrick;
        public SixtySix2PlayerMainGameClass(IGamePackageResolver container) : base(container) { }
        internal TwoPlayerTrickViewModel<EnumSuitList, SixtySix2PlayerCardInformation, SixtySix2PlayerPlayerItem, SixtySix2PlayerSaveInfo>? Trick1;
        public SixtySix2PlayerViewModel? ThisMod;

        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            ThisMod = MainContainer.Resolve<SixtySix2PlayerViewModel>();
        }

        public override async Task FinishGetSavedAsync()
        {
            LoadControls();
            LoadVM();
            SaveRoot!.LoadMod(ThisMod!);
            await base.FinishGetSavedAsync();
            _aTrick!.LoadGame();
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            LoadTrickAreas();
            _aTrick = MainContainer.Resolve<IAdvancedTrickProcesses>(); //decided to be here.
            Trick1 = MainContainer.Resolve<TwoPlayerTrickViewModel<EnumSuitList, SixtySix2PlayerCardInformation, SixtySix2PlayerPlayerItem, SixtySix2PlayerSaveInfo>>();
            IsLoaded = true; //i think needs to be here.
        }
        protected override async Task ComputerTurnAsync()
        {
            if (SingleInfo!.MainHandList.Count == 0)
                throw new BasicBlankException("It should have gone to a new round automatically");
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(.25);
            var tempList = SingleInfo.MainHandList.Where(items => IsValidMove(items.Deck)).ToRegularDeckDict();
            await PlayCardAsync(tempList.GetRandomItem().Deck);
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            LoadControls();
            LoadVM();
            SaveRoot!.LoadMod(ThisMod!);
            SaveRoot.LastTrickWon = 0;
            SaveRoot.CardList.Clear();
            _aTrick!.ClearBoard(); //try this too.
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.MarriageList.Clear();
                thisPlayer.FirstMarriage = EnumMarriage.None;
                thisPlayer.TricksWon = 0;
                thisPlayer.ScoreRound = 0;
            });
            return base.StartSetUpAsync(isBeginning);
        }
        protected override Task LastPartOfSetUpBeforeBindingsAsync()
        {
            SaveRoot!.TrumpSuit = ThisMod!.Pile1!.GetCardInfo().Suit;
            return base.LastPartOfSetUpBeforeBindingsAsync();
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                //put in cases here.
                case "exchangediscard":
                    await ExchangeDiscardAsync(int.Parse(content));
                    return;
                case "announcemarriage":
                    CustomBasicList<int> thisList = await js.DeserializeObjectAsync<CustomBasicList<int>>(content);
                    await AnnounceMarriageAsync(thisList);
                    return;
                case "goout":
                    await GoOutAsync();
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task EndTurnAsync()
        {
            await Task.CompletedTask;
            throw new BasicBlankException("Should never have called the endturn.  Rethink");
        }
        public override async Task EndRoundAsync()
        {
            await Task.CompletedTask;
            throw new BasicBlankException("EndRound should not be called directly");
        }
        public override bool IsValidMove(int deck)
        {
            if (SaveRoot!.CardsForMarriage.Count > 0)
            {
                if (SaveRoot.CardsForMarriage.Any(items => items == deck) == false)
                {
                    PlayErrorMessage = "Since a marriage was announced; must choose one of those cards to play";
                    return false; //can't show a custom message unfortunately anymore.
                }
            }
            if (ThisMod!.Pile1!.PileEmpty())
                return true; //if there is nothing in the pile, you can play anything no matter what
            return base.IsValidMove(deck);
        }
        protected override Task PlayCardAsync(int deck)
        {
            SaveRoot!.CardsForMarriage.Clear();
            return base.PlayCardAsync(deck);
        }
        public override async Task ContinueTrickAsync()
        {
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            await StartNewTurnAsync(); //i think.
        }
        private int WhoWonTrick(DeckObservableDict<SixtySix2PlayerCardInformation> thisCol)
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
            var thisPlayer = PlayerList![player];
            var secondPoints = thisPlayer.MarriageList.Sum(items => (int)items);
            if (SaveRoot.LastTrickWon == player)
                firstPoints += 10;
            thisPlayer.ScoreRound = firstPoints + secondPoints;
        }
        private void DrawPlayerCards()
        {
            var thisCard = ThisMod!.Deck1!.DrawCard();
            thisCard.Drew = true;
            SingleInfo!.MainHandList.Add(thisCard);
            int newTurn;
            if (WhoTurn == 1)
                newTurn = 2;
            else
                newTurn = 1;
            var thisPlayer = PlayerList![newTurn];
            if (ThisMod.Deck1.IsEndOfDeck())
            {
                thisCard = ThisMod.Pile1!.GetCardInfo();
                ThisMod.Pile1.ClearCards();
            }
            else
            {
                thisCard = ThisMod.Deck1.DrawCard();
            }

            thisCard.Drew = true;
            thisPlayer.MainHandList.Add(thisCard);
        }
        public override async Task EndTrickAsync()
        {
            var trickList = SaveRoot!.TrickList;
            int wins = WhoWonTrick(trickList);
            SixtySix2PlayerPlayerItem ThisPlayer = PlayerList![wins];
            ThisPlayer.TricksWon++;
            await _aTrick!.AnimateWinAsync(wins);
            trickList.ForEach(thisCard =>
            {
                thisCard.Points = thisCard.PinochleCardValue;
                thisCard.Player = wins;
            });
            SaveRoot.CardList.AddRange(trickList);
            WhoTurn = wins; //most of the time, whoever wins leads again.
            SingleInfo = PlayerList.GetWhoPlayer();
            if (SingleInfo.FirstMarriage != EnumMarriage.None)
                SingleInfo.MarriageList.Add(SingleInfo.FirstMarriage);
            SingleInfo.FirstMarriage = EnumMarriage.None;
            int lefts = SingleInfo.MainHandList.Count;
            if (lefts == 0)
            {
                if (ThisMod!.Pile1!.PileEmpty() == false)
                    throw new BasicBlankException("Never went to end");
                SaveRoot.LastTrickWon = WhoTurn;
                await ThisMod.ShowGameMessageAsync($"{SingleInfo.NickName} gets an extra 10 points for winning the last trick");
            }
            CalculateScore(wins);
            if (lefts == 0)
            {
                SaveRoot.BonusPoints++;
                this.RoundOverNext();
                return;
            }
            if (ThisMod!.Pile1!.PileEmpty() == false)
                DrawPlayerCards();

            SingleInfo = PlayerList.GetSelf();
            ThisMod.PlayerHand1!.EndTurn();
            SortCards();
            SingleInfo = PlayerList.GetWhoPlayer();
            await StartNewTrickAsync();
        }
        private async Task StartNewTrickAsync()
        {
            _aTrick!.ClearBoard();
            ThisMod!.CommandContainer!.ManuelFinish = true; //because it could be somebody else's turn.
            await StartNewTurnAsync(); //hopefully this simple.
        }
        Task IStartNewGame.ResetAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.GamePointsGame = 0;
                thisPlayer.GamePointsRound = 0;
            });
            return Task.CompletedTask;
        }
        public bool CanExchangeForDiscard()
        {
            if (ThisMod!.Pile1!.PileEmpty() == true)
                return false;
            var thisCard = ThisMod.Pile1.GetCardInfo();
            if (thisCard.Value == EnumCardValueList.Nine)
                return false;
            return SaveRoot!.CardList.Any(items => items.Player == WhoTurn);
        }
        private int CalculateGamePoints()
        {
            if (SingleInfo!.ScoreRound < 66)
                return -2; //means opponent will get them.
            int otherPlayer;
            if (WhoTurn == 1)
                otherPlayer = 2;
            else
                otherPlayer = 1;
            if (!SaveRoot!.CardList.Any(items => items.Player == otherPlayer))
                return 3 + SaveRoot.BonusPoints;
            var thisPlayer = PlayerList![otherPlayer];
            if (thisPlayer.ScoreRound < 33)
                return 2 + SaveRoot.BonusPoints;
            return 1 + SaveRoot.BonusPoints;
        }
        public async Task ExchangeDiscardAsync(int deck)
        {
            var thisCard = ThisMod!.Pile1!.GetCardInfo();
            ThisMod.Pile1.RemoveFromPile();
            await ThisE.AnimatePickUpDiscardAsync(thisCard);
            SingleInfo!.MainHandList.RemoveObjectByDeck(deck);
            var newCard = DeckList!.GetSpecificItem(deck);
            thisCard.Drew = true;
            SingleInfo.MainHandList.Add(thisCard);
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                SortCards();
            await AnimatePlayAsync(newCard);
            await ContinueTurnAsync();
        }
        public async Task GoOutAsync()
        {
            int points = CalculateGamePoints();
            int newTurn;
            if (WhoTurn == 1)
                newTurn = 2;
            else
                newTurn = 1;
            var thisPlayer = PlayerList![newTurn];
            if (points > 0)
            {
                SingleInfo!.GamePointsRound = points;
                SingleInfo.GamePointsGame += points;
            }
            else if (points == -2)
            {
                thisPlayer.GamePointsRound = 2;
                thisPlayer.GamePointsGame += 2;
            }
            else
            {
                SingleInfo!.GamePointsRound = 0;
                thisPlayer.GamePointsRound = 0;
            }
            if (!PlayerList.Any(items => items.GamePointsGame >= 7))
            {
                SaveRoot!.BonusPoints = 0;
                this.RoundOverNext();
                return;
            }
            SingleInfo = PlayerList.OrderByDescending(items => items.GamePointsGame).First();
            await ShowWinAsync();
        }
        public EnumMarriage WhichMarriage(IDeckDict<SixtySix2PlayerCardInformation> thisList)
        {
            if (thisList.Count != 2)
                return EnumMarriage.None;
            if (thisList.First().Suit != thisList.Last().Suit)
                return EnumMarriage.None;//the suits must match.
            if (!thisList.Any(items => items.Value == EnumCardValueList.King))
                return EnumMarriage.None;
            if (!thisList.Any(items => items.Value == EnumCardValueList.Queen))
                return EnumMarriage.None;
            if (thisList.First().Suit == SaveRoot!.TrumpSuit)
                return EnumMarriage.Trumps;
            return EnumMarriage.Regular;
        }
        public bool CanAnnounceMarriageAtBeginning => SaveRoot!.CardList.Count == 0 && Trick1!.IsLead == false && SaveRoot.CardsForMarriage.Count == 0;
        public bool CanShowMarriage(EnumMarriage whichMarriage)
        {
            if (whichMarriage == EnumMarriage.None)
                throw new BasicBlankException("If there was no marriage, this function should not be called");
            SingleInfo = PlayerList!.GetWhoPlayer();
            int newValue = SingleInfo.ScoreRound + (int)whichMarriage;
            return newValue < 66;
        }
        public async Task AnnounceMarriageAsync(CustomBasicList<int> thisList)
        {
            var newList = thisList.GetNewObjectListFromDeckList(DeckList!);
            var thisMarriage = WhichMarriage(newList);
            if (thisMarriage == EnumMarriage.None)
                throw new BasicBlankException("Must have been a marriage.  Otherwise; can't process");
            ThisMod!.Marriage1!.PopulateObjects(newList);
            ThisMod.Marriage1.Visible = true;
            if (CanAnnounceMarriageAtBeginning)
            {
                SingleInfo!.FirstMarriage = thisMarriage;
            }
            else
            {
                SingleInfo!.MarriageList.Add(thisMarriage);
                CalculateScore(WhoTurn);
            }
            SaveRoot!.CardsForMarriage = thisList;
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(2);
            ThisMod.Marriage1.Visible = false;
            await ContinueTurnAsync();
        }
    }
}