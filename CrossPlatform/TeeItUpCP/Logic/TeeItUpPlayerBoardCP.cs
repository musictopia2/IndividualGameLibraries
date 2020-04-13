using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.DrawableListsViewModels;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;
using TeeItUpCP.Cards;
using TeeItUpCP.Data;

namespace TeeItUpCP.Logic
{
    public class TeeItUpPlayerBoardCP : GameBoardObservable<TeeItUpCardInformation>
    {
        private enum EnumColumnType
        {
            IsFrozen = 1,
            IsGone,
            IsUnknown,
            IsValid1, //first card is known
            IsValid2 //second card is known.
        }
        private TeeItUpCardInformation? _previousCard;
        private TeeItUpPlayerItem? _thisPlayer; //try this way.
        public int Player { get; private set; }
        private readonly TeeItUpGameContainer _gameContainer;

        public TeeItUpPlayerBoardCP(TeeItUpGameContainer gameContainer) : base(gameContainer.Command)
        {
            Rows = 2;
            Columns = 4;
            _gameContainer = gameContainer;
        }
        protected override async Task ClickProcessAsync(TeeItUpCardInformation thisObject)
        {
            if (_gameContainer.BoardClickedAsync == null)
            {
                throw new BasicBlankException("The board clicked async was never set up.  Rethink");
            }
            await _gameContainer.BoardClickedAsync.Invoke(_thisPlayer!, thisObject);
        }
        public void LoadBoard(TeeItUpPlayerItem thisPlayer)
        {
            _thisPlayer = thisPlayer;
            Player = _thisPlayer.Id;
            Text = $"{thisPlayer.NickName} Board"; //hopefully does not have to mark as isenabled to false (?)
            if (thisPlayer.MainHandList.Count != 8)
            {
                throw new BasicBlankException("Must have 8 cards for the list");
            }
            _previousCard = null;
            ObjectList.ReplaceRange(thisPlayer.MainHandList);

        }
        //public void ClearBoard(IDeckDict<TeeItUpCardInformation> thisCol)
        //{
        //    if (thisCol.Count != 8)
        //        throw new BasicBlankException("Must have 8 cards for the list");
        //    thisCol.ForEach(thisCard =>
        //    {
        //        thisCard.IsUnknown = true;
        //        thisCard.IsSelected = false;
        //        thisCard.Visible = true;
        //        thisCard.MulliganUsed = false;
        //    });
        //    _previousCard = null;
        //    ObjectList.ReplaceRange(thisCol);
        //}
        public bool CanStart => ObjectList.Count(items => items.IsUnknown == false) == 2;

