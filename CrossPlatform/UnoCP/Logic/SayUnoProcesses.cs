using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks;
using UnoCP.Data;

namespace UnoCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class SayUnoProcesses : ISayUnoProcesses
    {
        private readonly UnoGameContainer _gameContainer;

        public SayUnoProcesses(UnoGameContainer gameContainer)
        {
            _gameContainer = gameContainer;
        }
        async Task ISayUnoProcesses.ProcessUnoAsync(bool saiduno)
        {
            _gameContainer.AlreadyUno = true;
            if (_gameContainer.SingleInfo!.CanSendMessage(_gameContainer.BasicData!) == true)
                await _gameContainer.Network!.SendAllAsync("uno", saiduno);
            if (saiduno == false)
            {
                if (_gameContainer.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                    await UIPlatform.ShowMessageAsync("You had one card left.  However, you did not say uno.  Therefore, you have to draw 2 cards");
                _gameContainer.LeftToDraw = 2;
                await _gameContainer.DrawAsync!.Invoke();
                return;
            }
            if (_gameContainer.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
                await UIPlatform.ShowMessageAsync($"Uno From {_gameContainer.SingleInfo.NickName}");
            if (_gameContainer.DoFinishAsync == null)
            {
                throw new BasicBlankException("Nobody is handling the dofinishasync.  Rethink");
            }
            if (_gameContainer.CloseSaidUnoAsync == null)
            {
                throw new BasicBlankException("Nobody is closing uno screen.  Rethink");
            }
            await _gameContainer.CloseSaidUnoAsync.Invoke();
            await _gameContainer.DoFinishAsync.Invoke();
        }
    }
}
