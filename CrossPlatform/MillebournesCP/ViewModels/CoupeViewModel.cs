using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using MillebournesCP.Data;
using MillebournesCP.Logic;
using System.Threading.Tasks;

namespace MillebournesCP.ViewModels
{
    [InstanceGame]
    public class CoupeViewModel : Screen, IBlankGameVM
    {
        private readonly MillebournesVMData _model;
        private readonly MillebournesGameContainer _gameContainer;
        private readonly MillebournesMainGameClass _mainGame;


        public CoupeViewModel(CommandContainer commandContainer, MillebournesVMData model, MillebournesGameContainer gameContainer, MillebournesMainGameClass mainGame)
        {
            CommandContainer = commandContainer;
            _model = model;
            _gameContainer = gameContainer;
            _mainGame = mainGame;
            _model.Stops.TimeUp += Stops_TimeUp;
        }
        protected override Task TryCloseAsync()
        {
            _model.Stops.TimeUp -= Stops_TimeUp;
            return base.TryCloseAsync();
        }
        public CommandContainer CommandContainer { get; set; }
        private async Task CloseCoupeAsync()
        {
            if (_gameContainer.CloseCoupeAsync == null)
            {
                throw new BasicBlankException("Nobody is handling closing coupe.  Rethink");
            }
            await _gameContainer.CloseCoupeAsync.Invoke();

        }
        private async void Stops_TimeUp()
        {
            await CloseCoupeAsync();
            CommandContainer!.IsExecuting = true;
            CommandContainer.ManuelFinish = true;
            if (_gameContainer.BasicData!.MultiPlayer == false)
            {
                await _mainGame.EndPartAsync(false);
                return;
            }
            MillebournesPlayerItem thisPlayer = _gameContainer!.PlayerList!.GetSelf();
            await _gameContainer.Network!.SendAllAsync("timeup", thisPlayer.Id);
            await _mainGame.EndCoupeAsync(thisPlayer.Id); //iffy.
        }
        [Command(EnumCommandCategory.Plain)]
        public async Task CoupeAsync()
        {
            _model.Stops!.PauseTimer();
            await CloseCoupeAsync();
            _gameContainer!.SingleInfo = _gameContainer.PlayerList!.GetSelf();
            _gameContainer.CurrentCP = _gameContainer.FindTeam(_gameContainer.SingleInfo.Team);
            bool rets = _mainGame.HasCoupe(out int newDeck);
            SendPlay thisSend;
            if (rets == false)
            {
                await UIPlatform.ShowMessageAsync("No Coup Foures Here");
                if (_gameContainer.BasicData!.MultiPlayer == true)
                {
                    thisSend = new SendPlay();
                    thisSend.Player = _gameContainer.SingleInfo.Id;
                    thisSend.Team = _gameContainer.SingleInfo.Team;
                    await _gameContainer.Network!.SendAllAsync("nocoupe", thisSend); //looks like multiplayer has a bug with no coupe.  has to be fixed.
                }
                _gameContainer.CurrentCP.IncreaseWrongs();
                _mainGame.UpdateGrid(_gameContainer.SingleInfo.Team);
                if (_mainGame.BasicData.MultiPlayer == false)
                {
                    await _mainGame.EndPartAsync(false);
                    return;
                }
                await _mainGame.EndCoupeAsync(_mainGame.SingleInfo!.Id);
                return;
            }
            if (_mainGame.BasicData!.MultiPlayer == false)
            {
                await _mainGame.ProcessCoupeAsync(newDeck, _mainGame.SingleInfo!.Id);
                return;
            }
            thisSend = new SendPlay();
            thisSend.Player = _mainGame.SingleInfo!.Id;
            thisSend.Deck = newDeck; //i guess that team may not have mattered.
            await _mainGame.Network!.SendAllAsync("hascoupe", thisSend);
            _gameContainer.CurrentCoupe.Player = _gameContainer.SingleInfo.Id;
            _gameContainer.CurrentCoupe.Card = newDeck;
            await _mainGame.EndCoupeAsync(_mainGame.SingleInfo.Id);
        }
    }
}