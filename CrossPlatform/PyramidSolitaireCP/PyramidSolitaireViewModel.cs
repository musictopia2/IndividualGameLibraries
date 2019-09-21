using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BasicGameFramework.MainViewModels;
using BasicGameFramework.ViewModelInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.DIContainers;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using BasicGameFramework.CommandClasses; //its common to have command classes.
using BaseSolitaireClassesCP.Cards;
using CommonBasicStandardLibraries.Messenging;
using BaseSolitaireClassesCP.TriangleClasses;
namespace PyramidSolitaireCP
{
    public class PyramidSolitaireViewModel : SimpleGameVM, ISoloCardGameVM<SolitaireCard>, ITriangleVM
    {
        public PyramidSolitaireViewModel(ISimpleUI tempUI, IGamePackageResolver tempC) : base(tempUI, tempC)
        {
        }

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
        private bool _CanEnableGame;

        public bool CanEnableGame
        {
            get { return _CanEnableGame; }
            set
            {
                if (SetProperty(ref _CanEnableGame, value))
                {
                    //can decide what to do when property changes
                    if (value == true)
                        NewGameVisible = true;
                }

            }
        }

        public async Task DeckClicked()
        {
            await _mainGame!.DrawCardAsync();
        }
        private PyramidSolitaireGameClass? _mainGame;

        public DeckViewModel<SolitaireCard>? DeckPile { get; set; }
        public PlayList? PlayList1;
        public TriangleBoard? GameBoard1;
        public PileViewModel<SolitaireCard>? Discard;
        public PileViewModel<SolitaireCard>? CurrentPile;
        public PlainCommand? PlaySelectedCardsCommand { get; set; }

        public override void Init()
        {
            _mainGame = MainContainer!.Resolve<PyramidSolitaireGameClass>();
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
            CurrentPile = new PileViewModel<SolitaireCard>(thisE, this);
            CurrentPile.SendEnableProcesses(this, () => CurrentPile.PileEmpty() == false);
            CurrentPile.Visible = true;
            CurrentPile.Text = "Current";
            CurrentPile.CurrentOnly = true;
            CurrentPile.PileClickedAsync += CurrentPile_PileClickedAsync;
            Discard = new PileViewModel<SolitaireCard>(thisE, this);
            Discard.SendEnableProcesses(this, () => Discard.PileEmpty() == false);
            Discard.Visible = true;
            Discard.Text = "Discard";
            Discard.PileClickedAsync += Discard_PileClickedAsync;
            PlayList1 = new PlayList(this);
            PlayList1.SendEnableProcesses(this, () => PlayList1.HasChosenCards());
            PlayList1.Visible = true;
            GameBoard1 = new TriangleBoard(this);
            PlaySelectedCardsCommand = new PlainCommand(async items =>
            {
                if (_mainGame.HasPlayedCard() == false)
                {
                    await ShowGameMessageAsync("Sorry, there is no card to play");
                    return;
                }
                if (_mainGame.IsValidMove() == false)
                {
                    await ShowGameMessageAsync("Illegal Move");
                    _mainGame.PutBack();
                    return;
                }
                await _mainGame.PlayCardsAsync();
            }, items => true, this, CommandContainer);
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
            if (PlayList1!.AlreadyHasTwoCards())
            {
                await ShowGameMessageAsync("Sorry, 2 has already been selected");
                return;
            }
            PlayList1.AddCard(thisCard);
            GameBoard1!.MakeInvisible(thisCard.Deck);
        }
    }
}