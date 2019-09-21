using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace Spades2PlayerCP
{
    public class Spades2PlayerViewModel : TrickGamesVM<EnumSuitList, Spades2PlayerCardInformation, Spades2PlayerPlayerItem, Spades2PlayerMainGameClass>
    {
        public NumberPicker? Bid1;
        private int _BidAmount = -1;

        public int BidAmount
        {
            get { return _BidAmount; }
            set
            {
                if (SetProperty(ref _BidAmount, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public bool BeginningVisible => MainGame!.SaveRoot!.GameStatus == EnumGameStatus.ChooseCards;
        public bool BiddingVisible => MainGame!.SaveRoot!.GameStatus == EnumGameStatus.Bidding;
        public PileViewModel<Spades2PlayerCardInformation>? Pile2;

        private int _RoundNumber;

        public int RoundNumber
        {
            get { return _RoundNumber; }
            set
            {
                if (SetProperty(ref _RoundNumber, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        internal void ShowVisibleChange()
        {
            OnPropertyChanged(nameof(BeginningVisible));
            OnPropertyChanged(nameof(BiddingVisible));
            MainGame!.TrickArea1!.Visible = MainGame.SaveRoot!.GameStatus == EnumGameStatus.Normal;
        }
        public Spades2PlayerViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableDeck()
        {
            return false; //otherwise, can't compile.
        }

        protected override bool CanEnablePile1()
        {
            return MainGame!.SaveRoot!.GameStatus == EnumGameStatus.ChooseCards;
        }

        protected override async Task ProcessDiscardClickedAsync()
        {
            Pile2!.Visible = false;
            await MainGame!.SendDiscardMessageAsync(Pile2.GetCardInfo().Deck);
            await MainGame!.DiscardAsync(Pile2.GetCardInfo().Deck);
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
        public BasicGameCommand? TakeCardCommand { get; set; }
        public BasicGameCommand? BidCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.  most trick taking games does not reshuffle.
            Pile2 = new PileViewModel<Spades2PlayerCardInformation>(ThisE!, this);
            Pile2.Text = "Current";
            Pile2.IsEnabled = false;
            MainGame!.OtherPile = Pile2;
            Pile2.Visible = false;
            TakeCardCommand = new BasicGameCommand(this, async items =>
            {
                if (ThisData!.MultiPlayer == true)
                    await ThisNet!.SendAllAsync("acceptcard");
                await MainGame.AcceptCardAsync();
            }, items =>
            {
                return MainGame.SaveRoot!.GameStatus == EnumGameStatus.ChooseCards;
            }, this, CommandContainer!);
            BidCommand = new BasicGameCommand(this, async items =>
            {
                if (ThisData!.MultiPlayer == true)
                    await ThisNet!.SendAllAsync("bid", BidAmount);
                await MainGame.ProcessBidAsync();
            }, items =>
            {
                if (BidAmount == -1)
                    return false;
                return MainGame.SaveRoot!.GameStatus == EnumGameStatus.Bidding;
            }, this, CommandContainer!);
            Bid1 = new NumberPicker(this);
            Bid1.SendEnableProcesses(this, () =>
            {
                return MainGame.SaveRoot!.GameStatus == EnumGameStatus.Bidding;
            });
            Bid1.ChangedNumberValueAsync += Bid1_ChangedNumberValueAsync;
            Bid1.LoadNormalNumberRangeValues(0, 13);
            Bid1.Visible = true; //something else does it anyways.
        }
        private Task Bid1_ChangedNumberValueAsync(int chosen)
        {
            BidAmount = chosen;
            return Task.CompletedTask;
        }
    }
}