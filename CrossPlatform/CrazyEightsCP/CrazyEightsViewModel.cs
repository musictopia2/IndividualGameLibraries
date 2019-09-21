using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.DIContainers;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace CrazyEightsCP
{
    public class CrazyEightsViewModel : BasicCardGamesVM<RegularSimpleCard, CrazyEightsPlayerItem, CrazyEightsMainGameClass>
    {
        public SimpleEnumPickerVM<EnumSuitList, DeckPieceCP, SuitListChooser>? SuitChooser;
        public CrazyEightsViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableDeck()
        {
            return !SuitChooser!.Visible;
        }

        protected override bool CanEnablePile1()
        {
            return !SuitChooser!.Visible;
        }
        public void MarkSuitVisible(bool Visible)
        {
            SuitChooser!.Visible = Visible;
        }

        protected override async Task ProcessDiscardClickedAsync()
        {
            int deck = PlayerHand1!.ObjectSelected();
            if (deck == 0)
            {
                await ShowGameMessageAsync("You must select a card first");
                return;
            }
            if (MainGame!.IsValidMove(deck) == false)
            {
                await ShowGameMessageAsync("Illegal Move");
                PlayerHand1.UnselectAllObjects();
                return;
            }
            await MainGame.PlayCardAsync(deck);
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            SuitChooser = new SimpleEnumPickerVM<EnumSuitList, DeckPieceCP, SuitListChooser>(this);
            SuitChooser.ItemClickedAsync += SuitChooser_ItemClickedAsync;
            SuitChooser.AutoSelectCategory = EnumAutoSelectCategory.AutoEvent;
            Deck1!.NeverAutoDisable = true;
        }
        private async Task SuitChooser_ItemClickedAsync(EnumSuitList ThisPiece)
        {
            await MainGame!.SuitChosenAsync(ThisPiece);
        }

    }
}