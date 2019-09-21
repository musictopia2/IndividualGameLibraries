using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.Extensions;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
namespace RookCP
{
    public class DummyHandCP : HandViewModel<RookCardInformation>
    {
        public DummyHandCP(IBasicGameVM thisMod) : base(thisMod)
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