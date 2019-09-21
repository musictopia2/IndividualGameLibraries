using BaseSolitaireClassesCP.MainClasses;
using BasicGameFramework.DIContainers;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
namespace KlondikeSolitaireCP
{
    public class KlondikeSolitaireViewModel : SolitaireMainViewModel<KlondikeSolitaireSaveInfo>
    {

        public KlondikeSolitaireViewModel(ISimpleUI tempUI, IGamePackageResolver tempC) : base(tempUI, tempC) { }
    }
}