using BaseSolitaireClassesCP.Cards;
using BaseSolitaireClassesCP.TriangleClasses;
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace TriangleSolitaireCP
{
    public class TriangleSolitaireViewModel : SimpleGameVM, ISoloCardGameVM<SolitaireCard>, ITriangleVM
    {
        public TriangleSolitaireViewModel(ISimpleUI tempUI, IGamePackageResolver tempC) : base(tempUI, tempC)
        {
        }

        public DeckViewModel<SolitaireCard>? DeckPile { get; set; }
        public async Task DeckClicked()
        {
            if (DeckPile!.IsEndOfDeck())
            {
                await ShowGameMessageAsync($"You left {Triangle1!.HowManyCardsLeft} cards");
                await _mainGame!.DeleteGameAsync();
                NewGameVisible = true;
                _mainGame.GameGoing = false;
                return;
            }
            _mainGame!.DrawCard();
        }
        private TriangleSolitaireGameClass? _mainGame;
        public PileViewModel<SolitaireCard>? Pile1;
        public TriangleBoard? Triangle1;

        public override void Init()
        {
            _mainGame = MainContainer!.Resolve<TriangleSolitaireGameClass>();
            DeckPile = MainContainer.Resolve<DeckViewModel<SolitaireCard>>(); //i think.
            CommandContainer!.ExecutingChanged += CommandContainer_ExecutingChanged;
            DeckPile.NeverAutoDisable = true; //if it needs to autoreshuffle. do this.  if you don't want to allow reshuffling set to false.
            DeckPile.SendEnableProcesses(this, () =>
            {
                if (_mainGame.GameGoing == false)
                    return false;
                return true; //if other logic is needed for deck, put here.

            });
            EventAggregator thisE = MainContainer.Resolve<EventAggregator>();
            Pile1 = new PileViewModel<SolitaireCard>(thisE, this);
            Triangle1 = new TriangleBoard(this);
            Pile1.Text = "Discard";
            Pile1.Visible = true;
            Pile1.SendEnableProcesses(this, () => false);

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

        async Task ITriangleVM.CardClickedAsync(SolitaireCard thisCard)
        {
            var pileCard = Pile1!.GetCardInfo();
            if (_mainGame!.IsValidMove(thisCard, pileCard, out EnumIncreaseList tempi) == false)
            {
                await ShowGameMessageAsync("Sorry, wrong card");
                return;
            }
            await _mainGame.MakeMoveAsync(thisCard.Deck, tempi);
        }
    }
}