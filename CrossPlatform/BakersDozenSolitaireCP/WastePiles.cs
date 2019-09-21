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
using BaseSolitaireClassesCP.PileViewModels;
using BaseSolitaireClassesCP.Cards;
using BasicGameFramework.ViewModelInterfaces;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.RegularDeckOfCards;
using BaseSolitaireClassesCP.MiscClasses;
//i think this is the most common things i like to do
namespace BakersDozenSolitaireCP
{
    public class WastePiles : WastePilesCP
    {
        public WastePiles(IBasicGameVM thisMod) : base(thisMod)
        {
        }
        public override void ClearBoard(IDeckDict<SolitaireCard> thisCol)
        {
            base.ClearBoard(thisCol);
            //below is custom code.
            int x;
            int y = 0;
            for (x = 1; x <= 4; x++)
            {
                foreach (var thisPile in Piles.PileList)
                {
                    if ((x != 1) & (thisCol[y].Value == EnumCardValueList.King))
                        throw new BasicBlankException("A king must be put into the first row");
                    thisPile.CardList.Add(thisCol[y]);
                    y += 1;
                }
            }
        }
        public override bool CanAddSingleCard(int whichOne, SolitaireCard thisCard)
        {
            return false;
        }

        public override bool CanMoveCards(int whichOne, out int lastOne)
        {
            lastOne = -1; //until i figure out what else to do.
            return false;
        }

        public override bool CanMoveToAnotherPile(int whichOne)
        {
            SolitaireCard newCard;
            SolitaireCard oldCard;
            if (Piles.HasCardInColumn(whichOne) == false)
                return false;// cannot move because the empty piles are never filled
            oldCard = Piles.GetLastCard(whichOne);
            newCard = GetCard(); // i think
            return oldCard.Value - 1 == newCard.Value;
        }
    }
}
