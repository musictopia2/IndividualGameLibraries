using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using PaydayCP.Data;
using BasicGameFrameworkLibrary.Attributes;
//i think this is the most common things i like to do
namespace PaydayCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class LotteryProcesses : ILotteryProcesses
    {
        private readonly PaydayGameContainer _gameContainer;
        private readonly PaydayVMData _model;

        public LotteryProcesses(PaydayGameContainer gameContainer, PaydayVMData model)
        {
            _gameContainer = gameContainer;
            _model = model;
        }
        bool ILotteryProcesses.CanStartLotteryProcess()
        {
            return _gameContainer.PlayerList.Count(x => x.ChoseNumber > 0) >= 2; //i think
        }

        void ILotteryProcesses.LoadLotteryList()
        {
            CustomBasicList<int> thisList = Enumerable.Range(1, 6).ToCustomBasicList();
            thisList.RemoveAllOnly(yy => _gameContainer.PlayerList.Any(Items => Items.ChoseNumber == yy));
            thisList.InsertBeginning(0);
            CustomBasicList<string> tempList = thisList.CastIntegerListToStringList();
            _model.AddPopupLists(tempList);
        }

        async Task ILotteryProcesses.ProcessLotteryAsync()
        {
            await _gameContainer.StartProcessPopUpAsync(_model);
            //this is what a person chose for lottery.
            _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetOtherPlayer();
            _gameContainer.SingleInfo.ChoseNumber = int.Parse(_model.PopUpChosen);
            if (_gameContainer.Test.NoAnimations == false)
            {
                await _gameContainer.Delay!.DelaySeconds(1);
            }
            if (_gameContainer.OtherTurnProgressAsync == null)
            {
                throw new BasicBlankException("Nobody is handling the other turn process.  Rethink");
            }
            await _gameContainer.OtherTurnProgressAsync.Invoke();
        }

        async Task ILotteryProcesses.RollLotteryAsync()
        {
            //hopefully can make this do the rolling for the lottery.
            decimal newAmount;
            if (_gameContainer.PlayerList.Any(items => items.ChoseNumber == _model.Cup!.ValueOfOnlyDice))
            {
                var tempList = _gameContainer.PlayerList.Where(items => items.ChoseNumber > 0).ToCustomBasicList();
                newAmount = tempList.Count() * 100;
                newAmount += 1000;
                tempList.ForEach(items => items.ReduceFromPlayer(100));
                _gameContainer.SingleInfo = _gameContainer.PlayerList.Where(items => items.ChoseNumber == _model.Cup!.ValueOfOnlyDice).Single();
                _gameContainer.SingleInfo.MoneyHas += newAmount;
                _gameContainer.SaveRoot!.Instructions = $"{_gameContainer.SingleInfo.NickName} has won {newAmount.ToCurrency(0)} for the lottery";
                if (_gameContainer.Test!.NoAnimations == false)
                    await _gameContainer.Delay!.DelaySeconds(2);
                _gameContainer.SaveRoot.GameStatus = EnumStatus.EndingTurn;
            }
            await _gameContainer.ContinueTurnAsync!.Invoke();
        }
    }
}