        protected override void ChangeEnabled()
        {
            if (_gameContainer.SaveRoot!.GameStatus == EnumStatusType.Beginning)
            {
                if (CanStart == true)
                    IsEnabled = false; //hopefully this simple.
            }
        }
        private TeeItUpCardInformation GetCard(int deck)
        {
            //return _mainGame.DeckList!.GetSpecificItem(deck); //try this way now.  hopefully won't cause a different issue.
            return ObjectList.GetSpecificItem(deck); //try this way.
        }
        public void ChooseCard(int deck)
        {
            var thisCard = GetCard(deck);
            thisCard.IsUnknown = false;
        }
        public void UseFirstMulligan(int deck)
        {
            _previousCard = GetCard(deck);
            _previousCard.Visible = false;
        }
        public void ReplaceFirstWith(int deck)
        {
            var tempCard = _gameContainer.DeckList!.GetSpecificItem(deck);
            TradeObject(_previousCard!.Deck, tempCard);
        }
        public bool IsCardKnown(int deck)
        {
            var tempCard = GetCard(deck);
            return !tempCard.IsUnknown;
        }
        public bool IsSelf
        {
            get
            {
                if (_gameContainer.BasicData!.MultiPlayer == false)
                {
                    return Player == _gameContainer.WhoTurn;
                }
                return _thisPlayer!.PlayerCategory == EnumPlayerCategory.Self;
            }
        }
        public bool CanStealCard(int deck)
        {
            var thisCard = GetCard(deck);
            if (thisCard.IsMulligan == true)
                return false;
            if (IsSelf == true)
                return true;
            return !thisCard.IsUnknown;
        }
        public DeckRegularDict<TeeItUpCardInformation> GetFinalCards()
        {
            var tempList = ObjectList.Where(items => items.Visible == true).ToRegularDeckDict();
            tempList.ForEach(thisCard => thisCard.IsUnknown = false);
            return tempList;
        }
        public bool IsPartFrozen(int deck)
        {
            var thisCard = GetCard(deck);
            var (oldRow, column) = GetRowColumnData(thisCard);
            int newRow;
            if (oldRow == 1)
                newRow = 2;
            else
                newRow = 1;
            var newCard = GetObject(newRow, column);
            return thisCard.IsUnknown == false && newCard.IsUnknown == false;
        }
        public bool CanTradeCards(int oldDeck, int newDeck)
        {
            if (IsSelf == true)
            {
                return !IsPartFrozen(newDeck);
            }
            var thisCard = GetCard(oldDeck);
            if (thisCard.IsMulligan == true)
                return false;
            return CanStealCard(newDeck);
        }
        public bool IsMulliganValid(int deck)
        {
            var thisCard = GetCard(deck);
            if (thisCard.IsMulligan == false)
                throw new BasicBlankException("Not even a mulligan");
            return !thisCard.MulliganUsed;
        }
        public void TradeCard(int oldDeck, int newDeck)
        {
            TeeItUpCardInformation thisCard = new TeeItUpCardInformation();
            thisCard.Populate(newDeck);
            thisCard.IsUnknown = false;
            if (thisCard.IsMulligan == true)
                thisCard.MulliganUsed = true;
            if (ObjectList.ObjectExist(thisCard.Deck))
                return;
            //ObjectList.RemoveObjectByDeck(thisCard.Deck); //looks like causes deeper issues though.
            TradeObject(oldDeck, thisCard);
        }
        private EnumColumnType ColumnStatus(TeeItUpCardInformation oldCard, TeeItUpCardInformation newCard)
        {
            if (oldCard.Visible == false)
                return EnumColumnType.IsGone;
            if (oldCard.IsUnknown == false && newCard.IsUnknown == false)
                return EnumColumnType.IsFrozen;
            if (oldCard.IsUnknown == true && newCard.IsUnknown == true)
                return EnumColumnType.IsUnknown;
            if (oldCard.IsUnknown == false)
                return EnumColumnType.IsValid1;
            return EnumColumnType.IsValid2;
        }
        public void MatchCard(int oldDeck, out int newDeck)
        {
            newDeck = 0;
            TeeItUpCardInformation thisCard = new TeeItUpCardInformation();
            thisCard.Populate(oldDeck);
            int x;
            TeeItUpCardInformation firstCard;
            TeeItUpCardInformation secondCard;
            EnumColumnType statuss;
            int secondPoints;
            for (x = 1; x <= 4; x++)
            {
                firstCard = GetObject(1, x);
                secondCard = GetObject(2, x);
                statuss = ColumnStatus(firstCard, secondCard);
                if (statuss == EnumColumnType.IsValid1)
                    secondPoints = firstCard.Points;
                else if (statuss == EnumColumnType.IsValid2)
                    secondPoints = secondCard.Points;
                else
                    secondPoints = 10;
                if (secondPoints != 10)
                {
                    if (secondPoints == thisCard.Points)
                    {
                        if (statuss == EnumColumnType.IsValid1)
                            newDeck = secondCard.Deck;
                        else
                            newDeck = firstCard.Deck;
                        if (oldDeck == secondCard.Deck || oldDeck == firstCard.Deck)
                        {
                            firstCard.Visible = false;
                            secondCard.Visible = false;
                        }
                    }
                }
            }
            if (newDeck == 0)
                throw new BasicBlankException("When matching card, newdeck cannot be 0.  Find out what happened");
        }
        public bool IsFinished
        {
            get
            {
                var tempList = ObjectList.Where(items => items.Visible == true).ToRegularDeckDict();
                if (tempList.Count == 0)
                    return true;
                return tempList.All(items => items.IsUnknown == false);
            }
        }
        public void DoubleCheck()
        {
            if (ObjectList.Count != 8)
                throw new BasicBlankException($"Somehow does not have 8 cards.  Only had {ObjectList.Count} for {_thisPlayer!.NickName}");
        }
        public int ColumnMatched(int deck) //make it one based.
        {
            TeeItUpCardInformation thisCard = new TeeItUpCardInformation(); //try this way.
            thisCard.Populate(deck);
            if (thisCard.IsMulligan)
                return 0;
            int x;
            TeeItUpCardInformation firstCard;
            TeeItUpCardInformation secondCard;
            EnumColumnType statuss;
            int newPoints;
            bool newMulligan = false;
            if (ObjectList.Count != 8)
                throw new BasicBlankException($"Must have 8 cards, not {ObjectList.Count} when figuring out whether to match card");
            for (x = 1; x <= 4; x++)
            {
                try
                {
                    firstCard = GetObject(1, x);
                    secondCard = GetObject(2, x); //problem is here.
                }
                catch (Exception ex)
                {
                    throw new BasicBlankException($"Exception when getting object Method was columnmatched.  Message was {ex.Message}");
                }
                statuss = ColumnStatus(firstCard, secondCard);
                if (statuss == EnumColumnType.IsValid1)
                {
                    newPoints = firstCard.Points;
                    newMulligan = firstCard.IsMulligan;
                }
                else if (statuss == EnumColumnType.IsValid2)
                {
                    newPoints = secondCard.Points;
                    newMulligan = secondCard.IsMulligan;
                }
                else
                {
                    newPoints = 10;
                }

                if (newPoints != 10)
                {
                    if (newPoints == thisCard.Points && newMulligan == false && thisCard.IsMulligan == false)
                        return x;
                }
            }
            return 0;
        }
    }
}
