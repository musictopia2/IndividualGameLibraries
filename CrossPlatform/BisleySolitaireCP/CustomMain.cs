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
using BaseSolitaireClassesCP.PileInterfaces;
using BaseSolitaireClassesCP.Cards;
using BasicGameFramework.BasicDrawables.Dictionary;
using BaseSolitaireClassesCP.PileViewModels;
using BaseSolitaireClassesCP.BasicVMInterfaces;
using BasicGameFramework.RegularDeckOfCards;
//i think this is the most common things i like to do
namespace BisleySolitaireCP
{
    public class CustomMain : MainPilesCP
    {
        public override bool CanAddCard(int pile, SolitaireCard thisCard)
        {
            if (pile > 3)
                return base.CanAddCard(pile, thisCard);
            //for this; starts from kings and moves down
            if (Piles.HasCard(pile) == false)
                return thisCard.Value == EnumCardValueList.King;
            var oldCard = Piles.GetLastCard(pile);
            if (oldCard.Suit != thisCard.Suit)
                return false;
            return oldCard.Value == thisCard.Value + 1;
        }
        public override void ClearBoard(IDeckDict<SolitaireCard> thisList)
        {
            if (thisList.Count != CardsNeededToBegin)
                throw new BasicBlankException($"Needs {CardsNeededToBegin} not {thisList.Count}");
            if (CardsNeededToBegin != 4)
                throw new BasicBlankException("Cards To begin with must be 4");
            _thisMod.Score = thisList.Count;
            Piles.ClearBoard();
            var tempList = Piles.PileList.Skip(4).ToCustomBasicList();
            if (tempList.Count == 0)
                throw new BasicBlankException("There has to be 4 more piles left.  Rethink");
            tempList.ForEach(thisPile =>
            {
                thisPile.ObjectList.Add(thisList[tempList.IndexOf(thisPile)]);
            });
        }
        private readonly IBasicScoreVM _thisMod;
        public CustomMain(IBasicScoreVM thisMod) : base(thisMod)
        {
            _thisMod = thisMod;
        }
    }
}
