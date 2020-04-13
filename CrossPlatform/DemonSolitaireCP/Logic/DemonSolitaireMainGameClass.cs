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
using DemonSolitaireCP.Data;
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

namespace DemonSolitaireCP.Logic
{
    [SingletonGame]
    public class DemonSolitaireMainGameClass : SolitaireGameClass<DemonSolitaireSaveInfo>
    {
        public DemonSolitaireMainGameClass(ISolitaireData solitaireData1, 
            ISaveSinglePlayerClass thisState, 
            IEventAggregator aggregator,
            IScoreData score
            )
            : base(solitaireData1, thisState, aggregator, score)
        {
        }
        protected async override Task ContinueOpenSavedAsync()
        {
            //anything else you need will be here
            var newList = SaveRoot.HeelList.GetNewObjectListFromDeckList(DeckList);
            GlobalClass.MainModel!.Heel1.OriginalList(newList);
            await base.ContinueOpenSavedAsync();
        }
        protected async override Task FinishSaveAsync()
        {
            //anything else to finish save will be here.
            SaveRoot.HeelList = GlobalClass.MainModel!.Heel1.GetCardIntegers();
            await base.FinishSaveAsync();
        }
        protected override SolitaireCard CardPlayed()
        {
            var thisCard = base.CardPlayed();
            return thisCard;
            //if any changes, will be here.
        }

        protected override void AfterShuffleCards()
        {
            var thisCol = CardList.Take(13).ToRegularDeckDict();
            CardList!.RemoveRange(0, 13);
            GlobalClass.MainModel!.Heel1.OriginalList(thisCol);
            thisCol = CardList.Take(1).ToRegularDeckDict();
            CardList.RemoveRange(0, 1);
            AfterShuffle(thisCol);
            CardList.Clear();
        }

    }
}
