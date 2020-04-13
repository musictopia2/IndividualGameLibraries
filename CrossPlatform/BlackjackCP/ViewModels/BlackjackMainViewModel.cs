using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using BlackjackCP.Data;
using BlackjackCP.Logic;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using BasicGameFrameworkLibrary.BasicEventModels;
namespace BlackjackCP.ViewModels
{
    [InstanceGame]
    public class BlackjackMainViewModel : Screen,
        IBasicEnableProcess,
        IBlankGameVM,
        IAggregatorContainer
    {
        private readonly IEventAggregator _aggregator;
        private readonly StatsClass _stats;
        private readonly BlackjackMainGameClass _mainGame;

        public PlayerStack? ComputerStack; //decided to make it more clear now.
        public PlayerStack? HumanStack;
        public enum EnumAceChoice
        {
            Low = 1,
            High = 2
        }

        private bool _needsAceChoice;
        public bool NeedsAceChoice
        {
            get
            {
                return _needsAceChoice;
            }

            set
            {
                if (SetProperty(ref _needsAceChoice, value) == true)
                {
                    Reprocess();
                }
            }
        }
        private bool _selectedYet;

        public bool SelectedYet
        {
            get { return _selectedYet; }
            set
            {
                if (SetProperty(ref _selectedYet, value))
                {
                    Reprocess();
                }

            }
        }
        private void Reprocess()
        {
            if (NeedsAceChoice == false && SelectedYet == true)
                CanHitOrStay = true;
            else
                CanHitOrStay = false;
        }

        private bool _canHitOrStay;
        public bool CanHitOrStay
        {
            get
            {
                return _canHitOrStay;
            }

            set
            {
                if (SetProperty(ref _canHitOrStay, value) == true)
                {
                }
            }
        }


        private int _humanPoints;
        public int HumanPoints
        {
            get
            {
                return _humanPoints;
            }

            set
            {
                if (SetProperty(ref _humanPoints, value) == true)
                {
                }
            }
        }

        private int _computerPoints;
        public int ComputerPoints
        {
            get
            {
                return _computerPoints;
            }

            set
            {
                if (SetProperty(ref _computerPoints, value) == true)
                {
                }
            }
        }

        private int _draws;
        public int Draws
        {
            get
            {
                return _draws;
            }

            set
            {
                if (SetProperty(ref _draws, value) == true)
                {
                    _stats.Draws = value;
                }
            }
        }

        private int _wins;
        public int Wins
        {
            get
            {
                return _wins;
            }

            set
            {
                if (SetProperty(ref _wins, value) == true)
                {
                    _stats.Wins = value;
                }
            }
        }

        private int _losses;
        public int Losses
        {
            get
            {
                return _losses;
            }

            set
            {
                if (SetProperty(ref _losses, value) == true)
                {
                    _stats.Losses = value;
                }
            }
        }
        public bool CanAce => NeedsAceChoice;
        [Command(EnumCommandCategory.Game)]
        public async Task AceAsync(EnumAceChoice choice)
        {
            await _mainGame.HumanAceAsync(choice);
        }
        public bool CanHit => CanHitOrStay;
        [Command(EnumCommandCategory.Game)]
        public async Task HitAsync()
        {
            await _mainGame.HumanHitAsync();
        }
        public bool CanStay => CanHitOrStay;
        [Command(EnumCommandCategory.Game)]
        public async Task StayAsync()
        {
            await _mainGame.HumanStayAsync();
        }
        public DeckObservablePile<BlackjackCardInfo> DeckPile { get; set; }

        public BlackjackMainViewModel(IEventAggregator aggregator,
            CommandContainer commandContainer,
            IGamePackageResolver resolver,
            StatsClass stats
            )
        {
            //i got around the deckpile by doing resolve.  i think this way is better.
            _aggregator = aggregator;
            CommandContainer = commandContainer;
            CommandContainer.ExecutingChanged += CommandContainer_ExecutingChanged; //hopefully no problem (?)
            //this means i can no longer ask for it because it needs a new one.  otherwise, causes problems.

            DeckPile = resolver.ReplaceObject<DeckObservablePile<BlackjackCardInfo>>();
            DeckPile.DeckClickedAsync += DeckPile_DeckClickedAsync;
            _stats = stats;
            IsNotifying = false;
            HumanStack = new PlayerStack(commandContainer);
            ComputerStack = new PlayerStack(commandContainer);
            HumanStack.ProcessLabel(false);
            HumanStack.CardSelectedAsync += HumanStack_CardSelectedAsync;
            ComputerStack.ProcessLabel(true);
            ComputerStack.AlwaysDisabled = true;
            HumanStack.SendFunction(() => NeedsAceChoice == false && SelectedYet == false);
            Wins = stats.Wins;
            Losses = stats.Losses;
            Draws = stats.Draws;
            IsNotifying = true;
            DeckPile.NeverAutoDisable = true;
            DeckPile.IsEnabled = false;
            DeckPile.SendEnableProcesses(this, () =>
            {
                return false; //false this time.
            });
            _mainGame = resolver.ReplaceObject<BlackjackMainGameClass>(); //hopefully this works.  means you have to really rethink.
        }
        private async Task HumanStack_CardSelectedAsync(bool HasChoice)
        {
            await _mainGame!.HumanSelectAsync(HasChoice);
        }
        private Task DeckPile_DeckClickedAsync()
        {
            return Task.CompletedTask;
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
            await _mainGame.NewGameAsync(DeckPile, this);
            await _aggregator.SendLoadAsync();
            CommandContainer.ManualReport(); //just had to run the code to manually report.
        }
    }
}
