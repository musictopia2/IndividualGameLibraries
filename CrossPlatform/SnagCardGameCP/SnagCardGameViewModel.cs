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
namespace SnagCardGameCP
{
    public class SnagCardGameViewModel : TrickGamesVM<EnumSuitList, SnagCardGameCardInformation, SnagCardGamePlayerItem, SnagCardGameMainGameClass>
        , ITrickDummyHand<EnumSuitList, SnagCardGameCardInformation>
    {
        public SnagCardGameViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
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
        internal void LoadControls()
        {
            Bar1 = new BarViewModel(this);
            Bar1.SendEnableProcesses(this, () =>
            {
                if (MainGame!.SaveRoot!.GameStatus != EnumStatusList.Normal)
                    return false;
                return !MainGame.SaveRoot.FirstCardPlayed;
            });
            Bar1.Visible = true;
            Bar1.AutoSelect = HandViewModel<SnagCardGameCardInformation>.EnumAutoType.SelectOneOnly;
            Human1 = new HandViewModel<SnagCardGameCardInformation>(this);
            Opponent1 = new HandViewModel<SnagCardGameCardInformation>(this);
            Human1.Text = "Your Cards Won";
            Opponent1.Text = "Opponent Cards Won";

        }
        public BarViewModel? Bar1;
        public HandViewModel<SnagCardGameCardInformation>? Human1;
        public HandViewModel<SnagCardGameCardInformation>? Opponent1;
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.  most trick taking games does not reshuffle.
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
        protected override bool AlwaysEnableHand()
        {
            return false;
        }
        protected override bool CanEnableHand()
        {
            if (MainGame!.SaveRoot!.GameStatus != EnumStatusList.Normal)
                return false;
            return MainGame.SaveRoot.FirstCardPlayed;
        }
        public DeckObservableDict<SnagCardGameCardInformation> GetCurrentHandList()
        {
            if (MainGame!.SaveRoot!.FirstCardPlayed == false)
                return Bar1!.PossibleList.ToObservableDeckDict();
            return MainGame.SingleInfo!.MainHandList.ToObservableDeckDict();
        }
        public int CardSelected()
        {
            if (MainGame!.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
                throw new BasicBlankException("Only self should know what is selected");
            if (MainGame.SaveRoot!.FirstCardPlayed == false)
                return Bar1!.ObjectSelected();
            return PlayerHand1!.ObjectSelected();
        }
        public void RemoveCard(int deck)
        {
            if (MainGame!.SaveRoot!.FirstCardPlayed == false)
                Bar1!.HandList.RemoveObjectByDeck(deck);
            else
                MainGame.SingleInfo!.MainHandList.RemoveObjectByDeck(deck); //try this way instead.
        }
    }
}