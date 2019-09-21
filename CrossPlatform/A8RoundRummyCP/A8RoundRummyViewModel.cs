using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace A8RoundRummyCP
{
    public class A8RoundRummyViewModel : BasicCardGamesVM<A8RoundRummyCardInformation, A8RoundRummyPlayerItem, A8RoundRummyMainGameClass>
    {
        private string _NextTurn = "";

        public string NextTurn
        {
            get { return _NextTurn; }
            set
            {
                if (SetProperty(ref _NextTurn, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public A8RoundRummyViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableDeck()
        {
            return !MainGame!.AlreadyDrew; //otherwise, can't compile.
        }
        protected override bool CanEnablePile1()
        {
            return true;
        }
        protected override async Task ProcessDiscardClickedAsync()
        {
            if (MainGame!.CanProcessDiscard(out bool pickUp, out int deck, out string message) == false)
            {
                await ShowGameMessageAsync(message);
                return;
            }
            if (pickUp == true)
            {
                await MainGame.PickupFromDiscardAsync();
                return;
            }
            await MainGame.SendDiscardMessageAsync(deck);
            await MainGame.DiscardAsync(deck);
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
        public BasicGameCommand? GoOutCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            Deck1!.NeverAutoDisable = true; //if you want reshuffling, use this.  otherwise, comment or delete.
            PlayerHand1!.Maximum = 8;
            GoOutCommand = new BasicGameCommand(this, async items =>
            {
                if (MainGame!.CanGoOut() == false)
                {
                    await ShowGameMessageAsync("Sorry; you cannot go out");
                    return;
                }
                if (ThisData!.MultiPlayer == true)
                {
                    MultiplayerOut thisOut = new MultiplayerOut();
                    thisOut.Deck = MainGame.CardForDiscard!.Deck;
                    thisOut.WasGuaranteed = MainGame.WasGuarantee;
                    await ThisNet!.SendAllAsync("goout", thisOut);
                }
                await MainGame.GoOutAsync();
            }, Items =>
            {
                return PlayerHand1.HandList.Count == 8; //if 8, you can go out since you have to draw first.
            }, this, CommandContainer!);
        }
    }
}