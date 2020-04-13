using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using PickelCardGameCP.Data;
using PickelCardGameCP.Logic;
using System.Threading.Tasks;

namespace PickelCardGameCP.ViewModels
{
    [InstanceGame]
    public class PickelBidViewModel : Screen, IBlankGameVM
    {
        private readonly PickelCardGameVMData _model;
        private readonly PickelCardGameGameContainer _gameContainer;
        private readonly IBidProcesses _processes;

        public PickelBidViewModel(
            CommandContainer commandContainer,
            PickelCardGameVMData model,
            PickelCardGameGameContainer gameContainer,
            IBidProcesses processes
            )
        {
            CommandContainer = commandContainer;
            _model = model;
            _gameContainer = gameContainer;
            _processes = processes;
            _model.Bid1.ChangedNumberValueAsync += Bid1_ChangedNumberValueAsync;
            _model.Suit1.ItemSelectionChanged += Suit1_ItemSelectionChanged;
        }
        protected override Task TryCloseAsync()
        {
            _model.Bid1.ChangedNumberValueAsync -= Bid1_ChangedNumberValueAsync;
            _model.Suit1.ItemSelectionChanged -= Suit1_ItemSelectionChanged;
            return base.TryCloseAsync();
        }
        private void Suit1_ItemSelectionChanged(EnumSuitList? piece)
        {
            if (piece.HasValue == false)
                _gameContainer!.SaveRoot!.TrumpSuit = EnumSuitList.None;
            else
                _gameContainer!.SaveRoot!.TrumpSuit = piece!.Value;
        }

        private Task Bid1_ChangedNumberValueAsync(int chosen)
        {
            _model.BidAmount = chosen;
            return Task.CompletedTask;
        }

        public CommandContainer CommandContainer { get; set; }
        public bool CanProcessBid()
        {
            if (_model.BidAmount == -1 || _model.TrumpSuit == EnumSuitList.None)
                return false;
            return true;
        }
        [Command(EnumCommandCategory.Plain)]
        public async Task ProcessBidAsync()
        {
            await _processes.ProcessBidAsync();
        }
        public bool CanPass => _processes.CanPass();
        [Command(EnumCommandCategory.Plain)]
        public async Task PassAsync()
        {
            await _processes.PassBidAsync();
        }

    }
}
