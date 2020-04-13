using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using System;
using System.Collections.Generic;
using System.Text;

namespace FourSuitRummyCP.Data
{
    public class SavedSet
    {
        public DeckRegularDict<RegularRummyCard> CardList = new DeckRegularDict<RegularRummyCard>();
    }
}