using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using FluxxCP.Cards;
using FluxxCP.Containers;
using FluxxCP.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FluxxCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class RotateTradeHandProcesses : IRotateTradeHandProcesses
    {
        private readonly FluxxGameContainer _gameContainer;
        private readonly IAnalyzeProcesses _processes;
        private readonly ActionContainer _actionContainer;

        public RotateTradeHandProcesses(FluxxGameContainer gameContainer, IAnalyzeProcesses processes, ActionContainer actionContainer)
        {
            _gameContainer = gameContainer;
            _processes = processes;
            _actionContainer = actionContainer;
        }
        async Task IRotateTradeHandProcesses.RotateHandAsync(EnumDirection direction)
        {
            _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetWhoPlayer();
            if (_gameContainer.SingleInfo.CanSendMessage(_gameContainer.BasicData!))
                await _gameContainer.Network!.SendAllAsync("rotatehands", direction);
            DeckRegularDict<FluxxCardInformation> oldList;
            DeckRegularDict<FluxxCardInformation> newList;
            int oldTurn = _gameContainer.WhoTurn;
            bool oldReverse = _gameContainer.SaveRoot!.PlayOrder.IsReversed;
            FluxxPlayerItem oldPlayer;
            _gameContainer.SaveRoot.PlayOrder.IsReversed = direction == EnumDirection.Right;
            oldList = _gameContainer.SingleInfo.MainHandList.ToRegularDeckDict();
            int count = _gameContainer.PlayerList.Count();
            await count.TimesAsync(async x =>
            {
                oldPlayer = _gameContainer.SingleInfo;
                _gameContainer.WhoTurn = await _gameContainer.PlayerList.CalculateWhoTurnAsync();
                _gameContainer.SingleInfo = _gameContainer.PlayerList.GetWhoPlayer();
                if (oldPlayer.NickName == _gameContainer.SingleInfo.NickName)
                    throw new BasicBlankException("Its the same player.  A problem");
                newList = _gameContainer.SingleInfo.MainHandList.ToRegularDeckDict();
                _gameContainer.SingleInfo.MainHandList.ReplaceRange(oldList);
                if (newList.Count == _gameContainer.SingleInfo.MainHandList.Count && _gameContainer.SingleInfo.MainHandList.Count > 0 && newList.First().Deck == _gameContainer.SingleInfo.MainHandList.First().Deck)
                    throw new BasicBlankException("Did not rotate the cards");
                oldList = newList;
            });
            _gameContainer.SaveRoot.PlayOrder.IsReversed = oldReverse;
            _gameContainer.WhoTurn = oldTurn;
            _gameContainer.SingleInfo = _gameContainer.PlayerList.GetSelf();
            _gameContainer.SortCards!();
            _gameContainer.SingleInfo = _gameContainer.PlayerList.GetWhoPlayer();
            _gameContainer!.CurrentAction = null;
            await _processes.AnalyzeQueAsync();
        }

        async Task IRotateTradeHandProcesses.TradeHandAsync(int selectedIndex)
        {
            if (_gameContainer.SingleInfo!.CanSendMessage(_gameContainer.BasicData!))
                await _gameContainer.Network!.SendAllAsync("tradehands", selectedIndex);
            int player = _actionContainer.GetPlayerIndex(selectedIndex);
            if (player == _gameContainer.WhoTurn)
                throw new BasicBlankException("Cannot pick yourself");
            _actionContainer.SetUpGoals();
            _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetWhoPlayer();
            var oldList = _gameContainer.SingleInfo.MainHandList.ToRegularDeckDict();
            var thisPlayer = _gameContainer.PlayerList[player];
            var newList = thisPlayer.MainHandList.ToRegularDeckDict();
            _gameContainer.SingleInfo.MainHandList.ReplaceRange(newList);
            thisPlayer.MainHandList.ReplaceRange(oldList);
            int myID = _gameContainer.PlayerList.GetSelf().Id;
            if (player == myID || _gameContainer.WhoTurn == myID)
            {
                _gameContainer.SingleInfo = _gameContainer.PlayerList.GetSelf();
                _gameContainer.SortCards!();
            }
            _gameContainer.SingleInfo = _gameContainer.PlayerList.GetWhoPlayer();
            _gameContainer!.CurrentAction = null;
            await _processes.AnalyzeQueAsync();
        }
    }
}
