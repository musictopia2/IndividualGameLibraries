using BasicGameFramework.Attributes;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace A8RoundRummyCP
{
    [SingletonGame]
    public class A8RoundRummyMainGameClass : CardGameClass<A8RoundRummyCardInformation, A8RoundRummyPlayerItem, A8RoundRummySaveInfo>, IMiscDataNM
    {
        public A8RoundRummyMainGameClass(IGamePackageResolver container) : base(container) { }
        internal A8RoundRummyViewModel? ThisMod;
        internal A8RoundRummyCardInformation? LastCard;
        internal bool LastSuccessful;
        public bool WasGuarantee { get; set; } //not sure if needed or not (?)
        public A8RoundRummyCardInformation? CardForDiscard { get; set; }
        public EnumPlayerStatus PlayerStatus { get; set; }
        private bool WasNew; //hopefully okay.
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            ThisMod = MainContainer.Resolve<A8RoundRummyViewModel>();
        }
        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            WasNew = false; //i think.  if i am wrong, rethink.
            CardForDiscard = null; //hopefully that works.  if something else needs to be there, rethink.
            WasGuarantee = false;
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
            PlayerList!.ForEach(thisPlayer => thisPlayer.TotalScore = 0);
            WasNew = false;
            WasGuarantee = false;
            CardForDiscard = null;
            CustomBasicList<RoundClass> tempList = new CustomBasicList<RoundClass>();
            RoundClass thisRound = new RoundClass();
            thisRound.Description = "Round 1:  Same Color";
            thisRound.Category = EnumCategory.Colors;
            thisRound.Rummy = EnumRummyType.Regular;
            thisRound.Points = 1;
            tempList.Add(thisRound);
            thisRound = new RoundClass();
            thisRound.Description = "Round 2:  Same Shape";
            thisRound.Category = EnumCategory.Shapes;
            thisRound.Rummy = EnumRummyType.Regular;
            thisRound.Points = 2;
            tempList.Add(thisRound);
            thisRound = new RoundClass();
            thisRound.Description = "Round 3:  Same Shape And Color";
            thisRound.Category = EnumCategory.Both;
            thisRound.Rummy = EnumRummyType.Regular;
            thisRound.Points = 3;
            tempList.Add(thisRound);
            thisRound = new RoundClass();
            thisRound.Description = "Round 4:  Straight";
            thisRound.Category = EnumCategory.None;
            thisRound.Rummy = EnumRummyType.Straight;
            thisRound.Points = 4;
            tempList.Add(thisRound);
            thisRound = new RoundClass();
            thisRound.Description = "Round 5:  Straight Same Color";
            thisRound.Category = EnumCategory.Colors;
            thisRound.Rummy = EnumRummyType.Straight;
            thisRound.Points = 5;
            tempList.Add(thisRound);
            thisRound = new RoundClass();
            thisRound.Description = "Round 6:  Straight Same Shape";
            thisRound.Category = EnumCategory.Shapes;
            thisRound.Rummy = EnumRummyType.Straight;
            thisRound.Points = 6;
            tempList.Add(thisRound);
            thisRound = new RoundClass();
            thisRound.Description = "Round 7:  Straight Same Shape And Color";
            thisRound.Category = EnumCategory.Both;
            thisRound.Rummy = EnumRummyType.Straight;
            thisRound.Points = 7;
            tempList.Add(thisRound);
            thisRound = new RoundClass();
            thisRound.Description = "Round 8:  Same Number";
            thisRound.Category = EnumCategory.None;
            thisRound.Rummy = EnumRummyType.Kinds;
            thisRound.Points = 8;
            tempList.Add(thisRound);
            if (ThisTest!.DoubleCheck)
                tempList.KeepConditionalItems(items => items.Points >= 4);
            SaveRoot!.RoundList.ReplaceRange(tempList); //i think.
            return base.StartSetUpAsync(isBeginning);
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "goout":
                    MultiplayerOut thisOut = await js.DeserializeObjectAsync<MultiplayerOut>(content);
                    WasGuarantee = thisOut.WasGuaranteed;
                    CardForDiscard = DeckList!.GetSpecificItem(thisOut.Deck);
                    await GoOutAsync();
                    break;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            await ShowNextTurnAsync(); //i think here too.
            PlayerDraws = 0;
            if (PlayerStatus != EnumPlayerStatus.Regular)
            {
                PlayerStatus = EnumPlayerStatus.Regular;
                LeftToDraw = 0;
                await DrawAsync();
                return;
            }
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task EndTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.MainHandList.UnhighlightObjects(); //i think this is best.
            WasNew = false;
            ThisMod!.CommandContainer!.ManuelFinish = true; //because it could be somebody else's turn.
            WhoTurn = await PlayerList.CalculateWhoTurnAsync(true);
            await StartNewTurnAsync();
        }
        public override async Task ContinueTurnAsync()
        {
            await ShowNextTurnAsync(); //i think this could be a better way to do it.
            await base.ContinueTurnAsync();
        }
        private async Task ShowNextTurnAsync()
        {
            A8RoundRummyPlayerItem newPlayer;
            int newTurn = await PlayerList!.CalculateWhoTurnAsync();
            newPlayer = PlayerList[newTurn];
            ThisMod!.NextTurn = newPlayer.NickName;
        }
        protected override bool ShowNewCardDrawn(A8RoundRummyPlayerItem tempPlayer)
        {
            if (WasNew == true)
                return false;
            return base.ShowNewCardDrawn(tempPlayer);
        }
        public bool CanProcessDiscard(out bool pickUp, out int deck, out string message)
        {
            message = "";
            deck = 0; //has to populate defaults first.
            if (AlreadyDrew == false && PreviousCard == 0)
                pickUp = true;
            else
                pickUp = false;
            int Selected = ThisMod!.PlayerHand1!.ObjectSelected();
            if (pickUp == true)
            {
                if (Selected > 0)
                {
                    message = "There is at least one card selected.  Unselect all the cards before picking up from discard";
                    return false;
                }
                return true;
            }
            if (Selected == 0)
            {
                message = "Sorry, you must select a card to discard";
                return false;
            }
            if (Selected == PreviousCard)
            {
                deck = 0;
                message = "Sorry, you cannot discard the one that was picked up since there is more than one card to choose from";
                return false;
            }
            deck = Selected;
            return true;
        }
        public bool CanGoOut()
        {
            WasGuarantee = SingleInfo!.MainHandList.GuaranteedVictory(this);
            if (WasGuarantee == true)
            {
                CardForDiscard = LastCard;
                return true;
            }
            if (SingleInfo.MainHandList.HasRummy(this) == false)
                return false;
            CardForDiscard = LastCard;
            return true;
        }
        private async Task GameOverAsync()
        {
            SaveRoot!.RoundList.Clear();
            await ShowWinAsync();
        }
        public async Task GoOutAsync()
        {
            if (SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
                await ThisMod!.ShowGameMessageAsync($"{SingleInfo.NickName} went out");
            else
                ThisMod!.PlayerHand1!.EndTurn();
            var thisList = SingleInfo.MainHandList.ToRegularDeckDict();
            thisList.RemoveObjectByDeck(CardForDiscard!.Deck); //hopefully this works.
            await thisList.ForEachAsync(async thisCard =>
            {
                thisCard.Drew = false;
                thisCard.IsSelected = false;
                SingleInfo.MainHandList.RemoveObjectByDeck(thisCard.Deck);
                await AnimatePlayAsync(thisCard); //i think still this.
                if (ThisTest!.NoAnimations == false)
                    await Delay!.DelaySeconds(.25);
            });
            SingleInfo.MainHandList.RemoveObjectByDeck(CardForDiscard.Deck);
            await AnimatePlayAsync(CardForDiscard);
            if (WasGuarantee == true)
            {
                await GameOverAsync();
                return;
            }
            SingleInfo.TotalScore += SaveRoot!.RoundList.First().Points;
            if (SaveRoot.RoundList.Count == 1)
            {
                SingleInfo = PlayerList.OrderByDescending(Items => Items.TotalScore).First();
                await GameOverAsync();
                return;
            }
            WasNew = true;
            PlayerStatus = EnumPlayerStatus.WentOut;
            if (SingleInfo.MainHandList.Count > 0)
                throw new BasicBlankException("Can't have any cards.  Maybe trying clearing to double check but not sure");
            LeftToDraw = 7;
            await DrawAsync();
        }
        protected override async Task AfterDrawingAsync()
        {
            WasNew = false;
            if (SingleInfo!.MainHandList.Count > 8)
                throw new BasicBlankException("Can't have more than 8 cards in hand");
            if (PlayerStatus == EnumPlayerStatus.WentOut)
            {
                PlayerStatus = EnumPlayerStatus.Regular;
                SaveRoot!.RoundList.RemoveFirstItem();
                await EndTurnAsync();
                return;
            }
            if (PlayerStatus == EnumPlayerStatus.Reversed)
            {
                SaveRoot!.PlayOrder.IsReversed = !SaveRoot.PlayOrder.IsReversed;
                if (SaveRoot.PlayOrder.OtherTurn == 0)
                    throw new BasicBlankException("Otherturn must be filled in");
                WhoTurn = SaveRoot.PlayOrder.OtherTurn;
                SaveRoot.PlayOrder.OtherTurn = 0;
                PlayerStatus = EnumPlayerStatus.Regular; //maybe this too.  not sure.
                await StartNewTurnAsync();
                return;
            }
            AlreadyDrew = true;
            await base.AfterDrawingAsync();
        }
        public override async Task DiscardAsync(A8RoundRummyCardInformation thisCard)
        {
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                ThisMod!.PlayerHand1!.EndTurn();
            if (thisCard.CardType == EnumCardType.Reverse)
            {
                thisCard.Drew = false;
                thisCard.IsSelected = false;
                if (SingleInfo.PlayerCategory != EnumPlayerCategory.Computer)
                    SingleInfo.MainHandList.RemoveObjectByDeck(thisCard.Deck);
                bool ShowMessageBox;
                await AnimatePlayAsync(thisCard);
                ShowMessageBox = SingleInfo.PlayerCategory != EnumPlayerCategory.Self;
                SaveRoot!.PlayOrder.OtherTurn = WhoTurn; //after being reversed then the current player will get another turn.
                PlayerStatus = EnumPlayerStatus.Reversed;
                WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
                SingleInfo = PlayerList.GetWhoPlayer();
                if (ShowMessageBox == true)
                    await ThisMod!.ShowGameMessageAsync($"{SingleInfo.NickName} has to discard all the cards and draw new cards");
                var thisList = SingleInfo.MainHandList.ToRegularDeckDict();
                await thisList.ForEachAsync(async NewCard =>
                {
                    SingleInfo.MainHandList.RemoveObjectByDeck(NewCard.Deck);
                    await AnimatePlayAsync(NewCard);
                });
                LeftToDraw = 7;
                WasNew = true;
                PlayerDraws = WhoTurn;
                await DrawAsync();
                return;
            }
            if (SingleInfo.PlayerCategory != EnumPlayerCategory.Computer)
            {
                SingleInfo.MainHandList.RemoveObjectByDeck(thisCard.Deck);
                if (SingleInfo.MainHandList.Count == 8)
                    throw new BasicBlankException("Failed To Discard");
            }
            await AnimatePlayAsync(thisCard);
            await EndTurnAsync();
        }
    }
}