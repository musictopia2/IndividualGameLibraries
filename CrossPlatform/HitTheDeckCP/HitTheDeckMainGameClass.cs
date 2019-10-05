using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFramework.NetworkingClasses.Extensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace HitTheDeckCP
{
    [SingletonGame]
    public class HitTheDeckMainGameClass : CardGameClass<HitTheDeckCardInformation, HitTheDeckPlayerItem, HitTheDeckSaveInfo>, IMiscDataNM, IStartNewGame
    {
        public HitTheDeckMainGameClass(IGamePackageResolver container) : base(container) { }

        private HitTheDeckViewModel? _thisMod;

        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            _thisMod = MainContainer.Resolve<HitTheDeckViewModel>();
        }

        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            SaveRoot!.LoadMod(_thisMod!);
            return base.FinishGetSavedAsync();
        }
        private HitTheDeckCardInformation ManuallyChooseCard
        {
            get
            {
                var tempList = DeckList.Where(items => items.CardType != EnumTypeList.Flip).ToRegularDeckDict();
                do
                {
                    var thisCard = tempList.GetRandomItem();
                    if (_thisMod!.Deck1!.CardExists(thisCard.Deck))
                        return thisCard;
                } while (true);
            }
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;

            IsLoaded = true; //i think needs to be here.
        }
        protected override Task LastPartOfSetUpBeforeBindingsAsync()
        {
            //has to figure out what card will be added manually.  refer to cousins for ideas.
            var tempCard = ManuallyChooseCard;
            _thisMod!.Deck1!.ManuallyRemoveSpecificCard(tempCard);
            _thisMod.Pile1!.AddCard(tempCard);
            SaveRoot!.LoadMod(_thisMod!);
            SaveRoot.ImmediatelyStartTurn = true; //so it shows startnewturn.
            return base.LastPartOfSetUpBeforeBindingsAsync();
        }
        private int ComputerToPlay()
        {
            var thisList = SingleInfo!.MainHandList.Where(items => CanPlay(items.Deck)).ToRegularDeckDict();
            if (thisList.Count == 0)
                return 0;
            return thisList.GetRandomItem().Deck;
        }
        protected override async Task ComputerTurnAsync()
        {
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(.75);
            var thisCard = _thisMod!.Pile1!.GetCardInfo();
            if (thisCard.Instructions == EnumInstructionList.Flip)
            {
                if (ThisData!.MultiPlayer)
                    await ThisNet!.SendAllAsync("flipdeck");
                await FlipDeckAsync();
                return;
            }
            if (thisCard.Instructions == EnumInstructionList.Cut)
            {
                await CutDeckAsync();
                return;
            }
            if (AlreadyDrew == true)
            {
                thisCard = SingleInfo!.MainHandList.Single(items => items.Drew);
                if (CanPlay(thisCard.Deck))
                {
                    await ProcessPlayAsync(thisCard.Deck); //hopefully okay.
                    return;
                }
                if (ThisData!.MultiPlayer == true)
                    await ThisNet!.SendEndTurnAsync();
                await EndTurnAsync();
                return;
            }
            int decks = ComputerToPlay();
            if (decks > 0)
            {
                await ProcessPlayAsync(decks);
                return;
            }
            SingleInfo!.MainHandList.UnhighlightObjects(); //so the computer knows which is the last one drawn.
            if (ThisData!.MultiPlayer == true)
                await ThisNet!.SendDrawAsync(); //i think.
            await DrawAsync();
        }
        private RandomGenerator? _rs;
        private async Task RandomDraw4Async()
        {
            if (_rs == null)
                _rs = MainContainer.Resolve<RandomGenerator>();
            PlayerDraws = _rs.GetRandomNumber(PlayerList.Count());
            var tempPlayer = PlayerList![PlayerDraws];
            if (ThisData!.MultiPlayer == true)
                await ThisNet!.SendAllAsync("playerdraw", PlayerDraws);
            await _thisMod!.ShowGameMessageAsync($"{tempPlayer.NickName} will be drawing 4 cards");
            SaveRoot!.HasDrawn = true;
            LeftToDraw = 4;
            await DrawAsync(); //maybe this is good enough.
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            LoadControls();
            return base.StartSetUpAsync(isBeginning);
        }

        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "playerdraw":
                    LeftToDraw = 4;
                    PlayerDraws = int.Parse(content);
                    var tempPlayer = PlayerList![PlayerDraws];
                    await _thisMod!.ShowGameMessageAsync($"{tempPlayer.NickName} will be drawing 4 cards");
                    SaveRoot!.HasDrawn = true;
                    await DrawAsync();
                    break;
                case "cutdeck":
                    await CutDeckAsync();
                    return;
                case "flipdeck":
                    await FlipDeckAsync();
                    return;
                case "play":
                    await ProcessPlayAsync(int.Parse(content));
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task EndTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.MainHandList.UnhighlightObjects(); //i think this is best.
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Computer)
            {
                SingleInfo.MainHandList.ForEach(thisCard => thisCard.Drew = false);
            }
            _thisMod!.CommandContainer!.ManuelFinish = true; //because it could be somebody else's turn.
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
        }
        public async override Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            PlayerDraws = 0;
            var newTurn = await PlayerList!.CalculateWhoTurnAsync();
            var newPlayer = PlayerList[newTurn];
            SaveRoot!.NextPlayer = newPlayer.NickName;
            SingleInfo = PlayerList.GetWhoPlayer();
            var thisCard = _thisMod!.Pile1!.GetCardInfo();
            if (thisCard.Instructions == EnumInstructionList.Flip && SaveRoot.WasFlipped == false)
                SaveRoot.WasFlipped = true;
            else
                SaveRoot.WasFlipped = false;
            if (thisCard.Instructions == EnumInstructionList.RandomDraw && SaveRoot.HasDrawn == false)
            {
                int previousTurn = await PlayerList.CalculateOldTurnAsync();
                SingleInfo = PlayerList[previousTurn];
                if (ThisData!.MultiPlayer == true)
                {
                    if (SingleInfo.CanSendMessage(ThisData) == false)
                    {
                        ThisCheck!.IsEnabled = true;
                        return;
                    }
                }
                await RandomDraw4Async();
                return;
            }
            await ContinueTurnAsync();
        }
        public bool CanPlay(int deck)
        {
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self && ThisTest!.AllowAnyMove)
                return true;
            var oldCard = _thisMod!.Pile1!.GetCardInfo();
            var newCard = DeckList!.GetSpecificItem(deck);
            if (newCard.Instructions == EnumInstructionList.Flip && _thisMod.Deck1!.CanFlipDeck() == false)
                return false;
            if (oldCard.Instructions == EnumInstructionList.RandomDraw)
            {
                return oldCard.FirstSort == newCard.FirstSort || newCard.AnyColor;
            }
            if (oldCard.Instructions == EnumInstructionList.None)
            {
                return oldCard.FirstSort == newCard.FirstSort || oldCard.Number == newCard.Number || newCard.AnyColor;
            }
            if (oldCard.Instructions == EnumInstructionList.Flip || oldCard.Instructions == EnumInstructionList.Cut)
                throw new BasicBlankException("Cannot ever play a card from your cards if the instructions is flip or cut the deck");
            if (oldCard.Instructions == EnumInstructionList.PlayColor)
            {
                if (newCard.AnyColor)
                    return false;
                return newCard.FirstSort == oldCard.FirstSort;
            }
            if (oldCard.Instructions == EnumInstructionList.PlayNumber)
            {
                if (newCard.Instructions != EnumInstructionList.None)
                    return false;
                return newCard.Number == oldCard.Number;
            }
            throw new BasicBlankException("Cannot find out whether to play the card or not");
        }
        public async Task ProcessPlayAsync(int deck)
        {
            if (SingleInfo!.MainHandList.ObjectExist(deck))
                SingleInfo.MainHandList.RemoveObjectByDeck(deck);
            if (SingleInfo.CanSendMessage(ThisData!))
                await ThisNet!.SendAllAsync("play", deck);
            var thisCard = DeckList!.GetSpecificItem(deck);
            thisCard.Drew = false;
            await AnimatePlayAsync(thisCard);
            thisCard = _thisMod!.Pile1!.GetCardInfo();
            SaveRoot!.HasDrawn = false;
            if (thisCard.Deck != deck)
                throw new BasicBlankException("This is not the card played.  That means there is a problem (possibly with the discard)");
            if (SingleInfo.MainHandList.Count == 0)
            {
                await EndRoundAsync();
                return;
            }
            await EndTurnAsync();
        }
        public async Task FlipDeckAsync()
        {
            _thisMod!.CommandContainer!.ManuelFinish = true; //should be this way anyways.
            var deckList = _thisMod!.Deck1!.FlipCardList();
            PlayerList!.ChangeReverse();
            var discardList = _thisMod.Pile1!.FlipCardList();
            if (discardList.Count > 110)
                throw new BasicBlankException("Cannot have over 110 cards since that is all the cards total");
            if (discardList.Count + deckList.Count > 110)
                throw new BasicBlankException("The discard and decklist combined cannot be more than 110");
            _thisMod.Deck1.OriginalList(discardList);
            _thisMod.Pile1.NewList(deckList);
            var thisCard = _thisMod.Pile1.GetCardInfo();
            bool useDeck;
            if (thisCard.Instructions == EnumInstructionList.Flip)
            {
                useDeck = _thisMod.Pile1.CanCutDiscard();
                do
                {
                    if (useDeck == false)
                        _thisMod.Pile1.CutDeck();
                    else
                    {
                        _thisMod.Deck1.PutInMiddle(_thisMod.Pile1.GetCardInfo());
                        _thisMod.Pile1.RemoveFromPile();
                    }
                    thisCard = _thisMod.Pile1.GetCardInfo();
                    if (thisCard.Instructions != EnumInstructionList.Flip)
                    {
                        thisCard.IsUnknown = false; //has to be here.  so a person knows what to do next.
                        break;
                    }
                } while (true);
            }
            await EndTurnAsync();
        }
        private bool CanProcessPlay
        {
            get
            {
                if (ThisData!.MultiPlayer == false)
                    return true;
                return SingleInfo!.CanSendMessage(ThisData);
            }
        }
        public async Task CutDeckAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            if (CanProcessPlay)
            {
                if (_thisMod!.Deck1!.CanCutDeck() == false)
                {
                    var deckList = _thisMod.Deck1.FlipCardList();
                    await _thisMod.ShowGameMessageAsync("Its the end of the deck; therefore; the cards are being reshuffled");
                    _thisMod.Pile1!.AddRestOfDeck(deckList);
                    await ReshuffleCardsAsync(true);
                }
                if (ThisData!.MultiPlayer == true)
                {
                    await ThisNet!.SendAllAsync("cutdeck");
                }
            }
            _thisMod!.Pile1!.AddCard(_thisMod.Deck1!.CutTheDeck());
            await EndTurnAsync();
        }
        private void UpdateScores()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.PreviousPoints = thisPlayer.MainHandList.Sum(items => items.Points);
                thisPlayer.TotalPoints += thisPlayer.PreviousPoints;
            });
        }
        public override async Task EndRoundAsync()
        {
            SaveRoot!.NextPlayer = "None";
            UpdateScores();
            if (CanEndGame == false)
            {
                this.RoundOverNext();
                return;
            }
            SingleInfo = PlayerList.OrderBy(items => items.TotalPoints).First();
            await ShowWinAsync();
        }

        Task IStartNewGame.ResetAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.TotalPoints = 0;
                thisPlayer.PreviousPoints = 0;
            });
            return Task.CompletedTask;
        }
        private bool CanEndGame
        {
            get
            {
                int pointsNeeded;
                if (PlayerList.Count() == 2)
                    pointsNeeded = 70;
                else
                    pointsNeeded = 100;
                return PlayerList.Any(items => items.TotalPoints >= pointsNeeded);
            }
        }
    }
}