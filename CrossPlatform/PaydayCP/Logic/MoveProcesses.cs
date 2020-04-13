using BasicGameFrameworkLibrary.Attributes;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using PaydayCP.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using vb = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.Constants;

namespace PaydayCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class MoveProcesses : IMoveProcesses
    {
        private readonly PaydayGameContainer _gameContainer;
        private readonly PaydayVMData _model;
        private readonly GameBoardProcesses _gameBoard;
        private readonly ILotteryProcesses _lotteryProcesses;
        private readonly IMailProcesses _mailProcesses;
        private readonly IYardSaleProcesses _yardSaleProcesses;
        private readonly IBuyProcesses _buyProcesses;
        private readonly IDealProcesses _dealProcesses;

        public MoveProcesses(
            PaydayGameContainer gameContainer,
            PaydayVMData model,
            GameBoardProcesses gameBoard,
            ILotteryProcesses lotteryProcesses,
            IMailProcesses mailProcesses,
            IYardSaleProcesses yardSaleProcesses,
            IBuyProcesses buyProcesses,
            IDealProcesses dealProcesses
            )
        {
            _gameContainer = gameContainer;
            _model = model;
            _gameBoard = gameBoard;
            _lotteryProcesses = lotteryProcesses;
            _mailProcesses = mailProcesses;
            _yardSaleProcesses = yardSaleProcesses;
            _buyProcesses = buyProcesses;
            _dealProcesses = dealProcesses;
        }
        private async Task Pause3Async()
        {
            if (_gameContainer.Test.NoAnimations == false)
            {
                await _gameContainer.Delay.DelaySeconds(3);
            }
        }


        private async Task ProcessJackPotAsync()
        {
            decimal newAmount = _gameContainer.SaveRoot!.LotteryAmount;
            if (newAmount > 0)
            {
                _gameContainer.SaveRoot.Instructions = $"Congratulations, {_gameContainer.SingleInfo!.NickName} has won.  {newAmount.ToCurrency(0)} for rolling a six";
                _gameContainer.SingleInfo.MoneyHas += newAmount;
                _gameBoard!.ClearJackPot();
                if (_gameContainer.Test.NoAnimations == false)
                {
                    await _gameContainer.Delay.DelaySeconds(1);
                }
            }
        }

        private async Task MonthProcessingAsync()
        {
            _gameContainer.SingleInfo!.MoneyHas += 3500;
            _gameContainer.SaveRoot!.Instructions = "End of month.  Here is your paycheck of $3,500.  Please pay your bills";
            _gameContainer.SaveRoot.EndOfMonth = false;
            if (_gameContainer.Test.NoAnimations == false)
            {
                await _gameContainer.Delay.DelaySeconds(2);
            }
            var interest = Math.Ceiling(((decimal)10 / 100) * _gameContainer.SingleInfo.Loans);
            if (interest > 0)
            {
                _gameContainer.SingleInfo.MoneyHas -= interest;
            }
            _gameContainer.SingleInfo.CurrentMonth++;
            PayLoans();
            PayBills();
            _mailProcesses.PopulateMails();
            _gameContainer.SaveRoot.Instructions = "Bills and loans has been paid.  View the results";
            if (_gameContainer.Test.NoAnimations == false)
            {
                await _gameContainer.Delay.DelaySeconds(1.5);
            }
            if (_gameContainer.SaveRoot.EndGame == true)
            {
                _gameBoard!.MoveToLastSpace();
                _gameContainer.SaveRoot.EndGame = false;
            }
            if (_gameContainer.SaveRoot.RemainingMove > 0)
            {
                if (_gameContainer.SingleInfo.InGame == false)
                {
                    throw new BasicBlankException("Cannot be any remaining move because the player is no longer in the game");
                }
                _gameContainer.MonthLabel(_model);
                _gameContainer.SaveRoot.Instructions = "Finish the move by moving to the highlighted space";
                _gameBoard!.HighlightDay(_gameContainer.SaveRoot.RemainingMove);
                _gameContainer.SaveRoot.GameStatus = EnumStatus.MakeMove;
            }
            else
            {
                _gameContainer.SaveRoot.GameStatus = EnumStatus.EndingTurn;
            }
            await _gameContainer.ContinueTurnAsync!.Invoke();
        }
        private int Thousands(decimal amount)
        {
            int x = default;
            if (amount < 1000)
                return 0;
            var NewAmount = 0;
            do
            {
                x += 1;
                NewAmount += 1000;
                if (NewAmount >= amount)
                    return x - 1;
                if (x == 5000)
                {
                    throw new BasicBlankException("Don't think it can be 5000 times.  If I am wrong; change it");
                }
            }
            while (true);
        }
        private void PayLoans()
        {
            if (_gameContainer.SingleInfo!.Loans == 0)
                return;
            if (_gameContainer.SingleInfo.MoneyHas >= _gameContainer.SingleInfo.Loans)
            {
                _gameContainer.SingleInfo.MoneyHas -= _gameContainer.SingleInfo.Loans;
                _gameContainer.SingleInfo.Loans = 0;
                return;
            }
            int nThousands = Thousands(_gameContainer.SingleInfo.MoneyHas);
            if (nThousands == 0)
                return;
            int amountPaid = nThousands * 1000;
            _gameContainer.SingleInfo.MoneyHas -= amountPaid;
            _gameContainer.SingleInfo.Loans -= amountPaid;
            if (_gameContainer.SingleInfo.MoneyHas < 0 || _gameContainer.SingleInfo.Loans < 0)
            {
                throw new BasicBlankException("The loans and money a player has must be greater or equal to 0");
            }
        }
        private void PayBills()
        {
            var tempList = _model.CurrentMailList!.HandList;
            tempList.ForEach(mail =>
            {
                _gameContainer.SingleInfo!.ReduceFromPlayer(Math.Abs(mail.AmountReceived));
                _gameContainer.SaveRoot!.OutCards.Add(mail);
                _gameContainer.SingleInfo!.Hand.RemoveSpecificItem(mail);
            });
        }
        
        private void ProcessBirthday()
        {
            decimal moneyGained = (_gameContainer.PlayerList.Count() - 1) * 100;
            _gameContainer.SingleInfo!.MoneyHas += moneyGained;
            CustomBasicList<PaydayPlayerItem> tempList = _gameContainer.PlayerList.Where(items => items.Id != _gameContainer.WhoTurn).ToCustomBasicList();
            tempList.ForEach(x => x.ReduceFromPlayer(100));
        }
        private void ProcessSweepStakes()
        {
            _gameContainer.SingleInfo!.MoneyHas += 5000;
        }
        public async Task ResultsOfMoveAsync(int day)
        {
            _gameContainer.SaveRoot!.GameStatus = EnumStatus.EndingTurn; //you have to prove otherwise.
            if (_model!.Cup!.TotalDiceValue == 6)
                await ProcessJackPotAsync();
            if (day == 31 && _gameContainer.SaveRoot.RemainingMove == 0)
            {
                await MonthProcessingAsync();
                return;
            }
            if (_gameContainer.SaveRoot.EndOfMonth == true)
            {
                await MonthProcessingAsync();
                return;
            }
            EnumDay details = _gameBoard.GetDayDetails(day);
            switch (details)
            {
                case EnumDay.Mail:
                    //_model.Cup!.CanShowDice = false; //because its showing mail instead.
                    await _mailProcesses.ProcessMailAsync();
                    break;
                case EnumDay.SweepStakes:
                    _gameContainer.SaveRoot.Instructions = $"{_gameContainer.SingleInfo!.NickName} won $5000.00 dollars in the sweepstakes";
                    await Pause3Async();
                    ProcessSweepStakes();
                    await _gameContainer.ContinueTurnAsync!.Invoke();
                    break;
                case EnumDay.Deal:
                    //_model.Cup!.CanShowDice = false; //because its showing mail instead.
                    await _dealProcesses.ProcessDealAsync(false);
                    break;
                case EnumDay.Buyer:
                    await _buyProcesses.ProcessBuyerAsync();
                    break;
                case EnumDay.Lottery:
                    await ProcessLotteryAsync();
                    break;
                case EnumDay.YardSale:
                    //_model.Cup!.CanShowDice = false;
                    _gameContainer.SaveRoot.GameStatus = EnumStatus.ViewYardSale;
                    await _yardSaleProcesses.ProcessYardSaleAsync();
                    break;
                case EnumDay.ShoppingSpree:
                    _gameContainer.SaveRoot.Instructions = "Shopping Spree.  Please pay $500.00 to the jackpot";
                    await Pause3Async();
                    _gameContainer.ProcessExpense(_gameBoard, 500);
                    await _gameContainer.ContinueTurnAsync!.Invoke();
                    break;
                case EnumDay.SkiWeekEnd:
                    _gameContainer.SaveRoot.Instructions = "Ski Weekend.  Please pay $500.00 to the jackpot";
                    await Pause3Async();
                    _gameContainer.ProcessExpense(_gameBoard, 500);
                    await _gameContainer.ContinueTurnAsync!.Invoke();
                    break;
                case EnumDay.HappyBirthday:
                    _gameContainer.SaveRoot.Instructions = "Happy birthday.  Each player pays you $100.00";
                    await Pause3Async();
                    ProcessBirthday();
                    await _gameContainer.ContinueTurnAsync!.Invoke();
                    break;
                case EnumDay.CharityConcert:
                    _gameContainer.SaveRoot.Instructions = "Charity Concert.  Please pay $400.00 to the jackpot";
                    await Pause3Async();
                    _gameContainer.ProcessExpense(_gameBoard, 400);
                    await _gameContainer.ContinueTurnAsync!.Invoke();
                    break;
                case EnumDay.RadioContest:
                    await ProcessRadioAsync();
                    break;
                case EnumDay.Food:
                    _gameContainer.SaveRoot.Instructions = "Pay Food For The Month.  Please pay $600.00 to the jackpot";
                    await Pause3Async();
                    _gameContainer.ProcessExpense(_gameBoard, 600);
                    await _gameContainer.ContinueTurnAsync!.Invoke();
                    break;
                case EnumDay.WalkForCharity:
                    _gameContainer.OtherTurn = _gameContainer.WhoTurn;
                    _gameContainer.SaveRoot.GameStatus = EnumStatus.RollCharity;
                    _gameContainer.SaveRoot.Instructions = "Walk for charity.  Rol the dice and pay 100 times the amount you roll to the jackpot";
                    await Pause3Async();
                    if (_gameContainer.OtherTurnProgressAsync == null)
                    {
                        throw new BasicBlankException("Nobody is handling other turn progress.  Rethink");
                    }
                    await _gameContainer.OtherTurnProgressAsync.Invoke();
                    break;
                default:
                    throw new BasicBlankException($"Don't know what to do with {_gameContainer.SaveRoot.GameStatus.ToString()}");
            }
        }

        private async Task ProcessRadioAsync()
        {
            _gameContainer.OtherTurn = _gameContainer.WhoTurn;
            _gameContainer.SaveRoot!.Instructions = "Roll the dice.  The person to roll a 3 wins $1,000";
            _gameContainer.SaveRoot.GameStatus = EnumStatus.RollRadio;
            await _gameContainer.ContinueTurnAsync!.Invoke();
        }

        private async Task ProcessLotteryAsync()
        {
            _gameContainer.OtherTurn = _gameContainer.WhoTurn;
            _lotteryProcesses.LoadLotteryList();
            _gameContainer.SaveRoot!.Instructions = $"Please choose a number between 0 and 6.  Choose 0 to not participate{vb.vbCrLf} The cost is $100.00.  If you win, then you receive $100.00 times the number of players that participate in addition to the $1,000 the bank donates";
            _gameContainer.SaveRoot.GameStatus = EnumStatus.ChooseLottery;
            await _gameContainer.ContinueTurnAsync!.Invoke();
        }

        

    }
}