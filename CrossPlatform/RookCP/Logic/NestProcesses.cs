using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using CommonBasicStandardLibraries.Exceptions;
using RookCP.Cards;
using RookCP.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RookCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class NestProcesses : INestProcesses
    {
        private readonly RookVMData _model;
        private readonly RookGameContainer _gameContainer;

        public NestProcesses(RookVMData model,
            RookGameContainer gameContainer
            )
        {
            _model = model;
            _gameContainer = gameContainer;
        }
        async Task INestProcesses.ProcessNestAsync(DeckRegularDict<RookCardInformation> list)
        {
            if (list.Count != 5)
                throw new BasicBlankException("The nest must contain exactly 5 cards");
            if (_gameContainer.SingleInfo!.CanSendMessage(_gameContainer.BasicData!))
                await _gameContainer.Network!.SendAllAsync("nestlist", list);
            _gameContainer.SaveRoot!.NestList.ReplaceRange(list);
            var newCol = _model!.Deck1!.DrawSeveralCards(3);
            _gameContainer.SaveRoot.NestList.AddRange(newCol);
            _gameContainer.SingleInfo!.MainHandList.RemoveSelectedItems(list); //hopefully this works.
            _gameContainer.SingleInfo.MainHandList.UnhighlightObjects();
            _gameContainer.StartingStatus!.Invoke(); //hopefully this simple.
            _gameContainer.SaveRoot.GameStatus = EnumStatusList.Normal;
            _model.PlayerHand1!.AutoSelect = HandObservable<RookCardInformation>.EnumAutoType.SelectOneOnly;
            if (_gameContainer.PlayerList.Count() == 3)
            {
                if (_gameContainer.SaveRoot.WonSoFar == 1)
                    _gameContainer.SingleInfo = _gameContainer.PlayerList![3];
                else if (_gameContainer.SaveRoot.WonSoFar == 2)
                    _gameContainer.SingleInfo = _gameContainer.PlayerList![1];
                else
                    _gameContainer.SingleInfo = _gameContainer.PlayerList![2];
                _gameContainer.SingleInfo.IsDummy = true;
                _gameContainer.WhoTurn = _gameContainer.SingleInfo.Id;
            }
            else
            {
                if (_gameContainer.SaveRoot.WonSoFar == 1)
                    _gameContainer.WhoTurn = 2;
                else
                    _gameContainer.WhoTurn = 1;
            }
            _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetWhoPlayer();
            _gameContainer.SaveRoot.DummyPlay = true;
            await _gameContainer.StartNewTrickAsync!.Invoke();
        }
    }
}