using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace RackoCP
{
    public class RackoViewModel : BasicCardGamesVM<RackoCardInformation, RackoPlayerItem, RackoMainGameClass>
    {
        public RackoViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableDeck()
        {
            return !MainGame!.AlreadyDrew;
        }
        protected override bool CanEnablePile1()
        {
            return !MainGame!.AlreadyDrew;
        }
        protected override async Task ProcessDiscardClickedAsync()
        {
            await MainGame!.PickupFromDiscardAsync();
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
        public PileViewModel<RackoCardInformation>? Pile2;
        public BasicGameCommand<RackoCardInformation>? PlayOnPileCommand { get; set; }
        public BasicGameCommand? DiscardCurrentCommand { get; set; }
        public BasicGameCommand? RackoCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            Deck1!.NeverAutoDisable = true; //if you want reshuffling, use this.  otherwise, comment or delete.
            PlayOnPileCommand = new BasicGameCommand<RackoCardInformation>(this, async thisCard =>
            {
                await MainGame!.SendDiscardMessageAsync(thisCard.Deck); //i think
                await MainGame.DiscardAsync(thisCard);
            }, items =>
            {
                return MainGame!.AlreadyDrew;
            }, this, CommandContainer!);
            DiscardCurrentCommand = new BasicGameCommand(this, async items =>
            {
                if (Pile2!.PileEmpty() == true)
                {
                    await ShowGameMessageAsync("You must have a card to discard");
                    return;
                }
                if (ThisData!.MultiPlayer == true)
                    await ThisNet!.SendAllAsync("discardcurrent");
                await MainGame!.DiscardCurrentAsync();
            }, items =>
            {
                return MainGame!.AlreadyDrew;
            }, this, CommandContainer!);
            RackoCommand = new BasicGameCommand(this, async items =>
            {
                if (MainGame!.HasRacko() == false)
                {
                    await ShowGameMessageAsync("There is no Racko");
                    return;
                }
                if (ThisData!.MultiPlayer == true)
                    await ThisNet!.SendAllAsync("racko");
                await MainGame.EndRoundAsync();
            }, items =>
            {
                return !MainGame!.AlreadyDrew;
            }, this, CommandContainer!);
            Pile2 = new PileViewModel<RackoCardInformation>(ThisE!, this);
            Pile2.Text = "Current Card"; //risking putting here.
            Pile2.CurrentOnly = true;
            Pile2.FirstLoad(new RackoCardInformation()); //hopefully that still works.
            Pile2.Visible = true;
            MainGame!.OtherPile = Pile2;
        }
    }
}