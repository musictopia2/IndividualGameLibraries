using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using CommonBasicStandardLibraries.Exceptions;
using LifeBoardGameCP.Cards;
using LifeBoardGameCP.Data;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace LifeBoardGameCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class ReturnStockProcesses : IReturnStockProcesses
    {
        private readonly LifeBoardGameVMData _model;
        private readonly LifeBoardGameGameContainer _gameContainer;

        public ReturnStockProcesses(LifeBoardGameVMData model, LifeBoardGameGameContainer gameContainer)
        {
            _model = model;
            _gameContainer = gameContainer;
        }

        public void LoadCurrentPlayerStocks()
        {
            if (_gameContainer!.SingleInfo!.FirstStock == 0 || _gameContainer.SingleInfo.SecondStock == 0)
            {
                throw new BasicBlankException("Must have both stock items.  Otherwise; should do automatically");
            }
            DeckRegularDict<LifeBaseCard> tempList = new DeckRegularDict<LifeBaseCard>
            {
                _gameContainer.SingleInfo.GetStockCard(_gameContainer.SingleInfo.FirstStock),
                _gameContainer.SingleInfo.GetStockCard(_gameContainer.SingleInfo.SecondStock)
            };
            _model.HandList!.HandList.ReplaceRange(tempList);
            _model.HandList.AutoSelect = HandObservable<LifeBaseCard>.EnumAutoType.SelectOneOnly;
            _model.HandList.Text = "List Of Stocks";
        }

        public async Task StockReturnedAsync(int deck)
        {
            if (_gameContainer.CanSendMessage())
            {
                await _gameContainer.Network!.SendAllAsync("stockreturned", deck);
            }
            var thisStock = CardsModule.GetStockCard(deck);
            await _gameContainer.ShowCardAsync(thisStock);
            _gameContainer.SingleInfo!.Hand.RemoveObjectByDeck(thisStock.Deck);
            if (thisStock.Value == _gameContainer.SingleInfo.FirstStock)
            {
                _gameContainer.SingleInfo.FirstStock = _gameContainer.SingleInfo.SecondStock;
                _gameContainer.SingleInfo.FirstStock = 0;
            }
            else if (thisStock.Value == _gameContainer.SingleInfo.SecondStock)
            {
                _gameContainer.SingleInfo.SecondStock = 0;
            }
            else
            {
                throw new BasicBlankException("Cannot update the stock");
            }
            _gameContainer.GameStatus = EnumWhatStatus.NeedToEndTurn;
            await _gameContainer.ContinueTurnAsync!.Invoke();
        }
    }
}