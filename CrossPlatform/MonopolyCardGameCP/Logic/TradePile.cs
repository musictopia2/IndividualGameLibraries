using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using MonopolyCardGameCP.Cards;
using MonopolyCardGameCP.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MonopolyCardGameCP.Logic
{
    public class TradePile : HandObservable<MonopolyCardGameCardInformation>
    {
        private bool _didProcess;
        public IMonopolyScroll? Scroll;
        public int GetPlayerIndex { get; }
        private readonly int _myID;
        private readonly MonopolyCardGameGameContainer _gameContainer;
        private readonly MonopolyCardGameVMData _model;

        public TradePile(MonopolyCardGameGameContainer gameContainer, MonopolyCardGameVMData model, int player) : base(gameContainer.Command)
        {
            _gameContainer = gameContainer;
            _model = model;
            GetPlayerIndex = player;
            AutoSelect = EnumAutoType.None;
            _myID = _gameContainer.PlayerList!.GetSelf().Id;
            Text = "Trade " + player; //hopefully okay.
            Visible = true;
        }
        public bool IsCardSelected(int deck)
        {
            var thisCard = HandList.GetSpecificItem(deck);
            return thisCard.IsSelected;
        }
        public void ScrollToBottom() // i think
        {
            if (Scroll == null == true || HandList.Count == 0)
                return;
            Scroll!.ScrollToBottom();
        }
        public void SelectPastPoint(int deck)
        {
            bool starts = false;
            HandList.ForEach(thisCard =>
            {
                if (thisCard.Deck == deck)
                    starts = true;
                thisCard.IsSelected = starts;
            });
            ScrollToBottom();
        }
        public void SelectSpecificNumberOfCards(int howMany)
        {
            int x;
            int y = 0;
            for (x = HandList.Count; x >= 1; x += -1)
            {
                HandList[x - 1].IsSelected = true;
                y += 1;
                if (y == howMany)
                    break;
            }
            ScrollToBottom();
        }
        public void ClearBoard(MonopolyCardGameCardInformation thisCard)
        {
            HandList.ReplaceAllWithGivenItem(thisCard);
            ScrollToBottom();
        }
        public void RemoveCards()
        {
            HandList.Clear();
            ScrollToBottom();
        }
        public void AddCard(int deck, bool isSelected = false)
        {
            MonopolyCardGameCardInformation thisCard;
            thisCard = _gameContainer.DeckList!.GetSpecificItem(deck);
            thisCard.IsSelected = isSelected;
            HandList.Add(thisCard); // not sure about scroll to bottom (?)
            ScrollToBottom(); // might as well.
        }
        public int HowManyCardsSelected() => HandList.Count(items => items.IsSelected);
        public DeckRegularDict<MonopolyCardGameCardInformation> GetSelectedItems => HandList.GetSelectedItems(); //hopefully this simple
        public void GetNumberOfItems(int howMany)
        {
            howMany.Times(x =>
            {
                var thisCard = HandList.Last();
                thisCard.Drew = true;
                HandList.RemoveSpecificItem(thisCard);
                _gameContainer.SingleInfo!.MainHandList.Add(thisCard);
            });
            if (_gameContainer.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                _gameContainer.SingleInfo.MainHandList.Sort();
        }

        private async Task ProcessSelfPileAsync()
        {
            int numselected;
            int numunselected;
            numselected = _model.PlayerHand1!.HowManySelectedObjects;
            numunselected = _model.PlayerHand1.HowManyUnselectedObjects;
            if (numselected == 0)
            {
                await UIPlatform.ShowMessageAsync("Sorry, you must select a card from your had to put to the trade pile");
                return;
            }
            if (_gameContainer.SaveRoot!.GameStatus == EnumWhatStatus.DrawOrTrade && numselected != 1)
            {
                await UIPlatform.ShowMessageAsync("Sorry, only one card can be put from your hand to the trade pile because noone traded with you in the last turn");
                return;
            }
            if (_gameContainer.SaveRoot.GameStatus == EnumWhatStatus.TradeOnly)
            {
                await UIPlatform.ShowMessageAsync("Sorry, you already selected a card.  Therefore, you have to choose who to trade with");
                return;
            }
            if (_gameContainer.SaveRoot.GameStatus == EnumWhatStatus.Either && numunselected < 9)
            {
                await UIPlatform.ShowMessageAsync("Sorry, you must leave at least 9 cards in your hand");
                return;
            }
            if (_gameContainer.SaveRoot.GameStatus == EnumWhatStatus.Discard && numunselected < 10)
            {
                await UIPlatform.ShowMessageAsync("Sorry, you cannot discard cards to make your hand equal to less than 10");
                return;
            }
            TradePile thisTrade;
            thisTrade = this;
            if (_gameContainer.SaveRoot.GameStatus == EnumWhatStatus.Discard)
                thisTrade.EndTurn();
            DeckRegularDict<MonopolyCardGameCardInformation> thisCol = _model.PlayerHand1.ListSelectedObjects(true);
            if (_gameContainer.BasicData!.MultiPlayer)
            {
                var newCol = thisCol.GetDeckListFromObjectList();
                await _gameContainer.Network!.SendAllAsync("trade1", newCol);
            }
            foreach (var thisCard in thisCol)
                thisTrade.AddCard(thisCard.Deck, true);
            thisTrade.ScrollToBottom();
            if (_gameContainer.SaveRoot.GameStatus == EnumWhatStatus.Either)
            {
                thisTrade.UnselectAllObjects();
                _model.PlayerHand1.UnselectAllObjects();
            }
            if (_gameContainer.SaveRoot.GameStatus == EnumWhatStatus.Discard && _gameContainer.SingleInfo!.MainHandList.Count == 10)
            {
                await _gameContainer.EndTurnAsync!.Invoke(); //hopefully this simple.
                return;
            }
            if (_gameContainer.SaveRoot.GameStatus == EnumWhatStatus.Discard)
            {
                return;
            }
            if (_gameContainer.SaveRoot.GameStatus == EnumWhatStatus.DrawOrTrade)
            {
                _gameContainer.SaveRoot.GameStatus = EnumWhatStatus.TradeOnly;
                return;
            }
            if (_gameContainer.SingleInfo!.MainHandList.Count == 9)
                _gameContainer.SaveRoot.GameStatus = EnumWhatStatus.TradeOnly;
            if (_gameContainer.SaveRoot.GameStatus == EnumWhatStatus.Either && _gameContainer.SingleInfo.MainHandList.Count < 10)
                _gameContainer.SaveRoot.GameStatus = EnumWhatStatus.TradeOnly;
        }
        private async Task ProcessOtherPilesAsync()
        {
            int numselected;
            MonopolyCardGamePlayerItem tempPlayer;
            tempPlayer = _gameContainer.PlayerList!.GetSelf();
            TradePile thisTrade;
            TradePile selfTrade;
            selfTrade = tempPlayer.TradePile!;
            numselected = selfTrade!.HowManyCardsSelected();
            if (numselected == 0)
            {
                await UIPlatform.ShowMessageAsync("Sorry, you must select a card to trade here");
                return;
            }
            if (_gameContainer.SaveRoot!.GameStatus == EnumWhatStatus.Discard)
            {
                await UIPlatform.ShowMessageAsync("Sorry, you must now discard to add to your trade pile");
                return;
            }
            tempPlayer = _gameContainer.PlayerList[GetPlayerIndex];
            thisTrade = tempPlayer.TradePile!;
            if (thisTrade.HandList.Count == 0)
            {
                await UIPlatform.ShowMessageAsync("Sorry, there is no cards in the trade pile to trade for");
                return;
            }
            if (numselected > thisTrade.HandList.Count)
            {
                await UIPlatform.ShowMessageAsync("Sorry, you cannot trade for more than what the other player has");
                return;
            }
            DeckRegularDict<MonopolyCardGameCardInformation> oldCol = selfTrade.GetSelectedItems;
            if (_gameContainer.BasicData!.MultiPlayer)
            {
                var ThisSend = new SendTrade() { Player = thisTrade.GetPlayerIndex };
                ThisSend.CardList = oldCol.GetDeckListFromObjectList();
                await _gameContainer.Network!.SendAllAsync("trade2", ThisSend);
            }
            if (_gameContainer.ProcessTrade == null)
            {
                throw new BasicBlankException("Nobody is processing trade.  Rethink");
            }
            if (_gameContainer.SortCards == null)
            {
                throw new BasicBlankException("Nobody is sorting cards.  Rethink");
            }
            _gameContainer.ProcessTrade(thisTrade, oldCol, selfTrade);
            _gameContainer.SortCards(); //hopefully that works.
            if (_gameContainer.SingleInfo!.MainHandList.Count == 10)
            {
                await _gameContainer.EndTurnAsync!.Invoke();
                return;
            }
            await _gameContainer.ContinueTurnAsync!.Invoke();
        }
        private async Task PlayerClickedAsync()
        {
            if (GetPlayerIndex == _myID)
            {
                await ProcessSelfPileAsync();
                return;
            }
            await ProcessOtherPilesAsync();
        }
        protected override async Task PrivateBoardSingleClickedAsync()
        {

            if (_gameContainer.BasicData!.IsXamarinForms)
            {
                if (HandList.Count > 0)
                    return;

            }
            if (_didProcess)
            {
                _didProcess = false;
                return;
            }
            await PlayerClickedAsync();
        }
        protected override async Task ProcessObjectClickedAsync(MonopolyCardGameCardInformation ThisObject, int Index)
        {
            _didProcess = true;
            if (_model.PlayerHand1!.HasSelectedObject() == false)
            {
                if (ThisObject.Deck != _model.AdditionalInfo1!.CurrentCard.Deck)
                {
                    _model.AdditionalInfo1.AdditionalInfo(ThisObject.Deck);
                    return;
                }
            }
            if (_myID != GetPlayerIndex)
            {
                await PlayerClickedAsync();
                return;
            }
            int deck = ThisObject.Deck;
            int nums = _model.PlayerHand1.HowManySelectedObjects;
            if (nums > 0)
            {
                await PlayerClickedAsync();
                return;
            }
            if (IsCardSelected(deck))
            {
                EndTurn();
                return;
            }
            SelectPastPoint(deck);
        }
    }
}
