using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.Extensions;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.NetworkingClasses.Extensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using vb = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.Constants;
namespace PaydayCP
{
    [SingletonGame]
    public class PaydayMainGameClass : BoardDiceGameClass<EnumColorChoice, PawnPiecesCP<EnumColorChoice>,
        PaydayPlayerItem, PaydaySaveInfo, int>, IMiscDataNM, IFinishStart
    {
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
        public PaydayMainGameClass(IGamePackageResolver container) : base(container) { }
        internal GameBoardProcesses? GameBoard1;
        internal PaydayViewModel? ThisMod;
        private RandomGenerator? _rs;
        GlobalFunctions? _thisGlobals;
        private bool _autoResume;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            ThisMod = MainContainer.Resolve<PaydayViewModel>();
            _rs = MainContainer.Resolve<RandomGenerator>(); //hopefully this works.
        }
        async Task IFinishStart.FinishStartAsync()
        {
            if (_autoResume == false && DidChooseColors == true)
                return;
            if (DidChooseColors == true)
            {
                ThisMod!.MainOptionsVisible = true; //maybe required because of how .net core works.
                await GameBoard1!.ReloadSavedStateAsync();
                if (SaveRoot!.GameStatus == EnumStatus.MakeMove)
                    ThisMod!.ThisCup!.CanShowDice = true;
                else
                    ThisMod!.ThisCup!.CanShowDice = false;
                if (SaveRoot.GameStatus != EnumStatus.ChooseLottery)
                {
                    if (SaveRoot.CurrentMail != null && SaveRoot.CurrentMail.Deck > 0)
                    {
                        ThisMod.MailPile!.Visible = true;
                        ThisMod.MailPile.AddCard(SaveRoot.CurrentMail);
                    }
                    if (SaveRoot.CurrentDeal != null && SaveRoot.CurrentDeal.Deck > 0)
                    {
                        ThisMod.DealPile!.Visible = true;
                        ThisMod.DealPile.AddCard(SaveRoot.CurrentDeal);
                    }
                    if (ThisMod!.DealPile!.Visible == true)
                        ThisMod.MailPile!.Visible = false; //can't both be visible.
                }
                if (SaveRoot.GameStatus == EnumStatus.Starts)
                {
                    PrepStartTurn(); //i think.
                }
                else
                {
                    base.PrepStartTurn();
                    GameBoard1.NewTurn(); //so you can see that part.
                    MonthLabel();
                    PopulateDeals();
                    PopulateMails();
                }
            }
            else
                PrepStartTurn();
        }
        public override Task FinishGetSavedAsync()
        {
            _autoResume = true;
            CanPrepTurnOnSaved = false;
            LoadControls();
            AfterRestoreDice();
            BoardGameSaved(); //i think.
            SaveRoot!.LoadMod(ThisMod!, this);
            CardInformation thisCard;
            if (DidChooseColors == true)
            {
                var tempList = SaveRoot.OutCards.ToRegularDeckDict();
                SaveRoot.OutCards.Clear();
                tempList.ForEach(temps =>
                {
                    thisCard = _thisGlobals!.GetCard(temps.Deck);
                    SaveRoot.OutCards.Add(thisCard);
                });
            }
            PlayerList!.ForEach(thisPlayer =>
            {
                var nextList = thisPlayer.Hand.ToRegularDeckDict();
                DeckRegularDict<CardInformation> finList = new DeckRegularDict<CardInformation>();
                nextList.ForEach(temps =>
                {
                    thisCard = _thisGlobals!.GetCard(temps.Deck);
                    finList.Add(thisCard);
                });
                thisPlayer.Hand.ReplaceRange(finList); //so we don't lose the data.
            });
            return Task.CompletedTask;
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            LoadUpDice();
            _thisGlobals = MainContainer.Resolve<GlobalFunctions>();
            GameBoard1 = MainContainer.Resolve<GameBoardProcesses>(); //i think this is when it should be done.
            GameBoard1.GraphicsBoard = MainContainer.Resolve<GameBoardGraphicsCP>(); //i think
            ThisMod!.CurrentDealList = new HandViewModel<DealCard>(ThisMod);
            ThisMod.CurrentDealList.SendEnableProcesses(ThisMod, () => SaveRoot!.GameStatus == EnumStatus.ChooseBuy);
            ThisMod.CurrentMailList = new HandViewModel<MailCard>(ThisMod);
            ThisMod.HookUpEvents();
            if (ThisData!.IsXamarinForms == false)
                ThisMod.CurrentDealList.AutoSelect = HandViewModel<DealCard>.EnumAutoType.None;
            ThisMod.DealPile = new PileViewModel<DealCard>(ThisE, ThisMod); //there is more than one.  so can't use dependency injection this time
            ThisMod.MailPile = new PileViewModel<MailCard>(ThisE, ThisMod);
            ThisMod.DealPile.Text = "Deal Pile";
            ThisMod.MailPile.Text = "Mail Pile";
            ThisMod.CurrentDealList.Text = "Deal List";
            ThisMod.CurrentMailList.Text = "Mail List";
            ThisMod.DealPile.CurrentOnly = true;
            ThisMod.MailPile.CurrentOnly = true;
            ThisMod.CurrentDealList.Visible = true;
            IsLoaded = true; //i think needs to be here.
        }
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            _autoResume = false;
            LoadControls();
            if (isBeginning == true)
            {
                SaveRoot!.LoadMod(ThisMod!, this);
                if (PlayerList.Count() == 2 || PlayerList.Count() == 3)
                    SaveRoot.MaxMonths = 4;
                else if (PlayerList.Count() == 4)
                    SaveRoot.MaxMonths = 3;
                else
                    SaveRoot.MaxMonths = 2;
            }
            EraseColors();
            SetUpDice();
            SaveRoot!.ImmediatelyStartTurn = true; //most of the time, needs to immediately start turn.  if i am wrong, rethink.
            await ThisLoader!.FinishUpAsync(isBeginning);
        }
        protected override async Task AfterChoosingColorsAsync()
        {
            if (ThisData!.MultiPlayer == true && ThisData.Client == true)
            {
                ThisMod!.CommandContainer!.ManuelFinish = true;
                ThisCheck!.IsEnabled = true;
                return; //has to wait to hear from host again.  when they hear back, will act like its resuming game.
            }
            CustomBasicList<int> tempList = _rs!.GenerateRandomList(47 + 24, 47, 25);
            SetUpMail(tempList);
            tempList = _rs.GenerateRandomList(24);
            SetUpDeal(tempList);
            await GameBoard1!.ResetBoardAsync();
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            SaveRoot!.GameStatus = EnumStatus.Starts; //i think needs to be here so the other player can know about it.
            if (ThisData.MultiPlayer == true)
                await ThisNet!.SendRestoreGameAsync(SaveRoot); //hopefully this works.
            await StartNewTurnAsync(); //i think it will happen here now.
        }
        private void MonthLabel()
        {
            ThisMod!.MonthLabel = $"Payday, month {SingleInfo!.CurrentMonth} of {SaveRoot!.MaxMonths}";
        }
        private void SetUpMail(CustomBasicList<int> thisList)
        {
            if (thisList.Count != 47)
                throw new BasicBlankException($"Must have 47 mail cards, not {thisList.Count} cards");
            SaveRoot!.MailListLeft.Clear();
            thisList.ForEach(index =>
            {
                MailCard thisCard = (MailCard)_thisGlobals!.GetCard(index);
                SaveRoot.MailListLeft.Add(thisCard);
            });
        }
        private void PopulateMails()
        {
            var tempList = SingleInfo!.Hand.GetMailOrDealList<MailCard>(EnumCardCategory.Mail);
            ThisMod!.CurrentMailList!.HandList.ReplaceRange(tempList);
        }
        private void PopulateDeals()
        {
            var tempList = SingleInfo!.Hand.GetMailOrDealList<DealCard>(EnumCardCategory.Deal);
            ThisMod!.CurrentDealList!.HandList.ReplaceRange(tempList);
        }
        private void SetUpDeal(CustomBasicList<int> thisList)
        {
            if (thisList.Count != 24)
                throw new BasicBlankException($"Must have 24 deal cards, not {thisList.Count} cards");
            SaveRoot!.DealListLeft.Clear();
            thisList.ForEach(index =>
            {
                DealCard thisCard = (DealCard)_thisGlobals!.GetCard(index);
                SaveRoot.DealListLeft.Add(thisCard);
            });
            SaveRoot.YardSaleDealCard = SaveRoot.DealListLeft.First();
            SaveRoot.DealListLeft.RemoveFirstItem();
        }
        private async Task GameOverAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.MoneyHas -= thisPlayer.Loans;
                thisPlayer.Loans = 0; //since the money has been reduced.
            });
            SingleInfo = PlayerList.OrderByDescending(items => items.MoneyHas).Take(1).Single();
            await ShowWinAsync();
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "popupchosen":
                    ThisMod!.PopUpChosen = content; //try this way.
                    await ResumeAfterPopUpAsync();
                    break;
                case "finishyardsale":
                    await FinishYardSaleAsync();
                    break;
                case "continuedealprocesses":
                    await ContinueDealProcessesAsync();
                    break;
                case "buyerselected":
                    await BuyerSelectedAsync(int.Parse(content));
                    break;
                case "reshuffledeallist":
                    CustomBasicList<int> tempList1 = await js.DeserializeObjectAsync<CustomBasicList<int>>(content);
                    DeckRegularDict<DealCard> finList1 = new DeckRegularDict<DealCard>();
                    tempList1.ForEach(x =>
                    {
                        DealCard thisCard = (DealCard)_thisGlobals!.GetCard(x);
                        finList1.Add(thisCard);
                    });
                    await ReshuffleDealsAsync(finList1);
                    ThisCheck!.IsEnabled = true; //wait for another message.
                    break;
                case "reshufflemaillist":
                    CustomBasicList<int> tempList2 = await js.DeserializeObjectAsync<CustomBasicList<int>>(content);
                    DeckRegularDict<MailCard> finList2 = new DeckRegularDict<MailCard>();
                    tempList2.ForEach(x =>
                    {
                        MailCard thisCard = (MailCard)_thisGlobals!.GetCard(x);
                        finList2.Add(thisCard);
                    });
                    await ReshuffleMailAsync(finList2); //mail does not require waiting for another message.
                    break;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            if (DidChooseColors == true)
            {
                GameBoard1!.NewTurn();
                SaveRoot!.EndGame = false;
                SaveRoot.EndOfMonth = false;
                SaveRoot.GameStatus = EnumStatus.Starts;
                SaveRoot.RemainingMove = 0;
                OtherTurn = 0;
                MonthLabel();
                PlayerList!.ForEach(items => items.ChoseNumber = 0);
                SaveRoot.Instructions = "Roll the dice to start your turn";
                PopulateDeals();
                PopulateMails();
                ThisMod!.ThisCup!.CanShowDice = false;
            }
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task MakeMoveAsync(int space)
        {
            await GameBoard1!.AnimateMoveAsync(space);
        }
        public override async Task EndTurnAsync()
        {
            ThisMod!.MailPile!.Visible = false;
            ThisMod.DealPile!.Visible = false;
            if (PlayerList.All(items => items.InGame == false))
            {
                await GameOverAsync();
                return;
            }
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync(true);
            await StartNewTurnAsync();
        }
        private void ReloadLotteryList()
        {
            CustomBasicList<int> thisList = Enumerable.Range(1, 6).ToCustomBasicList();
            thisList.RemoveAllOnly(yy => PlayerList.Any(Items => Items.ChoseNumber == yy));
            thisList.InsertBeginning(0);
            CustomBasicList<string> tempList = thisList.CastIntegerListToStringList();
            ThisMod!.AddPopupLists(tempList);
        }
        private void ListPlayers()
        {
            CustomBasicList<string> tempList = PlayerList.Where(items => items.Id != WhoTurn).Select(Items => Items.NickName).ToCustomBasicList();
            ThisMod!.AddPopupLists(tempList);
        }
        public override async Task AfterRollingAsync()
        {
            await ResultsOfDiceAsync();
        }
        public override async Task ContinueTurnAsync()
        {
            if (DidChooseColors == false)
            {
                await base.ContinueTurnAsync();
                return;
            }
            switch (SaveRoot!.GameStatus)
            {
                case EnumStatus.None:
                    throw new BasicBlankException("Can't be none.  Rethink");
                case EnumStatus.ViewMail:
                case EnumStatus.EndingTurn:
                    SaveRoot.Instructions = "Turn is ending";
                    if (ThisTest!.NoAnimations == false)
                        await Delay!.DelayMilli(250);
                    await EndTurnAsync();
                    return;
                case EnumStatus.MakeMove:
                    SaveRoot.Instructions = "Click on the highlighted space to make your move";
                    break;
                case EnumStatus.ChooseLottery:
                    ReloadLotteryList();
                    break;
                case EnumStatus.RollCharity:
                    SaveRoot.Instructions = "Walk for charity.  Roll the dice and pay 100 times the amount you roll";
                    break;
                case EnumStatus.DealOrBuy:
                    SaveRoot.Instructions = "Please either choose to move to the nearest buy or the nearest deal space";
                    ThisMod!.AddPopupLists(new CustomBasicList<string>() { "Buy", "Deal" });
                    break;
                case EnumStatus.ChooseDeal:
                    if (SaveRoot.CurrentDeal!.Deck == 0)
                        throw new BasicBlankException("Must have a deal to look at to decide whether to choose a deal or not");
                    ThisMod!.DealPile!.Visible = true;
                    ThisMod.AddPopupLists(new CustomBasicList<string>() { "Yes", "No" });
                    break;
                case EnumStatus.RollRadio:
                    ThisMod!.ThisCup!.CanShowDice = false;
                    break;
                case EnumStatus.ChoosePlayer:
                    ListPlayers();
                    break;
                default:
                    break;
            }
            if (OtherTurn > 0)
            {
                SingleInfo = PlayerList!.GetOtherPlayer();
                ThisMod!.OtherLabel = SingleInfo.NickName;
            }
            else
            {
                SingleInfo = PlayerList!.GetWhoPlayer();
                ThisMod!.OtherLabel = "None";
            }
            await base.ContinueTurnAsync();
        }
        private void RemovePlayerDeal(DealCard thisCard)
        {
            SingleInfo!.Hand.RemoveSpecificItem(thisCard);
            SaveRoot!.OutCards.Add(thisCard);
        }
        private PaydayPlayerItem PlayerChosen()
        {
            return PlayerList.Where(items => items.NickName == ThisMod!.PopUpChosen).Single();
        }
        private async Task ReshuffleDealsAsync(DeckRegularDict<DealCard> thisList)
        {
            await ThisMod!.ShowGameMessageAsync("Deal is being reshuffled");
            SaveRoot!.DealListLeft = thisList;
            SaveRoot.OutCards.RemoveAllOnly(items => items.Deck > 24);
        }
        private async Task ReshuffleDealsAsync()
        {
            var thisList = SaveRoot!.OutCards.GetMailOrDealList<DealCard>(EnumCardCategory.Deal);
            thisList.ShuffleList();
            if (ThisData!.MultiPlayer == true)
                await ThisNet!.SendAllAsync("reshuffledeallist", thisList.GetDeckListFromObjectList());
            await ReshuffleDealsAsync(thisList);
        }
        private async Task ReshuffleMailAsync()
        {
            var thisList = SaveRoot!.OutCards.GetMailOrDealList<MailCard>(EnumCardCategory.Mail);
            thisList.ShuffleList();
            if (ThisData!.MultiPlayer == true)
                await ThisNet!.SendAllAsync("reshufflemaillist", thisList.GetDeckListFromObjectList());
            await ReshuffleMailAsync(thisList);
            await ProcessMailAsync();
        }
        private async Task ReshuffleMailAsync(DeckRegularDict<MailCard> thisList) //somehow the list was not even used.
        {
            await ThisMod!.ShowGameMessageAsync("Mail is being reshuffled");
            SaveRoot!.MailListLeft = thisList;
            SaveRoot.OutCards.RemoveAllOnly(items => items.Deck <= 24);
        }
        private void ProcessSweepStakes()
        {
            SingleInfo!.MoneyHas += 5000;
        }
        private async Task ProcessMailAsync()
        {
            if (SaveRoot!.MailListLeft.Count == 0)
            {
                SingleInfo = PlayerList!.GetWhoPlayer();
                bool NeedsToReshuffle;
                NeedsToReshuffle = ShouldReshuffle();
                if (NeedsToReshuffle == false)
                {
                    ThisCheck!.IsEnabled = true;
                    return;
                }
                await ReshuffleMailAsync();
                return;
            }
            var ThisCard = SaveRoot.MailListLeft.First();
            SaveRoot.MailListLeft.RemoveFirstItem();
            await ContinueMailProcessesAsync(ThisCard);
        }
        protected override void GetPlayerToContinueTurn() { }
        private bool ShouldReshuffle()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            if (ThisData!.MultiPlayer == false)
                return true;
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                return true;
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.OtherHuman)
                return false;
            if (ThisData.Client == false)
                return true;
            return false;
        }
        private async Task ContinueMailProcessesAsync(MailCard currentMail)
        {
            ThisMod!.MailPile!.Visible = true;
            ThisMod.MailPile.AddCard(currentMail);
            SaveRoot!.CurrentMail = currentMail;
            SaveRoot.GameStatus = EnumStatus.ViewMail;
            decimal Pays;
            switch (currentMail.MailType)
            {
                case EnumMailType.MadMoney:
                    SaveRoot.Instructions = $"Please choose the player to collect {currentMail.AmountReceived.ToCurrency(0)} from";
                    SaveRoot.GameStatus = EnumStatus.ChoosePlayer;
                    break;
                case EnumMailType.Charity:
                    Pays = Math.Abs(currentMail.AmountReceived);
                    SaveRoot.Instructions = $"{currentMail.Description} for charity. {vb.vbCrLf} Please pay {Pays.ToCurrency(0)}";
                    if (ThisTest!.NoAnimations == false)
                        await Delay!.DelaySeconds(2);
                    ProcessExpense(Pays);
                    if (ThisTest.NoAnimations == false)
                        await Delay!.DelaySeconds(1);
                    SaveRoot.OutCards.Add(currentMail);
                    break;
                case EnumMailType.MoveAhead:
                    SaveRoot.GameStatus = EnumStatus.DealOrBuy;
                    break;
                case EnumMailType.MonsterCharge:
                    Pays = Math.Abs(currentMail.AmountReceived);
                    SaveRoot.Instructions = $"You received a monster charge of {Pays.ToCurrency(0)} {vb.vbCrLf}Pay at the end of the month.";
                    await MailBillsAsync(currentMail);
                    break;
                case EnumMailType.Bill:
                    Pays = Math.Abs(currentMail.AmountReceived);
                    SaveRoot.Instructions = $"You received a bill in the amount of {Pays.ToCurrency(0)}{ vb.vbCrLf}Pay at the end of the month.";
                    await MailBillsAsync(currentMail);
                    break;
                case EnumMailType.PayNeighbor:
                    Pays = Math.Abs(currentMail.AmountReceived);
                    SaveRoot.Instructions = $"Please choose the player to pay {Pays.ToCurrency(0)} to";
                    SaveRoot.GameStatus = EnumStatus.ChoosePlayer;
                    break;
                default:
                    break;
            }
            await ContinueTurnAsync();
        }
        private async Task MailBillsAsync(MailCard thisCard)
        {
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(1);
            ThisMod!.MailPile!.Visible = false;
            SaveRoot!.CurrentMail = new MailCard();
            SingleInfo!.Hand.Add(thisCard);
            PopulateMails();
            SaveRoot.GameStatus = EnumStatus.EndingTurn;
            if (ThisTest.NoAnimations == false)
                await Delay!.DelaySeconds(2);
        }
        public async Task FinishYardSaleAsync()
        {
            SaveRoot!.YardSaleDealCard = SaveRoot.DealListLeft.First();
            SaveRoot.DealListLeft.RemoveFirstItem();
            await ContinueTurnAsync();
        }
        private async Task ProcessYardSaleAsync()
        {
            await ProcessDealAsync(true);
            if (SaveRoot!.DealListLeft.Count == 0)
            {
                bool rets;
                rets = await ProcessShuffleDealsAsync();
                if (rets == false)
                    return;
                if (ThisData!.MultiPlayer == true)
                    await ThisNet!.SendAllAsync("finishyardsale");
            }
            await FinishYardSaleAsync();
        }
        private async Task ProcessDealAsync(bool isYardSale)
        {
            if (isYardSale == true)
            {
                decimal yardSale = ThisMod!.ThisCup!.TotalDiceValue * 100;
                ReduceFromPlayer(SingleInfo!, yardSale);
                var thisDeal = SaveRoot!.YardSaleDealCard;
                SingleInfo!.Hand.Add(thisDeal!);
                ThisMod.DealPile!.Visible = true;
                ThisMod.DealPile.AddCard(thisDeal!);
                SaveRoot.Instructions = $"Yard sale.  You received {thisDeal!.Business} for {yardSale.ToCurrency(0)}";
                SaveRoot.GameStatus = EnumStatus.ViewYardSale;
                if (ThisTest!.NoAnimations == false)
                    await Delay!.DelaySeconds(2);
                ThisMod.DealPile.Visible = false;
                SaveRoot.GameStatus = EnumStatus.EndingTurn;
                return;
            }
            if (SaveRoot!.DealListLeft.Count == 0)
            {
                bool rets;
                rets = await ProcessShuffleDealsAsync();
                if (rets == false)
                    return;
                if (ThisData!.MultiPlayer == true)
                    await ThisNet!.SendAllAsync("continuedealprocesses");
            }
            await ContinueDealProcessesAsync();
        }
        private async Task ContinueDealProcessesAsync()
        {
            var thisDeal = SaveRoot!.DealListLeft.First();
            SaveRoot.DealListLeft.RemoveFirstItem();
            SaveRoot.Instructions = $"Do you want to buy {thisDeal.Business} for {thisDeal.Cost.ToCurrency(0)}?  The value is {thisDeal.Value.ToCurrency(0)}";
            SaveRoot.CurrentDeal = thisDeal;
            ThisMod!.DealPile!.AddCard(thisDeal);
            SaveRoot.GameStatus = EnumStatus.ChooseDeal;
            await ContinueTurnAsync();
        }
        private async Task<bool> ProcessShuffleDealsAsync()
        {
            if (SaveRoot!.DealListLeft.Count != 0)
                throw new BasicBlankException("Should not have ran the processes for reshuffling because not needed");
            bool rets = ShouldReshuffle();
            if (rets == false)
            {
                ThisCheck!.IsEnabled = true;
                return false;
            }
            await ReshuffleDealsAsync();
            return true;
        }
        private int Thousands(decimal amount)
        {
            int x = default;
            if (amount < 1000)
                return 0;
            var NewAmount = 0;
            do
            {
                x += 1;
                NewAmount += 1000;
                if (NewAmount >= amount)
                    return x - 1;
                if (x == 5000)
                    throw new BasicBlankException("Don't think it can be 5000 times.  If I am wrong; change it");
            }
            while (true);
        }
        private async Task MonthProcessingAsync()
        {
            SingleInfo!.MoneyHas += 3500;
            SaveRoot!.Instructions = "End of month.  Here is your paycheck of $3,500.  Please pay your bills";
            SaveRoot.EndOfMonth = false;
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(2);
            var interest = Math.Ceiling(((decimal)10 / 100) * SingleInfo.Loans);
            if (interest > 0)
                SingleInfo.MoneyHas -= interest;
            SingleInfo.CurrentMonth++;
            PayLoans();
            PayBills();
            PopulateMails();
            SaveRoot.Instructions = "Bills and loans has been paid.  View the results";
            if (ThisTest.NoAnimations == false)
                await Delay!.DelaySeconds(1.5);
            if (SaveRoot.EndGame == true)
            {
                GameBoard1!.MoveToLastSpace();
                SaveRoot.EndGame = false;
            }
            if (SaveRoot.RemainingMove > 0)
            {
                if (SingleInfo.InGame == false)
                    throw new BasicBlankException("Cannot be any remaining move because the player is no longer in the game");
                MonthLabel();
                SaveRoot.Instructions = "Finish the move by moving to the highlighted space";
                GameBoard1!.HighlightDay(SaveRoot.RemainingMove);
                SaveRoot.GameStatus = EnumStatus.MakeMove;
            }
            else
            {
                SaveRoot.GameStatus = EnumStatus.EndingTurn;
            }
            await ContinueTurnAsync();
        }
        private void PayLoans()
        {
            if (SingleInfo!.Loans == 0)
                return;
            if (SingleInfo.MoneyHas >= SingleInfo.Loans)
            {
                SingleInfo.MoneyHas -= SingleInfo.Loans;
                SingleInfo.Loans = 0;
                return;
            }
            int nThousands = Thousands(SingleInfo.MoneyHas);
            if (nThousands == 0)
                return;
            int amountPaid = nThousands * 1000;
            SingleInfo.MoneyHas -= amountPaid;
            SingleInfo.Loans -= amountPaid;
            if (SingleInfo.MoneyHas < 0 || SingleInfo.Loans < 0)
                throw new BasicBlankException("The loans and money a player has must be greater or equal to 0");
        }
        private void PayBills()
        {
            var tempList = ThisMod!.CurrentMailList!.HandList;
            tempList.ForEach(ThisMail =>
            {
                ReduceFromPlayer(SingleInfo!, Math.Abs(ThisMail.AmountReceived));
                SaveRoot!.OutCards.Add(ThisMail);
                SingleInfo!.Hand.RemoveSpecificItem(ThisMail);
            });
        }
        private void ProcessExpense(decimal amount)
        {
            GameBoard1!.AddToJackPot(amount);
            ReduceFromPlayer(SingleInfo!, amount);
        }
        private void ReduceFromPlayer(PaydayPlayerItem thisPlayer, decimal amount)
        {
            thisPlayer.MoneyHas -= amount;
            if (thisPlayer.MoneyHas < 0)
            {
                thisPlayer.Loans += Math.Abs(thisPlayer.MoneyHas);
                thisPlayer.MoneyHas = 0;
            }
            if (thisPlayer.MoneyHas < 0 || thisPlayer.Loans < 0)
                throw new BasicBlankException("The money and loans must be 0 or greater");
        }
        private async Task ProcessBuyerAsync()
        {
            if (ThisMod!.CurrentDealList!.HandList.Count == 0)
            {
                SaveRoot!.Instructions = "No deals for the bank to buy";
                if (ThisTest!.NoAnimations == false)
                    await Delay!.DelaySeconds(3);
                await ContinueTurnAsync();
                return;
            }
            SaveRoot!.GameStatus = EnumStatus.ChooseBuy;
            SaveRoot.Instructions = "Please select a deal for the bank to buy from you.";
            await ContinueTurnAsync();
        }
        private async Task ProcessLotteryAsync()
        {
            OtherTurn = WhoTurn;
            ReloadLotteryList();
            SaveRoot!.Instructions = $"Please choose a number between 0 and 6.  Choose 0 to not participate{vb.vbCrLf} The cost is $100.00.  If you win, then you receive $100.00 times the number of players that participate in addition to the $1,000 the bank donates";
            SaveRoot.GameStatus = EnumStatus.ChooseLottery;
            await ContinueTurnAsync();
        }
        private async Task ProcessJackPotAsync()
        {
            decimal newAmount = SaveRoot!.LotteryAmount;
            if (newAmount > 0)
            {
                SaveRoot.Instructions = $"Congratulations, {SingleInfo!.NickName} has won.  {newAmount.ToCurrency(0)} for rolling a six";
                SingleInfo.MoneyHas += newAmount;
                GameBoard1!.ClearJackPot();
                if (ThisTest!.NoAnimations == false)
                    await Delay!.DelaySeconds(1);
            }
        }
        private async Task ProcessRadioAsync()
        {
            OtherTurn = WhoTurn;
            SaveRoot!.Instructions = "Roll the dice.  The person to roll a 3 wins $1,000";
            SaveRoot.GameStatus = EnumStatus.RollRadio;
            await ContinueTurnAsync();
        }
        private void ProcessBirthday()
        {
            decimal moneyGained = (PlayerList.Count() - 1) * 100;
            SingleInfo!.MoneyHas += moneyGained;
            CustomBasicList<PaydayPlayerItem> tempList = PlayerList.Where(items => items.Id != WhoTurn).ToCustomBasicList();
            tempList.ForEach(Items => ReduceFromPlayer(Items, 100));
        }
        public async Task ResultsOfMoveAsync(int day)
        {
            SaveRoot!.GameStatus = EnumStatus.EndingTurn; //you have to prove otherwise.
            if (ThisMod!.ThisCup!.TotalDiceValue == 6)
                await ProcessJackPotAsync();
            if (day == 31 && SaveRoot.RemainingMove == 0)
            {
                await MonthProcessingAsync();
                return;
            }
            if (SaveRoot.EndOfMonth == true)
            {
                await MonthProcessingAsync();
                return;
            }
            EnumDay details = GameBoard1!.GetDayDetails(day);
            switch (details)
            {
                case EnumDay.Mail:
                    ThisCup!.CanShowDice = false; //because its showing mail instead.
                    await ProcessMailAsync();
                    break;
                case EnumDay.SweepStakes:
                    SaveRoot.Instructions = $"{SingleInfo!.NickName} won $5000.00 dollars in the sweepstakes";
                    if (ThisTest!.NoAnimations == false)
                        await Delay!.DelaySeconds(3);
                    ProcessSweepStakes();
                    await ContinueTurnAsync();
                    break;
                case EnumDay.Deal:
                    ThisCup!.CanShowDice = false; //because its showing mail instead.
                    await ProcessDealAsync(false);
                    break;
                case EnumDay.Buyer:
                    await ProcessBuyerAsync();
                    break;
                case EnumDay.Lottery:
                    await ProcessLotteryAsync();
                    break;
                case EnumDay.YardSale:
                    ThisCup!.CanShowDice = false;
                    await ProcessYardSaleAsync();
                    break;
                case EnumDay.ShoppingSpree:
                    SaveRoot.Instructions = "Shopping Spree.  Please pay $500.00 to the jackpot";
                    if (ThisTest!.NoAnimations == false)
                        await Delay!.DelaySeconds(3);
                    ProcessExpense(500);
                    await ContinueTurnAsync();
                    break;
                case EnumDay.SkiWeekEnd:
                    SaveRoot.Instructions = "Ski Weekend.  Please pay $500.00 to the jackpot";
                    if (ThisTest!.NoAnimations == false)
                        await Delay!.DelaySeconds(3);
                    ProcessExpense(500);
                    await ContinueTurnAsync();
                    break;
                case EnumDay.HappyBirthday:
                    SaveRoot.Instructions = "Happy birthday.  Each player pays you $100.00";
                    if (ThisTest!.NoAnimations == false)
                        await Delay!.DelaySeconds(3);
                    ProcessBirthday();
                    await ContinueTurnAsync();
                    break;
                case EnumDay.CharityConcert:
                    SaveRoot.Instructions = "Charity Concert.  Please pay $400.00 to the jackpot";
                    if (ThisTest!.NoAnimations == false)
                        await Delay!.DelaySeconds(3);
                    ProcessExpense(400);
                    await ContinueTurnAsync();
                    break;
                case EnumDay.RadioContest:
                    await ProcessRadioAsync();
                    break;
                case EnumDay.Food:
                    SaveRoot.Instructions = "Pay Food For The Month.  Please pay $600.00 to the jackpot";
                    if (ThisTest!.NoAnimations == false)
                        await Delay!.DelaySeconds(3);
                    ProcessExpense(600);
                    await ContinueTurnAsync();
                    break;
                case EnumDay.WalkForCharity:
                    OtherTurn = WhoTurn;
                    SaveRoot.GameStatus = EnumStatus.RollCharity;
                    SaveRoot.Instructions = "Walk for charity.  Rol the dice and pay 100 times the amount you roll to the jackpot";
                    if (ThisTest!.NoAnimations == false)
                        await Delay!.DelaySeconds(3);
                    await OtherTurnProgressAsync();
                    break;
                default:
                    throw new BasicBlankException($"Don't know what to do with {SaveRoot.GameStatus.ToString()}");
            }
        }
        private async Task OtherTurnProgressAsync()
        {
            bool rets;
            if (SaveRoot!.GameStatus == EnumStatus.RollRadio)
                rets = true;
            else
                rets = false;
            OtherTurn = await PlayerList!.CalculateOtherTurnAsync(rets);
            if (OtherTurn > 0)
                SingleInfo = PlayerList.GetOtherPlayer(); //i think.
            if (SaveRoot.GameStatus == EnumStatus.RollRadio)
            {
                SaveRoot.Instructions = $"{SingleInfo!.NickName} will roll for the radio contest";
                await ContinueTurnAsync();
                return;
            }
            if (OtherTurn == 0)
            {
                if (SaveRoot.GameStatus == EnumStatus.RollCharity)
                {
                    SaveRoot.GameStatus = EnumStatus.EndingTurn;
                    await ContinueTurnAsync();
                    return;
                }
                if (SaveRoot.GameStatus == EnumStatus.ChooseLottery)
                {
                    SingleInfo = PlayerList.GetWhoPlayer();
                    await StartLotteryAsync();
                    return;
                }
                throw new BasicBlankException($"Don't know what to do with {SaveRoot.GameStatus.ToString()}");
            }
            await ContinueTurnAsync();
        }
        private bool CanStartLottery()
        {
            return PlayerList.Count(Items => Items.ChoseNumber > 0) >= 2; //i think
        }
        private async Task StartLotteryAsync()
        {
            if (CanStartLottery() == false)
            {
                SaveRoot!.Instructions = "No lottery will be played because fewer than 2 players chose to participate";
                if (ThisTest!.NoAnimations == false)
                    await Delay!.DelaySeconds(1);
                SaveRoot.GameStatus = EnumStatus.EndingTurn;
                await ContinueTurnAsync();
                return;
            }
            SaveRoot!.GameStatus = EnumStatus.RollLottery;
            SingleInfo = PlayerList!.GetWhoPlayer();
            SaveRoot.Instructions = $"{SingleInfo.NickName} will roll for the lottery";
            await ContinueTurnAsync();
        }
        public int CalculateDay()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            int currentDay = SingleInfo.DayNumber;
            int newDay = currentDay + ThisCup!.TotalDiceValue;
            if (newDay > 31 && currentDay < 31)
            {
                SaveRoot!.EndOfMonth = true;
                SaveRoot.RemainingMove = newDay - 31;
                if (SaveRoot.RemainingMove < 0)
                    SaveRoot.RemainingMove = 0;
                if (SingleInfo.CurrentMonth == SaveRoot.MaxMonths)
                {
                    SaveRoot.EndGame = true;
                    SaveRoot.RemainingMove = 0;
                }
                else
                {
                    SaveRoot.EndGame = false;
                }
                return 31;
            }
            if (newDay == 31)
            {
                if (SingleInfo.CurrentMonth == SaveRoot!.MaxMonths)
                    SaveRoot.EndGame = true;
            }
            if (currentDay == 31 && SaveRoot!.RemainingMove > 0)
                return SaveRoot.RemainingMove;
            if (currentDay == 31)
                return ThisCup.TotalDiceValue;
            return currentDay + ThisCup.TotalDiceValue;
        }
        private async Task ResultsOfDiceAsync()
        {
            switch (SaveRoot!.GameStatus)
            {
                case EnumStatus.Starts:
                    GameBoard1!.HighlightDay(CalculateDay());
                    SaveRoot.GameStatus = EnumStatus.MakeMove;
                    await ContinueTurnAsync();
                    break;
                case EnumStatus.RollRadio:
                    await ContinueRadioAsync();
                    break;
                case EnumStatus.RollCharity:
                    await ContinueCharityAsync();
                    break;
                case EnumStatus.RollLottery:
                    await ContinueLotteryAsync();
                    break;
                default:
                    throw new BasicBlankException($"Don't know what to do with the results for status {SaveRoot.GameStatus.ToString()}");
            }
        }
        private async Task ContinueRadioAsync()
        {
            if (ThisCup!.TotalDiceValue != 3)
            {
                await OtherTurnProgressAsync();
                return;
            }
            SaveRoot!.Instructions = $"{SingleInfo!.NickName} has won $1,000 for being a radio contest winnner by rolling a 3.";
            SingleInfo.MoneyHas += 1000;
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(1);
            SaveRoot.GameStatus = EnumStatus.EndingTurn;
            await ContinueTurnAsync();
        }
        private async Task ContinueCharityAsync()
        {
            decimal amounts = ThisCup!.ValueOfOnlyDice * 100;
            SaveRoot!.Instructions = $"{SingleInfo!.NickName} owes {amounts.ToCurrency(0)} for the walk for charity";
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(2);
            ProcessExpense(amounts);
            await OtherTurnProgressAsync();
        }
        private async Task ContinueLotteryAsync()
        {
            decimal newAmount;
            if (PlayerList.Any(items => items.ChoseNumber == ThisCup!.ValueOfOnlyDice))
            {
                var tempList = PlayerList.Where(items => items.ChoseNumber > 0).ToCustomBasicList();
                newAmount = tempList.Count() * 100;
                newAmount += 1000;
                tempList.ForEach(items => ReduceFromPlayer(items, 100));
                SingleInfo = PlayerList.Where(items => items.ChoseNumber == ThisCup!.ValueOfOnlyDice).Single();
                SingleInfo.MoneyHas += newAmount;
                SaveRoot!.Instructions = $"{SingleInfo.NickName} has won {newAmount.ToCurrency(0)} for the lottery";
                if (ThisTest!.NoAnimations == false)
                    await Delay!.DelaySeconds(2);
                SaveRoot.GameStatus = EnumStatus.EndingTurn;
            }
            await ContinueTurnAsync();
        }
        public async Task BuyerSelectedAsync(int deck)
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            if (SingleInfo.CanSendMessage(ThisData!) == true)
                await ThisNet!.SendAllAsync("buyerselected", deck);
            var thisDeal = ThisMod!.CurrentDealList!.HandList.GetSpecificItem(deck);
            thisDeal.IsSelected = false;
            ThisMod.CurrentDealList.HandList.ReplaceAllWithGivenItem(thisDeal);
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(.5);
            RemovePlayerDeal(thisDeal);
            SingleInfo.MoneyHas += thisDeal.Value;
            var tempList = SingleInfo.Hand.GetMailOrDealList<DealCard>(EnumCardCategory.Deal);
            ThisMod.CurrentDealList.HandList.ReplaceRange(tempList);
            if (ThisTest.NoAnimations == false)
                await Delay!.DelaySeconds(1.5);
            PopulateDeals();
            if (ThisTest.NoAnimations == false)
                await Delay!.DelaySeconds(1.5);
            SaveRoot!.GameStatus = EnumStatus.EndingTurn;
            await ContinueTurnAsync();
        }
        public async Task ResumeAfterPopUpAsync()
        {
            if (SingleInfo!.CanSendMessage(ThisData!) == true)
                await ThisNet!.SendAllAsync("popupchosen", ThisMod!.PopUpChosen);
            ThisMod!.PopUpList!.ShowOnlyOneSelectedItem(ThisMod.PopUpChosen);
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(1.5);
            switch (SaveRoot!.GameStatus)
            {
                case EnumStatus.ChooseDeal:
                    if (ThisMod.PopUpChosen == "Yes")
                    {
                        var thisDeal = ThisMod.DealPile!.GetCardInfo();
                        ReduceFromPlayer(SingleInfo!, Math.Abs(thisDeal.Cost));
                        SingleInfo!.Hand.Add(thisDeal);
                        PopulateDeals();
                        if (ThisTest.NoAnimations == false)
                            await Delay!.DelaySeconds(2);
                    }
                    else
                    {
                        SaveRoot.OutCards.Add(SaveRoot.CurrentDeal!);
                    }
                    ThisMod.DealPile!.Visible = false;
                    SaveRoot.CurrentDeal = new DealCard();
                    SaveRoot.GameStatus = EnumStatus.EndingTurn;
                    await ContinueTurnAsync();
                    break;
                case EnumStatus.DealOrBuy:
                    SaveRoot.OutCards.Add(SaveRoot.CurrentMail!);
                    ThisMod.MailPile!.Visible = false;
                    SaveRoot.CurrentMail = new MailCard();
                    int nextSpace;
                    if (ThisMod.PopUpChosen == "Buy")
                        nextSpace = GameBoard1!.NextBuyerSpace();
                    else
                        nextSpace = GameBoard1!.NextDealSpace();
                    if (ThisData!.MultiPlayer == false)
                    {
                        await GameBoard1.AnimateMoveAsync(nextSpace);
                        return;
                    }
                    if (SingleInfo!.CanSendMessage(ThisData) == true)
                    {
                        await GameBoard1.AnimateMoveAsync(nextSpace);
                        return;
                    }
                    ThisCheck!.IsEnabled = true; //waiting for message to continue.
                    break;
                case EnumStatus.ChoosePlayer:
                    SaveRoot.OutCards.Add(SaveRoot.CurrentMail!);
                    var thisPlayer = PlayerChosen();
                    ThisMod.MailPile!.Visible = false;
                    SaveRoot.CurrentMail = new MailCard();
                    MailCard thisMail = ThisMod.MailPile.GetCardInfo();
                    if (thisMail.MailType == EnumMailType.MadMoney)
                    {
                        SingleInfo!.MoneyHas += Math.Abs(thisMail.AmountReceived);
                        ReduceFromPlayer(thisPlayer, Math.Abs(thisMail.AmountReceived));
                    }
                    else
                    {
                        ReduceFromPlayer(SingleInfo!, Math.Abs(thisMail.AmountReceived));
                        thisPlayer.MoneyHas += Math.Abs(thisMail.AmountReceived);
                    }
                    SaveRoot.GameStatus = EnumStatus.EndingTurn;
                    await ContinueTurnAsync();
                    break;
                case EnumStatus.ChooseLottery:
                    SingleInfo = PlayerList!.GetOtherPlayer();
                    SingleInfo.ChoseNumber = int.Parse(ThisMod.PopUpChosen);
                    if (ThisTest.NoAnimations == false)
                        await Delay!.DelaySeconds(1);
                    await OtherTurnProgressAsync();
                    break;
                default:
                    throw new BasicBlankException($"Should not do combo for {SaveRoot.GameStatus.ToString()}");
            }
        }
        protected override async Task ComputerTurnAsync()
        {
            if (DidChooseColors == false)
            {
                await ComputerChooseColorsAsync();
                return;
            }
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(.75);
            bool rets;
            switch (SaveRoot!.GameStatus)
            {
                case EnumStatus.Starts:
                case EnumStatus.RollCharity:
                case EnumStatus.RollLottery:
                case EnumStatus.RollRadio:
                    await ThisRoll!.RollDiceAsync(); //i think
                    break;
                case EnumStatus.MakeMove:
                    await GameBoard1!.AnimateMoveAsync(SaveRoot.NumberHighlighted);
                    break;
                case EnumStatus.ChooseBuy:
                    await BuyerSelectedAsync(PaydayComputerAI.BuyerSelected(this));
                    break;
                case EnumStatus.ChooseDeal:
                    rets = PaydayComputerAI.PurchaseDeal(this);
                    if (rets == true)
                        ThisMod!.PopUpChosen = "Yes";
                    else
                        ThisMod!.PopUpChosen = "No";
                    await ResumeAfterPopUpAsync();
                    break;
                case EnumStatus.ChooseLottery:
                    ThisMod!.PopUpChosen = PaydayComputerAI.NumberChosen(this).ToString();
                    await ResumeAfterPopUpAsync();
                    break;
                case EnumStatus.ChoosePlayer:
                    ThisMod!.PopUpChosen = PaydayComputerAI.PlayerChosen(this);
                    await ResumeAfterPopUpAsync();
                    break;
                case EnumStatus.DealOrBuy:
                    rets = PaydayComputerAI.LandDeal(this);
                    if (rets == true)
                        ThisMod!.PopUpChosen = "Deal";
                    else
                        ThisMod!.PopUpChosen = "Buy";
                    await ResumeAfterPopUpAsync();
                    break;
                default:
                    throw new BasicBlankException($"Can't figure out what to do about {SaveRoot.GameStatus.ToString()}");
            }
        }
    }
}