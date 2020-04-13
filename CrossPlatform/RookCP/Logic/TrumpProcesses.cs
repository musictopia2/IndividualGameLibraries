using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.ColorCards;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using RookCP.Cards;
using RookCP.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RookCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class TrumpProcesses : ITrumpProcesses
    {
        private readonly RookVMData _model;
        private readonly RookGameContainer _gameContainer;

        public TrumpProcesses(RookVMData model, RookGameContainer gameContainer)
        {
            _model = model;
            _gameContainer = gameContainer;
        }
        public async Task ProcessTrumpAsync()
        {
            _gameContainer.SaveRoot!.TrumpSuit = _model!.ColorChosen;
            if (_gameContainer.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
                _model.Color1!.SelectSpecificItem(_model.ColorChosen);
            if (_gameContainer.SingleInfo.CanSendMessage(_gameContainer.BasicData!))
                await _gameContainer.Network!.SendAllAsync("colorselected", _model.ColorChosen);
            if (_gameContainer.Test!.NoAnimations == false)
                await _gameContainer.Delay!.DelaySeconds(1);
            ResetTrumps();
            _gameContainer.SaveRoot.GameStatus = EnumStatusList.SelectNest;
            _gameContainer.SingleInfo.MainHandList.AddRange(_gameContainer.SaveRoot.NestList);
            if (_gameContainer.SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
            {
                _model.PlayerHand1!.AutoSelect = HandObservable<RookCardInformation>.EnumAutoType.SelectAsMany;
                _gameContainer.SortCards!.Invoke(); //hopefully this simple.
            }
            if (_gameContainer.PlayerList.Count() == 2)
            {
                _model.Dummy1!.MakeAllKnown();
                _model.Dummy1.HandList.Sort(); //hopefully this simple
            }
            _model.Status = "Choose the 5 cards to get rid of";
            await _gameContainer.StartNewTurnAsync!.Invoke();
        }

        public void ResetTrumps()
        {
            _model.ColorChosen = EnumColorTypes.None; //i think
            _model.Color1!.UnselectAll();
        }
    }
}