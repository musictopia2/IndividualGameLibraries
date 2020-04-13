using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using CommonBasicStandardLibraries.Exceptions;
using PickelCardGameCP.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PickelCardGameCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class BidProcesses : IBidProcesses
    {
        private readonly PickelCardGameVMData _model;
        private readonly PickelCardGameGameContainer _gameContainer;
        private readonly PickelDelegates _delegates;

        public BidProcesses(PickelCardGameVMData model, PickelCardGameGameContainer gameContainer, PickelDelegates delegates)
        {
            _model = model;
            _gameContainer = gameContainer;
            _delegates = delegates;
        }

        public bool CanPass()
        {
            int temps;
            if (_gameContainer.WhoTurn == 1)
                temps = 2;
            else if (_gameContainer.WhoTurn == 2 && _gameContainer.PlayerList.Count() == 3)
                temps = 3;
            else
                temps = 1; //has to do manually because otherwise, can't do it since its a function to determine whether to enable.  that can't be async unfortunately
            if (temps == _gameContainer.WhoStarts && _gameContainer.SaveRoot!.WonSoFar == 0)
                return false;
            return true;
        }

        public async Task PassBidAsync()
        {
            if (_gameContainer.SingleInfo!.CanSendMessage(_gameContainer.BasicData!) == true)
                await _gameContainer.Network!.SendAllAsync("pass");
            await ContinueBidProcessAsync();
        }
        private async Task EndBidAsync()
        {
            if (_delegates.CloseBiddingAsync == null)
            {
                throw new BasicBlankException("Nobody is closing the bidding.  Rethink");
            }
            _gameContainer.SaveRoot!.GameStatus = EnumStatusList.Normal;
            await _delegates.CloseBiddingAsync.Invoke();
            var ThisPlayer = _gameContainer.PlayerList![_gameContainer.SaveRoot.WonSoFar];
            _gameContainer.SaveRoot.TrumpSuit = ThisPlayer.SuitDesired;
            if (_gameContainer.StartNewTrickAsync == null)
            {
                throw new BasicBlankException("Nobody is handling startnewtrick.  Rethink");
            }
            await _gameContainer.StartNewTrickAsync();
        }
        private async Task ContinueBidProcessAsync()
        {
            if (_gameContainer.SaveRoot!.HighestBid == 10)
            {
                await EndBidAsync();
                return;
            }
            _gameContainer.WhoTurn = await _gameContainer.PlayerList!.CalculateWhoTurnAsync();
            if (_gameContainer.WhoTurn == _gameContainer.WhoStarts)
            {
                await EndBidAsync();
                return;
            }
            _gameContainer.SingleInfo = _gameContainer.PlayerList.GetWhoPlayer();
            await PopulateBidsAsync();
            await _gameContainer.StartNewTurnAsync!.Invoke(); //hopefully this simple.
        }

        public async Task ProcessBidAsync()
        {
            if (_gameContainer.SingleInfo!.CanSendMessage(_gameContainer.BasicData!) == true)
            {
                SendBid thisSend = new SendBid()
                {
                    Bid = _model!.BidAmount,
                    Suit = _model.TrumpSuit
                };
                await _gameContainer.Network!.SendAllAsync("bid", thisSend);
            }
            _gameContainer.SingleInfo!.BidAmount = _model!.BidAmount;
            _gameContainer.SingleInfo.SuitDesired = _model.TrumpSuit;
            _gameContainer.SaveRoot!.HighestBid = _model.BidAmount;
            _gameContainer.SaveRoot.WonSoFar = _gameContainer.WhoTurn;
            if (_gameContainer.Test!.NoAnimations == false)
                await _gameContainer.Delay!.DelaySeconds(1);

            _gameContainer.PlayerList!.ForEach(thisPlayer =>
            {
                if (thisPlayer.Equals(_gameContainer.SingleInfo) == false)
                {
                    thisPlayer.SuitDesired = EnumSuitList.None; //because somebody overrided it.
                    thisPlayer.BidAmount = 0;
                }
            });
            ResetBids();
            await ContinueBidProcessAsync();
        }

        public void ResetBids()
        {
            _model.BidAmount = -1;
            _gameContainer!.SaveRoot!.TrumpSuit = EnumSuitList.None;
            _model.Bid1!.UnselectAll();
            _model.Suit1!.UnselectAll();
        }

        public void SelectBidAndSuit(int bid, EnumSuitList suit)
        {
            _model.BidAmount = bid;
            _gameContainer!.SaveRoot!.TrumpSuit = suit;
            _model.Bid1!.SelectNumberValue(bid);
            _model.Suit1!.SelectSpecificItem(suit);
        }

        public async Task PopulateBidsAsync()
        {
            ResetBids();
            if (_delegates.LoadBiddingAsync == null)
            {
                throw new BasicBlankException("Nobody is loadin bids.  Rethink");
            }
            await _delegates.LoadBiddingAsync.Invoke();
            if (_gameContainer!.SaveRoot!.HighestBid == 0)
                throw new BasicBlankException("The highest bid cannot be 0");
            _model.Bid1!.LoadNormalNumberRangeValues(_gameContainer.SaveRoot.HighestBid + 1, 10);
        }
    }
}