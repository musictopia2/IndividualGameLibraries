using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.Extensions;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace YahtzeeHandsDownCP
{
    public class YahtzeeHandsDownViewModel : BasicCardGamesVM<YahtzeeHandsDownCardInformation, YahtzeeHandsDownPlayerItem, YahtzeeHandsDownMainGameClass>
    {
        public YahtzeeHandsDownViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableDeck()
        {
            return false;
        }
        protected override bool CanEnablePile1()
        {
            if (MainGame!.PlayerList.Count() > 2)
                return !MainGame.AlreadyDrew;
            if (MainGame.SaveRoot!.ExtraTurns < 4)
                return !MainGame.AlreadyDrew;
            return false;
        }
        protected override async Task ProcessDiscardClickedAsync()
        {
            var thisList = PlayerHand1!.ListSelectedObjects();
            if (thisList.Count == 0)
            {
                await ShowGameMessageAsync("Must choose at least one card to discard to get new cards");
                return;
            }
            if (ThisData!.MultiPlayer)
            {
                var nextList = thisList.GetDeckListFromObjectList();
                await ThisNet!.SendAllAsync("replacecards", nextList);
            }
            await MainGame!.ReplaceCardsAsync(thisList);
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
        public HandViewModel<ComboCardInfo>? ComboHandList;
        public PileViewModel<ChanceCardInfo>? ChancePile;
        public BasicGameCommand? GoOutCommand { get; set; }
        public override bool CanEndTurn()
        {
            return !CanEnablePile1();
        }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            Deck1!.NeverAutoDisable = true;
            ComboHandList = new HandViewModel<ComboCardInfo>(this);
            PlayerHand1!.Maximum = 5;
            PlayerHand1.AutoSelect = HandViewModel<YahtzeeHandsDownCardInformation>.EnumAutoType.SelectAsMany;
            ComboHandList.Visible = true;
            ComboHandList.Text = "Category Cards";
            ChancePile = new PileViewModel<ChanceCardInfo>(ThisE!, this);
            ChancePile.Visible = false;
            ChancePile.CurrentOnly = true;
            ChancePile.Text = "Chance";
            GoOutCommand = new BasicGameCommand(this, async items =>
            {
                var results = MainGame!.GetResults();
                if (results.Count == 0)
                {
                    await ShowGameMessageAsync("Cannot go out");
                    return;
                }
                if (ThisData!.MultiPlayer)
                    await ThisNet!.SendAllAsync("wentout", results.First());
                await MainGame.PlayerGoesOutAsync(results.First());
            }, items => true, this, CommandContainer!);
        }
    }
}