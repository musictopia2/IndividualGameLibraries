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
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.ViewModelInterfaces;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.Extensions;
namespace GolfCardGameCP
{
    public class Beginnings : GameBoardViewModel<RegularSimpleCard>
    {

        private readonly GolfCardGameViewModel thisMod;
        public Beginnings(IBasicGameVM ThisMod) : base(ThisMod)
        {
            Columns = 2;
            Rows = 2;
            HasFrame = true;
            Text = "Dealt Cards";
            IsEnabled = true;
            thisMod = (GolfCardGameViewModel)ThisMod;
        }
        private void FinishClearing()
        {
            thisMod!.GolfHand1!.Visible = false;
            thisMod.HiddenCards1!.Visible = false;
            thisMod.KnockedVisible = false;
            thisMod.Deck1!.Visible = false;
            thisMod.Pile1!.Visible = false;
            thisMod.Pile2!.Visible = false;
            thisMod.ChooseFirstCardsVisible = true;
            Visible = true;
            IsEnabled = true;
        }
        public void ClearBoard(IDeckDict<RegularSimpleCard> thisList)
        {
            if (thisList.Count != 4)
                throw new BasicBlankException("The card list must have 4 cards");
            thisList.ForEach(thisCard =>
            {
                thisCard.IsUnknown = true;
            });
            ObjectList.ReplaceRange(thisList);
            FinishClearing();
        }
        protected override Task ClickProcessAsync(RegularSimpleCard ThisObject)
        {
            ThisObject.IsSelected = !ThisObject.IsSelected;
            return Task.CompletedTask;
        }
        public bool CanContinue => ObjectList.Count(items => items.IsSelected == true) == 2;
        public void GetSelectInfo(out DeckRegularDict<RegularSimpleCard> selectList, out DeckRegularDict<RegularSimpleCard> unselectList)
        {
            selectList = ObjectList.Where(items => items.IsSelected == true).ToRegularDeckDict();
            unselectList = ObjectList.Where(items => items.IsSelected == false).ToRegularDeckDict();
            if (selectList.Count != 2 || unselectList.Count != 2)
                throw new BasicBlankException("There must be 2 selected and 2 unselected cards.  Find out what happened");
            thisMod.Deck1!.Visible = true;
            thisMod.Pile1!.Visible = true;
            thisMod.Pile2!.Visible = true;
            thisMod.ChooseFirstCardsVisible = false;
            thisMod.KnockedVisible = true;
            Visible = false;
        }
    }
}