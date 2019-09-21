using BaseSolitaireClassesCP.Cards;
using BaseSolitaireClassesCP.ClockClasses;
using BasicGameFramework.CommandClasses; //its common to have command classes.
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace ClockSolitaireCP
{
    public class ClockSolitaireViewModel : SimpleGameVM, ISoloCardGameVM<SolitaireCard>, IClockVM
    {
        private int _CardsLeft;

        public int CardsLeft
        {
            get { return _CardsLeft; }
            set
            {
                if (SetProperty(ref _CardsLeft, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        public async Task DeckClicked()
        {
            //if we click on deck, will do code for this.
            await Task.CompletedTask;
        }

        public ClockSolitaireViewModel(ISimpleUI TempUI, IGamePackageResolver TempC) : base(TempUI, TempC)
        {
        }

        public DeckViewModel<SolitaireCard>? DeckPile { get; set; }


        private ClockSolitaireGameClass? _mainGame;
        public ClockBoard? Clock1;

        public override void Init()
        {
            _mainGame = MainContainer!.Resolve<ClockSolitaireGameClass>();
            DeckPile = MainContainer.Resolve<DeckViewModel<SolitaireCard>>(); //i think.
            CommandContainer!.ExecutingChanged += CommandContainer_ExecutingChanged;
            DeckPile.NeverAutoDisable = true; //if it needs to autoreshuffle. do this.  if you don't want to allow reshuffling set to false.
            DeckPile.SendEnableProcesses(this, () =>
            {
                if (_mainGame.GameGoing == false)
                    return false;
                return true; //if other logic is needed for deck, put here.

            });
            Clock1 = new ClockBoard(this);
        }

        private async void CommandContainer_ExecutingChanged()
        {
            if (CommandContainer!.IsExecuting)
                return;
            //code to run when its not busy.

            if (_mainGame!.GameGoing)
                await _mainGame.SaveStateAsync();
        }

        public override async Task StartNewGameAsync()
        {
            await _mainGame!.NewGameAsync();
        }
        public override bool CanEnableBasics() //since a person can do new game but still do other things.
        {
            return true;
        }

        async Task IClockVM.ClockClickedAsync(int index)
        {
            if (Clock1!.IsValidMove(index) == false)
            {
                await ShowGameMessageAsync("Illegal Move");
                return;
            }
            Clock1.MakeMove(index);
            if (Clock1.HasWon())
            {
                await _mainGame!.ShowWinAsync();
                return;
            }
            if (Clock1.IsGameOver())
            {
                await _mainGame!.ShowLossAsync();
            }
        }
    }
}