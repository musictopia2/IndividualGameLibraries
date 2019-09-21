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
namespace DemonSolitaireCP
{
    [SingletonGame]
    public class DemonSolitaireGameClass : SolitaireGameClass<DemonSolitaireSaveInfo>
    {
        private readonly DemonSolitaireViewModel _thisMod;
        public DemonSolitaireGameClass(IBasicSolitaireVM thisMod) : base(thisMod)
        {
            _thisMod = (DemonSolitaireViewModel)thisMod;
        }
        protected async override Task ContinueOpenSavedAsync()
        {
            //anything else you need will be here
            var newList = SaveRoot.HeelList.GetNewObjectListFromDeckList(DeckList);
            _thisMod.Heel1!.OriginalList(newList);
            await base.ContinueOpenSavedAsync();
        }
        protected async override Task FinishSaveAsync()
        {
            //anything else to finish save will be here.
            SaveRoot.HeelList = _thisMod.Heel1!.GetCardIntegers();
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
            _thisMod.Heel1!.OriginalList(thisCol);
            thisCol = CardList.Take(1).ToRegularDeckDict();
            CardList.RemoveRange(0, 1);
            AfterShuffle(thisCol);
            CardList.Clear();
        }
    }
}
