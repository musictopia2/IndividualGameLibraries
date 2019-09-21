using BasicGameFramework.CommandClasses; //its common to have command classes.
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace AccordianSolitaireCP
{
    public class AccordianSolitaireViewModel : SimpleGameVM, ISoloCardGameVM<AccordianSolitaireCardInfo>
    {
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
        public AccordianSolitaireViewModel(ISimpleUI tempUI, IGamePackageResolver tempC) : base(tempUI, tempC) { }

        public DeckViewModel<AccordianSolitaireCardInfo>? DeckPile { get; set; }

        public async Task DeckClicked()
        {
            //if we click on deck, will do code for this.
            await Task.CompletedTask;
        }
        private AccordianSolitaireGameClass? _mainGame;
        public GameBoard? GameBoard1;
        public PlainCommand? UnSelectCommand { get; set; }
        public override void Init()
        {
            _mainGame = MainContainer!.Resolve<AccordianSolitaireGameClass>();
            DeckPile = MainContainer!.Resolve<DeckViewModel<AccordianSolitaireCardInfo>>(); //i think.
            CommandContainer!.ExecutingChanged += CommandContainer_ExecutingChanged;
            DeckPile.NeverAutoDisable = true; //if it needs to autoreshuffle. do this.  if you don't want to allow reshuffling set to false.
            DeckPile.SendEnableProcesses(this, () =>
            {
                return false; //i think
            });
            GameBoard1 = new GameBoard(this);
            GameBoard1.Visible = true;
            GameBoard1.ObjectClickedAsync += GameBoard1_ObjectClickedAsync;
            UnSelectCommand = new PlainCommand(items =>
            {
                _mainGame.SaveRoot.DeckSelected = 0;
                GameBoard1.UnselectAllObjects();
            }, items => true, this, CommandContainer);
        }

        private async Task GameBoard1_ObjectClickedAsync(AccordianSolitaireCardInfo ThisObject, int Index)
        {
            if (Index == -1)
                throw new BasicBlankException("Index cannot be -1.  Rethink");
            if (GameBoard1!.IsCardSelected(ThisObject) == false)
            {
                GameBoard1.SelectUnselectCard(ThisObject);
                return;
            }
            if (GameBoard1.IsValidMove(ThisObject) == false)
            {
                await ShowGameMessageAsync("Illegal Move");
                return;
            }
            GameBoard1.MakeMove(ThisObject);
            if (Score == 52)
                await _mainGame!.ShowWinAsync();
        }

        private async void CommandContainer_ExecutingChanged()
        {
            if (CommandContainer!.IsExecuting)
                return;
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