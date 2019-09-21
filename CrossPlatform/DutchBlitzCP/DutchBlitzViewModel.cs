using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.NetworkingClasses.Extensions;
using BasicGameFramework.SpecializedGameTypes.StockClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.Misc;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace DutchBlitzCP
{
    public class DutchBlitzViewModel : BasicCardGamesVM<DutchBlitzCardInformation, DutchBlitzPlayerItem, DutchBlitzMainGameClass>
    {
        private string _ErrorMessage = "";

        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            set
            {
                if (SetProperty(ref _ErrorMessage, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private async Task SendErrorMessageAsync(string message)
        {
            ErrorMessage = message;
            if (ThisTest!.NoAnimations == false)
                await MainGame!.Delay!.DelayMilli(200);
            ErrorMessage = "";
        }
        public DutchBlitzViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableDeck()
        {
            if (Deck1!.IsEndOfDeck() && Pile1!.DiscardList().Count > 1)
                return false;
            return true;
        }

        protected override bool CanEnablePile1()
        {
            return !Pile1!.PileEmpty();
        }

        protected override async Task ProcessDiscardClickedAsync()
        {
            await Task.CompletedTask;
            if (Pile1!.GetCardInfo().Deck == 0)
                return;
            if (Pile1.CardSelected() > 0)
            {
                Pile1.IsSelected(false);
                return;
            }
            MainGame!.UnselectCards();
            Pile1.IsSelected(true);
        }
        internal bool DidStartTimer; //maybe here.

        public CustomStopWatchCP? Stops;
        public StockViewModel? StockPile;
        public DiscardPilesVM<DutchBlitzCardInformation>? DiscardPiles;
        public PublicViewModel? PublicPiles1;
        internal void LoadDiscards()
        {
            if (MainGame!.PlayerList.Count() == 2)
                MainGame.MaxDiscard = 5;
            else if (MainGame.PlayerList.Count() == 3)
                MainGame.MaxDiscard = 4;
            else
                MainGame.MaxDiscard = 3;
            DiscardPiles = new DiscardPilesVM<DutchBlitzCardInformation>(this);
            DiscardPiles.Init(MainGame.MaxDiscard);
            DiscardPiles.PileClickedAsync += DiscardPiles_PileClickedAsync;
        }
        private async Task DiscardPiles_PileClickedAsync(int Index, BasicGameFramework.MultiplePilesViewModels.BasicPileInfo<DutchBlitzCardInformation> ThisPile)
        {
            DutchBlitzCardInformation thisCard;
            if (DiscardPiles!.HasCard(Index) == false)
            {
                if (StockPile!.CardSelected() > 0)
                {
                    thisCard = StockPile.GetCard();
                    StockPile.RemoveCard();
                    DiscardPiles.AddToEmptyDiscard(thisCard);
                    MainGame!.SingleInfo!.StockLeft--;
                    await MainGame.SendStockAsync();
                    return;
                }
                if (Pile1!.CardSelected() == 0)
                {
                    await SendErrorMessageAsync("Sorry, no card was selected");
                    return;
                }
                thisCard = Pile1.GetCardInfo();
                thisCard.IsSelected = false;
                thisCard.Drew = false;
                DiscardPiles.AddToEmptyDiscard(thisCard);
                Pile1.RemoveFromPile();
                return;
            }
            thisCard = DiscardPiles.GetLastCard(Index);
            if (thisCard.IsSelected)
            {
                DiscardPiles.SelectUnselectSinglePile(Index);
                return;
            }
            MainGame!.UnselectCards();
            DiscardPiles.SelectUnselectSinglePile(Index);
        }
        public BasicGameCommand? DutchCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            Deck1!.NeverAutoDisable = true; //if you want reshuffling, use this.  otherwise, comment or delete.
            Stops = new CustomStopWatchCP();
            Stops.MaxTime = 7000;
            CanSendDrawMessage = false;
            Stops.TimeUp += Stops_TimeUp;
            CommandContainer!.ExecutingChanged += CommandContainer_ExecutingChanged;
            Pile1!.Text = "Waste";
            StockPile = new StockViewModel(this);
            StockPile.StockFrame.SendEnableProcesses(this, () => StockPile.CardsLeft() > 0);
            StockPile.StockClickedAsync += StockPile_StockClickedAsync;
            PublicPiles1 = new PublicViewModel(this);
            PublicPiles1.PileClickedAsync += PublicPiles1_PileClickedAsync;
            PublicPiles1.NewPileClickedAsync += PublicPiles1_NewPileClickedAsync;
            PublicPiles1.Visible = true; //this has to be set too.
            DutchCommand = new BasicGameCommand(this, async items =>
            {
                if (MainGame!.HasBlitz == false)
                {
                    await SendErrorMessageAsync("Sorry, you still have cards in your stock pile.  Therefore, the round has not ended yet");
                    return;
                }
                if (ThisData!.MultiPlayer)
                    await ThisNet!.SendAllAsync("blitz");
                Stops.PauseTimer();
                DidStartTimer = false; //because you got blitz.
                await MainGame.BlitzAsync();
            }, items =>
            {
                return true;
            }, this, CommandContainer);
        }
        private bool _didClickOld = false;

        private async Task PublicPiles1_NewPileClickedAsync()
        {
            if (_didClickOld == true)
            {
                _didClickOld = false;
                return;
            }
            await ProcessHumanPileAsync(-1);
        }

        private async Task PublicPiles1_PileClickedAsync(int index)
        {
            _didClickOld = true;
            await ProcessHumanPileAsync(index);
        }

        private async Task StockPile_StockClickedAsync()
        {
            int nums = StockPile!.CardSelected();
            if (nums > 0)
            {
                StockPile.UnselectCard();
                return;
            }
            MainGame!.UnselectCards();
            StockPile.SelectCard();
            await Task.CompletedTask;
        }

        private void CommandContainer_ExecutingChanged()
        {
            if (CommandContainer!.IsExecuting)
            {
                if (DidStartTimer == true)
                    Stops!.PauseTimer();
            }
            else
            {
                if (DidStartTimer)
                    Stops!.ContinueTimer();
            }
        }
        private async void Stops_TimeUp()
        {
            if (DidStartTimer == false)
                return;
            DidStartTimer = false;
            CommandContainer!.IsExecuting = true;
            CommandContainer.ManuelFinish = true;
            if (ThisData!.MultiPlayer == true)
                await ThisNet!.SendEndTurnAsync();
            await MainGame!.EndTurnAsync();
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
        private async Task ProcessHumanPileAsync(int pile)
        {
            if (MainGame!.HumanHasSelected() == false)
            {
                await SendErrorMessageAsync("No card was selected");
                return;
            }
            if (MainGame.CanHumanPlayCard(pile) == false)
            {
                await SendErrorMessageAsync("Illegal move");
                return;
            }
            DutchBlitzCardInformation thisCard;
            thisCard = MainGame.HumanCardToUse(out bool _, out bool sends);
            if (sends)
                await MainGame.SendStockAsync();
            if (pile == -1)
            {
                if (ThisData!.MultiPlayer)
                    await ThisNet!.SendAllAsync("newpile", thisCard.Deck);
                await MainGame.AddNewPublicPileAsync(thisCard);
                return;
            }
            if (ThisData!.MultiPlayer)
            {
                SendExpand temps = new SendExpand();
                temps.Deck = thisCard.Deck;
                temps.Pile = pile;
                await ThisNet!.SendAllAsync("expandpile", temps);
            }
            await MainGame.ExpandPublicPileAsync(thisCard, pile);
        }
    }
}