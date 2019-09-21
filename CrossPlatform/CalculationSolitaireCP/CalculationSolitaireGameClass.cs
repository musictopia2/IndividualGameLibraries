using BaseSolitaireClassesCP.BasicVMInterfaces;
using BaseSolitaireClassesCP.Cards;
using BaseSolitaireClassesCP.MainClasses;
using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.Extensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace CalculationSolitaireCP
{
    [SingletonGame]
    public class CalculationSolitaireGameClass : SolitaireGameClass<CalculationSolitaireSaveInfo>
    {
        public CalculationSolitaireGameClass(IBasicSolitaireVM thisMod) : base(thisMod) { }
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
            DeckRegularDict<SolitaireCard> output = new DeckRegularDict<SolitaireCard>();
            4.Times(x =>
            {
                var temps = CardList.Where(items => (int)items.Value == x).ToRegularDeckDict();
                if (temps.Count != 4)
                    throw new BasicBlankException("There must be 4 cards");
                var thisCard = temps.GetRandomItem();
                output.Add(thisCard);
                CardList!.RemoveSpecificItem(thisCard);
            });
            AfterShuffle(output);
            CardList!.Clear();
        }
    }
}