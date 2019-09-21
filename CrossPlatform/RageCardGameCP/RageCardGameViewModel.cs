using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace RageCardGameCP
{
    public class RageCardGameViewModel : TrickGamesVM<EnumColor, RageCardGameCardInformation, RageCardGamePlayerItem, RageCardGameMainGameClass>
    {
        private string _Lead = "";

        public string Lead
        {
            get { return _Lead; }
            set
            {
                if (SetProperty(ref _Lead, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public SimpleEnumPickerVM<EnumColor, CheckerChoiceCP<EnumColor>, ColorListChooser<EnumColor>>? ColorVM;
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

        private bool _BiddingVisible = false;

        public bool BiddingVisible
        {
            get { return _BiddingVisible; }
            set
            {
                if (SetProperty(ref _BiddingVisible, value))
                {
                    //can decide what to do when property changes
                    Bid1!.Visible = value;
                    MainOptionsVisible = !value;
                }

            }
        }
        private bool _ColorVisible = false;

        public bool ColorVisible
        {
            get { return _ColorVisible; }
            set
            {
                if (SetProperty(ref _ColorVisible, value))
                {
                    //can decide what to do when property changes
                    ColorVM!.Visible = value;
                    MainOptionsVisible = !value;
                }

            }
        }
        public EnumColor ColorChosen { get; set; } //hopefully this way is fine (?)

        public void LoadBiddingScreen()
        {
            BidAmount = -1;
            Bid1!.LoadNormalNumberRangeValues(0, MainGame!.SaveRoot!.CardsToPassOut);
            Bid1.UnselectAll();
            BiddingVisible = true;
        }
        public RageCardGameViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
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
            await Task.CompletedTask;
        }
        public BasicGameCommand? BidCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.  most trick taking games does not reshuffle.
            Bid1 = new NumberPicker(this);
            Bid1.SendEnableProcesses(this, () => MainGame!.SaveRoot!.Status == EnumStatus.Bidding);
            ColorVM = new SimpleEnumPickerVM<EnumColor, CheckerChoiceCP<EnumColor>, ColorListChooser<EnumColor>>(this);
            ColorVM.AutoSelectCategory = EnumAutoSelectCategory.AutoEvent;
            ColorVM.ItemClickedAsync += ColorVM_ItemClickedAsync;
            Bid1.ChangedNumberValueAsync += Bid1_ChangedNumberValueAsync;
            PlayerHand1!.Maximum = 11; //never has more than 11 cards period.
            BidCommand = new BasicGameCommand(this, async items =>
            {
                await MainGame!.ProcessBidAsync();
            }, Items =>
            {
                return BidAmount > -1;
            }, this, CommandContainer!);
        }
        private Task Bid1_ChangedNumberValueAsync(int chosen)
        {
            BidAmount = chosen;
            return Task.CompletedTask;
        }
        private async Task ColorVM_ItemClickedAsync(EnumColor thisPiece)
        {
            if (thisPiece == MainGame!.SaveRoot!.TrumpSuit && MainGame!.SaveRoot!.TrickList.Last().SpecialType == EnumSpecialType.Change)
            {
                await ShowGameMessageAsync($"{thisPiece.ToString()} is already current trump.  Choose a different one");
                return;
            }
            ColorChosen = thisPiece;
            await MainGame!.ColorChosenAsync();
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
    }
}