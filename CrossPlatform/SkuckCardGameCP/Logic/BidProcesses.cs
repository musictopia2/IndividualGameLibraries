using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.Exceptions;
using SkuckCardGameCP.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SkuckCardGameCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class BidProcesses : IBidProcesses
    {
        private readonly SkuckCardGameVMData _model;
        private readonly SkuckCardGameGameContainer _gameContainer;

        public BidProcesses(SkuckCardGameVMData model, SkuckCardGameGameContainer gameContainer)
        {
            _model = model;
            _gameContainer = gameContainer;
        }
        private bool HasException()
        {
            if (_gameContainer.Test!.DoubleCheck == true)
                return true;
            int diffs = _gameContainer.PlayerList.First().StrengthHand - _gameContainer.PlayerList.Last().StrengthHand;
            if (Math.Abs(diffs) >= 12)
                return true;
            return false;
        }
        async Task IBidProcesses.ProcessBidAmountAsync(int id)
        {
            if (_model!.BidAmount == -1)
                throw new BasicBlankException("Did not choose a bid amount");
            var thisPlayer = _gameContainer.PlayerList![id];
            thisPlayer.BidAmount = _model.BidAmount;
            if (thisPlayer.PlayerCategory == EnumPlayerCategory.Self)
            {
                thisPlayer.BidVisible = true;
                _gameContainer.SaveRoot!.WhatStatus = EnumStatusList.WaitForOtherPlayers;
                if (_gameContainer.BasicData!.MultiPlayer == false)
                {
                    await _gameContainer.ComputerTurnAsync!.Invoke();
                    return;
                }
                _gameContainer!.Command!.ManuelFinish = true; //because you can't change your mind.
                _gameContainer.Check!.IsEnabled = true; //wait for other players.
                return;
            }
            if (HasException() == true)
            {
                _gameContainer!.Command!.ManuelFinish = true; //just in case.
                _gameContainer.SaveRoot!.WhatStatus = EnumStatusList.ChoosePlay;
                _gameContainer.Command.IsExecuting = true; //make sure its executing no matter what as well.
                if (_gameContainer.PlayerList.First().StrengthHand > _gameContainer.PlayerList.Last().StrengthHand)
                    _gameContainer.WhoTurn = 2;
                else
                    _gameContainer.WhoTurn = 1; //to double check.
                _gameContainer.SingleInfo = _gameContainer.PlayerList.GetWhoPlayer();
                await _gameContainer.StartNewTurnAsync!.Invoke(); //hopefully this works.
                return;
            }
            _gameContainer.SaveRoot!.WhatStatus = EnumStatusList.NormalPlay;
            if (_gameContainer.WhoTurn == 1)
                _gameContainer.WhoTurn = 2;
            else
                _gameContainer.WhoTurn = 1;
            _gameContainer.SingleInfo = _gameContainer.PlayerList.GetWhoPlayer();
            await _gameContainer.StartNewTrickAsync!.Invoke();
        }
    }
}
