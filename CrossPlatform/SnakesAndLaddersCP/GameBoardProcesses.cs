using BasicGameFramework.Attributes;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.Messenging;
using System.Collections.Generic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace SnakesAndLaddersCP
{
    [SingletonGame] //i think.  if i am wrong, rethink
    public class GameBoardProcesses
    {
        private int _currentSpace;
        private readonly Dictionary<int, SpaceInfo> _spaceList;
        private readonly SnakesAndLaddersMainGameClass _mainGame;
        private readonly IAsyncDelayer _delay;
        private readonly EventAggregator _thisE;
        private readonly SnakesAndLaddersViewModel _thisMod;
        public GameBoardProcesses(SnakesAndLaddersMainGameClass mainGame, IAsyncDelayer delay,
            EventAggregator thisE, SnakesAndLaddersViewModel thisMod)
        {
            _mainGame = mainGame;
            _delay = delay;
            _thisE = thisE;
            _thisMod = thisMod;
            _spaceList = new Dictionary<int, SpaceInfo>();
            int x;
            for (x = 1; x <= 100; x++)
            {
                SpaceInfo thisSpace = new SpaceInfo();
                thisSpace.JumpTo = x;
                if (x == 4)
                    thisSpace.JumpTo = 39;
                else if (x == 30)
                    thisSpace.JumpTo = 12;
                else if (x == 33)
                    thisSpace.JumpTo = 52;
                else if (x == 36)
                    thisSpace.JumpTo = 8;
                else if (x == 59)
                    thisSpace.JumpTo = 63;
                else if (x == 70)
                    thisSpace.JumpTo = 50;
                else if (x == 26)
                    thisSpace.JumpTo = 75;
                else if (x == 73)
                    thisSpace.JumpTo = 93;
                else if (x == 86)
                    thisSpace.JumpTo = 57;
                else if (thisSpace.JumpTo == 99)
                    thisSpace.JumpTo = 42;
                _spaceList.Add(x, thisSpace);
            }
        }
        public void ClearBoard()
        {
            foreach (var thisPlayer in _mainGame.PlayerList!)
                thisPlayer.SpaceNumber = 0;// this means it won't even appear anymore
        }
        public async Task MakeMoveAsync(int space)
        {
            SpaceInfo thisSpace;
            var loopTo = space;
            int x;
            for (x = _currentSpace + 1; x <= loopTo; x++)
            {
                _mainGame.SingleInfo!.SpaceNumber = x;
                if (_mainGame.ThisData!.IsXamarinForms)
                    _thisE.RepaintBoard(); //hopefully this simple.
                if (_mainGame.ThisTest!.NoAnimations == false)
                {
                    if (_mainGame.ThisData.IsXamarinForms == false)
                        await _delay.DelaySeconds(0.1);
                    else
                        await _delay.DelayMilli(200);
                }
            }
            _currentSpace = space;
            thisSpace = _spaceList[space];
            if (thisSpace.JumpTo != space)
                _mainGame.SingleInfo!.SpaceNumber = thisSpace.JumpTo;
            if (_mainGame!.ThisData!.IsXamarinForms)
                _thisE.RepaintBoard();
        }
        public bool IsValidMove(int space)
        {
            int Values;
            if (_mainGame.ThisTest!.AllowAnyMove == true && _mainGame.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                return true;
            Values = _thisMod.ThisCup!.ValueOfOnlyDice; // try this
            if ((_currentSpace + Values) == space)
                return true;
            return false;
        }
        public bool HasValidMove()
        {
            if (_mainGame.ThisTest!.AllowAnyMove == true && _mainGame.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                return true;
            if ((_currentSpace + _thisMod.ThisCup!.ValueOfOnlyDice) > 100)
                return false;
            return true;
        }
        public bool IsGameOver()
        {
            if (_currentSpace == 100)
                return true;
            return false;
        }
        public void NewTurn()
        {
            _mainGame.SingleInfo = _mainGame.PlayerList!.GetWhoPlayer();
            _currentSpace = _mainGame.SingleInfo.SpaceNumber;
            _thisE.Publish(new NewTurnEventModel());
        }
    }
}