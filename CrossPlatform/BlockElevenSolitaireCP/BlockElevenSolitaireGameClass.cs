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
namespace BlockElevenSolitaireCP
{
    [SingletonGame]
    public class BlockElevenSolitaireGameClass : SolitaireGameClass<BlockElevenSolitaireSaveInfo>
    {
        private readonly IBasicSolitaireVM _thisMod;
        public BlockElevenSolitaireGameClass(IBasicSolitaireVM thisMod) : base(thisMod)
        {
            _thisMod = thisMod;
        }
        public override Task NewGameAsync()
        {
            _thisMod.MainPiles1!.SetSavedScore(0);
            _thisMod.DeckPile!.ClearCards(); //just in case.
            return base.NewGameAsync();
        }
        protected override bool HasWon(int scores)
        {
            return scores == 40;
        }
        protected override void AfterMoveSingleCard()
        {
            _thisMod.Score += 2; //try this way.
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
            var thisList = CardList.Where(items => items.Value <= EnumCardValueList.Ten).Take(12).ToRegularDeckDict();
            CardList!.RemoveGivenList(thisList);
            var thisCard = CardList.Where(items => items.Value >= EnumCardValueList.Jack).Take(1).Single();
            CardList.RemoveSpecificItem(thisCard);
            thisList.ForEach(tempCard => CardList.InsertBeginning(tempCard));
            CardList.Add(thisCard); //put to last.
            AfterShuffle();
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
        
    }
}