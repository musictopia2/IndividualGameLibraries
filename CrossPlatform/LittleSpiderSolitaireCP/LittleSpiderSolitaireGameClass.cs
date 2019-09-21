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
using BasicGameFramework.BasicDrawables.Dictionary;

//i think this is the most common things i like to do
namespace LittleSpiderSolitaireCP
{
    [SingletonGame]
    public class LittleSpiderSolitaireGameClass : SolitaireGameClass<LittleSpiderSolitaireSaveInfo>
    {
        private readonly IBasicSolitaireVM _thisMod;
        public LittleSpiderSolitaireGameClass(IBasicSolitaireVM thisMod) : base(thisMod)
        {
            _thisMod = thisMod;
        }
        private WastePiles? _thisWaste;
        public override Task NewGameAsync()
        {
            if (_thisWaste == null)
                _thisWaste = _thisMod.MainContainer!.Resolve<WastePiles>();
            return base.NewGameAsync();
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

        public override void DrawCard()
        {
            var thisList = _thisMod.DeckPile!.DrawSeveralCards(8);
            _thisWaste!.AddCards(thisList);
        }
        protected override void AfterShuffleCards()
        {
            var firstKing = CardList.First(items => items.Value == EnumCardValueList.King);
            var thisList = new DeckRegularDict<SolitaireCard>
            {
                firstKing
            };
            CardList!.RemoveSpecificItem(firstKing);
            var nextKing = CardList.First(items => items.Value == EnumCardValueList.King && items.Color == firstKing.Color);
            CardList.RemoveSpecificItem(nextKing);
            thisList.Add(nextKing);
            var nextList = CardList.Where(items => items.Value == EnumCardValueList.LowAce && items.Color != firstKing.Color).ToRegularDeckDict();
            CardList.RemoveGivenList(nextList);
            thisList.AddRange(nextList);
            if (thisList.Count != 4)
                throw new BasicBlankException("Must have 4 cards for foundation");
            AfterShuffle(thisList);
            CardList.Clear();
        }
    }
}
