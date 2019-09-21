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
namespace EightOffSolitaireCP
{
    [SingletonGame]
    public class EightOffSolitaireGameClass : SolitaireGameClass<EightOffSolitaireSaveInfo>
    {
        protected async override Task ContinueOpenSavedAsync()
        {
            //anything else you need will be here
            _thisMod.ReservePiles1!.HandList.ReplaceRange(SaveRoot.ReserveList);
            await base.ContinueOpenSavedAsync();
        }
        protected async override Task FinishSaveAsync()
        {
            //anything else to finish save will be here.
            SaveRoot.ReserveList = _thisMod.ReservePiles1!.HandList.ToRegularDeckDict();
            await base.FinishSaveAsync();
        }
        private readonly EightOffSolitaireViewModel _thisMod;
        public EightOffSolitaireGameClass(IBasicSolitaireVM thisMod) : base(thisMod)
        {
            _thisMod = (EightOffSolitaireViewModel)thisMod;
        }
        public override Task NewGameAsync()
        {
            _thisMod.ReservePiles1!.ClearBoard();
            return base.NewGameAsync();
        }
        protected override SolitaireCard CardPlayed()
        {
            var thisCard = base.CardPlayed();
            if (thisCard.Deck > 0)
            {
                if (_thisMod.ReservePiles1!.ObjectSelected() > 0)
                    return new SolitaireCard();
                return thisCard;
            }
            return _thisMod.ReservePiles1!.GetCardSelected();
        }
        protected override void RemoveFromMiscPiles(SolitaireCard thisCard)
        {
            _thisMod.ReservePiles1!.RemoveCard(thisCard);
        }

        //for now, no double click.  if we decide to support it, then will be here.

        protected override async Task<bool> HasOtherAsync(int pile)
        {
            int reserves = _thisMod.ReservePiles1!.ObjectSelected();
            int wastes = _thisMod.WastePiles1!.OneSelected();
            if (reserves > 0 && wastes > -1)
            {
                await _thisMod.ShowGameMessageAsync("Must either select a reserve or a waste pile but not both");
                return true;
            }
            if (wastes == pile || wastes == -1 && reserves == 0)
            {
                _thisMod.WastePiles1.SelectUnselectPile(pile); //i think.
                return true;
            }
            if (reserves > 0)
            {
                if (_thisMod.WastePiles1.CanAddSingleCard(pile, _thisMod.ReservePiles1.GetCardSelected()) == false)
                {
                    await _thisMod.ShowGameMessageAsync("Illegal Move");
                    return true;
                }
                var oldCard = _thisMod.ReservePiles1.GetCardSelected();
                _thisMod.ReservePiles1.RemoveCard(oldCard);
                _thisMod.WastePiles1.AddSingleCard(pile, oldCard);
                return true;
            }
            var thisCard = _thisMod.WastePiles1.GetCard();
            if (_thisMod.WastePiles1.CanAddSingleCard(pile, thisCard) == false)
            {
                await _thisMod.ShowGameMessageAsync("Illegal Move");
                return true;
            }
            _thisMod.WastePiles1.MoveSingleCard(pile);
            return true;
        }
        public async Task AddToReserveAsync()
        {
            if (_thisMod.ReservePiles1!.HowManyCards >= 8)
            {
                await _thisMod.ShowGameMessageAsync("There can only be 8 cards to reserve.  Therefore, cannot add any more cards to reserve");
                return;
            }
            if (_thisMod.ReservePiles1.ObjectSelected() > 0)
            {
                await _thisMod.ShowGameMessageAsync("There is already a card selected.  Unselect the card first before adding a card to reserve");
                return;
            }
            if (_thisMod.WastePiles1!.OneSelected() == -1)
            {
                await _thisMod.ShowGameMessageAsync("There is no card selected to add to reserve");
                return;
            }
            var thisCard = _thisMod.WastePiles1.GetCard();
            _thisMod.ReservePiles1.AddCard(thisCard);
            _thisMod.WastePiles1.RemoveSingleCard();
        }
        protected override void AfterShuffleCards()
        {
            var aceList = GetAceList();
            AfterShuffle(aceList);
        }
    }
}
