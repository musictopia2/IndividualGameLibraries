using BaseSolitaireClassesCP.DataClasses;
using BaseSolitaireClassesCP.MainClasses;
using BasicGameFramework.CommandClasses;
using BasicGameFramework.DIContainers;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
namespace PersianSolitaireCP
{
    public class PersianSolitaireViewModel : SolitaireMainViewModel<PersianSolitaireSaveInfo>
    {
        public PersianSolitaireViewModel(ISimpleUI tempUI, IGamePackageResolver tempC) : base(tempUI, tempC) { }
        private int _DealNumber;

        public int DealNumber
        {
            get { return _DealNumber; }
            set
            {
                if (SetProperty(ref _DealNumber, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public PlainCommand? NewDealCommand { get; set; }

        public override void Init()
        {
            base.Init();
            _thisData = MainContainer!.Resolve<ISolitaireData>();
            _tempWaste = MainContainer.Resolve<WastePiles>();
            NewDealCommand = new PlainCommand(items =>
            {
                _tempWaste.Redeal();
            }, items =>
            {
                return _thisData.Deals != DealNumber;
            }, this, CommandContainer!);
        }

        //i don't know if we have anything extra.  if we do, rethink.
        ISolitaireData? _thisData;
        WastePiles? _tempWaste;
    }
}