using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.MultiplePilesObservable;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using HeapSolitaireCP.Data;
using HeapSolitaireCP.EventModels;
using HeapSolitaireCP.Logic;
using HeapSolitaireCP.Piles;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using BasicGameFrameworkLibrary.BasicEventModels;
namespace HeapSolitaireCP.ViewModels
{
    [InstanceGame]
    public class HeapSolitaireMainViewModel : Screen,
        IBasicEnableProcess,
        IBlankGameVM,
        IAggregatorContainer,
        IHandle<ScoreEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly HeapSolitaireMainGameClass _mainGame;

        public DeckObservablePile<HeapSolitaireCardInfo> DeckPile { get; set; }

        public WastePiles Waste1;
        public MainPiles Main1;
        private int _score;

        public int Score
        {
            get { return _score; }
            set
            {
                if (SetProperty(ref _score, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        public HeapSolitaireMainViewModel(IEventAggregator aggregator,
            CommandContainer commandContainer,
            IGamePackageResolver resolver
            )
        {
            _aggregator = aggregator;
            CommandContainer = commandContainer;
            CommandContainer.ExecutingChanged += CommandContainer_ExecutingChanged; //hopefully no problem (?)
            DeckPile = resolver.ReplaceObject<DeckObservablePile<HeapSolitaireCardInfo>>();
            DeckPile.DeckClickedAsync += DeckPile_DeckClickedAsync;
            DeckPile.NeverAutoDisable = true;
            _aggregator.Subscribe(this);
            DeckPile.SendEnableProcesses(this, () =>
            {
                return false;
            });
            _mainGame = resolver.ReplaceObject<HeapSolitaireMainGameClass>(); //hopefully this works.  means you have to really rethink.

            Waste1 = new WastePiles(CommandContainer, _aggregator, _mainGame);
            Waste1.PileClickedAsync += Waste1_PileClickedAsync;
            Main1 = new MainPiles(CommandContainer, _aggregator, _mainGame);
            Main1.PileClickedAsync += Main1_PileClickedAsync;

        }
        private async Task Main1_PileClickedAsync(int index, BasicPileInfo<HeapSolitaireCardInfo> pile)
        {
            await _mainGame!.SelectMainAsync(index);
        }

        private Task Waste1_PileClickedAsync(int index, BasicPileInfo<HeapSolitaireCardInfo> pile)
        {
            Waste1!.SelectPile(index);
            return Task.CompletedTask;
        }
        private async Task DeckPile_DeckClickedAsync()
        {
            //if we click on deck, will do code for this.
            await Task.CompletedTask;
        }

        private async void CommandContainer_ExecutingChanged()
        {
            if (CommandContainer.IsExecuting)
                return;
            //code to run when its not busy.

            if (_mainGame.GameGoing)
                await _mainGame.SaveStateAsync();
        }

        public CommandContainer CommandContainer { get; set; }

        IEventAggregator IAggregatorContainer.Aggregator => _aggregator;

        public bool CanEnableBasics()
        {
            return true; //because maybe you can't enable it.
        }
        protected override async Task ActivateAsync()
        {
            await base.ActivateAsync();
            await _mainGame.NewGameAsync(this);
            await _aggregator.SendLoadAsync();
        }
        protected override Task TryCloseAsync()
        {
            CommandContainer.ExecutingChanged -= CommandContainer_ExecutingChanged;
            return base.TryCloseAsync();
        }

        void IHandle<ScoreEventModel>.Handle(ScoreEventModel message)
        {
            Score = message.Score;
        }
    }
}
