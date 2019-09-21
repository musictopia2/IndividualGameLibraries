﻿using BaseSolitaireClassesCP.Cards;
using BaseSolitaireClassesCP.TriangleClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Collections.Generic;
using System.Linq;
namespace TriangleSolitaireCP
{
    public class TriangleBoard : TriangleViewModel
    {
        public override void LoadSavedTriangles(SavedTriangle thisT)
        {
            base.LoadSavedTriangles(thisT);
            if (CardList.Count != 15)
                throw new BasicBlankException("After loading saved game, must have 15 cards");
            if (PositionUI == null)
                return;
            PositionUI.RepositionCardsOnUI();
        }
        public TriangleBoard(ITriangleVM thisMod) : base(thisMod, 5) { }
        public void ClearCards(IEnumerable<SolitaireCard> thisCol)
        {
            if (thisCol.Count() != 15)
                throw new BasicBlankException("Must have 15 cards to place here");
            CardList.ReplaceRange(thisCol);
            ClearBoard();
            RecalculateEnables();
        }
        public void MakeInvisible(int deck)
        {
            var thisCard = CardList.Single(items => items.Deck == deck);
            thisCard.Visible = false;
            RecalculateEnables();
        }
        public int HowManyCardsLeft => CardList.Count(items => items.Visible == true);
    }
}