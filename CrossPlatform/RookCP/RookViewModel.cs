using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BasicGameFramework.MainViewModels;
using BasicGameFramework.DIContainers;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.ColorCards;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.BasicDrawables.Dictionary;

namespace RookCP
{
    public class RookViewModel : TrickGamesVM<EnumColorTypes, RookCardInformation, RookPlayerItem, RookMainGameClass>, ITrickDummyHand<EnumColorTypes, RookCardInformation>
    {
        internal void ShowVisibleChange()
        {

            MainGame!.TrickArea1!.Visible = MainGame.SaveRoot!.GameStatus == EnumStatusList.Normal;

            OnPropertyChanged(nameof(NestVisible));
            OnPropertyChanged(nameof(ColorVisible));
            OnPropertyChanged(nameof(BiddingVisible));
            if (Bid1 != null)
                Bid1.Visible = BiddingVisible;
        }

        public bool BiddingVisible
        {
            get
            {
                return MainGame!.SaveRoot!.GameStatus == EnumStatusList.Bidding;
            }
        }

        public bool NestVisible
        {
            get
            {
                if (MainGame!.SaveRoot!.GameStatus == EnumStatusList.SelectNest)
                    return true;
                return false;
            }
        }

        public bool ColorVisible
        {
            get
            {
                if (MainGame!.SaveRoot!.GameStatus == EnumStatusList.ChooseTrump)
                {
                    Color1!.Visible = true;
                    Color1.IsEnabled = true;
                    return true;
                }
                Color1!.Visible = false;
                return false;
            }
        }

        private int _BidChosen = -1;

        public int BidChosen
        {
            get { return _BidChosen; }
            set
            {
                if (SetProperty(ref _BidChosen, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public NumberPicker? Bid1;

        public SimpleEnumPickerVM<EnumColorTypes, CheckerChoiceCP<EnumColorTypes>, ColorListChooser<EnumColorTypes>>? Color1;

        public DummyHandCP? Dummy1;


        public EnumColorTypes ColorChosen { get; set; } //hopefully this way will work too like i did on rage.
        public RookViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
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
        public override bool CanEndTurn()
        {
            return false;
        }
        public BasicGameCommand? PassCommand { get; set; }
        public BasicGameCommand? NestCommand { get; set; }
        public BasicGameCommand? BidCommand { get; set; }
        public BasicGameCommand? TrumpCommand { get; set; }

        internal bool CanPass;
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.  most trick taking games does not reshuffle.
            Dummy1 = new DummyHandCP(this);
            Dummy1.SendEnableProcesses(this, () =>
            {
                if (MainGame!.SaveRoot!.GameStatus != EnumStatusList.Normal)
                    return false;
                if (MainGame.PlayerList.Count() == 3)
                    return false;
                return MainGame.SaveRoot.DummyPlay;
            });
            Bid1 = new NumberPicker(this);
            Bid1.ChangedNumberValueAsync += Bid1_ChangedNumberValueAsync;
            Bid1.SendEnableProcesses(this, () => MainGame!.SaveRoot!.GameStatus == EnumStatusList.Bidding);
            Color1 = new SimpleEnumPickerVM<EnumColorTypes, CheckerChoiceCP<EnumColorTypes>, ColorListChooser<EnumColorTypes>>(this);
            Color1.AutoSelectCategory = EnumAutoSelectCategory.AutoEvent;
            Color1.ItemClickedAsync += Color1_ItemClickedAsync;
            PassCommand = new BasicGameCommand(this, async items =>
            {
                await MainGame!.PassBidAsync();
            }, items => CanPass, this, CommandContainer!);
            BidCommand = new BasicGameCommand(this, async items =>
            {
                await MainGame!.ProcessBidAsync();
            }, items =>
            {
                if (MainGame!.SaveRoot!.GameStatus != EnumStatusList.Bidding)
                    return false;
                return BidChosen > -1;
            }, this, CommandContainer!);
            TrumpCommand = new BasicGameCommand(this, async items =>
            {
                await MainGame!.ProcessTrumpAsync();
            }, items =>
            {
                if (MainGame!.SaveRoot!.GameStatus != EnumStatusList.ChooseTrump)
                    return false;
                return ColorChosen != EnumColorTypes.None;
            }, this, CommandContainer!);
            NestCommand = new BasicGameCommand(this, async items =>
            {
                var thisList = PlayerHand1!.ListSelectedObjects();
                if (thisList.Count != 5)
                {
                    await ShowGameMessageAsync("Sorry, you must choose 5 cards to throw away");
                    return;
                }
                await MainGame!.ProcessNestAsync(thisList);
            }, items =>
            {
                return MainGame!.SaveRoot!.GameStatus == EnumStatusList.SelectNest;
            }, this, CommandContainer!);
        }
        private Task Color1_ItemClickedAsync(EnumColorTypes thisPiece)
        {
            ColorChosen = thisPiece; //hopefully this simple.
            TrumpSuit = thisPiece;// try this too.
            return Task.CompletedTask;
        }

        private Task Bid1_ChangedNumberValueAsync(int chosen)
        {
            BidChosen = chosen;
            return Task.CompletedTask;
        }

        public override bool CanEnableAlways()
        {
            return true;
        }
        internal void PopulateBids()
        {
            BidChosen = -1;
            if (MainGame!.SaveRoot!.HighestBidder == 1)
                throw new BasicBlankException("The highest bid cannot be 0");
            Bid1!.LoadNormalNumberRangeValues(MainGame.SaveRoot.HighestBidder + 5, 100, 5);
        }
        internal void ResetBids()
        {
            BidChosen = -1;
            Bid1!.UnselectAll();
        }
        internal void ResetTrumps()
        {
            ColorChosen = EnumColorTypes.None; //i think
            Color1!.UnselectAll();
        }
        protected override bool AlwaysEnableHand()
        {
            return false;
        }
        protected override bool CanEnableHand()
        {
            if (MainGame!.SaveRoot!.GameStatus == EnumStatusList.SelectNest)
                return true;
            if (MainGame.SaveRoot.GameStatus == EnumStatusList.Normal)
                return !MainGame.SaveRoot.DummyPlay;
            return false;
        }

        public DeckObservableDict<RookCardInformation> GetCurrentHandList()
        {
            if (MainGame!.SaveRoot!.DummyPlay == true && MainGame.PlayerList.Count() == 2)
                return Dummy1!.HandList;
            return MainGame.SingleInfo!.MainHandList; //try this way.  hopefully won't cause other problems.
        }

        public int CardSelected()
        {
            if (MainGame!.SaveRoot!.DummyPlay == true && MainGame.PlayerList.Count() == 2)
                return Dummy1!.ObjectSelected();
            else if (MainGame.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
                throw new BasicBlankException("Only self can show card selected.  If I am wrong, rethink");
            return PlayerHand1!.ObjectSelected();
        }

        public void RemoveCard(int deck)
        {
            if (MainGame!.SaveRoot!.DummyPlay == true && MainGame.PlayerList.Count() == 2)
                Dummy1!.RemoveDummyCard(deck);
            else
                MainGame.SingleInfo!.MainHandList.RemoveObjectByDeck(deck); //because computer player does this too.
        }
    }
}