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
namespace BakersDozenSolitaireCP
{
    [SingletonGame]
    public class BakersDozenSolitaireGameClass : SolitaireGameClass<BakersDozenSolitaireSaveInfo>
    {
        private readonly IBasicSolitaireVM _thisMod;
        public BakersDozenSolitaireGameClass(IBasicSolitaireVM thisMod) : base(thisMod)
        {
            _thisMod = thisMod;
        }

        protected override SolitaireCard CardPlayed()
        {
            var thisCard = base.CardPlayed();
            return thisCard;
            //if any changes, will be here.
        }

        protected override void AfterShuffleCards()
        {
            //BasicGameFramework.Extensions.SelectEnableDeckExtensions.ToRegularDeckDict
            var firstList = DeckList.Where(items => items.Value == EnumCardValueList.King).ToRegularDeckDict();
            DeckList.ReshuffleFirstObjects(firstList, 0, 12);
            CardList = DeckList.ToRegularDeckDict();
            _thisMod.MainPiles1!.ClearBoard();
            AfterShuffle();
        }
    }
}
