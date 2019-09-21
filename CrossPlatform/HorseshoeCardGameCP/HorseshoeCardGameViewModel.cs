using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.Extensions;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace HorseshoeCardGameCP
{
    public class HorseshoeCardGameViewModel : TrickGamesVM<EnumSuitList, HorseshoeCardGameCardInformation, HorseshoeCardGamePlayerItem, HorseshoeCardGameMainGameClass>
        , ITrickDummyHand<EnumSuitList, HorseshoeCardGameCardInformation>
    {
        protected override Task OnAutoSelectedHandAsync()
        {
            HorseshoeCardGamePlayerItem thisPlayer = MainGame!.PlayerList!.GetSelf();
            thisPlayer.TempHand!.UnselectAllCards();
            return base.OnAutoSelectedHandAsync();
        }
        internal void LoadPlayerControls()
        {
            MainGame!.PlayerList!.ForEach(thisPlayer =>
            {
                if (thisPlayer.TempHand == null)
                {
                    thisPlayer.TempHand = new PlayerBoardViewModel<HorseshoeCardGameCardInformation>(this);
                    thisPlayer.TempHand.Visible = true; //try this too.
                    thisPlayer.TempHand.SendEnableProcesses(this, () =>
                    {
                        if (thisPlayer.PlayerCategory != EnumPlayerCategory.Self)
                            return false;
                        return true;
                    });
                }
                thisPlayer.TempHand.Game = PlayerBoardViewModel<HorseshoeCardGameCardInformation>.EnumGameList.HorseShoe;
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
        public HorseshoeCardGameViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
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
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.  most trick taking games does not reshuffle.
            PlayerHand1!.Maximum = 6;
        }
        public DeckObservableDict<HorseshoeCardGameCardInformation> GetCurrentHandList()
        {
            DeckObservableDict<HorseshoeCardGameCardInformation> output = MainGame!.SingleInfo!.MainHandList.ToObservableDeckDict();
            output.AddRange(MainGame.SingleInfo.TempHand!.ValidCardList);
            return output; //hopefully this simple.
        }

        public int CardSelected()
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
        public void RemoveCard(int deck)
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