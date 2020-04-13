using BasicGameFrameworkLibrary.Attributes;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks;
using XactikaCP.Data;

namespace XactikaCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class BidProcesses : IBidProcesses
    {
        private readonly XactikaVMData _model;
        private readonly XactikaGameContainer _gameContainer;

        public BidProcesses(XactikaVMData model, XactikaGameContainer gameContainer)
        {
            _model = model;
            _gameContainer = gameContainer;
        }

        async Task IBidProcesses.ProcessBidAsync()
        {
            int bidAmount = _model.BidChosen;
            _gameContainer.SingleInfo!.BidAmount = bidAmount;
            _model!.Bid1!.SelectNumberValue(bidAmount);
            if (_gameContainer.Test!.NoAnimations == false)
                await _gameContainer.Delay!.DelaySeconds(1);
            _model.Bid1.UnselectAll();
            _model.BidChosen = -1;
            await ContinueBidProcessAsync();
        }
        private async Task ContinueBidProcessAsync()
        {
            _gameContainer.WhoTurn = await _gameContainer.PlayerList!.CalculateWhoTurnAsync(true);
            if (_gameContainer.WhoTurn == _gameContainer.WhoStarts)
            {
                await EndBidAsync();
                return;
            }
            _gameContainer.SingleInfo = _gameContainer.PlayerList.GetWhoPlayer();
            await BeginBiddingAsync();
        }
        public async Task EndBidAsync()
        {
            _gameContainer.SaveRoot!.GameStatus = EnumStatusList.Normal;
            if (_gameContainer.CloseBiddingAsync == null)
            {
                throw new BasicBlankException("Nobody is closing the bidding.  Rethink");
            }
            await _gameContainer.CloseBiddingAsync.Invoke();
            if (_gameContainer.StartNewTrickAsync == null)
            {
                throw new BasicBlankException("Nobody is starting new trick.  Rethink");
            }
            await _gameContainer.StartNewTrickAsync(); //not sure about the turn proceeses (?)
        }
        public async Task BeginBiddingAsync()
        {
            await PopulateBidAmountsAsync();
            await _gameContainer.StartNewTurnAsync!.Invoke();
        }
        public async Task PopulateBidAmountsAsync()
        {
            var nextPlayer = await _gameContainer.PlayerList!.CalculateWhoTurnAsync(true);
            int nonNumber = -1;
            if (nextPlayer == _gameContainer.WhoStarts)
            {
                var total = _gameContainer.PlayerList.Where(items => items.BidAmount > -1).Sum(items => items.BidAmount);
                if (total > 8)
                    nonNumber = -1;
                else
                    nonNumber = 8 - total;
            }
            int x;
            CustomBasicList<int> tempList = new CustomBasicList<int>();
            for (x = 0; x <= 8; x++)
            {
                if (x != nonNumber)
                    tempList.Add(x);
            }
            _model!.Bid1!.UnselectAll();
            _model.BidChosen = -1;
            _model.Bid1.LoadNumberList(tempList);
            await _gameContainer.ContinueTurnAsync!.Invoke();
        }
    }
}
