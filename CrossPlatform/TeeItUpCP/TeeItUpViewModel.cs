using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace TeeItUpCP
{
    public class TeeItUpViewModel : BasicCardGamesVM<TeeItUpCardInformation, TeeItUpPlayerItem, TeeItUpMainGameClass>
    {
        internal CustomBasicList<SendPlay> firstList = new CustomBasicList<SendPlay>();


        private int _Round;

        public int Round
        {
            get { return _Round; }
            set
            {
                if (SetProperty(ref _Round, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private string _Instructions = "";

        public string Instructions
        {
            get { return _Instructions; }
            set
            {
                if (SetProperty(ref _Instructions, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public TeeItUpViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableDeck()
        {
            return MainGame!.SaveRoot!.GameStatus != EnumStatusType.Beginning && Pile2!.PileEmpty();
        }
        protected override bool CanEnablePile1()
        {
            return MainGame!.SaveRoot!.GameStatus != EnumStatusType.Beginning;
        }
        protected override async Task ProcessDiscardClickedAsync()
        {
            int oldDeck;
            if (Pile2!.PileEmpty() == true)
                oldDeck = 0;
            else
                oldDeck = Pile2.GetCardInfo().Deck;
            if (oldDeck == 0)
            {
                await MainGame!.PickupFromDiscardAsync();
                return;
            }
            if (oldDeck == MainGame!.PreviousCard)
            {
                await ShowGameMessageAsync("Cannot discard the same card that was picked up");
                return;
            }
            TeeItUpPlayerItem tempItem;
            if (ThisData!.MultiPlayer == true)
                tempItem = MainGame!.PlayerList!.GetSelf();
            else
                tempItem = MainGame!.PlayerList!.GetWhoPlayer();
            try
            {
                int matches = tempItem.PlayerBoard!.ColumnMatched(oldDeck);
                if (matches > 0)
                {
                    await ShowGameMessageAsync("Cannot discard a card because there is a match");
                    return;
                }

            }
            catch(Exception ex)
            {
                throw new BasicBlankException($"Exception when trying to find out about column matching.  Message was {ex.Message}");
            }
            var thisCard = MainGame.DeckList!.GetSpecificItem(oldDeck);
            if (thisCard.IsMulligan == true)
            {
                await ShowGameMessageAsync("Cannot discard a Mulligan");
                return;
            }
            if (ThisData.MultiPlayer == true)
                await MainGame.SendDiscardMessageAsync(oldDeck);
            await MainGame.DiscardAsync(oldDeck);
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
        public PileViewModel<TeeItUpCardInformation>? Pile2;
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            Deck1!.NeverAutoDisable = true; //if you want reshuffling, use this.  otherwise, comment or delete.
            Pile2 = new PileViewModel<TeeItUpCardInformation>(ThisE!, this);
            Pile2.CurrentOnly = true;
            Pile2.Text = "Current Card";
            Pile2.Visible = true;
            Pile2.FirstLoad(new TeeItUpCardInformation());
            Pile2.SendEnableProcesses(this, () => MainGame!.SaveRoot!.GameStatus != EnumStatusType.Beginning);
            MainGame!.OtherPile = Pile2;
            PlayerHand1!.Visible = false; //i think.
        }

    }
}