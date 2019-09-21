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
using BasicGameFramework.DrawableListsViewModels;
using BaseSolitaireClassesCP.Cards;
using BasicGameFramework.ViewModelInterfaces;
//i think this is the most common things i like to do
namespace PyramidSolitaireCP
{
    public class PlayList : GameBoardViewModel<SolitaireCard>
    {
        public bool AlreadyHasTwoCards() => ObjectList.Count == 2;
        public bool HasChosenCards() => ObjectList.Count != 0;
        public void RemoveOneCard(SolitaireCard thisCard) => ObjectList.RemoveObjectByDeck(thisCard.Deck);
        public void AddCard(SolitaireCard thisCard)
        {
            if (AlreadyHasTwoCards())
                throw new BasicBlankException("Already has two cards.  Therefore, no cards can be added");
            var newCard = new SolitaireCard();
            newCard.Populate(thisCard.Deck); //to clone.
            newCard.Visible = true; //to double check.
            ObjectList.Add(newCard);
        }
        public void RemoveCards() => ObjectList.Clear();
        private readonly PyramidSolitaireViewModel _thisMod;
        public PlayList(IBasicGameVM ThisMod) : base(ThisMod)
        {
            _thisMod = (PyramidSolitaireViewModel)ThisMod;
            Rows = 1;
            Columns = 2;
            HasFrame = true;
            Text = "Chosen Cards";
        }

        protected override async Task ClickProcessAsync(SolitaireCard ThisObject)
        {
            if (AlreadyHasTwoCards())
            {
                await _thisMod.ShowGameMessageAsync("Sorry, 2 has already been selected");
                return;
            }
            AddCard(ThisObject);
            _thisMod.GameBoard1!.MakeInvisible(ThisObject.Deck);
        }
    }
}
