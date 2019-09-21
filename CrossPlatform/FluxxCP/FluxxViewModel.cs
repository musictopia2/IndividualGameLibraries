using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.NetworkingClasses.Extensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace FluxxCP
{
    public partial class FluxxViewModel : BasicCardGamesVM<FluxxCardInformation, FluxxPlayerItem, FluxxMainGameClass>, IFluxxEvent
    {
        private EnumActionScreen _FluxxScreenUsed;
        public EnumActionScreen FluxxScreenUsed
        {
            get { return _FluxxScreenUsed; }
            set
            {
                if (SetProperty(ref _FluxxScreenUsed, value))
                {
                    if (value == EnumActionScreen.OtherPlayer)
                        throw new BasicBlankException("Only a card can use the category of otherplayer");
                    OnPropertyChanged(nameof(ActionVisible));
                    OnPropertyChanged(nameof(KeeperVisible));
                    if (Action1 != null)
                        Action1.VisibleChange();
                    if (KeeperControl1 != null)
                        KeeperControl1.VisibleChange();
                    if (value == EnumActionScreen.None)
                        MainOptionsVisible = true;
                    else
                        MainOptionsVisible = false;// i think
                }
            }
        }
        private string _OtherTurn = "";
        public string OtherTurn
        {
            get { return _OtherTurn; }
            set
            {
                if (SetProperty(ref _OtherTurn, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public bool ActionVisible
        {
            get
            {
                if (FluxxScreenUsed == EnumActionScreen.ActionScreen)
                    return true;
                return false;
            }
        }
        public bool KeeperVisible
        {
            get
            {
                if (FluxxScreenUsed == EnumActionScreen.KeeperScreen)
                    return true;
                return false;
            }
        }
        private string _PlayGiveText = "Play";
        public string PlayGiveText
        {
            get
            {
                return _PlayGiveText;
            }

            set
            {
                if (SetProperty(ref _PlayGiveText, value) == true)
                {
                }
            }
        }
        public async Task ShowPlayCardAsync(FluxxCardInformation thisCard)
        {
            if (thisCard.Deck != ThisDetail!.CurrentCard.Deck)
            {
                ThisDetail.ShowCard(thisCard);
                if (ThisTest!.NoAnimations == false)
                    await MainGame!.Delay!.DelaySeconds(1);
            }
            ThisDetail.ResetCard();
        }
        protected override Task OnConsiderSelectOneCardAsync(FluxxCardInformation thisObject)
        {
            if (thisObject.Deck == ThisDetail!.CurrentCard.Deck)
                ThisDetail.ResetCard();
            else
                ThisDetail.ShowCard(thisObject);
            return Task.CompletedTask;
        }
        public FluxxViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableDeck()
        {
            return false;
        }
        protected override bool CanEnablePile1()
        {
            return false;
        }
        protected override async Task EndTurnProcess()
        {
            var ends = MainGame!.StatusEndRegularTurn();
            if (ends == EnumEndTurnStatus.Goal)
            {
                await ShowGameMessageAsync("Sorry; you must get rid of excess goals");
                return;
            }
            if (ends == EnumEndTurnStatus.Play)
            {
                await ShowGameMessageAsync("Sorry; you have plays remaining");
                return;
            }
            if (ends == EnumEndTurnStatus.Hand)
            {
                await ShowGameMessageAsync("Sorry; you have too many cards in your hand");
                return;
            }
            if (ends == EnumEndTurnStatus.Keeper)
            {
                await ShowGameMessageAsync("Sorry; you have too many keepers");
                return;
            }
            if (ThisData!.MultiPlayer)
                await ThisNet!.SendEndTurnAsync();
            await MainGame.EndTurnAsync();
        }
        protected override async Task ProcessDiscardClickedAsync()
        {
            await Task.CompletedTask;
        }
        private async Task DiscardProcessesAsync()
        {
            int goalDiscarded = Goal1!.ObjectSelected();
            int keepers = Keeper1!.HowManySelectedObjects;
            int yours = PlayerHand1!.HowManySelectedObjects;
            if (goalDiscarded == 0 && keepers == 0 && yours == 0)
            {
                await ShowGameMessageAsync("There is no cards selected to discard");
                return;
            }
            CustomBasicList<int> tempList = new CustomBasicList<int> { keepers, goalDiscarded, yours };
            if (tempList.Count(item => item > 0) > 1)
            {
                await ShowGameMessageAsync("Can choose a goal, keepers, or from your hand; not from more than one");
                return;
            }
            if (goalDiscarded > 0)
            {
                if (MainGame!.NeedsToRemoveGoal() == false)
                {
                    await ShowGameMessageAsync("Cannot discard any goals");
                    UnselectAllCards();
                    return;
                }
                if (goalDiscarded == (int)MainGame.SaveRoot!.GoalList.Last().Deck && MainGame.SaveRoot.GoalList.Count == 3)
                {
                    await ShowGameMessageAsync("Cannot discard the third goal on the list.  Must choose one of the 2 that was there before");
                    UnselectAllCards();
                    return;
                }
                await MainGame.DiscardGoalAsync(goalDiscarded);
                await MainGame.AnalyzeQueAsync();
                return;
            }
            if (MainGame!.NeedsToRemoveGoal())
            {
                await ShowGameMessageAsync("Must discard a goal before discarding anything else");
                UnselectAllCards();
                return;
            }
            if (yours > 0)
            {
                if (MainGame.SaveRoot!.HandLimit == -1)
                {
                    await ShowGameMessageAsync("There is no hand limit.  Therefore cannot discard any cards from your hand");
                    UnselectAllCards();
                    return;
                }
                int newCount = MainGame.SingleInfo!.MainHandList.Count - yours;
                if (newCount < MainGame.SaveRoot.HandLimit)
                {
                    await ShowGameMessageAsync($"Cannot discard from hand {yours} cards because that will cause you to discard too many cards");
                    UnselectAllCards();
                    return;
                }
                var firstList = PlayerHand1.ListSelectedObjects();
                await MainGame.DiscardFromHandAsync(firstList);
                return;
            }
            if (keepers > 0)
            {
                if (MainGame.SaveRoot!.KeeperLimit == -1)
                {
                    await ShowGameMessageAsync("There is no keeper limit.  Therefore; cannot discard any keepers");
                    UnselectAllCards();
                    return;
                }
                int newCount = MainGame.SingleInfo!.KeeperList.Count - yours;
                if (newCount < MainGame.SaveRoot.KeeperLimit)
                {
                    await ShowGameMessageAsync($"Cannot discard from keepers {keepers} cards because that will cause you to discard too many keepers");
                    UnselectAllCards();
                    return;
                }
                var firstList = Keeper1.ListSelectedObjects();
                DeckRegularDict<FluxxCardInformation> finList = new DeckRegularDict<FluxxCardInformation>(firstList);
                await MainGame.DiscardKeepersAsync(finList);
                return;
            }
            throw new BasicBlankException("Don't know how to discard from here");
        }
        private async Task GiveProcessAsync()
        {
            if (Keeper1!.HowManySelectedObjects > 0)
            {
                await ShowGameMessageAsync("Cannot select any keeper cards because you have to give the cards from your hand");
                UnselectAllCards();
                return;
            }
            if (Goal1!.ObjectSelected() > 0)
            {
                await ShowGameMessageAsync("Cannot select any goal cards because you have to give the cards from your hand");
                UnselectAllCards();
                return;
            }
            int howMany = MainGame!.ThisGlobal!.IncreaseAmount() + 1;
            if (PlayerHand1!.HowManySelectedObjects == howMany || PlayerHand1.HowManySelectedObjects == PlayerHand1.HandList.Count)
            {
                var thisList = PlayerHand1.ListSelectedObjects(true);
                await MainGame.GiveCardsForTaxationAsync(thisList);
                return;
            }
            if (howMany > PlayerHand1.HandList.Count)
                howMany = PlayerHand1.HandList.Count;
            await ShowGameMessageAsync($"Must give {howMany} not {PlayerHand1.HowManySelectedObjects} cards");
            UnselectAllCards();
        }
        private async Task PlayProcessAsync()
        {
            if (Goal1!.ObjectSelected() > 0)
            {
                await ShowGameMessageAsync("Cannot select any goal cards to play");
                UnselectAllCards();
                return;
            }
            if (Keeper1!.ObjectSelected() > 0)
            {
                await ShowGameMessageAsync("Cannot select any keeper cards to play");
                UnselectAllCards();
                return;
            }
            if (MainGame!.NeedsToRemoveGoal())
            {
                await ShowGameMessageAsync("Cannot choose any cards to play until you discard a goal");
                UnselectAllCards();
                return;
            }
            int howMany = PlayerHand1!.HowManySelectedObjects;
            if (howMany != 1)
            {
                await ShowGameMessageAsync("Must choose only one card to play");
                UnselectAllCards();
                return;
            }
            if (MainGame.SaveRoot!.PlaysLeft <= 0)
            {
                await ShowGameMessageAsync("Sorry; you don't have any plays left");
                UnselectAllCards();
                return;
            }
            int deck = PlayerHand1.ObjectSelected();
            await MainGame.SendPlayAsync(deck);
            await MainGame.PlayCardAsync(deck);
        }
        public override bool CanEndTurn()
        {
            if (FluxxScreenUsed == EnumActionScreen.ActionScreen || FluxxScreenUsed == EnumActionScreen.KeeperScreen)
                return false;
            return MainGame!.OtherTurn == 0;
        }
        private bool IsOtherTurn => MainGame!.OtherTurn > 0;
        private void UnselectAllCards()
        {
            PlayerHand1!.UnselectAllObjects();
            Keeper1!.UnselectAllObjects();
            Goal1!.UnselectAllObjects();
        }
        public DetailCardViewModel? ThisDetail;
        public IAction? Action1;
        public IKeeper? KeeperControl1;
        public HandViewModel<KeeperCard>? Keeper1;
        public HandViewModel<GoalCard>? Goal1;
        public BasicGameCommand? SelectAllCardsCommand { get; set; }
        public BasicGameCommand? UnselectAllCardsCommand { get; set; }
        public BasicGameCommand? ShowKeepersCommand { get; set; }
        public BasicGameCommand? DiscardCommand { get; set; }
        public BasicGameCommand? PlayCommand { get; set; }
        public override bool CanEnableAlways()
        {
            return true;
        }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            PlayerHand1!.AutoSelect = HandViewModel<FluxxCardInformation>.EnumAutoType.SelectAsMany;
            Deck1!.NeverAutoDisable = true; //if you want reshuffling, use this.  otherwise, comment or delete.
            Goal1 = new HandViewModel<GoalCard>(this);
            Goal1.Text = "Goal Cards";
            Goal1.Maximum = 3;
            Goal1.AutoSelect = HandViewModel<GoalCard>.EnumAutoType.SelectOneOnly;
            Goal1.Visible = true;
            Keeper1 = new HandViewModel<KeeperCard>(this);
            Keeper1.AutoSelect = HandViewModel<KeeperCard>.EnumAutoType.SelectAsMany;
            Keeper1.Text = "Your Keepers";
            Keeper1.Visible = true;
            Action1 = MainContainer!.Resolve<IAction>();
            KeeperControl1 = MainContainer.Resolve<IKeeper>();
            Action1.Init();
            ThisDetail = new DetailCardViewModel();
            SelectAllCardsCommand = new BasicGameCommand(this, items => PlayerHand1.SelectAllObjects(), items => true, this, CommandContainer!);
            UnselectAllCardsCommand = new BasicGameCommand(this, items => PlayerHand1.UnselectAllObjects(), items => true, this, CommandContainer!);
            ShowKeepersCommand = new BasicGameCommand(this, items =>
            {
                FluxxScreenUsed = EnumActionScreen.KeeperScreen;
                KeeperControl1.ShowKeepers();
            }, items => true, this, CommandContainer!);
            DiscardCommand = new BasicGameCommand(this, async items => await DiscardProcessesAsync(), items => true, this, CommandContainer!);
            PlayCommand = new BasicGameCommand(this, async items =>
            {
                if (PlayGiveText == "Give")
                    await GiveProcessAsync();
                else
                    await PlayProcessAsync();
            }, items =>
            {
                if (PlayGiveText == "Give")
                    return true;
                return !IsOtherTurn;
            }, this, CommandContainer!);
            Goal1.ConsiderSelectOneAsync += Goal1_ConsiderSelectOneAsync;
            Keeper1.ConsiderSelectOneAsync += Keeper1_ConsiderSelectOneAsync;
        }
        private async Task Keeper1_ConsiderSelectOneAsync(KeeperCard thisObject)
        {
            await OnConsiderSelectOneCardAsync(thisObject);
        }
        private async Task Goal1_ConsiderSelectOneAsync(GoalCard thisObject)
        {
            await OnConsiderSelectOneCardAsync(thisObject);
        }
        #region "Action/Keeper Events"
        public void CloseKeeperScreen()
        {
            if (MainGame!.ThisGlobal!.CurrentAction == null)
            {
                FluxxScreenUsed = EnumActionScreen.None;
                Title = "Fluxx";
            }
            else
            {
                FluxxScreenUsed = EnumActionScreen.ActionScreen;
                Title = "Action";
            }
        }
        public async Task KeepersExchangedAsync(KeeperPlayer keeperFrom, KeeperPlayer keeperTo)
        {
            await MainGame!.ProcessExchangeKeepersAsync(keeperFrom, keeperTo);
        }
        public async Task StealTrashKeeperAsync(KeeperPlayer currentKeeper, bool isTrashed)
        {
            await MainGame!.ProcessTrashStealKeeperAsync(currentKeeper, isTrashed);
        }
        public async Task CardChosenToPlayAtAsync(int deck, int selectedIndex)
        {
            int index = Action1!.GetPlayerIndex(selectedIndex);
            await MainGame!.PlayUseTakeAsync(deck, index);
        }
        public async Task CardToUseAsync(int deck)
        {
            await MainGame!.DrawUsedAsync(deck);
        }
        public async Task ChoseForEverybodyGetsOneAsync(CustomBasicList<int> selectedList, int selectedIndex)
        {
            await MainGame!.EverybodyGetsOneAsync(selectedList, selectedIndex);
        }
        public async Task ChosePlayerForCardChosenAsync(int selectedIndex)
        {
            await MainGame!.ChosePlayerOnActionAsync(selectedIndex);
            await MainGame.ContinueTurnAsync();
        }
        public async Task DirectionChosenAsync(int selectedIndex)
        {
            await MainGame!.RotateHandAsync((EnumDirection)selectedIndex);
        }
        public async Task DoAgainSelectedAsync(int selectedIndex)
        {
            var thisCard = Action1!.GetCardToDoAgain(selectedIndex);
            await MainGame!.DoAgainProcessPart1Async(selectedIndex);
            await MainGame.PlayCardAsync(thisCard);
        }
        public async Task FirstCardRandomChosenAsync(int deck)
        {
            await MainGame!.PlayRandomCardAsync(deck);
        }
        public async Task RulesSimplifiedAsync(CustomBasicList<int> simpleList)
        {
            await MainGame!.SimplifyRulesAsync(simpleList);
        }
        public async Task RuleTrashedAsync(int selectedIndex)
        {
            await MainGame!.TrashNewRuleAsync(selectedIndex);
        }
        public async Task TradeHandsAsync(int SelectedIndex)
        {
            await MainGame!.TradeHandAsync(SelectedIndex);
        }
        #endregion
    }
}