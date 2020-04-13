using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.Extensions;
using CommonBasicStandardLibraries.Exceptions;
using RookCP.Cards;
using System.Linq;

namespace RookCP.Logic
{
    public class DummyHandCP : HandObservable<RookCardInformation>
    {
        public DummyHandCP(CommandContainer command) : base(command)
        {
            Visible = false;
            Text = "Dummy Hand";
        }
        public void FirstDummy()
        {
            Visible = true;
        }
        public void RemoveDummyCard(int deck)
        {
            HandList.RemoveObjectByDeck(deck);
        }
        public void MakeAllKnown()
        {
            HandList.MakeAllObjectsKnown();
        }
        public void LoadDummyCards(IDeckDict<RookCardInformation> thisList, RookMainGameClass mainGame)
        {
            if (thisList.Count != 12)
                throw new BasicBlankException("Must have 12 cards for dummy cards");
            if (mainGame.PlayerList.Count() == 3)
                throw new BasicBlankException("There is no dummy hand because there are 3 players already");
            int x = 0;
            thisList.ForEach(thisCard =>
            {
                x++;
                if (x < 7)
                    thisCard.IsUnknown = false;
                else
                    thisCard.IsUnknown = true;
            });
            HandList.ReplaceRange(thisList);
        }
    }
}
