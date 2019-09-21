using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.NetworkingClasses.Extensions;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace OldMaidCP
{
    [SingletonGame]
    public class OldMaidMainGameClass : CardGameClass<RegularSimpleCard, OldMaidPlayerItem, OldMaidSaveInfo>, IMiscDataNM
    {
        public OldMaidMainGameClass(IGamePackageResolver container) : base(container) { }

        private OldMaidViewModel? _thisMod;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            _thisMod = MainContainer.Resolve<OldMaidViewModel>();
        }
        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            LinkOtherPlayer();
            return base.FinishGetSavedAsync();
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;

            IsLoaded = true; //i think needs to be here.
        }
        private void LinkOtherPlayer()
        {
            if (SaveRoot!.PlayOrder.OtherTurn > 0)
                _thisMod!.OtherPlayer = PlayerList![SaveRoot.PlayOrder.OtherTurn];
            if (SaveRoot.RemovePairs == true)
                _thisMod!.OpponentCards1!.Visible = false;
            else
            {
                SortOtherCards();
                _thisMod!.OpponentCards1!.Visible = true;
            }
        }
        protected override Task LastPartOfSetUpBeforeBindingsAsync()
        {
            LinkOtherPlayer();
            return base.LastPartOfSetUpBeforeBindingsAsync();
        }
        protected override async Task ComputerTurnAsync()
        {
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelayMilli(700);
            if (SaveRoot!.RemovePairs == false && SaveRoot.AlreadyChoseOne == false)
            {
                int deck = _thisMod!.OtherPlayer!.MainHandList.GetRandomItem().Deck;
                await SelectCardAsync(deck);
                return;
            }
            var tempList = ComputerPairList();
            if (tempList.Count() == 0)
            {
                await EndTurnAsync();
                return;
            }
            int deck1 = tempList.First().Deck;
            int deck2 = tempList.Last().Deck;
            await ProcessPlayAsync(deck1, deck2);
        }
        private DeckRegularDict<RegularSimpleCard> ComputerPairList()
        {
            DeckRegularDict<RegularSimpleCard> output = new DeckRegularDict<RegularSimpleCard>();
            foreach (var firstCard in SingleInfo!.MainHandList)
            {
                foreach (var secondCard in SingleInfo.MainHandList)
                {
                    if (firstCard.Deck != secondCard.Deck)
                    {
                        if (firstCard.Value == secondCard.Value)
                        {
                            output.Add(firstCard);
                            output.Add(secondCard);
                            return output;
                        }
                    }
                }
            }
            return output;
        }
        protected override async Task StartSetUpAsync(bool isBeginning)
        {
            LoadControls();
            SaveRoot!.PlayOrder.IsReversed = true;
            SaveRoot.PlayOrder.OtherTurn = await PlayerList!.CalculateWhoTurnAsync();
            SaveRoot.PlayOrder.IsReversed = false; //try this way.
            SaveRoot.RemovePairs = true;
            SaveRoot.AlreadyChoseOne = false;
            PlayerList.ForEach(thisPlayer => thisPlayer.InGame = true);
            await base.StartSetUpAsync(isBeginning);
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "processplay":
                    SendPlay thiss = await js.DeserializeObjectAsync<SendPlay>(content);
                    await ProcessPlayAsync(thiss.Card1, thiss.Card2);
                    break;
                case "cardselected":
                    await SelectCardAsync(int.Parse(content));
                    break;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task EndTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.MainHandList.UnhighlightObjects(); //i think this is best.
            int lost = WhoLost();
            if (lost > 0)
            {
                SingleInfo = PlayerList[lost];
                await ShowLossAsync();
                return;
            }
            TakeOutGame();
            if (SingleInfo.MainHandList.Count > 0)
                SaveRoot!.PlayOrder.OtherTurn = WhoTurn;
            else
            {
                SaveRoot!.PlayOrder.IsReversed = true;
                SaveRoot.PlayOrder.OtherTurn = await PlayerList.CalculateWhoTurnAsync();
                SaveRoot.PlayOrder.IsReversed = false;
            }
            _thisMod!.CommandContainer!.ManuelFinish = true; //because it could be somebody else's turn.
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            if (SaveRoot.RemovePairs == true && WhoTurn == WhoStarts)
                SaveRoot.RemovePairs = false;
            await StartNewTurnAsync();
        }
        public async override Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            SaveRoot!.AlreadyChoseOne = false;
            LinkOtherPlayer();
            await ContinueTurnAsync();
        }
        public async Task SelectCardAsync(int deck)
        {
            if (SingleInfo!.CanSendMessage(ThisData!) == true)
                await ThisNet!.SendAllAsync("cardselected", deck);
            if (_thisMod!.OtherPlayer!.PlayerCategory == EnumPlayerCategory.Self)
            {
                _thisMod.PlayerHand1!.SelectOneFromDeck(deck);
                if (ThisTest!.NoAnimations == false)
                    await Delay!.DelaySeconds(.75);
            }
            _thisMod.OtherPlayer.MainHandList.RemoveObjectByDeck(deck);
            var thisCard = DeckList!.GetSpecificItem(deck);
            thisCard.IsUnknown = false;
            thisCard.IsSelected = false;
            SortOtherCards();
            SingleInfo!.MainHandList.Add(thisCard);
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
            {
                thisCard.Drew = true;
                SortCards();
            }
            SaveRoot!.AlreadyChoseOne = true;
            await ContinueTurnAsync();
        }
        public async Task ProcessPlayAsync(int card1, int card2)
        {
            if (SingleInfo!.CanSendMessage(ThisData!) == true)
            {
                SendPlay thiss = new SendPlay();
                thiss.Card1 = card1;
                thiss.Card2 = card2;
                await ThisNet!.SendAllAsync("processplay", thiss);
            }
            SingleInfo!.MainHandList.RemoveObjectByDeck(card1);
            var secondCard = SingleInfo.MainHandList.RemoveObjectByDeck(card2);
            secondCard.Drew = false;
            secondCard.IsSelected = false;
            _thisMod!.Pile1!.AddCard(secondCard);
            await ContinueTurnAsync();
        }
        internal bool IsValidMove(IDeckDict<RegularSimpleCard> thisCol)
        {
            return thisCol.HasDuplicates(items => items.Value); //hopefully its easier this way and still works.
        }
        private void TakeOutGame()
        {
            PlayerList!.ForConditionalItems(items => items.MainHandList.Count == 0, items => items.InGame = false);
        }
        private int WhoLost() //okay to return id this time.
        {
            int x = 0;
            foreach (var thisPlayer in PlayerList!)
            {
                x++;
                if (thisPlayer.MainHandList.Count == 1)
                {
                    var thisCard = thisPlayer.MainHandList.Single();
                    if (thisCard.Value == EnumCardValueList.Queen && thisCard.Suit == EnumSuitList.Spades)
                        return x;
                }
            }
            return 0;
        }
        private void SortOtherCards()
        {
            DeckRegularDict<RegularSimpleCard> output = new DeckRegularDict<RegularSimpleCard>();
            _thisMod!.OtherPlayer!.MainHandList.ForEach(thisCard =>
            {
                RegularSimpleCard newCard = new RegularSimpleCard();
                newCard.Populate(thisCard.Deck);
                newCard.IsUnknown = true;
                output.Add(newCard);
            });
            output.ShuffleList();
            _thisMod.OpponentCards1!.HandList.ReplaceRange(output);
        }
    }
}