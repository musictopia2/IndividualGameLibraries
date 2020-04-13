using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using CommonBasicStandardLibraries.Exceptions;
using RageCardGameCP.Data;
using System.Threading.Tasks;

namespace RageCardGameCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class BidProcesses : IBidProcesses
    {
        private readonly RageCardGameGameContainer _gameContainer;
        private readonly RageCardGameVMData _model;
        private readonly RageDelgates _delgates;

        public BidProcesses(RageCardGameGameContainer gameContainer, RageCardGameVMData model, RageDelgates delgates)
        {
            _gameContainer = gameContainer;
            _model = model;
            _delgates = delgates;

        }

        public async Task LoadBiddingScreenAsync()
        {
            _model.BidAmount = -1;
            _model.Bid1!.LoadNormalNumberRangeValues(0, _gameContainer!.SaveRoot!.CardsToPassOut);
            _model.Bid1.UnselectAll();
            if (_delgates.LoadBidScreenAsync == null)
            {
                throw new BasicBlankException("Nobody is loading the bidding screen.  Rethink");
            }
            await _delgates.LoadBidScreenAsync.Invoke();
        }

        public async Task ProcessBidAsync()
        {
            _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetWhoPlayer();
            if (_gameContainer.SingleInfo.CanSendMessage(_gameContainer.BasicData!) == true)
                await _gameContainer.Network!.SendAllAsync("bid", _model!.BidAmount);
            _gameContainer.SingleInfo.BidAmount = _model!.BidAmount;
            if (_delgates.CloseBidScreenAsync == null)
            {
                throw new BasicBlankException("Nobody is closing bidding screen.  Rethink");
            }
            await _delgates.CloseBidScreenAsync.Invoke();
            await _gameContainer.EndTurnAsync!.Invoke();
        }
    }
}
