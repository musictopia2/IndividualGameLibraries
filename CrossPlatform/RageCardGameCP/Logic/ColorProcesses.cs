using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using CommonBasicStandardLibraries.Exceptions;
using RageCardGameCP.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RageCardGameCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class ColorProcesses : IColorProcesses
    {
        private readonly RageCardGameGameContainer _gameContainer;
        private readonly RageCardGameVMData _model;
        private readonly RageDelgates _delgates;

        public ColorProcesses(RageCardGameGameContainer gameContainer, RageCardGameVMData model, RageDelgates delgates)
        {
            _gameContainer = gameContainer;
            _model = model;
            _delgates = delgates;
            _gameContainer.ColorChosenAsync = ColorChosenAsync;
            _gameContainer.ShowLeadColor = ShowLeadColor;
            _gameContainer.ChooseColorAsync = ChooseColorAsync;
        }

        public Task ChooseColorAsync()
        {
            _gameContainer.SaveRoot.Status = EnumStatus.ChooseColor;
            return _gameContainer.ContinueTurnAsync!.Invoke();
        }

        public async Task ColorChosenAsync()
        {
            if (_gameContainer.SingleInfo!.CanSendMessage(_gameContainer.BasicData!) == true)
                await _gameContainer.Network!.SendAllAsync("color", _model!.ColorChosen);
            if (_gameContainer.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
            {
                _model!.Color1!.ChooseItem(_model.ColorChosen);
                if (_gameContainer.Test!.NoAnimations == false)
                    await _gameContainer.Delay!.DelaySeconds(.5);
            }
            if (_delgates.CloseColorScreenAsync == null)
            {
                throw new BasicBlankException("Nobody is closing colors.  Rethink");
            }
            await _delgates.CloseColorScreenAsync.Invoke();
            await _model.TrickArea1!.ColorChosenAsync(_model.ColorChosen); //hopefully this is it.
        }

        public async Task LoadColorListsAsync()
        {
            var thisCard = _model.TrickArea1.OrderList.Last();
            if (thisCard.SpecialType != EnumSpecialType.Wild && thisCard.SpecialType != EnumSpecialType.Change)
                throw new BasicBlankException("Only change or wilds can change colors");
            if (_gameContainer.SaveRoot!.Status != EnumStatus.ChooseColor)
                throw new BasicBlankException("Must be choosing color");
            if (thisCard.SpecialType == EnumSpecialType.Change)
            {
                _model.Color1.LoadEntireListExcludeOne(_gameContainer.SaveRoot.TrumpSuit);
            }
            else
                _model.Color1.LoadEntireList();
            _model.Color1.UnselectAll();
            //Visible = false; //this is now not visible because you are loading the lists.
            _model.ColorChosen = EnumColor.None;
            if (_delgates.LoadColorScreenAsync == null)
            {
                throw new BasicBlankException("Nobody is loading colors.  Rethink");
            }
            await _delgates.LoadColorScreenAsync.Invoke();
        }

        //the order list can no longer be protected because others may need to access to stop the overflows.
        public void ShowLeadColor()
        {
            var tempCard = _model.TrickArea1.OrderList.FirstOrDefault(items => items.Color != EnumColor.None);
            if (tempCard == null)
                _model.Lead = "None";
            else
                _model.Lead = tempCard.Color.ToString();
        }
    }
}
