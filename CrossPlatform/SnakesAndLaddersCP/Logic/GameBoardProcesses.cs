using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using SnakesAndLaddersCP.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace SnakesAndLaddersCP.Logic
{
    [SingletonGame] //i think.  if i am wrong, rethink
    [AutoReset]
    public class GameBoardProcesses
    {
        private int _currentSpace;
        private readonly Dictionary<int, SpaceInfo> _spaceList;
        private readonly IAsyncDelayer _delay;
        private readonly IEventAggregator _aggregator;
        private readonly BasicData _basicData;
        private readonly TestOptions _test;
        private readonly SnakesAndLaddersVMData _model;


        internal Func<SnakesAndLaddersPlayerItem>? CurrentPlayer { get; set; }

        public GameBoardProcesses(IAsyncDelayer delay,
            EventAggregator aggregator,
            BasicData basicData,
            TestOptions test,
            SnakesAndLaddersVMData model
            )
        {
            //_mainGame = mainGame;
            _delay = delay;
            _aggregator = aggregator;
            _basicData = basicData;
            _test = test;
            _model = model;
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
        //the player should clear everything out period now.

        //public void ClearBoard()
        //{
        //    foreach (var thisPlayer in _mainGame.PlayerList!)
        //        thisPlayer.SpaceNumber = 0;// this means it won't even appear anymore
        //}
        public async Task MakeMoveAsync(int space)
        {
            SpaceInfo thisSpace;
            var loopTo = space;
            int x;
            if (CurrentPlayer == null)
            {
                throw new BasicBlankException("No current player was set.  Rethink");
            }
            for (x = _currentSpace + 1; x <= loopTo; x++)
            {
                CurrentPlayer.Invoke().SpaceNumber = x;
                if (_basicData.IsXamarinForms)
                    _aggregator.RepaintBoard(); //hopefully this simple.
                if (_test.NoAnimations == false)
                {
                    if (_basicData.IsXamarinForms == false)
                        await _delay.DelaySeconds(0.1);
                    else
                        await _delay.DelayMilli(200);
                }
            }
            _currentSpace = space;
            thisSpace = _spaceList[space];
            if (thisSpace.JumpTo != space)
                CurrentPlayer.Invoke().SpaceNumber = thisSpace.JumpTo;
            if (_basicData.IsXamarinForms)
                _aggregator.RepaintBoard();
        }
        public bool IsValidMove(int space)
        {
            int Values;
            if (CurrentPlayer == null)
            {
                throw new BasicBlankException("No player function was set for isvalidmove.  Rethink");
            }
            if (_test.AllowAnyMove == true && CurrentPlayer.Invoke().PlayerCategory == EnumPlayerCategory.Self)
                return true;
            Values = _model.Cup!.ValueOfOnlyDice; // try this
            if ((_currentSpace + Values) == space)
                return true;
            return false;
        }
        public bool HasValidMove()
        {
            if (CurrentPlayer == null)
            {
                throw new BasicBlankException("No current player was set for hasvalidmove.  Rethink");
            }
            if (_test.AllowAnyMove == true && CurrentPlayer.Invoke().PlayerCategory == EnumPlayerCategory.Self)
                return true;
            if ((_currentSpace + _model.Cup!.ValueOfOnlyDice) > 100)
                return false;
            return true;
        }
        public bool IsGameOver()
        {
            if (_currentSpace == 100)
                return true;
            return false;
        }
        public void NewTurn(SnakesAndLaddersMainGameClass mainGame)
        {
            mainGame.SingleInfo = mainGame.PlayerList!.GetWhoPlayer();
            _currentSpace = mainGame.SingleInfo.SpaceNumber;
            _aggregator.Publish(new NewTurnEventModel());
        }
    }
}
