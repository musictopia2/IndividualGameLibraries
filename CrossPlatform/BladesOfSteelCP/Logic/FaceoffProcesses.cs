using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BladesOfSteelCP.Data;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Linq;
using System.Threading.Tasks;

namespace BladesOfSteelCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class FaceoffProcesses : IFaceoffProcesses
    {
        private readonly BladesOfSteelGameContainer _gameContainer;
        private readonly BladesOfSteelVMData _model;
        private readonly BladesOfSteelScreenDelegates _delegates;

        public FaceoffProcesses(BladesOfSteelGameContainer gameContainer, BladesOfSteelVMData model, BladesOfSteelScreenDelegates delegates)
        {
            _gameContainer = gameContainer;
            _model = model;
            _delegates = delegates;
        }
        async Task IFaceoffProcesses.FaceOffCardAsync(RegularSimpleCard card)
        {
            _gameContainer.SingleInfo!.FaceOff = card;
            if (_gameContainer.SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                _model!.YourFaceOffCard!.AddCard(card);
            else
                _model!.OpponentFaceOffCard!.AddCard(card);
            if (_gameContainer.PlayerList!.Any(items => items.FaceOff == null))
            {
                await _gameContainer.EndTurnAsync!.Invoke();
                return;
            }
            await AnalyzeFaceOffAsync();
        }
        private async Task AnalyzeFaceOffAsync()
        {
            int tempTurn = WhoWonFaceOff();
            if (tempTurn == 0)
            {
                await UIPlatform.ShowMessageAsync("There was a tie during the faceoff.  Therefore; the faceoff is being done again");
                ClearFaceOff();
                await _gameContainer.EndTurnAsync!.Invoke();
                return;
            }
            _gameContainer.WhoTurn = tempTurn;
            _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetWhoPlayer();
            await UIPlatform.ShowMessageAsync($"{_gameContainer.SingleInfo.NickName} has won the face off");
            ClearFaceOff();
            _gameContainer.SaveRoot!.IsFaceOff = false;
            if (_delegates.LoadMainGameAsync == null)
            {
                throw new BasicBlankException("Nobody is handling the load main screen.  Rethink");
            }
            await _delegates.LoadMainGameAsync.Invoke();
            await _gameContainer.StartNewTurnAsync!.Invoke();
        }
        private int WhoWonFaceOff()
        {
            if (_gameContainer.Test!.DoubleCheck == true)
                return _gameContainer.PlayerList.First(items => items.PlayerCategory == EnumPlayerCategory.Self).Id;
            if (_gameContainer.PlayerList.First().FaceOff!.Value > _gameContainer.PlayerList.Last().FaceOff!.Value)
                return 1;
            if (_gameContainer.PlayerList.Last().FaceOff!.Value > _gameContainer.PlayerList.First().FaceOff!.Value)
                return 2;
            return 0;
        }
        private void ClearFaceOff()
        {
            _gameContainer.PlayerList.First().FaceOff = null;
            _gameContainer.PlayerList.Last().FaceOff = null;
        }
    }
}
