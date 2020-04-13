using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using PaydayCP.Cards;
using PaydayCP.Data;
using System.Threading.Tasks;

namespace PaydayCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class DealBuyChoiceProcesses : IDealBuyChoiceProcesses
    {
        private readonly PaydayGameContainer _gameContainer;
        private readonly PaydayVMData _model;
        private readonly GameBoardProcesses _gameBoard;

        public DealBuyChoiceProcesses(PaydayGameContainer gameContainer, PaydayVMData model, GameBoardProcesses gameBoard)
        {
            _gameContainer = gameContainer;
            _model = model;
            _gameBoard = gameBoard;
        }

        async Task IDealBuyChoiceProcesses.ChoseDealOrBuyAsync()
        {
            await _gameContainer.StartProcessPopUpAsync(_model);
            _gameContainer.SaveRoot.OutCards.Add(_gameContainer.SaveRoot.CurrentMail!);
            _gameContainer.SaveRoot.CurrentMail = new MailCard();
            int nextSpace;
            if (_model.PopUpChosen == "Buy")
            {
                nextSpace = _gameBoard!.NextBuyerSpace();
            }
            else
            {
                nextSpace = _gameBoard!.NextDealSpace();
            }
            if (_gameContainer.BasicData!.MultiPlayer == false)
            {
                await _gameBoard.AnimateMoveAsync(nextSpace);
                return;
            }
            if (_gameContainer.SingleInfo!.CanSendMessage(_gameContainer.BasicData) == true)
            {
                await _gameBoard.AnimateMoveAsync(nextSpace);
                return;
            }
            _gameContainer.Check!.IsEnabled = true; //waiting for message to continue.
        }
    }
}
