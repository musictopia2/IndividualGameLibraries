using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using RageCardGameCP.Data;
using RageCardGameCP.Logic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace RageCardGameCP.ViewModels
{
    [InstanceGame]
    public class RageBiddingViewModel : Screen, IBlankGameVM
    {
        private readonly RageCardGameVMData _model;
        private readonly IBidProcesses _processes;

        public RageBiddingViewModel(CommandContainer commandContainer, RageCardGameVMData model, IBidProcesses processes)
        {
            CommandContainer = commandContainer;
            _model = model;
            _processes = processes;
            NormalTurn = _model.NormalTurn;
            TrumpSuit = _model.TrumpSuit;
            _model.PropertyChanged += Model_PropertyChanged;
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(NormalTurn))
            {
                NormalTurn = _model.NormalTurn;
            }
            if (e.PropertyName == nameof(TrumpSuit))
            {
                TrumpSuit = _model.TrumpSuit;
            }
        }

        protected override Task TryCloseAsync()
        {
            _model.PropertyChanged -= Model_PropertyChanged;
            return base.TryCloseAsync();
        }
        public CommandContainer CommandContainer { get; set; }
        public bool CanBid => _model.BidAmount > -1;
        [Command(EnumCommandCategory.Plain)]
        public async Task BidAsync()
        {
            await _processes.ProcessBidAsync();
        }
        private string _normalTurn = "";
        
        public string NormalTurn
        {
            get { return _normalTurn; }
            set
            {
                if (SetProperty(ref _normalTurn, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private EnumColor _trumpSuit;

        public EnumColor TrumpSuit
        {
            get { return _trumpSuit; }
            set
            {
                if (SetProperty(ref _trumpSuit, value))
                {
                    //can decide what to do when property changes
                }

            }
        }


    }
}