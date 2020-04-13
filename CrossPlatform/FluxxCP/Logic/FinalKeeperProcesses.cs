using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using FluxxCP.Cards;
using FluxxCP.Containers;
using FluxxCP.Data;
using System.Threading.Tasks;

namespace FluxxCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class FinalKeeperProcesses : IFinalKeeperProcesses
    {
        private readonly FluxxGameContainer _gameContainer;
        private readonly IAnalyzeProcesses _analyzeProcesses;
        private readonly FluxxDelegates _delegates;
        private readonly IDiscardProcesses _discardProcesses;

        public FinalKeeperProcesses(FluxxGameContainer gameContainer,
            IAnalyzeProcesses analyzeProcesses,
            FluxxDelegates delegates,
            IDiscardProcesses discardProcesses
            )
        {
            _gameContainer = gameContainer;
            _analyzeProcesses = analyzeProcesses;
            _delegates = delegates;
            _discardProcesses = discardProcesses;
        }
        async Task IFinalKeeperProcesses.ProcessExchangeKeepersAsync(KeeperPlayer keeperFrom, KeeperPlayer keeperTo)
        {
            if (_gameContainer.SingleInfo!.CanSendMessage(_gameContainer.BasicData!))
            {
                var thisList = new CustomBasicList<KeeperPlayer> { keeperFrom, keeperTo };
                await _gameContainer.Network!.SendAllAsync("exchangekeepers", thisList);
            }
            if (_delegates.LoadMainScreenAsync == null)
            {
                throw new BasicBlankException("Nobody is loading the main screen when processing exchange keepers.  Rethink");
            }
            await _delegates.LoadMainScreenAsync.Invoke();
            KeeperCard fromCard;
            KeeperCard toCard;
            FluxxPlayerItem fromPlayer;
            FluxxPlayerItem toPlayer;
            fromPlayer = _gameContainer.PlayerList![keeperFrom.Player];
            toPlayer = _gameContainer.PlayerList[keeperTo.Player];
            fromCard = fromPlayer.KeeperList.RemoveObjectByDeck(keeperFrom.Card);
            toCard = toPlayer.KeeperList.RemoveObjectByDeck(keeperTo.Card);
            toCard.IsSelected = false;
            fromCard.IsSelected = false;
            toCard.Drew = false;
            fromCard.Drew = false;
            fromPlayer.KeeperList.Add(toCard);
            toPlayer.KeeperList.Add(fromCard);
            _gameContainer!.CurrentAction = null;
            await _analyzeProcesses.AnalyzeQueAsync();
        }

        async Task IFinalKeeperProcesses.ProcessTrashStealKeeperAsync(KeeperPlayer thisKeeper, bool isTrashed)
        {
            _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetWhoPlayer();
            if (_gameContainer.SingleInfo.CanSendMessage(_gameContainer.BasicData!))
            {
                string status;
                if (isTrashed)
                    status = "trashkeeper";
                else
                    status = "stealkeeper";
                await _gameContainer.Network!.SendAllAsync(status, thisKeeper);
            }
            if (_delegates.LoadMainScreenAsync == null)
            {
                throw new BasicBlankException("Nobody is loading the main screen when trashing or stealing keeper.  Rethink");
            }
            await _delegates.LoadMainScreenAsync.Invoke();
            _gameContainer.SingleInfo = _gameContainer.PlayerList[thisKeeper.Player];
            if (isTrashed)
            {
                await _discardProcesses.DiscardKeeperAsync(thisKeeper.Card);
                _gameContainer!.CurrentAction = null;
                _gameContainer.SingleInfo = _gameContainer.PlayerList.GetWhoPlayer();
                await _analyzeProcesses.AnalyzeQueAsync();
                return;
            }
            var thisCard = _gameContainer.SingleInfo.KeeperList.RemoveObjectByDeck(thisKeeper.Card);
            _gameContainer.SingleInfo = _gameContainer.PlayerList.GetWhoPlayer();
            thisCard.IsSelected = false;
            thisCard.Drew = false;
            _gameContainer.SingleInfo.KeeperList.Add(thisCard);
            _gameContainer!.CurrentAction = null;
            await _analyzeProcesses.AnalyzeQueAsync();
        }
    }
}