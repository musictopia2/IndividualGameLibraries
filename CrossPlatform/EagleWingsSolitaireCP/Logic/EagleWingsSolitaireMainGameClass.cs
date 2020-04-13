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
using EagleWingsSolitaireCP.Data;
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

namespace EagleWingsSolitaireCP.Logic
{
    [SingletonGame]
    public class EagleWingsSolitaireMainGameClass : SolitaireGameClass<EagleWingsSolitaireSaveInfo>
    {
        public EagleWingsSolitaireMainGameClass(ISolitaireData solitaireData1, 
            ISaveSinglePlayerClass thisState, 
            IEventAggregator aggregator,
            IScoreData score
            )
            : base(solitaireData1, thisState, aggregator, score)
        {
        }
        protected override SolitaireCard CardPlayed()
        {
            var thisCard = base.CardPlayed();
            if (thisCard.Deck > 0)
            {
                if (GlobalClass.MainModel!.Heel1.IsSelected)
                    return new SolitaireCard();
                return thisCard;
            }
            thisCard = GlobalClass.MainModel!.Heel1!.DrawCard();
            thisCard.IsSelected = false;
            return thisCard;
        }

        protected override async Task<bool> HasOtherAsync(int pile)
        {
            if (GlobalClass.MainModel!.Heel1!.CardsLeft() != 1 || GlobalClass.MainModel.Heel1.IsSelected == false)
                return await base.HasOtherAsync(pile);
            int wastes = _thisMod!.WastePiles1!.OneSelected();
            if (wastes > -1)
            {
                await UIPlatform.ShowMessageAsync("Can choose either the waste pile or from heel; but not both");
                return true;
            }
            await UIPlatform.ShowMessageAsync("Cannot play from heel to wing since its the last card");
            return true;
        }
        public async Task HeelToMainAsync()
        {
            if (_thisMod!.WastePiles1!.OneSelected() > -1)
                return;
            int index = ValidMainColumn(GlobalClass.MainModel!.Heel1.RevealCard());
            if (index == -1)
            {
                GlobalClass.MainModel.Heel1.IsSelected = false;
                return;
            }
            var thisCard = GlobalClass.MainModel.Heel1.DrawCard();
            await FinishAddingToMainPileAsync(index, thisCard);
        }

        protected override void AfterShuffleCards()
        {
            var thisCol = CardList.Take(13).ToRegularDeckDict();
            CardList!.RemoveRange(0, 13);
            GlobalClass.MainModel!.Heel1.DeckStyle = DeckObservablePile<SolitaireCard>.EnumStyleType.Unknown;
            GlobalClass.MainModel.Heel1.OriginalList(thisCol);
            thisCol = CardList.Take(1).ToRegularDeckDict();
            CardList.RemoveRange(0, 1);
            AfterShuffle(thisCol);
            CardList.Clear(); //try this way.  hopefully i won't regret this.  because otherwise, the new game goes not work.
        }

        protected async override Task ContinueOpenSavedAsync()
        {
            //anything else you need will be here
            var newList = SaveRoot.HeelList.GetNewObjectListFromDeckList(DeckList);
            GlobalClass.MainModel!.Heel1.OriginalList(newList);
            if (newList.Count == 1)
                GlobalClass.MainModel.Heel1.DeckStyle = DeckObservablePile<SolitaireCard>.EnumStyleType.AlwaysKnown;
            else
                GlobalClass.MainModel.Heel1.DeckStyle = DeckObservablePile<SolitaireCard>.EnumStyleType.Unknown;
            await base.ContinueOpenSavedAsync();
        }
        protected async override Task FinishSaveAsync()
        {
            //anything else to finish save will be here.
            SaveRoot.HeelList = GlobalClass.MainModel!.Heel1.GetCardIntegers();
            await base.FinishSaveAsync();
        }

    }
}
