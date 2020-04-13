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
using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommonInterfaces;
using EightOffSolitaireCP.Data;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.SolitaireClasses.MainClasses;
using BasicGameFrameworkLibrary.SolitaireClasses.DataClasses;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;

namespace EightOffSolitaireCP.Logic
{
    [SingletonGame]
    public class EightOffSolitaireMainGameClass : SolitaireGameClass<EightOffSolitaireSaveInfo>
    {
        public EightOffSolitaireMainGameClass(ISolitaireData solitaireData1, 
            ISaveSinglePlayerClass thisState, 
            IEventAggregator aggregator,
            IScoreData score
            )
            : base(solitaireData1, thisState, aggregator, score)
        {
        }
        //rethink if i need view model.  hopefully won't happen though.

        protected async override Task ContinueOpenSavedAsync()
        {
            //anything else you need will be here
            GlobalClass.MainModel!.ReservePiles1.HandList.ReplaceRange(SaveRoot.ReserveList);
            await base.ContinueOpenSavedAsync();
        }
        protected async override Task FinishSaveAsync()
        {
            //anything else to finish save will be here.
            SaveRoot.ReserveList = GlobalClass.MainModel!.ReservePiles1.HandList.ToRegularDeckDict();
            await base.FinishSaveAsync();
        }
        public override Task NewGameAsync()
        {
            GlobalClass.MainModel!.ReservePiles1!.ClearBoard();
            return base.NewGameAsync();
        }
        protected override SolitaireCard CardPlayed()
        {
            var thisCard = base.CardPlayed();
            if (thisCard.Deck > 0)
            {
                if (GlobalClass.MainModel!.ReservePiles1!.ObjectSelected() > 0)
                    return new SolitaireCard();
                return thisCard;
            }
            return GlobalClass.MainModel!.ReservePiles1!.GetCardSelected();
        }
        protected override void RemoveFromMiscPiles(SolitaireCard thisCard)
        {
            GlobalClass.MainModel!.ReservePiles1!.RemoveCard(thisCard);
        }

        //for now, no double click.  if we decide to support it, then will be here.

        protected override async Task<bool> HasOtherAsync(int pile)
        {
            int reserves = GlobalClass.MainModel!.ReservePiles1!.ObjectSelected();
            int wastes = _thisMod!.WastePiles1!.OneSelected();
            if (reserves > 0 && wastes > -1)
            {
                await UIPlatform.ShowMessageAsync("Must either select a reserve or a waste pile but not both");
                return true;
            }
            if (wastes == pile || wastes == -1 && reserves == 0)
            {
                _thisMod.WastePiles1.SelectUnselectPile(pile); //i think.
                return true;
            }
            if (reserves > 0)
            {
                if (_thisMod.WastePiles1.CanAddSingleCard(pile, GlobalClass.MainModel.ReservePiles1.GetCardSelected()) == false)
                {
                    await UIPlatform.ShowMessageAsync("Illegal Move");
                    return true;
                }
                var oldCard = GlobalClass.MainModel!.ReservePiles1.GetCardSelected();
                GlobalClass.MainModel!.ReservePiles1.RemoveCard(oldCard);
                _thisMod.WastePiles1.AddSingleCard(pile, oldCard);
                return true;
            }
            var thisCard = _thisMod.WastePiles1.GetCard();
            if (_thisMod.WastePiles1.CanAddSingleCard(pile, thisCard) == false)
            {
                await UIPlatform.ShowMessageAsync("Illegal Move");
                return true;
            }
            _thisMod.WastePiles1.MoveSingleCard(pile);
            return true;
        }
        public async Task AddToReserveAsync()
        {
            if (GlobalClass.MainModel!.ReservePiles1!.HowManyCards >= 8)
            {
                await UIPlatform.ShowMessageAsync("There can only be 8 cards to reserve.  Therefore, cannot add any more cards to reserve");
                return;
            }
            if (GlobalClass.MainModel.ReservePiles1.ObjectSelected() > 0)
            {
                await UIPlatform.ShowMessageAsync("There is already a card selected.  Unselect the card first before adding a card to reserve");
                return;
            }
            if (_thisMod!.WastePiles1!.OneSelected() == -1)
            {
                await UIPlatform.ShowMessageAsync("There is no card selected to add to reserve");
                return;
            }
            var thisCard = _thisMod.WastePiles1.GetCard();
            GlobalClass.MainModel!.ReservePiles1.AddCard(thisCard);
            _thisMod.WastePiles1.RemoveSingleCard();
        }
        protected override void AfterShuffleCards()
        {
            var aceList = GetAceList();
            AfterShuffle(aceList);
        }


    }
}
