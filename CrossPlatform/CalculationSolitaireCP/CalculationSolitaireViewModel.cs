using BaseSolitaireClassesCP.MainClasses;
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
namespace CalculationSolitaireCP
{
    public class CalculationSolitaireViewModel : SolitaireMainViewModel<CalculationSolitaireSaveInfo>
    {
        public CalculationSolitaireViewModel(ISimpleUI tempUI, IGamePackageResolver tempC) : base(tempUI, tempC) { }
        public override void Init()
        {
            base.Init();
            DeckPile!.DeckStyle = DeckViewModel<BaseSolitaireClassesCP.Cards.SolitaireCard>.EnumStyleType.AlwaysKnown;
        }
    }
}