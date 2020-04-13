using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using GoFishCP.Data;
using GoFishCP.Logic;
using System.Threading.Tasks;

namespace GoFishCP.ViewModels
{
    [InstanceGame]
    public class AskViewModel : Screen, IBlankGameVM, IBasicEnableProcess
    {
        private readonly GoFishVMData _model;
        private readonly GoFishGameContainer _gameContainer;
        private readonly IAskProcesses _processes;

        public AskViewModel(CommandContainer commandContainer, GoFishVMData model, GoFishGameContainer gameContainer, IAskProcesses processes)
        {
            CommandContainer = commandContainer;
            _model = model;
            _gameContainer = gameContainer;
            _processes = processes;
            _model.AskList.ItemClickedAsync += AskList_ItemClickedAsync;
            _model.AskList.SendEnableProcesses(this, (() => _gameContainer.SaveRoot.RemovePairs == false && _gameContainer.SaveRoot.NumberAsked == false));
        }

        private Task AskList_ItemClickedAsync(EnumCardValueList piece)
        {
            _model.CardYouAsked = piece;
            _model.AskList!.SelectSpecificItem(piece); //try this.
            return Task.CompletedTask;
        }

        public CommandContainer CommandContainer { get; set; }
        protected override Task TryCloseAsync()
        {
            _model.AskList.ItemClickedAsync -= AskList_ItemClickedAsync;
            return base.TryCloseAsync();
        }

        bool IBasicEnableProcess.CanEnableBasics()
        {
            return true;
        }
        public bool CanAsk
        {
            get
            {
                if (_gameContainer.SaveRoot.RemovePairs == true || _gameContainer.SaveRoot.NumberAsked == true)
                {
                    return false;
                }
                return _model.CardYouAsked != EnumCardValueList.None;
            }
        }

        [Command(EnumCommandCategory.Game)]
        public async Task AskAsync()
        {
            if (_gameContainer.BasicData!.MultiPlayer == true)
            {
                await _gameContainer.Network!.SendAllAsync("numbertoask", _model.CardYouAsked);
            }
            await _processes!.NumberToAskAsync(_model.CardYouAsked);
        }
    }
}
