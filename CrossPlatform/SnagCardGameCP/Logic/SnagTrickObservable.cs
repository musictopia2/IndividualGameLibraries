using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.SpecializedGameTypes.TrickClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using SnagCardGameCP.Cards;
using SnagCardGameCP.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnagCardGameCP.Logic
{
    [SingletonGame] //hopefully no need for the other because its being done automatically.
    public class SnagTrickObservable : BasicTrickAreaObservable<EnumSuitList, SnagCardGameCardInformation>,
        ITrickPlay, IAdvancedTrickProcesses, IMultiplayerTrick<EnumSuitList, SnagCardGameCardInformation, SnagCardGamePlayerItem>
    {
        private readonly SnagCardGameGameContainer _gameContainer;

        public SnagTrickObservable(SnagCardGameGameContainer gameContainer) : base(gameContainer.Command, gameContainer.Aggregator)
        {
            _gameContainer = gameContainer;
        }

        public CustomBasicList<TrickCoordinate>? ViewList { get; set; }
        public void SelectCard(int deck)
        {
            SnagCardGameCardInformation thisCard = CardList.GetSpecificItem(deck);
            if (thisCard.IsSelected == true || thisCard.IsUnknown == true || thisCard.Visible == false)
                throw new BasicBlankException("Card already selecte or shows as unknown.  Rethink");
            thisCard.IsSelected = true;
        }
        public CustomBasicList<SnagCardGameCardInformation> ListCardsLeft()
        {
            var tempList = CardList.Where(items => items.Visible == true && items.IsUnknown == false).ToRegularDeckDict();
            if (tempList.Count == 4)
                throw new BasicBlankException("There can't be 4 cards left over");
            return tempList;
        }
        public int HowManyCardsLeft => CardList.Count(items => items.Visible == true && items.IsUnknown == false);
        public int GetLastCard => CardList.Single(items => items.Visible == true && items.IsUnknown == false).Deck;
        public void RemoveCard(int deck)
        {
            var thisCard = CardList.GetSpecificItem(deck);
            if (thisCard.Visible == false || thisCard.IsUnknown == true)
                throw new BasicBlankException("Cannot remove card because it was already unknown or not visible.  Rethink");
            thisCard.Visible = false;
        }
        private int GetCardIndex()
        {
            var thisC = (from items in ViewList
                         where items.Player == _gameContainer.WhoTurn && items.PossibleDummy == !_gameContainer.SaveRoot!.FirstCardPlayed
                         select items).Single(); // maybe its opposite (?)
            return ViewList!.IndexOf(thisC); // hopefully this simple (?)
        }
        private CustomBasicList<TrickCoordinate> GetCoordinateList()
        {
            if (_gameContainer.PlayerList.Count() != 2)
                throw new BasicBlankException("This is a 2 player game.");
            CustomBasicList<TrickCoordinate> output = new CustomBasicList<TrickCoordinate>();
            int y = _gameContainer.SelfPlayer;
            int x;
            TrickCoordinate thisPlayer;
            for (x = 1; x <= 2; x++)
            {
                thisPlayer = new TrickCoordinate();
                if (x == 1)
                {
                    thisPlayer.IsSelf = true;
                    thisPlayer.Column = 2;
                    thisPlayer.Text = "Your Hand:"; // had to summarize so it works better for mobile.
                }
                else
                {
                    thisPlayer.PossibleDummy = true;
                    thisPlayer.Column = 1;
                    thisPlayer.Text = "Your Bar:";
                }
                thisPlayer.Row = 2; // i think
                thisPlayer.Player = y;
                output.Add(thisPlayer);
            }
            if (y == 1)
                y = 2;
            else
                y = 1;
            for (x = 1; x <= 2; x++)
            {
                thisPlayer = new TrickCoordinate();
                thisPlayer.Row = 1;
                thisPlayer.Player = y;
                if (x == 1)
                {
                    thisPlayer.Column = 2;
                    thisPlayer.Text = "Opponent Hand:";
                }
                else
                {
                    thisPlayer.PossibleDummy = true; // forgot this part.
                    thisPlayer.Text = "Opponent Bar:";
                    thisPlayer.Column = 1;
                }
                output.Add(thisPlayer);
            }
            return output;
        }

        protected override async Task ProcessCardClickAsync(SnagCardGameCardInformation thisCard)
        {
            if (_gameContainer.TakeCardAsync == null)
            {
                throw new BasicBlankException("Nobody is handling takecardasync.  Rethink");
            }
            if (_gameContainer.SaveRoot!.GameStatus == EnumStatusList.ChooseCards)
            {
                if (thisCard.Visible == false || thisCard.IsUnknown == true)
                    return;
                
                await _gameContainer.TakeCardAsync(thisCard.Deck);
                return;
            }
            int index = CardList.IndexOf(thisCard);
            if (index > 1)
                return;
            if (_gameContainer.SaveRoot.FirstCardPlayed == true && index == 0)
            {
                await _gameContainer.CardClickedAsync!.Invoke();
                return;
            }
            if (_gameContainer.SaveRoot.FirstCardPlayed == false && index == 1)
            {
                await _gameContainer.CardClickedAsync!.Invoke();
                return;
            }
        }
        public void ClearBoard()
        {
            DeckRegularDict<SnagCardGameCardInformation> tempList = new DeckRegularDict<SnagCardGameCardInformation>();
            for (int x = 1; x <= 4; x++)
            {
                SnagCardGameCardInformation thisCard = new SnagCardGameCardInformation();
                thisCard.Populate(x);
                thisCard.Deck += 1000; //this was the workaround.
                thisCard.IsUnknown = true;
                thisCard.Visible = true;
                tempList.Add(thisCard); //hopefully this simple.
            }
            _gameContainer.SaveRoot!.TrickList.Clear();
            CardList.ReplaceRange(tempList); // hopefully its that simple.
            Visible = true; // now it is visible.
        }
        public void LoadGame()
        {
            var tempList = _gameContainer.SaveRoot!.TrickList.ToRegularDeckDict();
            ClearBoard();
            if (tempList.Count == 0)
                return;
            int index;
            int tempTurn;
            SnagCardGameCardInformation lastCard;
            tempTurn = _gameContainer.WhoTurn;
            DeckRegularDict<SnagCardGameCardInformation> otherList = new DeckRegularDict<SnagCardGameCardInformation>();
            int x = 0;
            tempList.ForEach(thisCard =>
            {
                if (thisCard.Player == 0)
                    throw new BasicBlankException("The Player Cannot Be 0");
                _gameContainer.WhoTurn = thisCard.Player;
                _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetWhoPlayer();
                x++;
                if (x == 1)
                    _gameContainer.SaveRoot.FirstCardPlayed = false;
                else
                    _gameContainer.SaveRoot.FirstCardPlayed = true;
                index = GetCardIndex();
                lastCard = _gameContainer.GetBrandNewCard(thisCard.Deck);
                lastCard.Player = thisCard.Player;
                lastCard.IsUnknown = thisCard.IsUnknown;
                lastCard.Visible = thisCard.Visible;
                TradeCard(index, lastCard);
                otherList.Add(lastCard); //i think
            });
            _gameContainer.SaveRoot.TrickList.ReplaceRange(otherList); //i think we have to do it this way this time.
            _gameContainer.SaveRoot.FirstCardPlayed = true;
            _gameContainer.WhoTurn = tempTurn;
        }
        public void FirstLoad()
        {
            if (_gameContainer.PlayerList.Count() != 2)
                throw new BasicBlankException("Must have 2 players only.");
            ViewList = GetCoordinateList();
            if (ViewList.First().IsSelf == false)
                throw new BasicBlankException("First must be self");
            int x;
            MaxPlayers = 4; //i think
            CardList.Clear();
            for (x = 1; x <= 4; x++)
            {
                SnagCardGameCardInformation newCard = new SnagCardGameCardInformation();
                newCard.Populate(x);
                newCard.IsUnknown = true;
                newCard.Deck = x + 1000; //try this way.  to at least make it work.
                CardList.Add(newCard); //i think
            }
        }
        public async Task PlayCardAsync(int deck)
        {
            SnagCardGameCardInformation thisCard = _gameContainer.GetSpecificCardFromDeck(deck);
            thisCard.Player = _gameContainer.WhoTurn;
            int index;
            index = GetCardIndex();
            if (index == -1)
                throw new BasicBlankException("Index cannot be -1");
            SnagCardGameCardInformation newCard;
            newCard = _gameContainer.GetBrandNewCard(deck);
            newCard.Player = _gameContainer.WhoTurn;
            newCard.Visible = true;
            _gameContainer.SaveRoot!.TrickList.Add(newCard);
            TradeCard(index, newCard); // try this
            if (_gameContainer.SaveRoot.TrickList.Count == 3)
                await _gameContainer.EndTrickAsync!.Invoke();
            else
                await _gameContainer.ContinueTrickAsync!.Invoke();
        }
        public Task AnimateWinAsync(int wins)
        {
            return Task.CompletedTask; //this does not have animations this time.
        }
        public SnagCardGamePlayerItem GetSpecificPlayer(int id)
        {
            return _gameContainer.PlayerList![id]; //i think.
        }

    }
}
