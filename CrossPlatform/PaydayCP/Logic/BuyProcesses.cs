using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using PaydayCP.Cards;
using PaydayCP.Data;
using System.Threading.Tasks;

namespace PaydayCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class BuyProcesses : IBuyProcesses
    {
        private readonly PaydayGameContainer _gameContainer;
        private readonly PaydayVMData _model;

        public BuyProcesses(PaydayGameContainer gameContainer, PaydayVMData model)
        {
            _gameContainer = gameContainer;
            _model = model;
        }
        private void RemovePlayerDeal(DealCard thisCard)
        {
            _gameContainer.SingleInfo!.Hand.RemoveSpecificItem(thisCard);
            _gameContainer.SaveRoot!.OutCards.Add(thisCard);
        }
        async Task IBuyProcesses.BuyerSelectedAsync(int deck)
        {
            _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetWhoPlayer();
            if (_gameContainer.SingleInfo.CanSendMessage(_gameContainer.BasicData!) == true)
                await _gameContainer.Network!.SendAllAsync("buyerselected", deck);
            var thisDeal = _model.CurrentDealList!.HandList.GetSpecificItem(deck);
            thisDeal.IsSelected = false;
            _model.CurrentDealList.HandList.ReplaceAllWithGivenItem(thisDeal);
            if (_gameContainer.Test!.NoAnimations == false)
            {
                await _gameContainer.Delay!.DelaySeconds(.5);
            }
            RemovePlayerDeal(thisDeal);
            _gameContainer.SingleInfo.MoneyHas += thisDeal.Value;
            var tempList = _gameContainer.SingleInfo.Hand.GetMailOrDealList<DealCard>(EnumCardCategory.Deal);
            _model.CurrentDealList.HandList.ReplaceRange(tempList);
            if (_gameContainer.Test!.NoAnimations == false)
            {
                await _gameContainer.Delay!.DelaySeconds(1.5);
            }
            _gameContainer.PopulateDeals(_model);
            if (_gameContainer.Test!.NoAnimations == false)
            {
                await _gameContainer.Delay!.DelaySeconds(1.5);
            }
            _gameContainer.SaveRoot!.GameStatus = EnumStatus.EndingTurn;
            await _gameContainer.ContinueTurnAsync!.Invoke();
        }

        async Task IBuyProcesses.ProcessBuyerAsync()
        {
            if (_model!.CurrentDealList!.HandList.Count == 0)
            {
                _gameContainer.SaveRoot!.Instructions = "No deals for the bank to buy";
                if (_gameContainer.Test!.NoAnimations == false)
                {
                    await _gameContainer.Delay!.DelaySeconds(3);
                }
                await _gameContainer.ContinueTurnAsync!.Invoke();
                return;
            }
            _gameContainer.SaveRoot!.GameStatus = EnumStatus.ChooseBuy;
            _gameContainer.SaveRoot.Instructions = "Please select a deal for the bank to buy from you.";
            await _gameContainer.ContinueTurnAsync!.Invoke();
        }
    }
}