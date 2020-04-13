using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGameFrameworkLibrary.SolitaireClasses.TriangleClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Collections.Generic;
using System.Linq;
namespace PyramidSolitaireCP.Logic
{
    public class TriangleBoard : TriangleObservable
    {
        private readonly PyramidSolitaireMainGameClass _mainGame;

        public TriangleBoard(ITriangleVM thisMod,
            CommandContainer command,
            IGamePackageResolver resolver,
            PyramidSolitaireMainGameClass mainGame
            ) : base(thisMod, command, resolver, 7)
        {
            _mainGame = mainGame;
        }

        private int FirstDeck
        {
            get => _mainGame.SaveRoot.FirstDeck;
            set => _mainGame.SaveRoot.FirstDeck = value;
        }
        private int SecondDeck
        {
            get => _mainGame.SaveRoot.SecondDeck;
            set => _mainGame.SaveRoot.SecondDeck = value;
        }
        public void ClearCards(IEnumerable<SolitaireCard> thisCol)
        {
            if (thisCol.Count() != 28)
                throw new BasicBlankException("Must have 28 cards to place here");
            CardList.ReplaceRange(thisCol);
            ClearBoard();
            RecalculateEnables();
        }
        public void MakeInvisible(int deck)
        {
            var thisCard = CardList.Single(items => items.Deck == deck);
            thisCard.Visible = false;
            if (FirstDeck > 0 && SecondDeck > 0)
                throw new BasicBlankException("There can only be 2 images selected");
            if (FirstDeck == 0)
                FirstDeck = deck;
            else
                SecondDeck = deck;
            RecalculateEnables();
        }
        public void MakePermanant()
        {
            FirstDeck = 0;
            SecondDeck = 0;
        }
        private SolitaireCard GetDeckCard(int deck) => CardList.Single(items => items.Deck == deck);
        public void PutBackAll()
        {
            SolitaireCard thisCard;
            if (FirstDeck > 0)
            {
                thisCard = GetDeckCard(FirstDeck);
                thisCard.Visible = true;
            }
            if (SecondDeck > 0)
            {
                thisCard = GetDeckCard(SecondDeck);
                thisCard.Visible = true;
            }
            MakePermanant();
            RecalculateEnables();
        }
        public override void LoadSavedTriangles(SavedTriangle thisT)
        {
            base.LoadSavedTriangles(thisT);
            if (CardList.Count != 28)
                throw new BasicBlankException("After loading saved game, must have 28 cards");
            if (PositionUI == null)
                return;
            PositionUI.RepositionCardsOnUI();
        }
        public void PutBackOne(int deck)
        {
            if (FirstDeck == 0 && SecondDeck == 0)
                throw new BasicBlankException("There was nothing previously selected at all");
            SolitaireCard thisCard;
            thisCard = GetDeckCard(deck);
            thisCard.Visible = true;
            if (FirstDeck > 0 && SecondDeck > 0)
            {
                if (FirstDeck != deck && SecondDeck != deck)
                    throw new BasicBlankException($"There was nothing previously selected with the deck of {deck}");

                if (FirstDeck == deck)
                {
                    FirstDeck = 0;
                    RecalculateEnables();
                    return;
                }
                SecondDeck = 0;
                RecalculateEnables();
                return;
            }
            if (FirstDeck > 0)
            {
                if (FirstDeck != deck)
                    throw new BasicBlankException($"There is no deck found for {deck}");
                FirstDeck = 0;
                RecalculateEnables();
                return;
            }
            if (SecondDeck != deck)
                throw new BasicBlankException($"There is no deck found for {deck}");
            SecondDeck = 0;
            RecalculateEnables();
            return;
        }
        public int WhichOneToPutBack(int deck)
        {
            if (FirstDeck == 0 && SecondDeck == 0)
                return 0;
            if (FirstDeck > 0 && SecondDeck > 0)
            {
                if (FirstDeck == deck)
                    return 1;
                if (SecondDeck == deck)
                    return 2;
                return 0;
            }
            if (FirstDeck > 0)
            {
                if (FirstDeck == deck)
                    return 1;
            }
            return 0;
        }
        public bool CanPutBack(int deck)
        {
            if (FirstDeck == 0 && SecondDeck == 0)
                return false;
            if (FirstDeck > 0 && SecondDeck > 0)
            {
                if (FirstDeck == deck)
                    return true;
                if (SecondDeck == deck)
                    return true;
                return false;
            }
            if (FirstDeck > 0)
            {
                if (FirstDeck == deck)
                    return true;
            }
            return false;
        }
    }
}
