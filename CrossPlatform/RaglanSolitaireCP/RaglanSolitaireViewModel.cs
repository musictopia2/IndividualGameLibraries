using BaseSolitaireClassesCP.Cards;
using BaseSolitaireClassesCP.MainClasses;
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
namespace RaglanSolitaireCP
{
    public class RaglanSolitaireViewModel : SolitaireMainViewModel<RaglanSolitaireSaveInfo>
    {
        public RaglanSolitaireViewModel(ISimpleUI tempUI, IGamePackageResolver tempC) : base(tempUI, tempC) { }
        public HandViewModel<SolitaireCard>? Stock1;
        public override void Init()
        {
            base.Init();
            Stock1 = new HandViewModel<SolitaireCard>(this);
            Stock1.Maximum = 6;
            Stock1.Visible = true;
            Stock1.Text = "Stock";
        }
    }
}