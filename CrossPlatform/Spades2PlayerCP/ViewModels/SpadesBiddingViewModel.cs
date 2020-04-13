using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using Spades2PlayerCP.Data;
using Spades2PlayerCP.Logic;
using System.Threading.Tasks;

namespace Spades2PlayerCP.ViewModels
{
    [InstanceGame]
    public class SpadesBiddingViewModel : Screen, IBlankGameVM
    {
        private readonly Spades2PlayerVMData _model;
        private readonly Spades2PlayerMainGameClass _mainGame;

        public SpadesBiddingViewModel(CommandContainer commandContainer, Spades2PlayerVMData model, Spades2PlayerMainGameClass mainGame)
        {
            CommandContainer = commandContainer;
            _model = model;
            _mainGame = mainGame;
            _model.Bid1.ChangedNumberValueAsync += Bid1_ChangedNumberValueAsync;
        }
        protected override Task TryCloseAsync()
        {
            _model.Bid1.ChangedNumberValueAsync -= Bid1_ChangedNumberValueAsync;
            return base.TryCloseAsync();
        }
        public CommandContainer CommandContainer { get; set; }

        private Task Bid1_ChangedNumberValueAsync(int chosen)
        {
            _model.BidAmount = chosen;
            return Task.CompletedTask;
        }
        public bool CanBid => _model.BidAmount > -1;
        [Command(EnumCommandCategory.Plain)]
        public async Task BidAsync()
        {
            if (_mainGame.BasicData!.MultiPlayer == true)
                await _mainGame.Network!.SendAllAsync("bid", _model.BidAmount);
            await _mainGame.ProcessBidAsync();
        }
    }
}