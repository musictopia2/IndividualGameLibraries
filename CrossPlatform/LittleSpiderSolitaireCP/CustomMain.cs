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
namespace LittleSpiderSolitaireCP
{
    public class CustomMain : MainPilesCP
    {
        private readonly LittleSpiderSolitaireViewModel _thisMod;
        public CustomMain(IBasicScoreVM thisMod) : base(thisMod)
        {
            _thisMod = (LittleSpiderSolitaireViewModel)thisMod;
        }
        public override bool CanAddCard(int pile, SolitaireCard thisCard)
        {
            if (_thisMod.DeckPile!.IsEndOfDeck() && pile > 1)
                return base.CanAddCard(pile, thisCard);
            if (_thisMod.DeckPile.IsEndOfDeck())
                return CanBuildFromKing(pile, thisCard);
            if (_thisMod.WastePiles1!.OneSelected() == -1)
                throw new BasicBlankException($"No card selected for placing to {pile}");
            if (_thisMod.WastePiles1.OneSelected() <= 3)
            {
                if (pile > 1)
                    return base.CanAddCard(pile, thisCard);
                return CanBuildFromKing(pile, thisCard);
            }
            int oldPile = _thisMod.WastePiles1.OneSelected() - 4;
            if (oldPile != pile)
                return false;
            if (pile > 1)
                return base.CanAddCard(pile, thisCard);
            return CanBuildFromKing(pile, thisCard);
        }
        private bool CanBuildFromKing(int pile, SolitaireCard thisCard)
        {
            if (pile > 1)
                throw new BasicBlankException($"Kings must be 0, or 1; not {pile}");
            var oldCard = Piles.GetLastCard(pile);
            if (oldCard.Suit != thisCard.Suit)
                return false;
            return oldCard.Value - 1 == thisCard.Value;
        }
    }
}
