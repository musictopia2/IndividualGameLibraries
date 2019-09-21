using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.NetworkingClasses.Extensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace FluxxCP
{
    [SingletonGame]
    public partial class FluxxMainGameClass : CardGameClass<FluxxCardInformation, FluxxPlayerItem, FluxxSaveInfo>, IMiscDataNM
    {
        protected override void SetUpSelfHand()
        {
            ThisMod!.Action1!.SetUpFrames();
            ThisMod.Keeper1!.HandList = SingleInfo!.KeeperList; //i think. hopefully everything else for keepers just work.
        }
        protected override void GetPlayerToContinueTurn() { }
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
        public FluxxMainGameClass(IGamePackageResolver container) : base(container) { }

        internal FluxxViewModel? ThisMod;
        internal GlobalClass? ThisGlobal;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            ThisMod = MainContainer.Resolve<FluxxViewModel>();
            ThisGlobal = MainContainer.Resolve<GlobalClass>();
        }
        public int WhoWonGame()
        {
            if (SaveRoot!.GoalList.Count > 2)
                return 0;
            int extras = ThisGlobal!.IncreaseAmount();
            var tempList = SaveRoot.GoalList.Select(items => items.WhoWon(extras, this)).ToCustomBasicList();
            if (tempList.Count == 0)
                return 0;
            if (tempList.Count == 2)
                return 0;
            int index = tempList.Single();
            if (index == 0)
                return 0; //hopefully this simple.
            if (index == WhoTurn)
                return index;
            var thisPlayer = PlayerList![index];
            if (thisPlayer.ObeyedRulesWhenNotYourTurn() == false)
                return 0;
            return index;
        }
        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            SaveRoot!.LoadMod(ThisMod!);
            LastStepAll();
            ThisMod!.Action1!.LoadSavedGame(); //i think here.
            return base.FinishGetSavedAsync();
        }
        protected override Task LastPartOfSetUpBeforeBindingsAsync()
        {
            if (ThisMod!.Deck1!.CardExists(1))
                throw new BasicBlankException("The first card cannot be in the deck");

            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.KeeperList.Clear();
            });
            SaveRoot!.GoalList.Clear();
            SaveRoot.QueList.Clear(); //i think.
            SaveRoot.SavedActionData.TempDiscardList.Clear();
            SaveRoot.SavedActionData.TempHandList.Clear();
            SaveRoot.SavedActionData.SelectedIndex = -1;
            SaveRoot.CurrentAction = 0;
            SaveRoot.RuleList.Clear();
            SaveRoot.RuleList.Add((RuleCard)DeckList!.GetSpecificItem(1));
            LastStepAll();
            if (ThisMod.FluxxScreenUsed == EnumActionScreen.ActionScreen)
                throw new BasicBlankException("Actions cannot be visible when finishing setup");
            return base.LastPartOfSetUpBeforeBindingsAsync();
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            ThisMod!.KeeperControl1!.Init(); //has to be here because needs the players.
            IsLoaded = true; //i think needs to be here.
        }
        protected override async Task ComputerTurnAsync()
        {
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(.5);
            if (OtherTurn > 0)
            {
                if (IsFirstPlayRandom())
                {
                    await PlayRandomCardAsync(ComputerAI.FirstRandomPlayed(this));
                    return;
                }
                if (ThisGlobal!.CurrentAction != null && ThisGlobal.CurrentAction.Deck == EnumActionMain.Taxation)
                {
                    await GiveCardsForTaxationAsync(ComputerAI.CardsForTaxation(this, ThisGlobal));
                    return;
                }
                int cardsNeeded = SingleInfo!.KeeperList.Count - SaveRoot!.KeeperLimit;
                if (cardsNeeded > 0 && SaveRoot.KeeperLimit > -1)
                {
                    await DiscardKeepersAsync(ComputerAI.DiscardKeepers(this, cardsNeeded));
                    return;
                }
                cardsNeeded = SingleInfo.MainHandList.Count - SaveRoot.HandLimit;
                if (cardsNeeded <= 0)
                    throw new BasicBlankException("Since keepers are not being discarded and no cards are being discarded; there are no limits that still needs to be obeyed");
                await DiscardFromHandAsync(ComputerAI.CardsToDiscardFromHand(this, cardsNeeded));
                return;
            }
            if (ThisMod!.FluxxScreenUsed == EnumActionScreen.KeeperScreen)
            {
                if (ThisGlobal!.CurrentAction!.Deck == EnumActionMain.TrashAKeeper || ThisGlobal.CurrentAction.Deck == EnumActionMain.StealAKeeper)
                {
                    bool isTrashed = ThisGlobal.CurrentAction.Deck == EnumActionMain.TrashAKeeper;
                    var thisKeeper = ComputerAI.KeeperToStealTrash(this, isTrashed);
                    ThisMod.KeeperControl1!.ShowSelectedKeepers(new CustomBasicList<KeeperPlayer> { thisKeeper });
                    await ProcessTrashStealKeeperAsync(thisKeeper, isTrashed);
                    return;
                }
                if (ThisGlobal.CurrentAction.Deck == EnumActionMain.ExchangeKeepers)
                {
                    var keeperTuple = ComputerAI.ExchangeKeepers(this);
                    ThisMod.KeeperControl1!.ShowSelectedKeepers(new CustomBasicList<KeeperPlayer> { keeperTuple.Item1, keeperTuple.Item2 });
                    await ProcessExchangeKeepersAsync(keeperTuple.Item1, keeperTuple.Item2);
                    return;
                }
                throw new BasicBlankException("The scroll keepers should not be visible for computer player.  Rethink");
            }
            int deck;
            int selectedIndex;
            CustomBasicList<int> tempList;
            if (ThisMod.FluxxScreenUsed == EnumActionScreen.ActionScreen)
            {
                switch (ThisGlobal!.CurrentAction!.Deck)
                {
                    case EnumActionMain.Draw3Play2OfThem:
                    case EnumActionMain.Draw2AndUseEm:
                        deck = ComputerAI.TempCardUse(ThisGlobal);
                        await ThisMod.Action1!.ShowCardUseAsync(deck);
                        await DrawUsedAsync(deck);
                        break;
                    case EnumActionMain.EverybodyGets1:
                        selectedIndex = ComputerAI.GetPlayerSelectedIndex(this);
                        deck = ComputerAI.TempCardUse(ThisGlobal);
                        tempList = new CustomBasicList<int> { deck };
                        await ThisMod.Action1!.ShowChosenForEverybodyGetsOneAsync(tempList, selectedIndex);
                        await EverybodyGetsOneAsync(tempList, selectedIndex);
                        break;
                    case EnumActionMain.LetsDoThatAgain:
                        selectedIndex = ComputerAI.CardToDoAgain(this);
                        await ThisMod.Action1!.ShowLetsDoAgainAsync(selectedIndex);
                        var thisCard = ThisMod.Action1.GetCardToDoAgain(selectedIndex);
                        await DoAgainProcessPart1Async(selectedIndex);
                        await PlayCardAsync(thisCard);
                        break;
                    case EnumActionMain.LetsSimplify:
                        tempList = ComputerAI.SimplifyRules(this);
                        await ThisMod.Action1!.ShowRulesSimplifiedAsync(tempList);
                        await SimplifyRulesAsync(tempList);
                        break;
                    case EnumActionMain.RotateHands:
                        var thisDirection = ComputerAI.GetDirection();
                        await ThisMod.Action1!.ShowDirectionAsync((int)thisDirection);
                        await RotateHandAsync(thisDirection);
                        break;
                    case EnumActionMain.TradeHands:
                        selectedIndex = ComputerAI.GetPlayerSelectedIndex(this);
                        await ThisMod.Action1!.ShowTradeHandAsync(selectedIndex);
                        await TradeHandAsync(selectedIndex);
                        break;
                    case EnumActionMain.TrashANewRule:
                        selectedIndex = ComputerAI.RuleToTrash(this);
                        await ThisMod.Action1!.ShowRuleTrashedAsync(selectedIndex);
                        await TrashNewRuleAsync(selectedIndex);
                        break;
                    case EnumActionMain.UseWhatYouTake:
                        if (SaveRoot!.SavedActionData.SelectedIndex == -1)
                        {
                            selectedIndex = ComputerAI.GetPlayerSelectedIndex(this);
                            await ThisMod.Action1!.ShowPlayerForCardChosenAsync(selectedIndex);
                            await ChosePlayerOnActionAsync(selectedIndex);
                            await ContinueTurnAsync();
                            return;
                        }
                        deck = ComputerAI.UseTake(this, SaveRoot.SavedActionData.SelectedIndex);
                        int index = ThisMod.Action1!.GetPlayerIndex(SaveRoot.SavedActionData.SelectedIndex);
                        await PlayUseTakeAsync(deck, index);
                        break;
                    default:
                        throw new BasicBlankException("No action found for computer on action screen");
                }
                return; //i think just in case.
            }
            var thisStatus = StatusEndRegularTurn();
            int numberNeeded;
            switch (thisStatus)
            {
                case EnumEndTurnStatus.Successful:
                    if (ThisData!.MultiPlayer)
                        await ThisNet!.SendEndTurnAsync();
                    await EndTurnAsync();
                    break;
                case EnumEndTurnStatus.Hand:
                    numberNeeded = SingleInfo!.MainHandList.Count - SaveRoot!.HandLimit;
                    if (numberNeeded <= 0)
                        throw new BasicBlankException("No hand limit or already followed it");
                    await DiscardFromHandAsync(ComputerAI.CardsToDiscardFromHand(this, numberNeeded));
                    break;
                case EnumEndTurnStatus.Play:
                    var tempCard = ComputerAI.CardToPlay(this);
                    await SendPlayAsync(tempCard.Deck);
                    await PlayCardAsync(tempCard);
                    break;
                case EnumEndTurnStatus.Keeper:
                    numberNeeded = SingleInfo!.KeeperList.Count - SaveRoot!.KeeperLimit;
                    if (numberNeeded <= 0)
                        throw new BasicBlankException("No keeper limit or already followed");
                    await DiscardKeepersAsync(ComputerAI.DiscardKeepers(this, numberNeeded));
                    break;
                case EnumEndTurnStatus.Goal:
                    await DiscardGoalAsync(ComputerAI.GoalToRemove(this));
                    await AnalyzeQueAsync(); //i think.
                    break;
                default:
                    throw new BasicBlankException("Wrong");
            }
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            if (ThisMod!.FluxxScreenUsed == EnumActionScreen.ActionScreen)
                throw new BasicBlankException("Action screen cannot be visible to begin with");
            if (ThisMod.Deck1!.CardExists(1))
                throw new BasicBlankException("The first card cannot be in the deck");
            LoadControls();
            SaveRoot!.LoadMod(ThisMod);
            SaveRoot.ImmediatelyStartTurn = true;
            return base.StartSetUpAsync(isBeginning);
        }
        private void LastStepAll()
        {
            ThisMod!.Action1!.SetUpGoals();
            if (SaveRoot!.CurrentAction > 0)
                ThisGlobal!.CurrentAction = (ActionCard)DeckList!.GetSpecificItem(SaveRoot.CurrentAction);
            else
                ThisGlobal!.CurrentAction = null;

            ThisGlobal.QuePlayList.Clear();
            ThisGlobal.QuePlayList = SaveRoot.QueList.GetNewObjectListFromDeckList(DeckList!).ToRegularDeckDict();
            SingleInfo = PlayerList!.GetSelf();
            ThisMod.Keeper1!.HandList = SingleInfo.KeeperList;
            ThisMod.Action1.SetUpFrames(); //i think.
            ThisMod.Goal1!.HandList = SaveRoot.GoalList;
            ThisMod.KeeperControl1!.LoadSavedGame();
        }
        protected override async Task AddCardAsync(FluxxCardInformation thisCard, FluxxPlayerItem tempPlayer)
        {
            if (thisCard.Deck == 1)
                throw new BasicBlankException("The basic rule can never be drawn");
            if (_allNewCards)
                thisCard.Drew = false;
            if (_doDrawTemporary == false)
            {
                await base.AddCardAsync(thisCard, tempPlayer);
                return;
            }
            thisCard.Drew = false;
            ThisGlobal!.TempActionHandList.Add(thisCard.Deck);
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "taxation":
                    var thisList1 = await content.GetObjectsFromDataAsync(SingleInfo!.MainHandList); //i think.
                    await GiveCardsForTaxationAsync(thisList1);
                    return;
                case "emptytrash":
                    var thisList2 = await content.GetNewObjectListFromDeckListAsync(DeckList!);
                    await FinishEmptyTrashAsync(thisList2);
                    return;
                case "discardfromhand":
                    var thisList3 = await content.GetSavedIntegerListAsync();
                    await DiscardFromHandAsync(thisList3);
                    return;
                case "discardkeepers":
                    var thisList4 = await content.GetSavedIntegerListAsync();
                    await DiscardKeepersAsync(thisList4);
                    return;
                case "discardgoal":
                    await DiscardGoalAsync(int.Parse(content));
                    await AnalyzeQueAsync();
                    return;
                case "trashkeeper":
                case "stealkeeper":
                    KeeperPlayer thisKeep = await js.DeserializeObjectAsync<KeeperPlayer>(content);
                    bool isTrashed = status == "trashkeeper";
                    var tempList1 = new CustomBasicList<KeeperPlayer> { thisKeep };
                    ThisMod!.KeeperControl1!.ShowSelectedKeepers(tempList1);
                    if (ThisTest!.NoAnimations == false)
                        await Delay!.DelaySeconds(1);
                    await ProcessTrashStealKeeperAsync(thisKeep, isTrashed);
                    return;
                case "exchangekeepers":
                    CustomBasicList<KeeperPlayer> thisList5 = await js.DeserializeObjectAsync<CustomBasicList<KeeperPlayer>>(content);
                    ThisMod!.KeeperControl1!.ShowSelectedKeepers(thisList5);
                    if (ThisTest!.NoAnimations == false)
                        await Delay!.DelaySeconds(1);
                    await ProcessExchangeKeepersAsync(thisList5.First(), thisList5.Last());
                    return;
                case "firstrandom":
                    await PlayRandomCardAsync(int.Parse(content));
                    return;
                case "usetake":
                    var thisList6 = await content.GetSavedIntegerListAsync();
                    ThisMod!.FluxxScreenUsed = EnumActionScreen.None;
                    ThisMod.Title = "Fluxx";
                    await PlayRandomCardAsync(thisList6.First(), thisList6.Last());
                    return;
                case "simplifyrules":
                    var thisList7 = await content.GetSavedIntegerListAsync();
                    await ThisMod!.Action1!.ShowRulesSimplifiedAsync(thisList7);
                    await SimplifyRulesAsync(thisList7);
                    return;
                case "trashnewrule":
                    await ThisMod!.Action1!.ShowRuleTrashedAsync(int.Parse(content));
                    await TrashNewRuleAsync(int.Parse(content));
                    return;
                case "doagain":
                    await ThisMod!.Action1!.ShowLetsDoAgainAsync(int.Parse(content));
                    var thisCard = ThisMod.Action1.GetCardToDoAgain(int.Parse(content));
                    await PlayCardAsync(thisCard);
                    return;
                case "rotatehands":
                    await ThisMod!.Action1!.ShowDirectionAsync(int.Parse(content));
                    await RotateHandAsync((EnumDirection)int.Parse(content));
                    return;
                case "tradehands":
                    await ThisMod!.Action1!.ShowTradeHandAsync(int.Parse(content));
                    await TradeHandAsync(int.Parse(content));
                    return;
                case "choseplayerforcardchosen":
                    await ThisMod!.Action1!.ShowPlayerForCardChosenAsync(int.Parse(content));
                    await ContinueTurnAsync();
                    return;
                case "everybodygetsone":
                    var thisList8 = await content.GetSavedIntegerListAsync();
                    int selectedIndex = thisList8.Last();
                    thisList8.RemoveLastItem();
                    await ThisMod!.Action1!.ShowChosenForEverybodyGetsOneAsync(thisList8, selectedIndex);
                    await EverybodyGetsOneAsync(thisList8, selectedIndex);
                    return;
                case "drawuse":
                    await ThisMod!.Action1!.ShowCardUseAsync(int.Parse(content));
                    await DrawUsedAsync(int.Parse(content));
                    return;
                case "playcard":
                    SingleInfo = PlayerList!.GetWhoPlayer();
                    await PlayCardAsync(int.Parse(content));
                    return;
                case "scramblekeepers":
                    var thisList = PlayerList.Where(items => items.KeeperList.Count > 0).ToCustomBasicList();
                    if (thisList.Count < 2)
                        throw new BasicBlankException("Must have at least 2 players with keepers in order to scramble keepers");
                    CustomBasicList<CustomBasicList<int>> finList = await js.DeserializeObjectAsync<CustomBasicList<CustomBasicList<int>>>(content);
                    if (finList.Count != thisList.Count)
                        throw new BasicBlankException("When other player is scrambling keepers, does not reconcile");
                    int x = 0;
                    thisList.ForEach(thisPlayer =>
                    {
                        if (thisPlayer.KeeperList.Count != finList[x].Count)
                            throw new BasicBlankException("Wrong count on other player end");
                        thisPlayer.KeeperList.Clear();
                        foreach (var thisItem in finList[x])
                        {
                            thisPlayer.KeeperList.Add((KeeperCard)DeckList!.GetSpecificItem(thisItem));
                        }
                        x++;
                    });
                    await AnalyzeQueAsync();
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            SaveRoot!.DoAnalyze = false;
            SingleInfo = PlayerList!.GetWhoPlayer();
            if (ThisGlobal!.CurrentAction != null)
                throw new BasicBlankException("Current action must be nothing to begin with");
            if (ThisGlobal.QuePlayList.Count > 0)
                throw new BasicBlankException("Cannot have any items on the que list when starting a new turn");
            SaveRoot.CardsPlayed = 0;
            SaveRoot.CardsDrawn = 0;
            SaveRoot.AnotherTurn = false;
            ThisMod!.OtherTurn = "";
            SaveRoot.PlaysLeft = 0;
            SaveRoot.PlayBonus = 0;
            int extras = ThisGlobal.IncreaseAmount();
            if (SaveRoot.RuleList.Any(items => items.Deck == EnumRuleText.NoHandBonus) && SingleInfo.MainHandList.Count == 0)
                SaveRoot.PreviousBonus += 3;
            else
                SaveRoot.PreviousBonus = 0;
            if (ThisMod.FluxxScreenUsed == EnumActionScreen.ActionScreen)
                throw new BasicBlankException("Actions cannot be visible when starting new turn");
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task ContinueTurnAsync()
        {
            if (SaveRoot!.DoAnalyze)
            {
                await AnalyzeQueAsync();
                return; //because upon autoresume, it could need to do this.
            }
            int wins = WhoWonGame();
            if (wins > 0)
            {
                SingleInfo = PlayerList![wins];
                await ShowWinAsync();
                return;
            }
            SingleInfo = PlayerList!.GetWhoPlayer(); //for sure needed here.
            AnalyzeRules();
            if (LeftToDraw > 0)
            {
                await DrawAsync();
                return;
            }
            var tempList = PlayerList.ToCustomBasicList();
            tempList.RemoveSpecificItem(SingleInfo);
            var thisPlayer = tempList.Where(items => items.ObeyedRulesWhenNotYourTurn() == false).Take(1).SingleOrDefault();
            if (thisPlayer != null)
                OtherTurn = thisPlayer.Id;
            if (ThisGlobal!.CurrentAction != null && ThisGlobal.CurrentAction.Deck == EnumActionMain.Taxation)
                ThisMod!.PlayGiveText = "Give";
            else if (thisPlayer == null)
                OtherTurn = 0;
            if (OtherTurn > 0)
            {
                SingleInfo = PlayerList.GetOtherPlayer();
                ThisMod!.OtherTurn = SingleInfo.NickName;
            }
            else
            {
                ThisMod!.OtherTurn = "";
            }

            if (IsFirstPlayRandom())
            {
                OtherTurn = await PlayerList.CalculateWhoTurnAsync();
                SingleInfo = PlayerList.GetOtherPlayer();
                ThisMod.OtherTurn = SingleInfo.NickName;
            }
            await base.ContinueTurnAsync();
        }
        public override async Task EndTurnAsync()
        {
            ThisMod!.CommandContainer!.ManuelFinish = true; //because it could be somebody else's turn.
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.MainHandList.UnhighlightObjects(); //i think this is best.
            ThisMod.Goal1!.EndTurn();
            ThisMod.Keeper1!.EndTurn();
            if (ThisGlobal!.QuePlayList.Count > 0)
            {
                await PlayCardAsync(ThisGlobal.QuePlayList.First());
                return;
            }
            if (SaveRoot!.AnotherTurn == false)
                WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
        }
        protected override async Task AfterDrawingAsync()
        {
            _doDrawTemporary = false;
            _allNewCards = false;
            if (SaveRoot!.DoAnalyze)
            {
                await AnalyzeQueAsync();
                return;
            }
            if (ThisGlobal!.CurrentAction == null)
            {
                await ContinueTurnAsync();
                return;
            }
            await AnalyzeQueAsync();
        }
        protected override Task LoadPossibleOtherScreensAsync()
        {
            EnumActionScreen thisScreen = ScreenToLoad();
            if (thisScreen == EnumActionScreen.ActionScreen)
            {
                ThisMod!.Action1!.LoadActionScreen();
                return Task.CompletedTask;
            }
            if (thisScreen == EnumActionScreen.KeeperScreen)
            {
                ThisMod!.KeeperControl1!.LoadKeeperScreen();
                return Task.CompletedTask;
            }
            return Task.CompletedTask;
        }
        public override Task PopulateSaveRootAsync()
        {
            SaveRoot!.QueList = ThisGlobal!.QuePlayList.GetDeckListFromObjectList();
            if (ThisGlobal.CurrentAction == null)
            {
                SaveRoot.CurrentAction = 0;
                SaveRoot.SavedActionData.SelectedIndex = -1;
            }
            else
            {
                SaveRoot.CurrentAction = (int)ThisGlobal.CurrentAction.Deck;
                if (ThisGlobal.CurrentAction.Deck == EnumActionMain.UseWhatYouTake)
                    SaveRoot.SavedActionData.SelectedIndex = ThisMod!.Action1!.IndexPlayer;
                else
                    SaveRoot.SavedActionData.SelectedIndex = -1;
            }
            return base.PopulateSaveRootAsync();
        }
        public void RemoveFromHandOnly(FluxxCardInformation thisCard)
        {
            SingleInfo!.MainHandList.RemoveObjectByDeck(thisCard.Deck);
        }
        public async Task DiscardRuleAsync(string name)
        {
            var thisRule = SaveRoot!.RuleList.Single(items => items.Deck.ToString().TextWithSpaces() == name);
            await DiscardRuleAsync(thisRule);
        }
        public async Task DiscardRuleAsync(RuleCard thisRule)
        {
            SaveRoot!.RuleList.RemoveObjectByDeck((int)thisRule.Deck);
            await AnimatePlayAsync(thisRule);
        }
        public void RefreshRules()
        {
            SaveRoot!.PlayOrder.IsReversed = SaveRoot.RuleList.Any(items => items.Deck == EnumRuleText.ReverseOrder);
        }
        public async Task DiscardFromHandAsync(IDeckDict<FluxxCardInformation> thisList)
        {
            if (SingleInfo!.CanSendMessage(ThisData!))
                await ThisNet!.SendAllAsync("discardfromhand", thisList.GetDeckListFromObjectList());
            await thisList.ForEachAsync(async thisCard =>
            {
                await DiscardFromHandAsync(thisCard);
                if (ThisTest!.NoAnimations == false)
                    await Delay!.DelaySeconds(.1);
            });
            await AnalyzeQueAsync();
        }
        public async Task DiscardFromHandAsync(CustomBasicList<int> thisList)
        {
            var newList = thisList.GetFluxxCardListFromDeck(this);
            await DiscardFromHandAsync(newList);
        }
        public async Task DiscardFromHandAsync(FluxxCardInformation thisCard)
        {
            RemoveFromHandOnly(thisCard);
            await AnimatePlayAsync(thisCard);
        }
        public async Task DiscardFromHandAsync(int deck)
        {
            var thisCard = SingleInfo!.MainHandList.GetSpecificItem(deck);
            await DiscardFromHandAsync(thisCard);
        }
        public async Task DiscardGoalAsync(int deck)
        {
            if (SingleInfo!.CanSendMessage(ThisData!))
                await ThisNet!.SendAllAsync("discardgoal", deck);
            var thisGoal = (GoalCard)DeckList!.GetSpecificItem(deck);
            await DiscardGoalAsync(thisGoal);
        }
        public async Task DiscardGoalAsync(GoalCard thisCard)
        {
            SaveRoot!.GoalList.RemoveObjectByDeck((int)thisCard.Deck);
            await AnimatePlayAsync(thisCard);
        }
        public async Task DiscardKeeperAsync(int deck)
        {
            var thisKeeper = (KeeperCard)DeckList!.GetSpecificItem(deck);
            await DiscardKeeperAsync(thisKeeper);
        }
        public async Task DiscardKeeperAsync(FluxxCardInformation thisCard)
        {
            SingleInfo!.KeeperList.RemoveObjectByDeck(thisCard.Deck);
            await AnimatePlayAsync(thisCard);
        }
        public async Task DiscardKeepersAsync(IDeckDict<FluxxCardInformation> thisList)
        {
            if (SingleInfo!.CanSendMessage(ThisData!))
                await ThisNet!.SendAllAsync("discardkeepers", thisList.GetDeckListFromObjectList());
            await thisList.ForEachAsync(async thisCard =>
            {
                await DiscardKeeperAsync(thisCard);
                if (ThisTest!.NoAnimations == false)
                    await Delay!.DelaySeconds(.1);
            });
            await AnalyzeQueAsync();
        }
        public async Task DiscardKeepersAsync(CustomBasicList<int> thisList)
        {
            var newList = thisList.GetFluxxCardListFromDeck(this);
            await DiscardKeepersAsync(newList);
        }
        public async Task TrashNewRuleAsync(int index)
        {
            if (SingleInfo!.CanSendMessage(ThisData!))
                await ThisNet!.SendAllAsync("trashnewrule", index);
            var thisRule = SaveRoot!.RuleList[index + 1];
            await DiscardRuleAsync(thisRule);
            RefreshRules();
            ThisGlobal!.CurrentAction = null;
            await AnalyzeQueAsync();
        }
        public async Task SimplifyRulesAsync(CustomBasicList<int> thisList)
        {
            if (SingleInfo!.CanSendMessage(ThisData!))
            {
                await ThisNet!.SendAllAsync("simplifyrules", thisList);
            }
            DeckRegularDict<RuleCard> newList = new DeckRegularDict<RuleCard>();
            thisList.ForEach(index =>
            {
                newList.Add(SaveRoot!.RuleList[index + 1]);
            });
            await newList.ForEachAsync(async thisRule =>
            {
                await DiscardRuleAsync(thisRule);
            });
            if (thisList.Count > 0)
                RefreshRules();
            ThisGlobal!.CurrentAction = null;
            await AnalyzeQueAsync();
        }
        public async Task GiveCardsForTaxationAsync(IDeckDict<FluxxCardInformation> thisList)
        {
            if (ThisGlobal!.CurrentAction == null)
                throw new BasicBlankException("Must have a current action in order to give cards for taxation");
            if (ThisGlobal.CurrentAction.Deck != EnumActionMain.Taxation)
                throw new BasicBlankException("The current action must be taxation");
            SingleInfo = PlayerList!.GetOtherPlayer();
            if (SingleInfo.CanSendMessage(ThisData!))
                await ThisNet!.SendAllAsync("taxation", thisList.GetDeckListFromObjectList());
            SingleInfo = PlayerList.GetWhoPlayer();
            thisList.ForEach(thisCard => thisCard.Drew = true);
            SingleInfo.MainHandList.AddRange(thisList);
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                SortCards();
            ThisMod!.PlayGiveText = "Play";
            do
            {
                OtherTurn = await PlayerList.CalculateOtherTurnAsync();
                if (OtherTurn == 0)
                {
                    ThisGlobal.CurrentAction = null;
                    break;
                }
                SingleInfo = PlayerList.GetOtherPlayer();
                if (SingleInfo.MainHandList.Count > 0)
                    break;
            } while (true);
            SingleInfo = PlayerList.GetWhoPlayer();
            await AnalyzeQueAsync();
        }
        public async Task ChosePlayerOnActionAsync(int selectedIndex)
        {
            if (SingleInfo!.CanSendMessage(ThisData!))
                await ThisNet!.SendAllAsync("choseplayerforcardchosen", selectedIndex);
        }
        public async Task PlayUseTakeAsync(int deck, int player)
        {
            if (SingleInfo!.CanSendMessage(ThisData!))
            {
                CustomBasicList<int> newList = new CustomBasicList<int> { deck, player };
                await ThisNet!.SendAllAsync("usetake", newList);
            }
            await PlayRandomCardAsync(deck, player);
        }
        public async Task PlayRandomCardAsync(int deck, int player)
        {
            var tempPlayer = PlayerList![player];
            var thisCard = tempPlayer.MainHandList.GetSpecificItem(deck);
            await PlayRandomCardAsync(thisCard, player);
        }
        public async Task PlayRandomCardAsync(FluxxCardInformation thisCard, int player)
        {
            ThisMod!.Action1!.ResetPlayers();
            if (OtherTurn > 0)
            {
                SingleInfo = PlayerList!.GetOtherPlayer();
                if (SingleInfo.PlayerCategory != EnumPlayerCategory.Self)
                    await ThisMod.Action1.ChoseOtherCardSelectedAsync(thisCard.Deck);
                SingleInfo = PlayerList.GetWhoPlayer();
                OtherTurn = 0;
                await PlayCardAsync(thisCard);
                return;
            }
            ThisGlobal!.CurrentAction = null;
            var tempPlayer = PlayerList![player];
            tempPlayer.MainHandList.RemoveObjectByDeck(thisCard.Deck);
            SingleInfo = PlayerList.GetWhoPlayer();
            ThisGlobal.QuePlayList.Add(thisCard);
            await AnalyzeQueAsync();
        }
        public async Task PlayRandomCardAsync(int deck)
        {
            if (SingleInfo!.CanSendMessage(ThisData!))
                await ThisNet!.SendAllAsync("firstrandom", deck);
            SingleInfo = PlayerList!.GetWhoPlayer();
            await PlayRandomCardAsync(deck, WhoTurn);
        }
        public async Task PlayRandomCardAsync(FluxxCardInformation thisCard)
        {
            await PlayRandomCardAsync(thisCard, WhoTurn);
        }
        public async Task ProcessTrashStealKeeperAsync(KeeperPlayer thisKeeper, bool isTrashed)
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            if (SingleInfo.CanSendMessage(ThisData!))
            {
                string status;
                if (isTrashed)
                    status = "trashkeeper";
                else
                    status = "stealkeeper";
                await ThisNet!.SendAllAsync(status, thisKeeper);
            }
            ThisMod!.FluxxScreenUsed = EnumActionScreen.None;
            ThisMod.Title = "Fluxx"; //i think.
            SingleInfo = PlayerList[thisKeeper.Player];
            if (isTrashed)
            {
                await DiscardKeeperAsync(thisKeeper.Card);
                ThisGlobal!.CurrentAction = null;
                SingleInfo = PlayerList.GetWhoPlayer();
                await AnalyzeQueAsync();
                return;
            }
            var thisCard = SingleInfo.KeeperList.RemoveObjectByDeck(thisKeeper.Card);
            SingleInfo = PlayerList.GetWhoPlayer();
            thisCard.IsSelected = false;
            thisCard.Drew = false;
            SingleInfo.KeeperList.Add(thisCard);
            ThisGlobal!.CurrentAction = null;
            await AnalyzeQueAsync();
        }
        public async Task ProcessExchangeKeepersAsync(KeeperPlayer keeperFrom, KeeperPlayer keeperTo)
        {
            if (SingleInfo!.CanSendMessage(ThisData!))
            {
                var thisList = new CustomBasicList<KeeperPlayer> { keeperFrom, keeperTo };
                await ThisNet!.SendAllAsync("exchangekeepers", thisList);
            }
            ThisMod!.FluxxScreenUsed = EnumActionScreen.None;
            ThisMod.Title = "Fluxx"; //i think.
            KeeperCard fromCard;
            KeeperCard toCard;
            FluxxPlayerItem fromPlayer;
            FluxxPlayerItem toPlayer;
            fromPlayer = PlayerList![keeperFrom.Player];
            toPlayer = PlayerList[keeperTo.Player];
            fromCard = fromPlayer.KeeperList.RemoveObjectByDeck(keeperFrom.Card);
            toCard = toPlayer.KeeperList.RemoveObjectByDeck(keeperTo.Card);
            toCard.IsSelected = false;
            fromCard.IsSelected = false;
            toCard.Drew = false;
            fromCard.Drew = false;
            fromPlayer.KeeperList.Add(toCard);
            toPlayer.KeeperList.Add(fromCard);
            ThisGlobal!.CurrentAction = null;
            await AnalyzeQueAsync();
        }
    }
}