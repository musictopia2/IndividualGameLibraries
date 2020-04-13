using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.MultiplePilesObservable;
using BasicGameFrameworkLibrary.NetworkingClasses.Extensions;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.Exceptions;
using DutchBlitzCP.Cards;
using DutchBlitzCP.Data;
using DutchBlitzCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace DutchBlitzCP.ViewModels
{
    [InstanceGame]
    public class DutchBlitzMainViewModel : BasicCardGamesVM<DutchBlitzCardInformation>
    {
        private readonly DutchBlitzMainGameClass _mainGame; //if we don't need, delete.
        private readonly DutchBlitzVMData _model;
        private readonly DutchBlitzGameContainer _gameContainer; //if not needed, delete.

        public DutchBlitzMainViewModel(CommandContainer commandContainer,
            DutchBlitzMainGameClass mainGame,
            DutchBlitzVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            DutchBlitzGameContainer gameContainer
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _mainGame = mainGame;
            _model = viewModel;
            _gameContainer = gameContainer;
            _model.Deck1.NeverAutoDisable = true;
            CanSendDrawMessage = false;
        }
        protected override Task ActivateAsync()
        {
            if (_model.DiscardPiles == null)
            {
                throw new BasicBlankException("No discard piles.  Rethink");
            }
            _model.DiscardPiles.PileClickedAsync += DiscardPiles_PileClickedAsync;
            _model.StockPile.StockFrame.SendEnableProcesses(this, () => _model.StockPile.CardsLeft() > 0);
            _model.StockPile.StockClickedAsync += StockPile_StockClickedAsync;
            _model.PublicPiles1.PileClickedAsync += PublicPiles1_PileClickedAsync;
            _model.PublicPiles1.NewPileClickedAsync += PublicPiles1_NewPileClickedAsync;
            CommandContainer.ExecutingChanged += CommandContainer_ExecutingChanged;
            _model.Stops.TimeUp += Stops_TimeUp;
            return base.ActivateAsync();
        }

        private void CommandContainer_ExecutingChanged()
        {
            if (CommandContainer!.IsExecuting)
            {
                if (_model.DidStartTimer == true)
                    _model.Stops!.PauseTimer();
            }
            else
            {
                if (_model.DidStartTimer)
                    _model.Stops!.ContinueTimer();
            }
        }
        private async void Stops_TimeUp()
        {
            if (_model.DidStartTimer == false)
                return;
            _model.DidStartTimer = false;
            CommandContainer!.IsExecuting = true;
            CommandContainer.ManuelFinish = true;
            if (_mainGame.BasicData!.MultiPlayer == true)
                await _mainGame.Network!.SendEndTurnAsync();
            await _mainGame!.EndTurnAsync();
        }

        protected override Task TryCloseAsync()
        {
            _model.DiscardPiles!.PileClickedAsync -= DiscardPiles_PileClickedAsync;
            _model.StockPile.StockClickedAsync -= StockPile_StockClickedAsync;
            _model.PublicPiles1.PileClickedAsync -= PublicPiles1_PileClickedAsync;
            _model.PublicPiles1.NewPileClickedAsync -= PublicPiles1_NewPileClickedAsync;
            CommandContainer.ExecutingChanged -= CommandContainer_ExecutingChanged;
            _model.Stops.TimeUp -= Stops_TimeUp;
            return base.TryCloseAsync();
        }
        private bool _didClickOld = false;

        private async Task PublicPiles1_NewPileClickedAsync()
        {
            if (_didClickOld == true)
            {
                _didClickOld = false;
                return;
            }
            await ProcessHumanPileAsync(-1);
        }

        private async Task PublicPiles1_PileClickedAsync(int index)
        {
            _didClickOld = true;
            await ProcessHumanPileAsync(index);
        }

        private async Task StockPile_StockClickedAsync()
        {
            int nums = _model.StockPile!.CardSelected();
            if (nums > 0)
            {
                _model.StockPile.UnselectCard();
                return;
            }
            _mainGame!.UnselectCards();
            _model.StockPile.SelectCard();
            await Task.CompletedTask;
        }
        private async Task DiscardPiles_PileClickedAsync(int index, BasicPileInfo<DutchBlitzCardInformation> thisPile)
        {
            DutchBlitzCardInformation thisCard;
            if (_model.DiscardPiles!.HasCard(index) == false)
            {
                if (_model.StockPile!.CardSelected() > 0)
                {
                    thisCard = _model.StockPile.GetCard();
                    _model.StockPile.RemoveCard();
                    _model.DiscardPiles.AddToEmptyDiscard(thisCard);
                    _mainGame!.SingleInfo!.StockLeft--;
                    await _mainGame.SendStockAsync();
                    return;
                }
                if (_model.Pile1!.CardSelected() == 0)
                {
                    await SendErrorMessageAsync("Sorry, no card was selected");
                    return;
                }
                thisCard = _model.Pile1.GetCardInfo();
                thisCard.IsSelected = false;
                thisCard.Drew = false;
                _model.DiscardPiles.AddToEmptyDiscard(thisCard);
                _model.Pile1.RemoveFromPile();
                return;
            }
            thisCard = _model.DiscardPiles.GetLastCard(index);
            if (thisCard.IsSelected)
            {
                _model.DiscardPiles.SelectUnselectSinglePile(index);
                return;
            }
            _mainGame!.UnselectCards();
            _model.DiscardPiles.SelectUnselectSinglePile(index);
        }

        protected override bool CanEnableDeck()
        {
            if (_model.Deck1!.IsEndOfDeck() && _model.Pile1!.DiscardList().Count > 1)
                return false;
            return true;
        }

        protected override bool CanEnablePile1()
        {
            return !_model.Pile1!.PileEmpty();
        }

        protected override async Task ProcessDiscardClickedAsync()
        {
            await Task.CompletedTask;
            if (_model.Pile1!.GetCardInfo().Deck == 0)
                return;
            if (_model.Pile1.CardSelected() > 0)
            {
                _model.Pile1.IsSelected(false);
                return;
            }
            _mainGame!.UnselectCards();
            _model.Pile1.IsSelected(true);
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
        private string _errorMessage = "";
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                if (SetProperty(ref _errorMessage, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private async Task SendErrorMessageAsync(string message)
        {
            ErrorMessage = message;
            if (_gameContainer.Test!.NoAnimations == false)
                await _gameContainer!.Delay!.DelayMilli(200);
            ErrorMessage = "";
        }

        private async Task ProcessHumanPileAsync(int pile)
        {
            if (_mainGame!.HumanHasSelected() == false)
            {
                await SendErrorMessageAsync("No card was selected");
                return;
            }
            if (_mainGame.CanHumanPlayCard(pile) == false)
            {
                await SendErrorMessageAsync("Illegal move");
                return;
            }
            DutchBlitzCardInformation thisCard;
            thisCard = _mainGame.HumanCardToUse(out bool _, out bool sends);
            if (sends)
                await _mainGame.SendStockAsync();
            if (pile == -1)
            {
                if (_mainGame.BasicData!.MultiPlayer)
                    await _mainGame.Network!.SendAllAsync("newpile", thisCard.Deck);
                await _mainGame.AddNewPublicPileAsync(thisCard);
                return;
            }
            if (_mainGame.BasicData!.MultiPlayer)
            {
                SendExpand temps = new SendExpand();
                temps.Deck = thisCard.Deck;
                temps.Pile = pile;
                await _mainGame.Network!.SendAllAsync("expandpile", temps);
            }
            await _mainGame.ExpandPublicPileAsync(thisCard, pile);
        }
        [Command(EnumCommandCategory.Game)]
        public async Task DutchAsync()
        {
            if (_mainGame!.HasBlitz == false)
            {
                await SendErrorMessageAsync("Sorry, you still have cards in your stock pile.  Therefore, the round has not ended yet");
                return;
            }
            if (_mainGame.BasicData!.MultiPlayer)
                await _mainGame.Network!.SendAllAsync("blitz");
            _model.Stops.PauseTimer();
            _model.DidStartTimer = false; //because you got blitz.
            await _mainGame.BlitzAsync();
        }

    }
}