using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace PickelCardGameCP
{
    public class PickelCardGameViewModel : TrickGamesVM<EnumSuitList, PickelCardGameCardInformation, PickelCardGamePlayerItem, PickelCardGameMainGameClass>
    {
        public PickelCardGameViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableDeck()
        {
            return false; //otherwise, can't compile.
        }

        protected override bool CanEnablePile1()
        {
            return false; //otherwise, can't compile.
        }

        protected override async Task ProcessDiscardClickedAsync()
        {
            //if we have anything, will be here.
            await Task.CompletedTask;
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
        private bool _BiddingScreenVisible;

        public bool BiddingScreenVisible
        {
            get { return _BiddingScreenVisible; }
            set
            {
                if (SetProperty(ref _BiddingScreenVisible, value))
                {
                    Bid1!.Visible = value;
                    Suit1!.Visible = value;
                    if (value == true)
                        MainOptionsVisible = false;
                    else
                        MainOptionsVisible = true;
                }

            }
        }
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
        public void ResetBids()
        {
            BidAmount = -1;
            MainGame!.SaveRoot!.TrumpSuit = EnumSuitList.None;
            Bid1!.UnselectAll();
            Suit1!.UnselectAll();
        }
        public void PopulateBids()
        {
            ResetBids();
            BiddingScreenVisible = true;
            if (MainGame!.SaveRoot!.HighestBid == 0)
                throw new BasicBlankException("The highest bid cannot be 0");
            Bid1!.LoadNormalNumberRangeValues(MainGame.SaveRoot.HighestBid + 1, 10);
        }
        public void SelectBidAndSuit(int Bid, EnumSuitList Suit)
        {
            BidAmount = Bid;
            MainGame!.SaveRoot!.TrumpSuit = Suit;
            Bid1!.SelectNumberValue(Bid);
            Suit1!.SelectSpecificItem(Suit);
        }

        public NumberPicker? Bid1;
        public SimpleEnumPickerVM<EnumSuitList, DeckPieceCP, SuitListChooser>? Suit1;
        public BasicGameCommand? ProcessBidCommand { get; set; }
        public BasicGameCommand? PassCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.  most trick taking games does not reshuffle.
            Bid1 = new NumberPicker(this);
            Suit1 = new SimpleEnumPickerVM<EnumSuitList, DeckPieceCP, SuitListChooser>(this);
            Bid1.ChangedNumberValueAsync += Bid1_ChangedNumberValueAsync;
            //Suit1.ItemClickedAsync += Suit1_ItemClickedAsync;
            Suit1.ItemSelectionChanged += Suit1_ItemSelectionChanged;
            Suit1.AutoSelectCategory = EnumAutoSelectCategory.AutoSelect; //i think
            Bid1.SendEnableProcesses(this, CanEnableBids);
            Suit1.SendEnableProcesses(this, CanEnableBids);
            ProcessBidCommand = new BasicGameCommand(this, async items =>
            {
                await MainGame!.ProcessBidAsync();
            }, items =>
            {
                if (MainGame!.SaveRoot!.GameStatus != EnumStatusList.Bidding)
                    return false;
                if (BidAmount == -1 || TrumpSuit == EnumSuitList.None)
                    return false;
                return true;
            }, this, CommandContainer!);
            PassCommand = new BasicGameCommand(this, async items =>
            {
                await MainGame!.PassBidAsync();
            }, items =>
            {
                if (MainGame!.SaveRoot!.GameStatus != EnumStatusList.Bidding)
                    return false;
                return MainGame.CanPass();
            }, this, CommandContainer!);
        }
        private void Suit1_ItemSelectionChanged(EnumSuitList? thisPiece)
        {
            if (thisPiece.HasValue == false)
                MainGame!.SaveRoot!.TrumpSuit = EnumSuitList.None;
            else
                MainGame!.SaveRoot!.TrumpSuit = thisPiece!.Value;
        }
        private bool CanEnableBids()
        {
            return MainGame!.SaveRoot!.GameStatus == EnumStatusList.Bidding;
        }
        private Task Bid1_ChangedNumberValueAsync(int chosen)
        {
            BidAmount = chosen;
            return Task.CompletedTask;
        }
    }
}