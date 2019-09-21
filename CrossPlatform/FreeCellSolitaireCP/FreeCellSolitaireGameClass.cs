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
using BaseSolitaireClassesCP.MainClasses;
using BaseSolitaireClassesCP.BasicVMInterfaces;
using BaseSolitaireClassesCP.Cards;
using BasicGameFramework.Attributes;
using BasicGameFramework.Extensions;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.DIContainers;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;

//i think this is the most common things i like to do
namespace FreeCellSolitaireCP
{
    [SingletonGame]
    public class FreeCellSolitaireGameClass : SolitaireGameClass<FreeCellSolitaireSaveInfo>
    {


        private readonly FreeCellSolitaireViewModel _thisMod;
        public FreeCellSolitaireGameClass(IBasicSolitaireVM thisMod) : base(thisMod)
        {
            _thisMod = (FreeCellSolitaireViewModel)thisMod;
        }
        private WastePiles? _thisWaste;
        public override Task NewGameAsync()
        {
            if (_thisWaste == null)
                _thisWaste = (WastePiles)_thisMod.WastePiles1!;
            return base.NewGameAsync();
        }
        public async Task FreeSelectedAsync(int pile)
        {
            if (_thisMod.WastePiles1!.OneSelected() == -1)
            {
                if (_thisMod.FreePiles1!.OneSelected == pile)
                {
                    _thisMod.FreePiles1.SelectUnselectCard(pile);
                    return;
                }
                if (_thisMod.FreePiles1.OneSelected > -1)
                {
                    await _thisMod.ShowGameMessageAsync("Illegal move");
                    return;
                }
                if (_thisMod.FreePiles1.HasCard(pile) == false)
                {
                    await _thisMod.ShowGameMessageAsync("Illegal move");
                    return;
                }
                _thisMod.FreePiles1.SelectUnselectCard(pile);
                return;
            }
            if (_thisMod.FreePiles1!.HasCard(pile))
            {
                await _thisMod.ShowGameMessageAsync("Illegal move");
                return;
            }
            var thisCard = _thisMod.WastePiles1.GetCard();
            _thisMod.FreePiles1.AddCardToPile(pile, thisCard);
            _thisMod.WastePiles1.RemoveSingleCard();
        }
        private int HowMuchToEmpty()
        {
            int freePiles = _thisWaste!.FreePiles;
            int freeCells = _thisMod.FreePiles1!.HowManyFreeCells;
            if (freeCells == 0)
                throw new BasicBlankException("There was no free piles");
            if (freeCells == 1 && freePiles == 0)
                return 1;
            return freeCells + 1;
        }
        private int HowMuchRoom()
        {
            int freePiles;
            int freeCells;
            freePiles = _thisWaste!.FreePiles;
            freeCells = _thisMod.FreePiles1!.HowManyFreeCells;
            if (freePiles == 0 && freeCells == 0)
                return 1;
            if (freePiles == 0)
                return freeCells + 1;
            if ((freePiles == 1) & (freeCells == 0))
                return 2;
            if ((freePiles == 1) & (freeCells == 0))
                return 2;
            if ((freePiles == 1) & (freeCells == 1))
                return 4;
            if ((freeCells == 2) & (freePiles == 1))
                return 6;
            if ((freeCells == 3) & (freePiles == 1))
                return 8;
            if ((freeCells == 4) & (freePiles == 1))
                return 10;
            if (freePiles == 2 && freeCells == 1)
                return 6;
            if (freePiles == 2 && freeCells == 2)
                return 9;
            if (freePiles == 2 && freeCells == 3)
                return 12;
            if (freeCells == 2 && freeCells == 4)
                return 13;
            if (freePiles == 3 && freeCells == 1)
                return 8;
            if (freePiles == 3 && freeCells == 2)
                return 10;
            if (freePiles == 3 && freeCells == 3)
                return 12;
            if (freePiles == 4 && freeCells == 1)
                return 10;
            return 13;
        }
        protected override async Task<bool> HasOtherAsync(int pile)
        {
            if (_thisMod.WastePiles1!.OneSelected() == pile)
            {
                _thisMod.WastePiles1.SelectUnselectPile(pile);
                return true;
            }
            if (_thisMod.WastePiles1.OneSelected() == -1 && _thisMod.FreePiles1!.OneSelected == -1)
            {
                if (_thisMod.WastePiles1.HasCard(pile) == false)
                    return true;
                _thisMod.WastePiles1.SelectUnselectPile(pile);
                return true;
            }
            if (_thisMod.FreePiles1!.OneSelected > -1)
            {
                var thisCard = _thisMod.FreePiles1.GetCard();
                if (_thisMod.WastePiles1.CanAddSingleCard(pile, thisCard) == false)
                {
                    await _thisMod.ShowGameMessageAsync("Illegal move");
                    return true;
                }
                _thisMod.WastePiles1.AddSingleCard(pile, thisCard);
                _thisMod.FreePiles1.RemoveCard();
                return true;
            }
            int rooms;
            if (_thisMod.WastePiles1.HasCard(pile) == false)
                rooms = HowMuchToEmpty();
            else
                rooms = HowMuchRoom();
            if (_thisWaste!.CanMoveCards(pile, out int lasts, rooms) == false)
            {
                await _thisMod.ShowGameMessageAsync("Illegal move");
                return true;
            }
            _thisMod.WastePiles1.MoveCards(pile, lasts);
            return true;
        }
        protected override SolitaireCard CardPlayed()
        {
            var thisCard = base.CardPlayed();
            if (thisCard.Deck > 0)
                return thisCard;
            int ones = _thisMod.FreePiles1!.OneSelected;
            if (ones == -1)
                return new SolitaireCard();
            return _thisMod.FreePiles1.GetCard();
        }

        protected override void AfterShuffleCards()
        {
            _thisMod.FreePiles1!.ClearBoard();
            _thisMod.MainPiles1!.ClearBoard();
            AfterShuffle();
        }
        //we don't have double click for now so no need to go directly to main.

        protected override void RemoveFromMiscPiles(SolitaireCard thisCard)
        {
            _thisMod.FreePiles1!.RemoveCard();
        }

        protected async override Task ContinueOpenSavedAsync()
        {
            //anything else you need will be here
            _thisMod.FreePiles1!.PileList = SaveRoot.FreeCards.ToCustomBasicList();
            await base.ContinueOpenSavedAsync();
        }
        protected async override Task FinishSaveAsync()
        {
            //anything else to finish save will be here.
            SaveRoot.FreeCards = _thisMod.FreePiles1!.PileList.ToCustomBasicList();
            await base.FinishSaveAsync();
        }
       
    }
}