using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.MultiplePilesObservable;
using BasicGameFrameworkLibrary.SpecializedGameTypes.StockClasses;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using FlinchCP.Cards;
using FlinchCP.Data;
using FlinchCP.Logic;
using System.Threading.Tasks;
using static FlinchCP.Data.GlobalConstants;

namespace FlinchCP.ViewModels
{
    [InstanceGame]
    public class PlayerPilesViewModel : Screen, IBlankGameVM
    {
        private readonly FlinchVMData _model;
        private readonly FlinchMainGameClass _mainGame;

        public PlayerPilesViewModel(CommandContainer commandContainer, FlinchGameContainer gameContainer, FlinchVMData model, FlinchMainGameClass mainGame)
        {
            gameContainer.SingleInfo = gameContainer.PlayerList!.GetWhoPlayer();
            CommandContainer = commandContainer;
            _model = model;
            _mainGame = mainGame;
            _model.StockPile.ClearCards();
            //model.StockPile = new Piles.StockViewModel(commandContainer, gameContainer.Aggregator);
            gameContainer.SingleInfo!.StockList.ForEach(x =>
            {
                model.StockPile.AddCard(x);
            });

            //model.StockPile.StockFrame.PileList!.ReplaceRange(gameContainer.SingleInfo!.StockList);

            _model.DiscardPiles = new DiscardPilesVM<FlinchCardInformation>(commandContainer, gameContainer.Aggregator);
            _model.DiscardPiles.Init(HowManyDiscards);
            if (gameContainer.SingleInfo!.DiscardList.Count > 0)
                model.DiscardPiles!.PileList!.ReplaceRange(gameContainer.SingleInfo.DiscardList);

            _model.DiscardPiles.PileClickedAsync += DiscardPiles_PileClickedAsync;
            _model.StockPile!.StockClickedAsync += StockPile_StockClickedAsync;
        }
        protected override Task TryCloseAsync()
        {
            _model.DiscardPiles!.PileClickedAsync -= DiscardPiles_PileClickedAsync;
            _model.StockPile!.StockClickedAsync -= StockPile_StockClickedAsync;
            return base.TryCloseAsync();
        }

        public CommandContainer CommandContainer { get; set; }


        private async Task DiscardPiles_PileClickedAsync(int index, BasicPileInfo<FlinchCardInformation> thisPile)
        {
            int playerDeck = _model!.PlayerHand1!.ObjectSelected();
            if (playerDeck > 0)
            {
                if (_mainGame!.SaveRoot!.GameStatus == EnumStatusList.FirstOne && _mainGame.SaveRoot.PlayerFound == 0)
                {
                    await UIPlatform.ShowMessageAsync("Sorry; cannot discard one because you need either a one or else pass on this turn");
                    _mainGame.UnselectAllCards();
                    return;
                }
                if (_model.DiscardPiles!.CanAddToDiscardFlinch(index) == false)
                {
                    await UIPlatform.ShowMessageAsync("Sorry, cannot discard here because there is an empty one that has to be used first");
                    return;
                }
                await _mainGame!.AddToDiscardAsync(index, playerDeck);
                return;
            }
            if (_model.DiscardPiles!.HasCard(index) == false)
                return;
            if (_model.DiscardPiles.PileList![index].IsSelected == true)
            {
                _model.DiscardPiles.PileList[index].IsSelected = false;
                return;
            }
            _mainGame!.UnselectAllCards();
            _model.DiscardPiles.SelectUnselectSinglePile(index);
        }
        private Task StockPile_StockClickedAsync()
        {
            int nums = _model.StockPile!.CardSelected();
            _mainGame!.UnselectAllCards();
            if (nums > 0)
            {
                _model.StockPile.UnselectCard();
                return Task.CompletedTask;
            }
            _model.StockPile.SelectCard();
            return Task.CompletedTask;
        }

    }
}