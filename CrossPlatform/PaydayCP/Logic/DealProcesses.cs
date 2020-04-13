using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.Extensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using PaydayCP.Cards;
using PaydayCP.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PaydayCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class DealProcesses : IDealProcesses
    {
        private readonly PaydayGameContainer _gameContainer;
        private readonly PaydayVMData _model;

        public DealProcesses(PaydayGameContainer gameContainer, PaydayVMData model)
        {
            _gameContainer = gameContainer;
            _model = model;
        }

        async Task IDealProcesses.ChoseWhetherToPurchaseDealAsync()
        {
            await _gameContainer.StartProcessPopUpAsync(_model);
            if (_model.PopUpChosen == "Yes")
            {
                //you chose to purchase deal.
                var thisDeal = _model.DealPile!.GetCardInfo();
                _gameContainer.SingleInfo!.ReduceFromPlayer(Math.Abs(thisDeal.Cost));
                _gameContainer.SingleInfo!.Hand.Add(thisDeal);
                _gameContainer.PopulateDeals(_model);
                if (_gameContainer.Test.NoAnimations == false)
                {
                    await _gameContainer.Delay!.DelaySeconds(2);
                }
            }
            else
            {
                _gameContainer.SaveRoot.OutCards.Add(_gameContainer.SaveRoot.CurrentDeal!);
            }
            _gameContainer.SaveRoot.CurrentDeal = new DealCard();
            _gameContainer.SaveRoot.GameStatus = EnumStatus.EndingTurn;
            await _gameContainer.ContinueTurnAsync!.Invoke();
        }

        async Task IDealProcesses.ContinueDealProcessesAsync()
        {
            await ContinueDealProcessesAsync();
        }

        private async Task ContinueDealProcessesAsync()
        {
            var thisDeal = _gameContainer.SaveRoot!.DealListLeft.First();
            _gameContainer.SaveRoot.DealListLeft.RemoveFirstItem();
            _gameContainer.SaveRoot.Instructions = $"Do you want to buy {thisDeal.Business} for {thisDeal.Cost.ToCurrency(0)}?  The value is {thisDeal.Value.ToCurrency(0)}";
            _gameContainer.SaveRoot.CurrentDeal = thisDeal;
            _model.DealPile!.AddCard(thisDeal);
            _gameContainer.SaveRoot.GameStatus = EnumStatus.ChooseDeal;
            await _gameContainer.ContinueTurnAsync!.Invoke();
        }

        void IDealProcesses.PopulateDeals()
        {
            var tempList = _gameContainer.SingleInfo!.Hand.GetMailOrDealList<DealCard>(EnumCardCategory.Deal);
            _model!.CurrentDealList!.HandList.ReplaceRange(tempList);
        }

        async Task IDealProcesses.ProcessDealAsync(bool isYardSale)
        {
            if (isYardSale == true)
            {
                decimal yardSale = _model!.Cup!.TotalDiceValue * 100;
                _gameContainer.SingleInfo!.ReduceFromPlayer(yardSale);
                var thisDeal = _gameContainer.SaveRoot!.YardSaleDealCard;
                _gameContainer.SingleInfo!.Hand.Add(thisDeal!);
                _model.DealPile.AddCard(thisDeal!);
                _gameContainer.SaveRoot.Instructions = $"Yard sale.  You received {thisDeal!.Business} for {yardSale.ToCurrency(0)}";
                _gameContainer.SaveRoot.GameStatus = EnumStatus.ViewYardSale;
                if (_gameContainer.Test.NoAnimations == false)
                {
                    await _gameContainer.Delay.DelaySeconds(2);
                }
                _gameContainer.SaveRoot.GameStatus = EnumStatus.EndingTurn;
                return;
            }
            if (_gameContainer.SaveRoot!.DealListLeft.Count == 0)
            {
                bool rets;
                rets = await ProcessShuffleDealsAsync();
                if (rets == false)
                {
                    return;
                }
                if (_gameContainer.BasicData.MultiPlayer)
                {
                    await _gameContainer.Network!.SendAllAsync("continuedealprocesses");
                }
            }
            await ContinueDealProcessesAsync();
        }

        async Task<bool> IDealProcesses.ProcessShuffleDealsAsync()
        {
            return await ProcessShuffleDealsAsync();
        }

        private async Task<bool> ProcessShuffleDealsAsync()
        {
            if (_gameContainer.SaveRoot!.DealListLeft.Count != 0)
            {
                throw new BasicBlankException("Should not have ran the processes for reshuffling because not needed");
            }
            bool rets = _gameContainer.ShouldReshuffle();
            if (rets == false)
            {
                _gameContainer.Check!.IsEnabled = true;
                return false;
            }
            await ReshuffleDealsAsync();
            return true;
        }

        private async Task ReshuffleDealsAsync()
        {
            var thisList = _gameContainer.SaveRoot!.OutCards.GetMailOrDealList<DealCard>(EnumCardCategory.Deal);
            thisList.ShuffleList();
            if (_gameContainer.BasicData!.MultiPlayer == true)
            {
                await _gameContainer.Network!.SendAllAsync("reshuffledeallist", thisList.GetDeckListFromObjectList());
            }
            await ReshuffleDealsAsync(thisList);
        }

        private async Task ReshuffleDealsAsync(DeckRegularDict<DealCard> list)
        {
            await UIPlatform.ShowMessageAsync("Deal is being reshuffled");
            _gameContainer.SaveRoot!.DealListLeft = list;
            _gameContainer.SaveRoot.OutCards.RemoveAllOnly(items => items.Deck > 24);
        }

        async Task IDealProcesses.ReshuffleDealsAsync(DeckRegularDict<DealCard> list)
        {
            await ReshuffleDealsAsync(list);
        }

        void IDealProcesses.SetUpDeal()
        {
            var list = _gameContainer.Random.GenerateRandomList(24);
            if (list.Count != 24)
            {
                throw new BasicBlankException($"Must have 24 deal cards, not {list.Count} cards");
            }
            _gameContainer.SaveRoot!.DealListLeft.Clear();
            list.ForEach(index =>
            {
                DealCard thisCard = (DealCard)_gameContainer!.GetCard(index);
                _gameContainer.SaveRoot.DealListLeft.Add(thisCard);
            });
            _gameContainer.SaveRoot.YardSaleDealCard = _gameContainer.SaveRoot.DealListLeft.First();
            _gameContainer.SaveRoot.DealListLeft.RemoveFirstItem();
        }
    }
}