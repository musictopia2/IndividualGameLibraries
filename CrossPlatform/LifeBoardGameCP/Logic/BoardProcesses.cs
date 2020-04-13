using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using BasicGameFrameworkLibrary.NetworkingClasses.Extensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using CommonBasicStandardLibraries.Exceptions;
using LifeBoardGameCP.Data;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace LifeBoardGameCP.Logic
{
    [SingletonGame]
    [AutoReset] //i think needs to be reset since it uses objects that can be reset.
    public class BoardProcesses : IBoardProcesses
    {
        private readonly LifeBoardGameVMData _model;
        private readonly LifeBoardGameGameContainer _gameContainer;

        public BoardProcesses(LifeBoardGameVMData model, LifeBoardGameGameContainer gameContainer)
        {
            _model = model;
            _gameContainer = gameContainer;
        }

        public bool CanTrade4Tiles => _gameContainer.CanTradeForBig(true);
        public bool CanPurchaseCarInsurance => _gameContainer.GameStatus == EnumWhatStatus.NeedToSpin && _gameContainer.SingleInfo!.CarIsInsured == false;
        public bool CanAttendNightSchool => _gameContainer.GameStatus == EnumWhatStatus.NeedNight;
        public bool CanPurchaseHouseInsurance => _gameContainer.SingleInfo!.HouseIsInsured == false
            && _gameContainer.GameStatus == EnumWhatStatus.NeedToSpin
            && _gameContainer.SingleInfo.HouseName != "";
        public bool CanPurchaseStock
        {
            get
            {
                if (_gameContainer.GameStatus != EnumWhatStatus.NeedToSpin)
                    return false;
                if (_gameContainer.SingleInfo!.FirstStock > 0 || _gameContainer.SingleInfo.SecondStock > 0)
                    return false;
                return true;
            }
        }
        //public bool CanSellHouse => false;
        public bool CanSellHouse => _gameContainer.GameStatus == EnumWhatStatus.NeedSellBuyHouse; //possibly used wrong status here.
        public bool CanEndTurn
        {
            get
            {
                if (_model.Instructions == "Choose one career or end turn and keep your current career")
                {
                    return true;
                }
                return _gameContainer!.GameStatus == EnumWhatStatus.NeedToEndTurn || _gameContainer.GameStatus == EnumWhatStatus.NeedTradeSalary || _gameContainer.GameStatus == EnumWhatStatus.NeedNight;
            }
        }

        public async Task AttendNightSchoolAsync()
        {
            if (_gameContainer.CanSendMessage())
            {
                await _gameContainer.Network!.SendAllAsync("attendednightschool");
            }
            _gameContainer.SaveRoot!.WasNight = true;
            _model.GameDetails = "Paid $20,000 to attend night school to possibly get a better career";
            _gameContainer.TakeOutExpense(20000);
            _gameContainer.GameStatus = EnumWhatStatus.NeedNewCareer;
            await _gameContainer.ContinueTurnAsync!.Invoke();
        }

        public async Task ComputerChoseSpaceAsync(int space)
        {
            if (_gameContainer.CanSendMessage())
            {
                await _gameContainer.Network!.SendMoveAsync(space);
            }
            IMoveProcesses move = _gameContainer.Resolver.Resolve<IMoveProcesses>();
            await move.DoAutomateMoveAsync(space); //hopefully this simple.
        }

        public string GetSpaceDetails(int space)
        {
            var thisSpace = _gameContainer!.SpaceList![space - 1];
            decimal newAmount;
            string output;
            switch (thisSpace.ActionInfo)
            {
                case EnumActionType.CollectPayMoney:
                    if (thisSpace.AmountReceived < 0)
                    {
                        newAmount = Math.Abs(thisSpace.AmountReceived);
                        output = $"{thisSpace.Description}{Constants.vbCrLf}Pay {newAmount.ToCurrency(0)}";
                        if (thisSpace.WhatInsurance != EnumInsuranceType.NoInsurance)
                            output += Constants.vbCrLf + " if not insured";
                        return output;
                    }
                    return thisSpace.Description + Constants.vbCrLf + thisSpace.AmountReceived.ToCurrency(0);
                case EnumActionType.AttendNightSchool:
                    return $"Night School.{Constants.vbCrLf} Pay $20,000";
                case EnumActionType.FindNewJob:
                    return thisSpace.Description;
                case EnumActionType.GetMarried:
                    return "Get Married";
                case EnumActionType.GetPaid:
                    return "Pay!" + Constants.vbCrLf + "Day";
                case EnumActionType.GotBabyBoy:
                    return "Baby boy!" + Constants.vbCrLf + "Life";
                case EnumActionType.GotBabyGirl:
                    return "Baby girl!" + Constants.vbCrLf + "Life";
                case EnumActionType.HadTwins:
                    return thisSpace.Description + Constants.vbCrLf + "Life";
                case EnumActionType.MayBuyHouse:
                    return "You may BUY A HOUSE" + Constants.vbCrLf + "Draw Deed";
                case EnumActionType.MaySellHouse:
                    return "You may sell your house and buy a new one.";
                case EnumActionType.ObtainLifeTile:
                    return thisSpace.Description + Constants.vbCrLf + "Life";
                case EnumActionType.PayTaxes:
                    return "Taxes due.";
                case EnumActionType.SpinAgainIfBehind:
                    return "Spin again if you are not in the lead.";
                case EnumActionType.StartCareer:
                    return "CAREER CHOICE";
                case EnumActionType.StockBoomed:
                    return "Stock market soars!" + Constants.vbCrLf + "Collect 1 stock.";
                case EnumActionType.StockCrashed:
                    return "Stock market crash." + Constants.vbCrLf + "Return 1 stock.";
                case EnumActionType.TradeSalary:
                    return "Trade salary card with any player.";
                case EnumActionType.WillMissTurn:
                    return thisSpace.Description + Constants.vbCrLf + "Miss next turn.";
                case EnumActionType.WillRetire:
                    return "RETIRE" + Constants.vbCrLf + "Go to Countryside Acres" + Constants.vbCrLf + "or Millionaire Estates.";
                default:
                    throw new BasicBlankException("No description for " + thisSpace.ActionInfo.ToString());
            }
        }
        public async Task HumanChoseSpaceAsync()
        {
            IMoveProcesses move = _gameContainer.Resolver.Resolve<IMoveProcesses>();
            if (_gameContainer.CanSendMessage())
            {
                await _gameContainer.Network!.SendMoveAsync(_gameContainer.CurrentSelected);
            }
            await move.DoAutomateMoveAsync(_gameContainer.CurrentSelected);
        }

        public async Task OpeningOptionAsync(EnumStart start)
        {
            if (_gameContainer.CanSendMessage() == true)
            {
                await _gameContainer.Network!.SendAllAsync("firstoption", start);
            }
            _gameContainer.SingleInfo!.OptionChosen = start;
            _gameContainer.RepaintBoard();
            if (start == EnumStart.College)
            {
                _gameContainer.SingleInfo.Loans = 100000;
                _gameContainer.GameStatus = EnumWhatStatus.NeedToSpin;
            }
            else
            {
                _gameContainer.GameStatus = EnumWhatStatus.NeedChooseFirstCareer;
            }

            await _gameContainer.ContinueTurnAsync!.Invoke();
        }

        public async Task PurchaseCarInsuranceAsync()
        {
            if (_gameContainer.CanSendMessage() == true)
            {
                await _gameContainer.Network!.SendAllAsync("purchasecarinsurance");
            }
            _gameContainer.SingleInfo!.CarIsInsured = true;
            _gameContainer.TakeOutExpense(5000);
            _model.GameDetails = "Paid $5,000 for car insurance.  Now you owe nothing for car damages or car accidents";
            await _gameContainer.ContinueTurnAsync!.Invoke();
        }

        public async Task PurchaseHouseInsuranceAsync()
        {
            if (_gameContainer.CanSendMessage() == true)
            {
                await _gameContainer.Network!.SendAllAsync("purchasedhouseinsurance");
            }
            decimal amountToPay = _gameContainer.SingleInfo!.InsuranceCost();
            _gameContainer.TakeOutExpense(amountToPay);
            _model.GameDetails = "Paid $5,000 for car insurance.  Now you owe nothing for car damages or car accidents";
            _gameContainer.SingleInfo!.HouseIsInsured = true;
            _gameContainer.ProcessCommission();
            _model!.GameDetails = $"Paid {amountToPay.ToCurrency(0)}.  Now you owe nothing for damages for the house";
            await _gameContainer.ContinueTurnAsync!.Invoke();
        }

        public async Task PurchaseStockAsync()
        {
            if (_gameContainer.CanSendMessage() == true)
            {
                await _gameContainer.Network!.SendAllAsync("purchasedstock");
            }
            _gameContainer.SaveRoot!.EndAfterStock = false;
            _gameContainer.TakeOutExpense(50000);
            _gameContainer.ProcessCommission();
            _model.GameDetails = "Paid $50,000 for stock";
            _gameContainer.GameStatus = EnumWhatStatus.NeedChooseStock;
            await _gameContainer.ContinueTurnAsync!.Invoke();
        }

        public async Task RetirementAsync(EnumFinal final)
        {
            if (_gameContainer.CanSendMessage() == true)
            {
                await _gameContainer.Network!.SendAllAsync("choseretirement", final);
            }
            _gameContainer.SingleInfo!.LastMove = final;
            _gameContainer.RepaintBoard();
            _gameContainer.SingleInfo.InGame = false;
            await _gameContainer.EndTurnAsync!.Invoke();
        }

        public async Task SellHouseAsync()
        {
            if (_gameContainer.CanSendMessage() == true)
            {
                await _gameContainer.Network!.SendAllAsync("willsellhouse");
            }
            _gameContainer.GameStatus = EnumWhatStatus.NeedFindSellPrice;
            await _gameContainer.ContinueTurnAsync!.Invoke();
        }

        public void SpaceDescription(int space)
        {
            _gameContainer.CurrentSelected = space;
            _model.GameDetails = GetSpaceDetails(space);
            _gameContainer.RepaintBoard();
        }

        public async Task Trade4TilesAsync()
        {
            if (_gameContainer.CanSendMessage())
            {
                await _gameContainer.Network!.SendAllAsync("tradedlifeforsalary");
            }
            _gameContainer.SingleInfo!.TilesCollected -= 4;
            await _gameContainer.TradeForBigAsync(); //i think.
        }
    }
}