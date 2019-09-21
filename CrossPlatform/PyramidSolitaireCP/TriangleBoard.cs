using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BaseSolitaireClassesCP.TriangleClasses;
using System.Collections.Generic;
using BaseSolitaireClassesCP.Cards;
//i think this is the most common things i like to do
namespace PyramidSolitaireCP
{
    public class TriangleBoard : TriangleViewModel
    {

        private int FirstDeck
        {
            get => _maingame.SaveRoot.FirstDeck;
            set => _maingame.SaveRoot.FirstDeck = value;
        }
        private int SecondDeck
        {
            get => _maingame.SaveRoot.SecondDeck;
            set => _maingame.SaveRoot.SecondDeck = value;
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
        readonly PyramidSolitaireGameClass _maingame;
        public TriangleBoard(ITriangleVM thisMod) : base(thisMod, 7)
        {
            _maingame = thisMod.MainContainer!.Resolve<PyramidSolitaireGameClass>();
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