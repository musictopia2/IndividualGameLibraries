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
namespace VegasSolitaireCP
{
    [SingletonGame]
    public class VegasSolitaireGameClass : SolitaireGameClass<VegasSolitaireSaveInfo>
    {
        private readonly VegasSolitaireViewModel _thisMod;

        public VegasSolitaireGameClass(IBasicSolitaireVM thisMod) : base(thisMod)
        {
            _thisMod = (VegasSolitaireViewModel)thisMod;
        }
        protected async override Task ContinueOpenSavedAsync()
        {
            //anything else you need will be here
            await base.ContinueOpenSavedAsync();
        }
        protected async override Task FinishSaveAsync()
        {
            //anything else to finish save will be here.
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
            //at this point; it already shuffled the cards.  now figure out what to do from here
            _thisMod.MainPiles1!.ClearBoard();
            _thisMod.Money -= 52;
            AfterShuffle();
        }
        protected override void AddToScore()
        {
            _thisMod.Money += 5;
        }
    }
}
