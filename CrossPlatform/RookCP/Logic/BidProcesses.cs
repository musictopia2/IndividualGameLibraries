using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using CommonBasicStandardLibraries.Exceptions;
using RookCP.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RookCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class BidProcesses : IBidProcesses
    {
        private readonly RookVMData _model;
        private readonly RookGameContainer _gameContainer;

        public BidProcesses(RookVMData model, RookGameContainer gameContainer)
        {
            _model = model;
            _gameContainer = gameContainer;
        }
        public async Task BeginBiddingAsync()
        {
            _gameContainer.ShowedOnce = true;
            if (_gameContainer.SaveRoot!.HighestBidder == 0)
                throw new BasicBlankException("The highest bidder cannot be 0");
            PopulateBids();
            _model.CanPass = await CanPassAsync();
            await _gameContainer.StartNewTurnAsync!.Invoke();
        }
        private void PopulateBids()
        {
            _model.BidChosen = -1;
            if (_gameContainer!.SaveRoot!.HighestBidder == 1)
                throw new BasicBlankException("The highest bid cannot be 0");
            _model.Bid1!.LoadNormalNumberRangeValues(_gameContainer.SaveRoot.HighestBidder + 5, 100, 5);
        }
        public async Task<bool> CanPassAsync()
        {
            var temps = await _gameContainer.PlayerList!.CalculateWhoTurnAsync();
            _gameContainer.SingleInfo = _gameContainer.PlayerList.GetWhoPlayer();
            if (temps == _gameContainer.WhoStarts && _gameContainer.SaveRoot!.WonSoFar == 0)
                return false;
            return true;
        }

        public async Task PassBidAsync()
        {
            if (_gameContainer.SingleInfo!.CanSendMessage(_gameContainer.BasicData!) == true)
                await _gameContainer.Network!.SendAllAsync("pass");
            _gameContainer.SingleInfo!.Pass = true;
            await ContinueBidProcessAsync();
        }

        public async Task ProcessBidAsync()
        {
            if (_model!.BidChosen == -1)
                throw new BasicBlankException("The bid amount cannot be -1");
            if (_gameContainer.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
                _model.Bid1!.SelectNumberValue(_model.BidChosen);
            if (_gameContainer.SingleInfo.CanSendMessage(_gameContainer.BasicData!))
                await _gameContainer.Network!.SendAllAsync("bid", _model.BidChosen);
            _gameContainer.SaveRoot!.WonSoFar = _gameContainer.WhoTurn;
            _gameContainer.SingleInfo.BidAmount = _model.BidChosen;
            _gameContainer.SaveRoot.HighestBidder = _model.BidChosen;
            if (_gameContainer.Test!.NoAnimations == false)
                await _gameContainer.Delay!.DelaySeconds(1);
            _gameContainer.PlayerList!.ForEach(thisPlayer =>
            {
                if (thisPlayer.Id != _gameContainer.WhoTurn)
                    thisPlayer.BidAmount = 0; //because somebody else won it.
            });
            ResetBids();
            await ContinueBidProcessAsync();
        }

        private async Task ContinueBidProcessAsync()
        {
            if (_gameContainer.SaveRoot!.HighestBidder == 100)
            {
                await EndBiddingAsync();
                return;
            }
            if (_gameContainer.SaveRoot.WonSoFar > 0)
            {
                if (_gameContainer.PlayerList.Count(items => items.Pass == true) == 1)
                {
                    await EndBiddingAsync();
                    return;
                }
            }
            int olds = _gameContainer.WhoTurn;
            do
            {
                _gameContainer.WhoTurn = await _gameContainer.PlayerList!.CalculateWhoTurnAsync();
                _gameContainer.SingleInfo = _gameContainer.PlayerList.GetWhoPlayer();
                if (_gameContainer.SingleInfo.Pass == false)
                    break;
            } while (true);
            if (_gameContainer.WhoTurn == olds)
                throw new BasicBlankException("Cannot be the same player again");
            _gameContainer.SingleInfo = _gameContainer.PlayerList.GetWhoPlayer();
            _model!.CanPass = await CanPassAsync();
            await BeginBiddingAsync();
        }

        private async Task EndBiddingAsync()
        {
            _model!.CanPass = false;
            _gameContainer.WhoTurn = _gameContainer.SaveRoot!.WonSoFar;
            _model.TrickArea1!.NewRound(); //i think.
            _gameContainer.SaveRoot.GameStatus = EnumStatusList.ChooseTrump;
            await _gameContainer.StartNewTurnAsync!.Invoke();
        }
        private void ResetBids()
        {
            _model.BidChosen = -1;
            _model.Bid1!.UnselectAll();
        }
    }
}