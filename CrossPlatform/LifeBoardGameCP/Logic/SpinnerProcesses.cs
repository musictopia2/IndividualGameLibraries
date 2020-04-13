using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using LifeBoardGameCP.Data;
using LifeBoardGameCP.Graphics;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace LifeBoardGameCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class SpinnerProcesses : ISpinnerProcesses
    {
        private readonly LifeBoardGameVMData _model;
        private readonly LifeBoardGameGameContainer _gameContainer;
        private readonly GameBoardProcesses _gameBoard;

        public SpinnerProcesses(LifeBoardGameVMData model,
            LifeBoardGameGameContainer gameContainer,
            GameBoardProcesses gameBoard
            )
        {
            _model = model;
            _gameContainer = gameContainer;
            _gameBoard = gameBoard;
        }

        public async Task StartSpinningAsync(SpinnerPositionData position)
        {
            if (_gameContainer.CardVisible!.Invoke())
            {
                await _gameContainer.HideCardAsync!.Invoke();
            }

            if (_gameContainer.SaveRoot!.GameStatus == EnumWhatStatus.NeedChooseHouse || _gameContainer.SaveRoot.GameStatus == EnumWhatStatus.NeedSellBuyHouse)
            {
                //if the hand has to be hidden, rethink.
                _gameContainer.SaveRoot.GameStatus = EnumWhatStatus.NeedToSpin;
                //ThisMod!.HandList!.Visible = false;
            }
            await SpinnerAnimationClass.AnimateSpinAsync(position, _gameContainer);
            var thisNumber = _gameContainer!.GetNumberSpun(_gameContainer.SpinnerPosition);
            _gameContainer.SaveRoot.SpinPosition = _gameContainer.SpinnerPosition;
            _gameContainer.SaveRoot.ChangePosition = 0;
            _gameContainer.SaveRoot.NumberRolled = thisNumber;
            await ResumeSpinnerCompletedAsync(); //forgor this part.
        }
        private void EarnProcessFromRoll(int rolled)
        {
            int whoHad;
            if (rolled == 1 || rolled == 10)
            {
                EnumCareerType careerNeeded;
                if (rolled == 1)
                    careerNeeded = EnumCareerType.Artist;
                else
                    careerNeeded = EnumCareerType.PoliceOfficer;
                whoHad = _gameContainer.WhoHadCareerName(careerNeeded);
                if (whoHad > 0 && whoHad != _gameContainer.WhoTurn)
                {
                    _gameContainer.TakeOutExpense(10000);
                    _gameContainer.CollectMoney(whoHad, 10000);
                }
                if (rolled == 10)
                    return;
            }
            whoHad = WhoHadStock(rolled);
            if (whoHad > 0)
            {
                _gameContainer.CollectMoney(whoHad, 10000);
            }
        }
        private int WhoHadStock(int whatNum)
        {
            if (whatNum == 0)
                return 0;
            var thisPlayer = _gameContainer.PlayerList!.SingleOrDefault(items => items.FirstStock == whatNum || items.SecondStock == whatNum);
            if (thisPlayer == null)
                return 0;
            return thisPlayer.Id;
        }
        private async Task ResumeSpinnerCompletedAsync()
        {
            _gameContainer.SaveRoot!.SpinList.Add(_gameBoard.NumberRolled);
            EarnProcessFromRoll(_gameBoard.NumberRolled);
            if (_gameContainer.Test!.NoAnimations == false)
                await _gameContainer.Delay!.DelaySeconds(1);
            if (_gameContainer.GameStatus == EnumWhatStatus.NeedToSpin)
            {
                _gameContainer.GameStatus = EnumWhatStatus.LastSpin;
            }
            if (_gameBoard.NumberRolled == 0)
            {
                BetweenNumbers();
                if (_gameContainer.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                    await UIPlatform.ShowMessageAsync("Spin again because it landed between two numbers");
                if (_gameContainer.GameStatus == EnumWhatStatus.LastSpin)
                {
                    _gameContainer.GameStatus = EnumWhatStatus.NeedToSpin;
                }
                await _gameContainer.ContinueTurnAsync!.Invoke();
                return;
            }
            await FinishSpinProcessAsync();
            if (_gameContainer.CanTradeForBig(false))
            {
                await UIPlatform.ShowMessageAsync("Current Player Is Getting The $100,000 for 2 8s, 9s or 10s spinned in a row for a lucky break for being entertainer");
                await _gameContainer.TradeForBigAsync();
                return;
            }
            IMoveProcesses move = _gameContainer.Resolver.Resolve<IMoveProcesses>();
            await move.PossibleAutomateMoveAsync();
        }
        private void BetweenNumbers()
        {
            int whoHad = _gameContainer.WhoHadCareerName(EnumCareerType.ComputerConsultant);
            if (whoHad > 0 && whoHad != _gameContainer.WhoTurn)
            {
                _gameContainer.CollectMoney(whoHad, 50000);
            }
        }

        private SpinnerPositionData GetSpinData
        {
            get
            {
                SpinnerPositionData output = new SpinnerPositionData();
                output.CanBetween = _gameContainer.Random.NextBool(3); //decided to decrease to 3
                output.HighSpeedUpTo = _gameContainer.Random.GetRandomNumber(30, 15);
                output.ChangePositions = _gameContainer.Random.GetRandomNumber(120, 60);
                return output;
            }
        }
        public async Task StartSpinningAsync()
        {
            var spin = GetSpinData;
            if (_gameContainer.CanSendMessage())
            {
                await _gameContainer.Network!.SendAllAsync("spin", spin);
            }
            await StartSpinningAsync(spin);
        }
        private async Task FinishSpinProcessAsync()
        {
            if (_gameContainer.GameStatus == EnumWhatStatus.LastSpin)
            {
                _gameContainer.RepaintBoard();
            }
            else if (_gameContainer.GameStatus == EnumWhatStatus.NeedFindSellPrice || _gameContainer.GameStatus == EnumWhatStatus.NeedSellHouse)
            {
                await SoldHouseAsync(_gameBoard.NumberRolled);
            }
            else
            {
                throw new BasicBlankException("When finishing spin process, not sure what to do.  Rethink");
            }
        }
        private async Task SoldHouseAsync(int numberRolled)
        {
            var thisHouse = _gameContainer.SingleInfo!.GetHouseCard();
            decimal moneyEarned = thisHouse.SellingPrices[numberRolled];
            _gameContainer.SingleInfo!.HouseIsInsured = false;
            _model.GameDetails = $"House sold for {moneyEarned.ToCurrency(0)}";
            _gameContainer.CollectMoney(_gameContainer.WhoTurn, moneyEarned);
            if (_gameContainer.Test.NoAnimations == false)
            {
                await _gameContainer.Delay.DelaySeconds(1);
            }
            _gameContainer.SingleInfo.Hand.RemoveSpecificItem(thisHouse);
            PopulatePlayerProcesses.FillInfo(_gameContainer.SingleInfo);
            //hopefully does it automatically (?)
            //ThisMod.ChosenPile!.Visible = false;
            if (_gameContainer.GameStatus == EnumWhatStatus.NeedFindSellPrice)
            {
                _gameContainer.GameStatus = EnumWhatStatus.NeedChooseHouse;
            }
            else
            {
                _gameContainer.GameStatus = EnumWhatStatus.NeedChooseRetirement;
            }
        }
    }
}