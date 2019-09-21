using BasicGameFramework.CommandClasses; //its common to have command classes.
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.MultiplePilesViewModels;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace HeapSolitaireCP
{
    public class HeapSolitaireViewModel : SimpleGameVM, ISoloCardGameVM<HeapSolitaireCardInfo>
    {
        public HeapSolitaireViewModel(ISimpleUI tempUI, IGamePackageResolver tempC) : base(tempUI, tempC)
        {
        }

        public DeckViewModel<HeapSolitaireCardInfo>? DeckPile { get; set; }

        public async Task DeckClicked()
        {
            //if we click on deck, will do code for this.
            await Task.CompletedTask;
        }
        private HeapSolitaireGameClass? _mainGame;
        public WastePiles? Waste1;
        public MainPiles? Main1;
        private int _Score;

        public int Score
        {
            get { return _Score; }
            set
            {
                if (SetProperty(ref _Score, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public override void Init()
        {
            _mainGame = MainContainer!.Resolve<HeapSolitaireGameClass>();
            DeckPile = MainContainer.Resolve<DeckViewModel<HeapSolitaireCardInfo>>(); //i think.
            CommandContainer!.ExecutingChanged += CommandContainer_ExecutingChanged;
            DeckPile.NeverAutoDisable = true; //if it needs to autoreshuffle. do this.  if you don't want to allow reshuffling set to false.
            DeckPile.SendEnableProcesses(this, () =>
            {
                if (_mainGame.GameGoing == false)
                    return false;
                return true; //if other logic is needed for deck, put here.

            });
            Waste1 = new WastePiles(this);
            Waste1.Visible = true;
            Waste1.PileClickedAsync += Waste1_PileClickedAsync;
            Main1 = new MainPiles(this);
            Main1.Visible = true;
            Main1.PileClickedAsync += Main1_PileClickedAsync;
        }

        private async Task Main1_PileClickedAsync(int Index, BasicPileInfo<HeapSolitaireCardInfo> ThisPile)
        {
            await _mainGame!.SelectMainAsync(Index);
        }

        private Task Waste1_PileClickedAsync(int Index, BasicPileInfo<HeapSolitaireCardInfo> ThisPile)
        {
            Waste1!.SelectPile(Index);
            return Task.CompletedTask;
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
            NewGameVisible = true; //most of the time, will be visible.  if i am wrong, rethink.
        }
        public override bool CanEnableBasics() //since a person can do new game but still do other things.
        {
            return true;
        }
    }
}