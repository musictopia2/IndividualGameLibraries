using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BaseSolitaireClassesCP.MainClasses;
using BasicGameFramework.DIContainers;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
//i think this is the most common things i like to do
namespace FreeCellSolitaireCP
{
    public class FreeCellSolitaireViewModel : SolitaireMainViewModel<FreeCellSolitaireSaveInfo>
    {
        public FreeCellSolitaireViewModel(ISimpleUI tempUI, IGamePackageResolver tempC) : base(tempUI, tempC) { }
        public FreePiles? FreePiles1;
        FreeCellSolitaireGameClass? _mainGame;
        public override void Init()
        {
            base.Init();
            FreePiles1 = new FreePiles(this);
            FreePiles1.PileClickedAsync += FreePiles1_PileClickedAsync;
            _mainGame = MainContainer!.Resolve<FreeCellSolitaireGameClass>();
        }

        private async Task FreePiles1_PileClickedAsync(int Index, BasicGameFramework.MultiplePilesViewModels.BasicPileInfo<BaseSolitaireClassesCP.Cards.SolitaireCard> ThisPile)
        {
            await _mainGame!.FreeSelectedAsync(Index);
        }
    }
}