using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.Extensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using PaydayCP.Cards;
using PaydayCP.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using vb = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.Constants;
namespace PaydayCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class MailProcesses : IMailProcesses
    {
        private readonly PaydayGameContainer _gameContainer;
        private readonly PaydayVMData _model;
        private readonly GameBoardProcesses _gameBoard;

        public MailProcesses(PaydayGameContainer gameContainer, PaydayVMData model, GameBoardProcesses gameBoard)
        {
            _gameContainer = gameContainer;
            _model = model;
            _gameBoard = gameBoard;
        }
        void IMailProcesses.PopulateMails()
        {
            PopulateMails();
        }
        private void PopulateMails()
        {
            var tempList = _gameContainer.SingleInfo!.Hand.GetMailOrDealList<MailCard>(EnumCardCategory.Mail);
            _model!.CurrentMailList!.HandList.ReplaceRange(tempList);
        }
        async Task IMailProcesses.ProcessMailAsync()
        {
            await ProcessMailAsync();
        }

        private async Task ReshuffleMailAsync()
        {
            var list = _gameContainer.SaveRoot!.OutCards.GetMailOrDealList<MailCard>(EnumCardCategory.Mail);
            list.ShuffleList();
            if (_gameContainer.BasicData!.MultiPlayer == true)
            {
                await _gameContainer.Network!.SendAllAsync("reshufflemaillist", list.GetDeckListFromObjectList());
            }
            await ReshuffleMailAsync(list);
            await ProcessMailAsync();
        }

        private async Task ProcessMailAsync()
        {
            if (_gameContainer.SaveRoot!.MailListLeft.Count == 0)
            {
                _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetWhoPlayer();
                bool toreshuffle;
                toreshuffle = _gameContainer.ShouldReshuffle();
                if (toreshuffle == false)
                {
                    _gameContainer.Check!.IsEnabled = true;
                    return;
                }
                await ReshuffleMailAsync();
                return;
            }
            var card = _gameContainer.SaveRoot.MailListLeft.First();
            _gameContainer.SaveRoot.MailListLeft.RemoveFirstItem();
            await ContinueMailProcessesAsync(card);
        }

        private async Task ReshuffleMailAsync(DeckRegularDict<MailCard> list)
        {
            await UIPlatform.ShowMessageAsync("Mail is being reshuffled");
            _gameContainer.SaveRoot!.MailListLeft = list;
            _gameContainer.SaveRoot.OutCards.RemoveAllOnly(items => items.Deck <= 24);
        }

        async Task IMailProcesses.ReshuffleMailAsync(DeckRegularDict<MailCard> list)
        {
            await ReshuffleMailAsync(list);
        }

        void IMailProcesses.SetUpMail()
        {
            CustomBasicList<int> list = _gameContainer.Random!.GenerateRandomList(47 + 24, 47, 25);
            if (list.Count != 47)
            {
                throw new BasicBlankException($"Must have 47 mail cards, not {list.Count} cards");
            }
            _gameContainer.SaveRoot!.MailListLeft.Clear();
            list.ForEach(index =>
            {
                MailCard thisCard = (MailCard)_gameContainer.GetCard(index);
                _gameContainer.SaveRoot.MailListLeft.Add(thisCard);
            });
        }

        private async Task ContinueMailProcessesAsync(MailCard currentMail)
        {
            _model.MailPile.AddCard(currentMail);
            _gameContainer.SaveRoot!.CurrentMail = currentMail;
            _gameContainer.SaveRoot.GameStatus = EnumStatus.ViewMail;
            decimal pays;
            switch (currentMail.MailType)
            {
                case EnumMailType.MadMoney:
                    _gameContainer.SaveRoot.Instructions = $"Please choose the player to collect {currentMail.AmountReceived.ToCurrency(0)} from";
                    _gameContainer.SaveRoot.GameStatus = EnumStatus.ChoosePlayer;
                    break;
                case EnumMailType.Charity:
                    pays = Math.Abs(currentMail.AmountReceived);
                    _gameContainer.SaveRoot.Instructions = $"{currentMail.Description} for charity. {vb.vbCrLf} Please pay {pays.ToCurrency(0)}";
                    if (_gameContainer.Test!.NoAnimations == false)
                    {
                        await _gameContainer.Delay!.DelaySeconds(2);
                    }
                    _gameContainer.ProcessExpense(_gameBoard, pays);
                    if (_gameContainer.Test.NoAnimations == false)
                    {
                        await _gameContainer.Delay!.DelaySeconds(1);
                    }
                    _gameContainer.SaveRoot.OutCards.Add(currentMail);
                    break;
                case EnumMailType.MoveAhead:
                    _gameContainer.SaveRoot.GameStatus = EnumStatus.DealOrBuy;
                    break;
                case EnumMailType.MonsterCharge:
                    pays = Math.Abs(currentMail.AmountReceived);
                    _gameContainer.SaveRoot.Instructions = $"You received a monster charge of {pays.ToCurrency(0)} {vb.vbCrLf}Pay at the end of the month.";
                    await MailBillsAsync(currentMail);
                    break;
                case EnumMailType.Bill:
                    pays = Math.Abs(currentMail.AmountReceived);
                    _gameContainer.SaveRoot.Instructions = $"You received a bill in the amount of {pays.ToCurrency(0)}{ vb.vbCrLf}Pay at the end of the month.";
                    await MailBillsAsync(currentMail);
                    break;
                case EnumMailType.PayNeighbor:
                    pays = Math.Abs(currentMail.AmountReceived);
                    _gameContainer.SaveRoot.Instructions = $"Please choose the player to pay {pays.ToCurrency(0)} to";
                    _gameContainer.SaveRoot.GameStatus = EnumStatus.ChoosePlayer;
                    break;
                default:
                    break;
            }
            await _gameContainer.ContinueTurnAsync!.Invoke();
        }

        private async Task MailBillsAsync(MailCard card)
        {
            _gameContainer.SaveRoot.GameStatus = EnumStatus.ViewMail;
            if (_gameContainer.Test!.NoAnimations == false)
            {
                await _gameContainer.Delay!.DelaySeconds(1);
            }
            //_model!.MailPile!.Visible = false;
            _gameContainer.SaveRoot!.CurrentMail = new MailCard();
            _gameContainer.SingleInfo!.Hand.Add(card);
            PopulateMails();
            _gameContainer.SaveRoot.GameStatus = EnumStatus.EndingTurn;
            if (_gameContainer.Test!.NoAnimations == false)
            {
                await _gameContainer.Delay!.DelaySeconds(2);
            }
        }
    }
}