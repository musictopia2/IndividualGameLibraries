using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.

namespace GoFishCP
{
    [SingletonGame]
    public class GoFishMainGameClass : CardGameClass<RegularSimpleCard, GoFishPlayerItem, GoFishSaveInfo>, IMiscDataNM, IFinishStart
    {
        public GoFishMainGameClass(IGamePackageResolver container) : base(container) { }

        private GoFishViewModel? _thisMod;
        private readonly GoFishComputerAI _ai = new GoFishComputerAI();
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            _thisMod = MainContainer.Resolve<GoFishViewModel>();
        }
        public async Task NumberToAskAsync(EnumCardValueList WhichOne)
        {
            SaveRoot!.NumberAsked = true;
            if (SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
                _thisMod!.AskList!.SelectSpecificItem(WhichOne); //i think.
            GoFishPlayerItem otherPlayer = GetPlayer();
            await NumberToAskAsync(WhichOne, otherPlayer);
        }
        private async Task NumberToAskAsync(EnumCardValueList whichOne, GoFishPlayerItem otherPlayer)
        {
            if (otherPlayer.MainHandList.Count == 0)
            {
                await ContinueTurnAsync();
                return;
            }
            _thisMod!.AskList!.SelectSpecificItem(whichOne); //i think.
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(1);
            DeckRegularDict<RegularSimpleCard> thisList;
            if (ThisTest.AllowAnyMove == false || otherPlayer.MainHandList.Count != 1)
                thisList = otherPlayer.MainHandList.Where(items => items.Value == whichOne).ToRegularDeckDict();
            else
                thisList = otherPlayer.MainHandList.ToRegularDeckDict();
            if (thisList.Count == 0)
            {
                if (_thisMod.Deck1!.IsEndOfDeck() == false)
                {
                    if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                        await _thisMod.ShowGameMessageAsync("Go Fish");
                    LeftToDraw = 0;
                    PlayerDraws = 0;
                    await DrawAsync();
                    return;
                }
                if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                    await _thisMod.ShowGameMessageAsync("No more cards left to draw.  Therefore; will have to end your turn");
                await ContinueTurnAsync();
                return;
            }
            if (otherPlayer.PlayerCategory == EnumPlayerCategory.Self && ThisTest.NoAnimations == false)
            {
                thisList.ForEach(items =>
                {
                    _thisMod.PlayerHand1!.SelectOneFromDeck(items.Deck);
                });
                await Delay!.DelaySeconds(1); //so you can see what you have to get rid of.
            }
            thisList.ForEach(items =>
            {
                otherPlayer.MainHandList.RemoveObjectByDeck(items.Deck);
                if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                    items.Drew = true;
                SingleInfo.MainHandList.Add(items);
            });
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                SortAfterDrawing(); //i think.
            if (otherPlayer.MainHandList.Count == 0)
            {
                int cards = _thisMod.Deck1!.CardsLeft();
                if (cards < 5 && cards > 0)
                {
                    LeftToDraw = cards;
                    PlayerDraws = otherPlayer.Id;
                    await DrawAsync();
                    return;
                }
                else if (cards > 0)
                {
                    LeftToDraw = 5;
                    PlayerDraws = otherPlayer.Id;
                    await DrawAsync();
                    return;
                }
            }
            await ContinueTurnAsync();

        }
        private GoFishPlayerItem GetPlayer()
        {
            if (PlayerList.Count() > 2)
                throw new BasicBlankException("Since there are more than 2 players; needs to know the nick name");
            int nums;
            GoFishPlayerItem tempPlayer;
            if (WhoTurn == 1)
                nums = 2;
            else
                nums = 1;
            SaveRoot!.PlayOrder.OtherTurn = nums;
            tempPlayer = PlayerList![nums]; //one based now
            return tempPlayer;
        }
        public bool IsValidMove(DeckRegularDict<RegularSimpleCard> thisCol)
        {
            if (ThisTest!.AllowAnyMove == true)
                return true;
            return thisCol.First().Value == thisCol.Last().Value;
        }
        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            return base.FinishGetSavedAsync();
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            IsLoaded = true; //i think needs to be here.
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            LoadControls();
            SaveRoot!.RemovePairs = true;
            SaveRoot.NumberAsked = false; //i think.
            _thisMod!.AskList!.Visible = false; //i think here has to be invisible (?)
            _thisMod.AskList.ItemList.Clear();
            PlayerList!.ForEach(items => items.Pairs = 0); //i think i forgot that part in the old version.
            return base.StartSetUpAsync(isBeginning);
        }
        public async Task ProcessPlayAsync(int deck1, int deck2)
        {
            SingleInfo!.MainHandList.RemoveObjectByDeck(deck1);
            RegularSimpleCard secondCard = SingleInfo.MainHandList.RemoveObjectByDeck(deck2);
            SingleInfo.Pairs++;
            secondCard.IsSelected = false;
            secondCard.Drew = false;
            if (_thisMod!.Pile1!.CardExist(secondCard.Deck) == false)
                _thisMod.Pile1.AddCard(secondCard);
            else if (_thisMod.Pile1.CardExist(deck2) == false)
            {
                secondCard = SingleInfo.MainHandList.RemoveObjectByDeck(deck1);
                _thisMod.Pile1.AddCard(secondCard);
            }
            if (SingleInfo.MainHandList.Count == 0)
            {
                int cards = _thisMod.Deck1!.CardsLeft();
                if (cards < 5 && cards > 0)
                {
                    LeftToDraw = cards;
                    PlayerDraws = WhoTurn;
                    await DrawAsync();
                    _thisMod.LoadAskList();
                    return;
                }
                else if (cards > 0)
                {
                    LeftToDraw = 5;
                    PlayerDraws = WhoTurn;
                    await DrawAsync();
                    _thisMod.LoadAskList();
                }
            }
            await ContinueTurnAsync();
        }
        protected override async Task ComputerTurnAsync()
        {
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(.5);
            if (SaveRoot!.RemovePairs == false && SaveRoot.NumberAsked == false)
            {
                if (PlayerList.Count() == 2)
                {
                    await NumberToAskAsync(_ai.NumberToAsk(SaveRoot));
                    return;
                }
                throw new BasicBlankException("Only 2 players are supported now");
            }
            var thisList = _ai.PairToPlay(SaveRoot);
            if (thisList.Count == 0)
            {
                await EndTurnAsync();
                return;
            }
            if (thisList.Count != 2)
                throw new BasicBlankException("Needed one pair to remove.  Rethink");
            await ProcessPlayAsync(thisList.First().Deck, thisList.Last().Deck);
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "numbertoask":
                    EnumCardValueList thisValue = await js.DeserializeObjectAsync<EnumCardValueList>(content);
                    await NumberToAskAsync(thisValue);
                    return;
                case "processplay":
                    SendPair thisPair = await js.DeserializeObjectAsync<SendPair>(content);
                    await ProcessPlayAsync(thisPair.Card1, thisPair.Card2);
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        private bool CanEndGame()
        {
            foreach (var thisPlayer in PlayerList!)
            {
                if (thisPlayer.MainHandList.Count() != 0)
                    return false;
            }
            return true;
        }
        private async Task GameOverAsync()
        {
            SingleInfo = PlayerList.OrderByDescending(items => items.Pairs).Take(1).Single();
            if (SingleInfo.Pairs == 13)
                await ShowTieAsync();
            else
                await ShowWinAsync();
        }
        public override async Task EndTurnAsync()
        {
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                _thisMod!.PlayerHand1!.EndTurn();
            if (CanEndGame() == true)
            {
                await GameOverAsync();
                return;
            }
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            if (SaveRoot!.RemovePairs == true && WhoTurn == WhoStarts)
                SaveRoot.RemovePairs = false;
            await StartNewTurnAsync();
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            SaveRoot!.NumberAsked = false;
            if (SaveRoot.RemovePairs == false)
                _thisMod!.LoadAskList();
            await ContinueTurnAsync();
        }
        public Task FinishStartAsync()
        {
            if (SaveRoot!.RemovePairs == false)
                _thisMod!.LoadAskList(); //i think.
            return Task.CompletedTask;
        }
    }
}