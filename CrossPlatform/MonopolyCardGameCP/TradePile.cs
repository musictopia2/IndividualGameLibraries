using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
//i think this is the most common things i like to do
namespace MonopolyCardGameCP
{
    public class TradePile : HandViewModel<MonopolyCardGameCardInformation>
    {
        private bool _didProcess;
        public IMonopolyScroll? ThisScroll;
        public int GetPlayerIndex { get; }
        private readonly MonopolyCardGameViewModel _thisMod;
        private readonly MonopolyCardGameMainGameClass _mainGame;
        private readonly int _myID;
        public TradePile(IBasicGameVM thisMod, int player) : base(thisMod)
        {
            _thisMod = (MonopolyCardGameViewModel)thisMod;
            GetPlayerIndex = player;
            AutoSelect = EnumAutoType.None;
            _mainGame = thisMod.MainContainer!.Resolve<MonopolyCardGameMainGameClass>();
            _myID = _mainGame.PlayerList!.GetSelf().Id;
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
            if (ThisScroll == null == true || HandList.Count == 0)
                return;
            ThisScroll!.ScrollToBottom();
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
            thisCard = _mainGame.DeckList!.GetSpecificItem(deck);
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
                _mainGame.SingleInfo!.MainHandList.Add(thisCard);
            });
            if (_mainGame.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                _mainGame.SingleInfo.MainHandList.Sort();
        }

        private async Task ProcessSelfPileAsync()
        {
            int numselected;
            int numunselected;
            numselected = _thisMod.PlayerHand1!.HowManySelectedObjects;
            numunselected = _thisMod.PlayerHand1.HowManyUnselectedObjects;
            if (numselected == 0)
            {
                await _thisMod.ShowGameMessageAsync("Sorry, you must select a card from your had to put to the trade pile");
                return;
            }
            if (_mainGame.SaveRoot!.GameStatus == EnumWhatStatus.DrawOrTrade && numselected != 1)
            {
                await _thisMod.ShowGameMessageAsync("Sorry, only one card can be put from your hand to the trade pile because noone traded with you in the last turn");
                return;
            }
            if (_mainGame.SaveRoot.GameStatus == EnumWhatStatus.TradeOnly)
            {
                await _thisMod.ShowGameMessageAsync("Sorry, you already selected a card.  Therefore, you have to choose who to trade with");
                return;
            }
            if (_mainGame.SaveRoot.GameStatus == EnumWhatStatus.Either && numunselected < 9)
            {
                await _thisMod.ShowGameMessageAsync("Sorry, you must leave at least 9 cards in your hand");
                return;
            }
            if (_mainGame.SaveRoot.GameStatus == EnumWhatStatus.Discard && numunselected < 10)
            {
                await _thisMod.ShowGameMessageAsync("Sorry, you cannot discard cards to make your hand equal to less than 10");
                return;
            }
            TradePile thisTrade;
            thisTrade = this;
            if (_mainGame.SaveRoot.GameStatus == EnumWhatStatus.Discard)
                thisTrade.EndTurn();
            DeckRegularDict<MonopolyCardGameCardInformation> thisCol = _thisMod.PlayerHand1.ListSelectedObjects(true);
            if (_mainGame.ThisData!.MultiPlayer)
            {
                var newCol = thisCol.GetDeckListFromObjectList();
                await _mainGame.ThisNet!.SendAllAsync("trade1", newCol);
            }
            foreach (var thisCard in thisCol)
                thisTrade.AddCard(thisCard.Deck, true);
            thisTrade.ScrollToBottom();
            if (_mainGame.SaveRoot.GameStatus == EnumWhatStatus.Either)
            {
                thisTrade.UnselectAllObjects();
                _thisMod.PlayerHand1.UnselectAllObjects();
            }
            if (_mainGame.SaveRoot.GameStatus == EnumWhatStatus.Discard && _mainGame.SingleInfo!.MainHandList.Count == 10)
            {
                await _mainGame.EndTurnAsync(); //hopefully this simple.
                return;
            }
            if (_mainGame.SaveRoot.GameStatus == EnumWhatStatus.Discard)
            {
                return;
            }
            if (_mainGame.SaveRoot.GameStatus == EnumWhatStatus.DrawOrTrade)
            {
                _mainGame.SaveRoot.GameStatus = EnumWhatStatus.TradeOnly;
                return;
            }
            if (_mainGame.SingleInfo!.MainHandList.Count == 9)
                _mainGame.SaveRoot.GameStatus = EnumWhatStatus.TradeOnly;
            if (_mainGame.SaveRoot.GameStatus == EnumWhatStatus.Either && _mainGame.SingleInfo.MainHandList.Count < 10)
                _mainGame.SaveRoot.GameStatus = EnumWhatStatus.TradeOnly;
        }
        private async Task ProcessOtherPilesAsync()
        {
            int numselected;
            MonopolyCardGamePlayerItem TempPlayer;
            TempPlayer = _mainGame.PlayerList!.GetSelf();
            TradePile thisTrade;
            TradePile selfTrade;
            selfTrade = TempPlayer.TradePile!;
            numselected = selfTrade!.HowManyCardsSelected();
            if (numselected == 0)
            {
                await _thisMod.ShowGameMessageAsync("Sorry, you must select a card to trade here");
                return;
            }
            if (_mainGame.SaveRoot!.GameStatus == EnumWhatStatus.Discard)
            {
                await _thisMod.ShowGameMessageAsync("Sorry, you must now discard to add to your trade pile");
                return;
            }
            TempPlayer = _mainGame.PlayerList[GetPlayerIndex];
            thisTrade = TempPlayer.TradePile!;
            if (thisTrade.HandList.Count == 0)
            {
                await _thisMod.ShowGameMessageAsync("Sorry, there is no cards in the trade pile to trade for");
                return;
            }
            if (numselected > thisTrade.HandList.Count)
            {
                await _thisMod.ShowGameMessageAsync("Sorry, you cannot trade for more than what the other player has");
                return;
            }
            DeckRegularDict<MonopolyCardGameCardInformation> oldCol = selfTrade.GetSelectedItems;
            if (_mainGame.ThisData!.MultiPlayer)
            {
                var ThisSend = new SendTrade() { Player = thisTrade.GetPlayerIndex };
                ThisSend.CardList = oldCol.GetDeckListFromObjectList();
                await _mainGame.ThisNet!.SendAllAsync("trade2", ThisSend);
            }
            _mainGame.ProcessTrade(thisTrade, oldCol, selfTrade);
            _mainGame.SortCards(); //hopefully that works.
            if (_mainGame.SingleInfo!.MainHandList.Count == 10)
            {
                await _mainGame.EndTurnAsync();
                return;
            }
            await _mainGame.ContinueTurnAsync();
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

            if (_mainGame.ThisData!.IsXamarinForms)
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
            if (_thisMod.PlayerHand1!.HasSelectedObject() == false)
            {
                if (ThisObject.Deck != _thisMod.AdditionalInfo1!.CurrentCard.Deck)
                {
                    _thisMod.AdditionalInfo1.AdditionalInfo(ThisObject.Deck);
                    return;
                }
            }
            if (_myID != GetPlayerIndex)
            {
                await PlayerClickedAsync();
                return;
            }
            int deck = ThisObject.Deck;
            int nums = _thisMod.PlayerHand1.HowManySelectedObjects;
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