using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGameFrameworkLibrary.SolitaireClasses.TriangleClasses;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using PyramidSolitaireCP.EventModels;
using PyramidSolitaireCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace PyramidSolitaireCP.ViewModels
{
    [InstanceGame]
    public class PyramidSolitaireMainViewModel : Screen,
        IBasicEnableProcess,
        IBlankGameVM,
        IAggregatorContainer,
        IHandle<MoveEventModel>,
        IHandle<ScoreEventModel>,
        ITriangleVM
    {
        private readonly IEventAggregator _aggregator;
        private readonly PyramidSolitaireMainGameClass _mainGame;

        public DeckObservablePile<SolitaireCard> DeckPile { get; set; }

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

        public PlayList PlayList1;
        public TriangleBoard GameBoard1;
        public PileObservable<SolitaireCard> Discard;
        public PileObservable<SolitaireCard> CurrentPile;
        [Command(EnumCommandCategory.Plain)]
        public async Task PlaySelectedCardsAsync()
        {
            if (_mainGame.HasPlayedCard() == false)
            {
                await UIPlatform.ShowMessageAsync("Sorry, there is no card to play");
                return;
            }
            if (_mainGame.IsValidMove() == false)
            {
                await UIPlatform.ShowMessageAsync("Illegal Move");
                _mainGame.PutBack();
                return;
            }
            await _mainGame.PlayCardsAsync();
        }

        public PyramidSolitaireMainViewModel(IEventAggregator aggregator,
            CommandContainer commandContainer,
            IGamePackageResolver resolver
            )
        {
            _aggregator = aggregator;
            CommandContainer = commandContainer;
            CommandContainer.ExecutingChanged += CommandContainer_ExecutingChanged; //hopefully no problem (?)
            DeckPile = resolver.ReplaceObject<DeckObservablePile<SolitaireCard>>();
            DeckPile.DeckClickedAsync += DeckPile_DeckClickedAsync;
            DeckPile.NeverAutoDisable = true;
            aggregator.Subscribe(this);
            DeckPile.SendEnableProcesses(this, () =>
            {
                if (_mainGame.GameGoing == false)
                    return false;
                return true; //if other logic is needed for deck, put here.
            });
            _mainGame = resolver.ReplaceObject<PyramidSolitaireMainGameClass>(); //hopefully this works.  means you have to really rethink.

            CurrentPile = new PileObservable<SolitaireCard>(_aggregator, commandContainer);
            CurrentPile.SendEnableProcesses(this, () => CurrentPile.PileEmpty() == false);
            CurrentPile.Text = "Current";
            CurrentPile.CurrentOnly = true;
            CurrentPile.PileClickedAsync += CurrentPile_PileClickedAsync;
            Discard = new PileObservable<SolitaireCard>(_aggregator, CommandContainer);
            Discard.SendEnableProcesses(this, () => Discard.PileEmpty() == false);
            Discard.Text = "Discard";
            Discard.PileClickedAsync += Discard_PileClickedAsync;
            PlayList1 = new PlayList(CommandContainer, aggregator);
            PlayList1.SendEnableProcesses(this, () => PlayList1.HasChosenCards());
            PlayList1.Visible = true;
            GameBoard1 = new TriangleBoard(this, CommandContainer, resolver, _mainGame);

        }

        private async Task DeckPile_DeckClickedAsync()
        {
            await _mainGame!.DrawCardAsync();
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
        }
        protected override Task TryCloseAsync()
        {
            CommandContainer.ExecutingChanged -= CommandContainer_ExecutingChanged;
            return base.TryCloseAsync();
        }

        void IHandle<MoveEventModel>.Handle(MoveEventModel message)
        {
            GameBoard1.MakeInvisible(message.Deck);
        }

        void IHandle<ScoreEventModel>.Handle(ScoreEventModel message)
        {
            Score = message.Score;
        }
        private async Task Discard_PileClickedAsync()
        {
            if (Discard!.PileEmpty())
                throw new BasicBlankException("Since there is no card here, should have been disabled");
            var thisCard = Discard.GetCardInfo();
            Discard.RemoveFromPile();
            PlayList1!.AddCard(thisCard);
            await Task.CompletedTask;
        }

        private async Task CurrentPile_PileClickedAsync()
        {
            if (CurrentPile!.PileEmpty())
                throw new BasicBlankException("Since there is no card here, should have been disabled");
            var thisCard = CurrentPile.GetCardInfo();
            CurrentPile.RemoveFromPile();
            PlayList1!.AddCard(thisCard);
            await Task.CompletedTask;
        }
        async Task ITriangleVM.CardClickedAsync(SolitaireCard thisCard)
        {
            if (PlayList1!.AlreadyHasTwoCards())
            {
                await UIPlatform.ShowMessageAsync("Sorry, 2 has already been selected");
                return;
            }
            PlayList1.AddCard(thisCard);
            GameBoard1!.MakeInvisible(thisCard.Deck);
        }
    }
}