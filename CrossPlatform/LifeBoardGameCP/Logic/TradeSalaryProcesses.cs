using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using BasicGameFrameworkLibrary.NetworkingClasses.Extensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using LifeBoardGameCP.Cards;
using LifeBoardGameCP.Data;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace LifeBoardGameCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class TradeSalaryProcesses : ITradeSalaryProcesses
    {
        private readonly LifeBoardGameVMData _model;
        private readonly LifeBoardGameGameContainer _gameContainer;

        public TradeSalaryProcesses(LifeBoardGameVMData model, LifeBoardGameGameContainer gameContainer)
        {
            _model = model;
            _gameContainer = gameContainer;
        }
        private string ComputerTradeWith
        {
            get
            {
                decimal maxSalary = _gameContainer.SingleInfo!.Salary;
                var tempList = _model.PlayerPicker!.TextList.Select(items => items.DisplayText).ToCustomBasicList(); //hopefully this simple.
                var nextList = tempList.Select(items => _gameContainer.PlayerList![items]).ToCustomBasicList();
                var maxOne = nextList.OrderByDescending(items => items.Salary).First();
                if (maxOne.Salary < maxSalary)
                    return "";
                return maxOne.NickName;
            }
        }
        public async Task ComputerTradeAsync()
        {
            string name = ComputerTradeWith;
            if (name == "")
            {
                if (_gameContainer.CanSendMessage())
                {
                    await _gameContainer.Network!.SendEndTurnAsync();
                }
                await _gameContainer.EndTurnAsync!.Invoke();
                return;
            }
            await TradedSalaryAsync(name); //hopefully this simple.
        }
        //hopefully does it automatically.  if not, rethink.
        public void LoadOtherPlayerSalaries()
        {
            var tempList = _gameContainer!.PlayerList!.AllPlayersExceptForCurrent();
            var newList = tempList.GetSalaryList();
            _gameContainer.AddPlayerChoices(newList);
        }

        public async Task TradedSalaryAsync(string player)
        {
            _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetWhoPlayer();
            if (_gameContainer.GameStatus == EnumWhatStatus.NeedTradeSalary && _gameContainer.CanSendMessage())
            {
                await _gameContainer.Network!.SendAllAsync("tradedsalary", player);
            }
            _model.PlayerPicker.ShowOnlyOneSelectedItem(player);
            if (_gameContainer.Test.NoAnimations == false)
            {
                await _gameContainer.Delay.DelaySeconds(.75);
            }
            var thisPlayer = _gameContainer.PlayerList[player];
            SalaryInfo firstSalary;
            SalaryInfo secondSalary;
            firstSalary = thisPlayer.GetSalaryCard();
            secondSalary = _gameContainer.SingleInfo.GetSalaryCard();
            _gameContainer.SingleInfo.Salary = firstSalary.PayCheck;
            thisPlayer.Salary = secondSalary.PayCheck;
            thisPlayer.Hand.ReplaceItem(firstSalary, secondSalary);
            _gameContainer.SingleInfo.Hand.ReplaceItem(secondSalary, firstSalary);
            PopulatePlayerProcesses.FillInfo(_gameContainer.SingleInfo);
            PopulatePlayerProcesses.FillInfo(thisPlayer);
            if (_gameContainer.GameStatus == EnumWhatStatus.NeedTradeSalary)
            {
                _gameContainer.GameStatus = EnumWhatStatus.NeedToEndTurn;
            }
            else if (_gameContainer.GameStatus == EnumWhatStatus.LastSpin)
            {
                //maybe do here to stop the possible overflow.
                IMoveProcesses move = _gameContainer.Resolver.Resolve<IMoveProcesses>();
                await move.PossibleAutomateMoveAsync();
                return; //i think.
            }
            else if (_gameContainer.GameStatus != EnumWhatStatus.NeedToSpin)
                throw new BasicBlankException("Rethinking is required");
            await _gameContainer.ContinueTurnAsync!.Invoke();
        }
    }
}