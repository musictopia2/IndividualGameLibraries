using BasicGameFrameworkLibrary.Attributes;
using CommonBasicStandardLibraries.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XactikaCP.Data;

namespace XactikaCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class ModeProcesses : IModeProcesses
    {
        private readonly XactikaVMData _model;
        private readonly XactikaDelegates _delegates;
        private readonly XactikaGameContainer _gameContainer;
        private readonly IBidProcesses _bidProcesses;
        private readonly IShapeProcesses _shapeProcesses;

        public ModeProcesses(XactikaVMData model, 
            XactikaDelegates delegates, 
            XactikaGameContainer gameContainer,
            IBidProcesses bidProcesses,
            IShapeProcesses shapeProcesses
            )
        {
            _model = model;
            _delegates = delegates;
            _gameContainer = gameContainer;
            _bidProcesses = bidProcesses;
            _shapeProcesses = shapeProcesses;
        }
        public async Task EnableOptionsAsync()
        {
            await _gameContainer.SaveStateAsync!.Invoke();

            if (_delegates.LoadModeAsync == null)
            {
                throw new BasicBlankException("Nobody is loading the mode.  Rethink");
            }
            await _delegates.LoadModeAsync();

            if (_gameContainer.BasicData!.MultiPlayer)
            {
                if (_gameContainer.BasicData.Client)
                {
                    _gameContainer.Check!.IsEnabled = true; //has to wait for host to choose game option.
                    return;
                }
            }
            if (_gameContainer.ShowHumanCanPlayAsync == null)
            {
                throw new BasicBlankException("Nobody is showing human can play.  Rethink");
            }
            await _gameContainer.ShowHumanCanPlayAsync!.Invoke(); //hopefully this simple.
        }

        private async Task LoadBidAsync()
        {
            if (_gameContainer.LoadBiddingAsync == null)
            {
                throw new BasicBlankException("Nobody is loading the bidding screen.  Rethink");
            }
            await _gameContainer.LoadBiddingAsync.Invoke();
        }
        public async Task ProcessGameOptionChosenAsync(EnumGameMode optionChosen, bool doShow)
        {
            if (doShow)
            {
                _model!.ModeChoose1!.SelectSpecificItem((int)optionChosen);
                if (_gameContainer.Test!.NoAnimations == false)
                    await _gameContainer.Delay!.DelaySeconds(.5);
                _gameContainer.SaveRoot!.GameMode = optionChosen;
            }
            else if (optionChosen != _gameContainer.SaveRoot!.GameMode)
            {
                throw new BasicBlankException("Had to show");
            }
            if (_delegates.CloseModeAsync == null)
            {
                throw new BasicBlankException("Nobody is closing the game mode.  Rethink");
            }
            await _delegates.CloseModeAsync.Invoke();
            if (_gameContainer.SaveRoot.GameStatus != EnumStatusList.ChooseGameType)
            {
                if (_gameContainer.SaveRoot.GameStatus == EnumStatusList.Bidding)
                {
                    _model.TrickArea1!.Visible = false; //just in case.
                    await LoadBidAsync();
                    await _bidProcesses.BeginBiddingAsync();
                    return;
                }
                if (_gameContainer.SaveRoot.GameStatus == EnumStatusList.CallShape)
                {
                    _model!.ShapeChoose1!.Visible = true;//just in case.
                    await _shapeProcesses.FirstCallShapeAsync();
                    return;
                }
                if (_gameContainer.SaveRoot.ClearTricks)
                {
                    _gameContainer.SaveRoot.ClearTricks = false;
                    await _bidProcesses.EndBidAsync();
                    return; //try this way.
                }
                if (_gameContainer.SaveRoot.ShapeChosen != EnumShapes.None)
                    _model!.ShapeChoose1!.ChoosePiece(_gameContainer.SaveRoot.ShapeChosen); //hopefully this simple.
                _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetWhoPlayer();
                if (_gameContainer.ShowTurn == null)
                {
                    throw new BasicBlankException("Nobody is showing turn.  Rethink");
                }
                _gameContainer.ShowTurn();
                await _gameContainer.ContinueTurnAsync!.Invoke();
                return;
            }
            if (optionChosen == EnumGameMode.ToBid)
            {
                _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetWhoPlayer();
                _gameContainer.SaveRoot.GameStatus = EnumStatusList.Bidding;
                await LoadBidAsync();
                await _bidProcesses.PopulateBidAmountsAsync();
                return;
            }
            await _bidProcesses.EndBidAsync();
        }
    }
}
