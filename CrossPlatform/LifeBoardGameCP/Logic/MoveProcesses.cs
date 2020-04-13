using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using LifeBoardGameCP.Cards;
using LifeBoardGameCP.Data;
using System;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace LifeBoardGameCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class MoveProcesses : IMoveProcesses, ITwinProcesses
    {
        private readonly LifeBoardGameVMData _model;
        private readonly LifeBoardGameGameContainer _gameContainer;
        private readonly GameBoardProcesses _gameBoard;

        public MoveProcesses(LifeBoardGameVMData model,
            LifeBoardGameGameContainer gameContainer,
            GameBoardProcesses gameBoard
            )
        {
            _model = model;
            _gameContainer = gameContainer;
            _gameBoard = gameBoard;
        }

        public async Task DoAutomateMoveAsync(int space)
        {
            IBoardProcesses processes = _gameContainer.Resolver.Resolve<IBoardProcesses>();

            _model!.GameDetails = processes.GetSpaceDetails(space);
            _model!.Instructions = "Making Move";
            _gameBoard.NewPosition = space;
            if (space == _gameBoard.SecondPossiblePosition)
                await _gameBoard.AnimateMoveAsync(true);
            else
                await _gameBoard.AnimateMoveAsync(false);
            await MoveResultsAsync(space, _gameContainer.SingleInfo!);
        }
        private bool TilesLeft
        {
            get
            {
                int howMany = _gameContainer.PlayerList.Sum(items => items.TilesCollected);
                if (howMany > 21)
                    throw new BasicBlankException("Only 21 tiles at the most");
                return howMany < 21;
            }
        }
        private bool CanStealTile => _gameContainer.PlayerList.Any(items => items.TilesCollected > 0 && items.LastMove != EnumFinal.CountrySideAcres);
        private async Task MoveResultsAsync(int index, LifeBoardGamePlayerItem thisPlayer)
        {
            var thisSpace = _gameContainer!.SpaceList![index - 1];
            PaydayProcessing(thisPlayer);
            decimal newAmount;
            int newPlayer;
            if (thisSpace.GetLifeTile)
            {
                if (TilesLeft)
                {
                    _gameContainer.ObtainLife(thisPlayer);
                    _gameContainer.GameStatus = EnumWhatStatus.NeedToEndTurn;
                }
                else if (CanStealTile)
                {
                    _gameContainer.GameStatus = EnumWhatStatus.NeedStealTile;
                }
                else
                {
                    _gameContainer.GameStatus = EnumWhatStatus.NeedToEndTurn;
                }
            }
            switch (thisSpace.ActionInfo)
            {
                case EnumActionType.CollectPayMoney:
                    _gameContainer.GameStatus = EnumWhatStatus.NeedToEndTurn;
                    if (thisSpace.AmountReceived < 0)
                    {
                        newAmount = Math.Abs(thisSpace.AmountReceived);
                        if (thisSpace.WhatInsurance == EnumInsuranceType.NeedCar)
                        {
                            if (thisPlayer.CarIsInsured == true)
                            {
                                _model!.GameDetails = $"{_model.GameDetails}{Constants.vbCrLf} Pay nothing because the player had car insurance";
                            }
                            else
                            {
                                _model!.GameDetails = $"{thisSpace.Description}{Constants.vbCrLf}Pay {newAmount.ToCurrency(0)} because the player had no car insurance";
                                _gameContainer.TakeOutExpense(newAmount);
                            }
                        }
                        else if (thisSpace.WhatInsurance == EnumInsuranceType.NeedHouse)
                        {
                            if (thisPlayer.HouseIsInsured || thisPlayer.GetHouseName() == "")
                            {
                                _model!.GameDetails = $"{_model.GameDetails}{Constants.vbCrLf} Pay nothing because the player had house insurance or had no house";
                            }
                            else
                            {
                                _model!.GameDetails = $"{thisSpace.Description}{Constants.vbCrLf}Pay {newAmount.ToCurrency(0)} because the player had no house insurance";
                                _gameContainer.TakeOutExpense(newAmount);
                            }
                        }
                        else
                        {
                            if (thisSpace.CareerSpace != EnumCareerType.None)
                            {
                                newPlayer = _gameContainer.WhoHadCareerName(thisSpace.CareerSpace);
                                if (newPlayer == _gameContainer.WhoTurn)
                                {
                                    _model!.GameDetails = "Owe nothing because the player owns the career";
                                }
                                else if (newPlayer > 0)
                                {
                                    var tempPlayer = _gameContainer.PlayerList![newPlayer];
                                    _model!.GameDetails = $"{_model.GameDetails}{Constants.vbCrLf} Pay {tempPlayer.NickName} since they own the career";
                                    _gameContainer.CollectMoney(newPlayer, newAmount);
                                }
                            }
                            _gameContainer.TakeOutExpense(newAmount);
                        }
                    }
                    else
                    {
                        _gameContainer.CollectMoney(_gameContainer.WhoTurn, thisSpace.AmountReceived); //i think
                    }

                    break;
                case EnumActionType.StartCareer:
                    _gameContainer.GameStatus = EnumWhatStatus.NeedChooseFirstCareer;
                    break;
                case EnumActionType.AttendNightSchool:
                    _gameContainer.GameStatus = EnumWhatStatus.NeedNight;
                    break;
                case EnumActionType.FindNewJob:
                    var tempList = thisPlayer.GetCareerList();
                    tempList.ForEach(thisCard => thisPlayer.Hand.RemoveSpecificItem(thisCard));
                    _gameContainer.GameStatus = EnumWhatStatus.NeedNewCareer;
                    break;
                case EnumActionType.GetMarried:
                    GetMarriedProcess(thisPlayer);
                    break;
                case EnumActionType.ObtainLifeTile:
                    _gameContainer.SaveRoot!.WasMarried = false;
                    if (_gameContainer.GameStatus != EnumWhatStatus.NeedStealTile)
                        _gameContainer.GameStatus = EnumWhatStatus.NeedToEndTurn;
                    break;
                case EnumActionType.GetPaid:
                    _gameContainer.GameStatus = EnumWhatStatus.NeedToEndTurn;
                    break;
                case EnumActionType.GotBabyBoy:
                    GetBaby(thisPlayer, EnumGender.Boy);
                    _gameContainer.SaveRoot!.WasMarried = false;
                    if (_gameContainer.GameStatus != EnumWhatStatus.NeedStealTile)
                        _gameContainer.GameStatus = EnumWhatStatus.NeedToEndTurn;
                    break;
                case EnumActionType.GotBabyGirl:
                    GetBaby(thisPlayer, EnumGender.Girl);
                    _gameContainer.SaveRoot!.WasMarried = false;
                    if (_gameContainer.GameStatus != EnumWhatStatus.NeedStealTile)
                        _gameContainer.GameStatus = EnumWhatStatus.NeedToEndTurn;
                    break;
                case EnumActionType.HadTwins:
                    if (_gameContainer.BasicData!.MultiPlayer && _gameContainer.SingleInfo!.CanSendMessage(_gameContainer.BasicData) == false)
                    {
                        _gameContainer.Check!.IsEnabled = true; //wait to see if they got boys or girls.
                        return;
                    }
                    var genderList = new CustomBasicList<EnumGender> { EnumGender.Boy, EnumGender.Girl };
                    CustomBasicList<EnumGender> newList = new CustomBasicList<EnumGender> { genderList.GetRandomItem(), genderList.GetRandomItem() };
                    if (_gameContainer.BasicData.MultiPlayer)
                        await _gameContainer.Network!.SendAllAsync("twins", newList);
                    await GetTwinsAsync(newList);
                    return;
                case EnumActionType.MayBuyHouse:
                    _gameContainer.GameStatus = EnumWhatStatus.NeedChooseHouse;
                    break;
                case EnumActionType.MaySellHouse:
                    if (thisPlayer.HouseName == "")
                        _gameContainer.GameStatus = EnumWhatStatus.NeedChooseHouse;
                    else
                        _gameContainer.GameStatus = EnumWhatStatus.NeedSellBuyHouse;
                    break;
                case EnumActionType.PayTaxes:
                    if (thisSpace.CareerSpace != EnumCareerType.Accountant)
                        throw new BasicBlankException("Only accountants can show up for paying taxes");
                    newPlayer = _gameContainer.WhoHadCareerName(EnumCareerType.Accountant);
                    newAmount = _gameContainer.SingleInfo!.TaxesDue();
                    if (newPlayer == _gameContainer.WhoTurn)
                    {
                        _model!.GameDetails = "Owe no taxes because the player is the accountant";
                    }
                    else if (newPlayer != 0)
                    {
                        var tempPlayer = _gameContainer.PlayerList![newPlayer];
                        _model!.GameDetails = $"{_model.GameDetails}{Constants.vbCrLf} Pay {tempPlayer.NickName} {newAmount.ToCurrency(0)} in taxes since they are the accountant";
                        _gameContainer.CollectMoney(newPlayer, newAmount);
                    }
                    else
                    {
                        _model!.GameDetails = $"{_model.GameDetails}{Constants.vbCrLf} Pay {newAmount.ToCurrency(0)} in taxes";
                    }
                    _gameContainer.TakeOutExpense(newAmount);
                    _gameContainer.GameStatus = EnumWhatStatus.NeedToEndTurn;
                    break;
                case EnumActionType.SpinAgainIfBehind:
                    if (_gameContainer.WhoTurn != _gameBoard!.PlayerInLead)
                        _gameContainer.GameStatus = EnumWhatStatus.NeedToSpin;
                    else
                        _gameContainer.GameStatus = EnumWhatStatus.NeedToEndTurn;
                    break;
                case EnumActionType.StockCrashed:
                    if (thisPlayer.FirstStock > 0 && thisPlayer.SecondStock > 0)
                    {
                        _gameContainer.GameStatus = EnumWhatStatus.NeedReturnStock;
                    }
                    else if (thisPlayer.FirstStock > 0 || thisPlayer.SecondStock > 0)
                    {
                        _model!.GameDetails = $"{_model.GameDetails}{Constants.vbCrLf} There was only one stock.  That has been returned automatically.";
                        await AutoReturnAsync();
                        return;
                    }
                    else
                    {
                        _model!.GameDetails = $"{_model.GameDetails}{Constants.vbCrLf} There was no stocks to return.";
                        _gameContainer.GameStatus = EnumWhatStatus.NeedToEndTurn;
                    }
                    break;
                case EnumActionType.StockBoomed:
                    _gameContainer.GameStatus = EnumWhatStatus.NeedChooseStock;
                    _gameContainer.SaveRoot!.EndAfterStock = true;
                    break;
                case EnumActionType.TradeSalary:
                    _gameContainer.SaveRoot!.EndAfterSalary = true;
                    _gameContainer.GameStatus = EnumWhatStatus.NeedTradeSalary;
                    break;
                case EnumActionType.WillMissTurn:
                    _gameContainer.GameStatus = EnumWhatStatus.NeedToEndTurn;
                    thisPlayer.MissNextTurn = true;
                    break;
                case EnumActionType.WillRetire:
                    await RetirementProcessAsync();
                    return;
                default:
                    throw new BasicBlankException("Move Results Status Not Found");
            }
            await _gameContainer.ContinueTurnAsync!.Invoke();
        }

        public async Task PossibleAutomateMoveAsync()
        {
            if (_gameContainer.GameStatus != EnumWhatStatus.NeedToSpin && _gameContainer.GameStatus != EnumWhatStatus.LastSpin)
            {
                await _gameContainer.ContinueTurnAsync!.Invoke();
                return;
            }
            if (_gameBoard.FirstPossiblePosition > 0 && _gameBoard.SecondPossiblePosition > 0)
            {
                _gameContainer.GameStatus = EnumWhatStatus.NeedToChooseSpace;
                await _gameContainer.ContinueTurnAsync!.Invoke();
                return;
            }
            if (_gameBoard.SecondPossiblePosition > 0)
                throw new BasicBlankException("Can't have a second possible option but not the first one");
            await DoAutomateMoveAsync(_gameBoard.FirstPossiblePosition);
        }
        private void PaydayProcessing(LifeBoardGamePlayerItem thisPlayer)
        {
            if (_gameBoard.PayDaysPassed == 0)
                return;
            decimal amountPaid = thisPlayer.Salary;
            _gameContainer.CollectMoney(thisPlayer.Id, amountPaid * _gameBoard.PayDaysPassed);
            _gameContainer.GameStatus = EnumWhatStatus.NeedToEndTurn;
        }
        private void GetBaby(LifeBoardGamePlayerItem thisPlayer, EnumGender gender)
        {
            thisPlayer.ChildrenList.Add(gender);
            _gameContainer.RepaintBoard();
        }
        public async Task GetTwinsAsync(CustomBasicList<EnumGender> twinList)
        {
            _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetWhoPlayer(); //just in case.
            twinList.ForEach(thisTwin => GetBaby(_gameContainer.SingleInfo, thisTwin));
            _gameContainer.SaveRoot!.WasMarried = false;
            await _gameContainer.ContinueTurnAsync!.Invoke();
        }
        private void GetMarriedProcess(LifeBoardGamePlayerItem thisPlayer)
        {
            thisPlayer.Married = true;
            if (_gameContainer.GameStatus != EnumWhatStatus.NeedStealTile)
            {
                _gameContainer.GameStatus = EnumWhatStatus.NeedToSpin;
            }
            _gameContainer.RepaintBoard();
            _gameContainer.SaveRoot!.WasMarried = true;
        }
        private async Task AutoReturnAsync()
        {
            StockInfo thisStock;
            if (_gameContainer.SingleInfo!.FirstStock > 0)
            {
                thisStock = (StockInfo)_gameContainer.SingleInfo.GetStockCard(_gameContainer.SingleInfo.FirstStock);
                _gameContainer.SingleInfo.FirstStock = 0;
            }
            else if (_gameContainer.SingleInfo.SecondStock > 0)
            {
                thisStock = (StockInfo)_gameContainer.SingleInfo.GetStockCard(_gameContainer.SingleInfo.SecondStock);
                _gameContainer.SingleInfo.SecondStock = 0;
            }
            else
            {
                throw new BasicBlankException("There is nothing to return");
            }
            _gameContainer.SingleInfo.Hand.RemoveSpecificItem(thisStock);
            await _gameContainer!.ShowCardAsync(thisStock);
            _gameContainer.GameStatus = EnumWhatStatus.NeedToEndTurn;
            await _gameContainer.ContinueTurnAsync!.Invoke();
        }
        private async Task RetirementProcessAsync()
        {
            var careerList = _gameContainer.SingleInfo!.GetCareerList();
            careerList.ForEach(thisCard =>
            {
                _gameContainer.SingleInfo!.Hand.RemoveSpecificItem(thisCard);
            });
            var thisSalary = _gameContainer.SingleInfo!.GetSalaryCard();
            _gameContainer.SingleInfo!.Hand.RemoveSpecificItem(thisSalary);
            _gameContainer.SingleInfo.HouseIsInsured = false;
            _gameContainer.SingleInfo.CarIsInsured = false;
            _gameContainer.SingleInfo.Salary = 0;
            RepayLoans();
            PopulatePlayerProcesses.FillInfo(_gameContainer.SingleInfo);
            if (_gameContainer.SingleInfo.GetHouseName() == "")
            {
                _gameContainer.GameStatus = EnumWhatStatus.NeedChooseRetirement;
            }
            else
            {
                _gameContainer.GameStatus = EnumWhatStatus.NeedSellHouse;
            }
            await _gameContainer.ContinueTurnAsync!.Invoke();
        }
        private void RepayLoans()
        {
            if (_gameContainer.SingleInfo!.Loans == 0)
                return;
            var numberLoans = _gameContainer.SingleInfo.Loans / 20000;
            _gameContainer.SingleInfo.Loans = 0;
            _gameContainer.TakeOutExpense(numberLoans * 25000);
        }
    }
}