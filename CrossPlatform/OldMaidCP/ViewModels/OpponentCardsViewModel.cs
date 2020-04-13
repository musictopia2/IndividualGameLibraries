using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using OldMaidCP.Data;
using OldMaidCP.Logic;
using System.Threading.Tasks;

namespace OldMaidCP.ViewModels
{
    [InstanceGame]
    public class OpponentCardsViewModel : Screen, IBlankGameVM, IBasicEnableProcess
    {
        private readonly OldMaidVMData _model;
        private readonly IOtherPlayerProcess _process;

        public CommandContainer CommandContainer { get; set; }

        public OpponentCardsViewModel(CommandContainer commandContainer, OldMaidGameContainer gameContainer, OldMaidVMData model, IOtherPlayerProcess process)
        {
            CommandContainer = commandContainer;
            _model = model;
            _process = process;
            _model.OpponentCards1.SendEnableProcesses(this, () => gameContainer.SaveRoot.RemovePairs == false && gameContainer.SaveRoot.AlreadyChoseOne == false);
            _model.OpponentCards1.ObjectClickedAsync += OpponentCards1_ObjectClickedAsync;
        }

        private Task OpponentCards1_ObjectClickedAsync(RegularSimpleCard payLoad, int index)
        {
            return _process.SelectCardAsync(payLoad.Deck);
        }

        public bool CanEnableBasics()
        {
            return true;
        }
    }
}
