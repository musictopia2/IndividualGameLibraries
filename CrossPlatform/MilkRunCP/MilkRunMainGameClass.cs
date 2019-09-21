using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace MilkRunCP
{
    [SingletonGame]
    public class MilkRunMainGameClass : CardGameClass<MilkRunCardInformation, MilkRunPlayerItem, MilkRunSaveInfo>, IMiscDataNM
    {
        public MilkRunMainGameClass(IGamePackageResolver container) : base(container) { }
        internal MilkRunViewModel? ThisMod;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            ThisMod = MainContainer.Resolve<MilkRunViewModel>();
        }
        public override async Task FinishGetSavedAsync()
        {
            LoadPiles();
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.LoadSavedData();
            });
            await base.FinishGetSavedAsync();
        }
        public override async Task PopulateSaveRootAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.SavePileData();
            });
            await base.PopulateSaveRootAsync();
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            LoadPiles();
            SaveRoot!.DrawnFromDiscard = false;
            SaveRoot.CardsDrawn = 0;
            PlayerList!.ForEach(thisPlayer => thisPlayer.ClearBoard());
            return base.StartSetUpAsync(isBeginning);
        }
        private ComputerAI? _ai;
        protected override async Task ComputerTurnAsync()
        {
            if (_ai == null)
                _ai = MainContainer.Resolve<ComputerAI>();
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(.5);
            if (_ai.CanDraw)
            {
                await DrawAsync();
                return;
            }
            ComputerAI.MoveInfo thisMove = _ai.MoveToMake();
            if (thisMove.ToDiscard == false)
                await ProcessPlayAsync(thisMove.Player, thisMove.Deck, thisMove.Pile, thisMove.Milk);
            else
                await DiscardAsync(thisMove.Deck);
        }
        public async Task PlayerPileClickedAsync(MilkRunPlayerItem thisPlayer, PileInfo pileData)
        {
            int newDeck = ThisMod!.PlayerHand1!.ObjectSelected();
            if (newDeck == 0)
            {
                await ThisMod.ShowGameMessageAsync("Sorry, must choose a card to play");
                return;
            }
            if (SaveRoot!.CardsDrawn < 2)
            {
                await ThisMod.ShowGameMessageAsync("Sorry, must draw the 2 cards first before playing");
                return;
            }
            int index = thisPlayer.Id;
            if (CanMakeMove(index, newDeck, pileData.Pile, pileData.Milk) == false)
            {
                await ThisMod.ShowGameMessageAsync("Illegal Move");
                return;
            }
            if (ThisData!.MultiPlayer == true)
            {
                SendPlay thisSend = new SendPlay();
                thisSend.Deck = newDeck;
                thisSend.Player = index;
                thisSend.Milk = pileData.Milk;
                thisSend.Pile = pileData.Pile;
                await ThisNet!.SendAllAsync("play", thisSend);
            }
            await ProcessPlayAsync(index, newDeck, pileData.Pile, pileData.Milk);
        }
        private void LoadPiles()
        {
            if (PlayerList.First().StrawberryPiles != null)
                return;
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.LoadPiles(this); //hopefully this simple
            });
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "play":
                    SendPlay thisSend = await js.DeserializeObjectAsync<SendPlay>(content);
                    await ProcessPlayAsync(thisSend.Player, thisSend.Deck, thisSend.Pile, thisSend.Milk);
                    break;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            SaveRoot!.CardsDrawn = 0;
            SaveRoot.DrawnFromDiscard = false;
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task EndTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.MainHandList.UnhighlightObjects(); //i think this is best.
            ThisMod!.CommandContainer!.ManuelFinish = true; //because it could be somebody else's turn.
            int wins = WhoWins();
            if (wins > 0)
            {
                SingleInfo = PlayerList[wins];
                await ShowWinAsync();
                return;
            }

            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
        }
        private int WhoWins()
        {
            foreach (var thisPlayer in PlayerList!)
            {
                if (thisPlayer.ReachedChocolateGoal == true && thisPlayer.ReachedStrawberryGoal == true)
                    return thisPlayer.Id;
            }
            return 0;
        }
        protected override Task AfterDrawingAsync()
        {
            if (SaveRoot!.CardsDrawn >= 2)
                throw new BasicBlankException("Can't draw more than 2 cards");
            SaveRoot.CardsDrawn++;
            PlayerDraws = 0;
            return base.AfterDrawingAsync();
        }
        protected override Task AfterPickupFromDiscardAsync()
        {
            SaveRoot!.DrawnFromDiscard = true;
            if (SaveRoot.CardsDrawn >= 2)
                throw new BasicBlankException("Can't draw more than 2 cards");
            SaveRoot.CardsDrawn++;
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                SortCards();
            return base.AfterPickupFromDiscardAsync();
        }
        public override async Task DiscardAsync(MilkRunCardInformation thisCard)
        {
            SingleInfo!.MainHandList.RemoveSpecificItem(thisCard);
            await AnimatePlayAsync(thisCard);
            await ContinuePlayAsync(); //better to do this way.
        }
        private async Task ContinuePlayAsync()
        {
            if (CanEndTurn() == true)
                await EndTurnAsync();
            else
                await ContinueTurnAsync();
        }
        private bool CanEndTurn()
        {
            if (SaveRoot!.CardsDrawn < 2)
                return false;
            if (SingleInfo!.MainHandList.Count > 8)
                throw new BasicBlankException("Can't have more than 8 cards");
            return (SingleInfo.MainHandList.Count == 6);
        }
        protected override DeckObservableDict<MilkRunCardInformation> GetReshuffleList()
        {
            DeckObservableDict<MilkRunCardInformation> output = new DeckObservableDict<MilkRunCardInformation>();
            var tempList = ThisMod!.Pile1!.FlipCardList();
            int x;
            for (x = 1; x <= 3; x++)
            {
                if (x > tempList.Count)
                    break;
                output.Add(tempList[x - 1]);
            }
            PlayerList!.ForEach(thisPlayer =>
            {
                output.AddRange(thisPlayer.GetPileCardList());
            });
            DeckObservableDict<MilkRunCardInformation> finalList = new DeckObservableDict<MilkRunCardInformation>();
            DeckList!.ForEach(thisCard =>
            {
                if (output.Contains(thisCard) == false)
                    finalList.Add(thisCard);
            });
            return finalList;
        }
        public bool CanMakeMove(int player, int deck, EnumPileType pile, EnumMilkType milk)
        {
            MilkRunPlayerItem tempPlayer = PlayerList![player];
            MilkRunCardInformation thisCard = DeckList!.GetSpecificItem(deck);
            if (thisCard.MilkCategory != milk)
                return false;
            if (pile == EnumPileType.Limit)
            {
                return thisCard.CardCategory == EnumCardCategory.Points;
            }
            if (pile == EnumPileType.Go && thisCard.CardCategory == EnumCardCategory.Points)
                return false;
            if (pile == EnumPileType.Deliveries && thisCard.CardCategory != EnumCardCategory.Points)
                return false;
            if (thisCard.CardCategory == EnumCardCategory.Joker && pile != EnumPileType.Go)
                return false;
            bool gos = tempPlayer.HasGo(milk);
            int limits;
            if (player == WhoTurn)
            {
                if (thisCard.CardCategory == EnumCardCategory.Joker)
                    return false;
                if (pile == EnumPileType.Deliveries)
                {
                    limits = tempPlayer.DeliveryLimit(milk);
                    return gos == true && thisCard.Points <= limits;
                }
            }
            if (pile == EnumPileType.Deliveries)
                return false;
            if (pile == EnumPileType.Go)
            {
                if (thisCard.CardCategory == EnumCardCategory.Joker)
                {
                    var selfPlayer = PlayerList.GetSelf();
                    if (selfPlayer.HasGo(milk) == false || tempPlayer.HasGo(milk) == false)
                        return false;
                    if (tempPlayer.HasCard(milk) == false)
                        return false;
                    int nums = tempPlayer.LastDelivery(milk);
                    return nums <= selfPlayer.DeliveryLimit(milk);
                }
                gos = tempPlayer.HasGo(milk);
                if (gos == true && thisCard.CardCategory == EnumCardCategory.Go)
                    return false;
                if (gos == false && thisCard.CardCategory == EnumCardCategory.Stop)
                    return false;
                return true;
            }
            throw new BasicBlankException("Cannot figure out whether the move can be made or not");
        }
        public async Task ProcessPlayAsync(int player, int deck, EnumPileType pile, EnumMilkType milk)
        {
            if (milk != EnumMilkType.Chocolate && milk != EnumMilkType.Strawberry)
                throw new BasicBlankException("Must be chocolate or strawberry at the beginning of process play");
            var tempPlayer = PlayerList![player];
            var thisCard = DeckList!.GetSpecificItem(deck);
            int cardsBefore = SingleInfo!.MainHandList.Count;
            SingleInfo.MainHandList.RemoveSpecificItem(thisCard);
            int cardsAfter = SingleInfo.MainHandList.Count;
            if (cardsBefore == cardsAfter)
                throw new BasicBlankException("Did not remove card");
            await tempPlayer.AnimatePlayAsync(thisCard, milk, pile, BasicGameFramework.BasicEventModels.EnumAnimcationDirection.StartUpToCard);
            if (pile == EnumPileType.Limit)
            {
                tempPlayer.AddLimit(deck, milk);
                await ContinuePlayAsync();
                return;
            }
            if (pile == EnumPileType.Deliveries)
            {
                tempPlayer.AddToDeliveries(deck, milk);
                await ContinuePlayAsync();
                return;
            }
            if (thisCard.CardCategory != EnumCardCategory.Joker)
            {
                tempPlayer.AddGo(deck, milk);
                await ContinuePlayAsync();
                return;
            }
            await tempPlayer.AnimatePlayAsync(thisCard, milk, EnumPileType.Deliveries, BasicGameFramework.BasicEventModels.EnumAnimcationDirection.StartCardToDown);
            tempPlayer.StealCard(milk, out int newDeck);
            tempPlayer.AddGo(deck, milk);
            MilkRunPlayerItem newPlayer;
            if (player == 1)
                newPlayer = PlayerList[2];
            else
                newPlayer = PlayerList[1];
            thisCard = DeckList.GetSpecificItem(newDeck);
            await newPlayer.AnimatePlayAsync(thisCard, milk, EnumPileType.Deliveries, BasicGameFramework.BasicEventModels.EnumAnimcationDirection.StartUpToCard);
            newPlayer.AddToDeliveries(newDeck, milk);
            await ContinuePlayAsync();
        }
    }
}