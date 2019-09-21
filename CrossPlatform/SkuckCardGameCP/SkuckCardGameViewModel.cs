using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.Extensions;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace SkuckCardGameCP
{
    public class SkuckCardGameViewModel : TrickGamesVM<EnumSuitList, SkuckCardGameCardInformation, SkuckCardGamePlayerItem, SkuckCardGameMainGameClass>
        , ITrickDummyHand<EnumSuitList, SkuckCardGameCardInformation>
    {
        public NumberPicker? Bid1;
        public SimpleEnumPickerVM<EnumSuitList, DeckPieceCP, SuitListChooser>? Suit1;
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

        public bool BiddingVisible
        {
            get
            {
                if (MainGame!.SaveRoot!.WhatStatus == EnumStatusList.ChooseBid)
                {
                    Bid1!.Visible = true;
                    return true;
                }
                else
                {
                    Bid1!.Visible = false;
                    return false;
                }
            }
        }
        public bool SuitVisible
        {
            get
            {
                if (MainGame!.SaveRoot!.WhatStatus == EnumStatusList.ChooseTrump)
                {
                    Suit1!.Visible = true;
                    return true;
                }
                else
                {
                    Suit1!.Visible = false;
                    return false;
                }

            }
        }
        public bool FirstOptionsVisible
        {
            get
            {
                if (MainGame!.SaveRoot!.WhatStatus == EnumStatusList.ChoosePlay)
                    return true;
                else
                    return false;
            }
        }
        //public bool TrickVisible
        //{
        //    get
        //    {
        //        //return false;
        //        //if (MainGame!.SaveRoot!.WhatStatus == EnumStatusList.NormalPlay)
        //        //    return true;
        //        //else
        //        //    return false;
        //    }
        //}
        protected override Task OnAutoSelectedHandAsync()
        {
            SkuckCardGamePlayerItem thisPlayer = MainGame!.PlayerList!.GetSelf();
            thisPlayer.TempHand!.UnselectAllCards();
            return base.OnAutoSelectedHandAsync();
        }
        public void ShowVisibleChange()
        {
            //OnPropertyChanged(nameof(TrickVisible));
            OnPropertyChanged(nameof(BiddingVisible));
            OnPropertyChanged(nameof(FirstOptionsVisible));
            OnPropertyChanged(nameof(SuitVisible));
            
        }
        public SkuckCardGameViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
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
        public BasicGameCommand<EnumChoiceOption>? FirstPlayCommand { get; set; }
        public BasicGameCommand? BidCommand { get; set; }
        public BasicGameCommand? TrumpCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.  most trick taking games does not reshuffle.
            FirstPlayCommand = new BasicGameCommand<EnumChoiceOption>(this, async option =>
            {
                if (option == EnumChoiceOption.None)
                    throw new BasicBlankException("Not Supported");
                if (option == EnumChoiceOption.Play)
                {
                    if (ThisData!.MultiPlayer == true)
                        await ThisNet!.SendAllAsync("choosetoplay");
                    await MainGame!.ChooseToPlayAsync();
                    return;
                }
                if (option == EnumChoiceOption.Pass)
                {
                    if (ThisData!.MultiPlayer == true)
                        await ThisNet!.SendAllAsync("choosetopass");
                    await MainGame!.ChooseToPassAsync();
                    return;
                }
                throw new BasicBlankException("Not Supported");
            }, Option =>
            {
                return true;
            }, this, CommandContainer!);
            TrumpCommand = new BasicGameCommand(this, async items =>
            {
                if (ThisData!.MultiPlayer == true)
                    await ThisNet!.SendAllAsync("trump", TrumpSuit);
                await MainGame!.TrumpChosenAsync();
            }, items =>
            {
                if (MainGame!.SaveRoot!.WhatStatus != EnumStatusList.ChooseTrump)
                    return false;
                return TrumpSuit != EnumSuitList.None;
            }, this, CommandContainer!);
            BidCommand = new BasicGameCommand(this, async x =>
            {
                if (ThisData!.MultiPlayer == true)
                    await ThisNet!.SendAllAsync("bid", BidAmount);
                int id = MainGame!.PlayerList.Where(items => items.PlayerCategory == EnumPlayerCategory.Self).Single().Id;
                await MainGame.ProcessBidAmountAsync(id);
            }, items =>
            {
                if (MainGame!.SaveRoot!.WhatStatus != EnumStatusList.ChooseBid)
                    return false;
                return BidAmount != -1;
            }, this, CommandContainer!);
            Bid1 = new NumberPicker(this);
            Suit1 = new SimpleEnumPickerVM<EnumSuitList, DeckPieceCP, SuitListChooser>(this);
            Suit1.AutoSelectCategory = EnumAutoSelectCategory.AutoSelect;
            Bid1.ChangedNumberValueAsync += Bid1_ChangedNumberValueAsync;
            Suit1.ItemSelectionChanged += Suit1_ItemSelectionChanged;
            Bid1.LoadNormalNumberRangeValues(1, 26);
        }
        internal void LoadPlayerControls()
        {
            MainGame!.PlayerList!.ForEach(thisPlayer =>
            {
                if (thisPlayer.TempHand == null)
                {
                    thisPlayer.TempHand = new PlayerBoardViewModel<SkuckCardGameCardInformation>(this);
                    thisPlayer.TempHand.Visible = true; //try this too.
                    thisPlayer.TempHand.SendEnableProcesses(this, () =>
                    {
                        if (thisPlayer.PlayerCategory != EnumPlayerCategory.Self)
                            return false;
                        return MainGame.SaveRoot!.WhatStatus == EnumStatusList.NormalPlay;
                    });
                }
                thisPlayer.TempHand.Game = PlayerBoardViewModel<SkuckCardGameCardInformation>.EnumGameList.Skuck;
                thisPlayer.TempHand.IsSelf = thisPlayer.PlayerCategory == EnumPlayerCategory.Self; //hopefully this works.
                if (thisPlayer.SavedTemp.Count != 0)
                {
                    thisPlayer.TempHand.CardList.ReplaceRange(thisPlayer.SavedTemp);
                }
                if (thisPlayer.PlayerCategory == EnumPlayerCategory.Self)
                {
                    thisPlayer.TempHand.SelectedCard -= TempHand_SelectedCard;
                    thisPlayer.TempHand.SelectedCard += TempHand_SelectedCard;
                }
            });
        }
        private void TempHand_SelectedCard()
        {
            PlayerHand1!.UnselectAllObjects();
        }
        private void Suit1_ItemSelectionChanged(EnumSuitList? thisPiece)
        {
            if (thisPiece.HasValue == false)
                MainGame!.SaveRoot!.TrumpSuit = EnumSuitList.None;
            else
                MainGame!.SaveRoot!.TrumpSuit = thisPiece!.Value;
        }
        private Task Bid1_ChangedNumberValueAsync(int chosen)
        {
            BidAmount = chosen;
            return Task.CompletedTask;
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
        DeckObservableDict<SkuckCardGameCardInformation> ITrickDummyHand<EnumSuitList, SkuckCardGameCardInformation>.GetCurrentHandList()
        {
            DeckObservableDict<SkuckCardGameCardInformation> output = MainGame!.SingleInfo!.MainHandList.ToObservableDeckDict();
            output.AddRange(MainGame.SingleInfo.TempHand!.ValidCardList);
            return output; //hopefully this simple.
        }
        int ITrickDummyHand<EnumSuitList, SkuckCardGameCardInformation>.CardSelected()
        {
            if (MainGame!.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
                throw new BasicBlankException("Only self can get card selected.  If I am wrong, rethink");
            int selects = PlayerHand1!.ObjectSelected();
            int others = MainGame.SingleInfo.TempHand!.CardSelected;
            if (selects != 0 && others != 0)
                throw new BasicBlankException("You cannot choose from both hand and temps.  Rethink");
            if (selects != 0)
                return selects;
            return others;
        }
        void ITrickDummyHand<EnumSuitList, SkuckCardGameCardInformation>.RemoveCard(int deck)
        {
            bool rets = MainGame!.SingleInfo!.MainHandList.ObjectExist(deck);
            if (rets == true)
            {
                MainGame.SingleInfo.MainHandList.RemoveObjectByDeck(deck);
                return;
            }
            var thisCard = MainGame.SingleInfo.TempHand!.CardList.GetSpecificItem(deck);
            if (thisCard.IsEnabled == false)
                throw new BasicBlankException("Card was supposed to be disabled");
            MainGame.SingleInfo.TempHand.HideCard(thisCard);
        }
    }
}