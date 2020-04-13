using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DrawableListsViewModels;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using CommonBasicStandardLibraries.Exceptions;
using GolfCardGameCP.Data;
using GolfCardGameCP.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GolfCardGameCP.CustomPiles
{
    public class Beginnings : GameBoardObservable<RegularSimpleCard>
    {


        public Beginnings(CommandContainer command) : base(command)
        {
            Columns = 2;
            Rows = 2;
            HasFrame = true;
            Text = "Dealt Cards";
            IsEnabled = true;
            Visible = true;
            IsEnabled = true;
        }
        //private void FinishClearing()
        //{
        //    thisMod!.GolfHand1!.Visible = false;
        //    thisMod.HiddenCards1!.Visible = false;
        //    thisMod.KnockedVisible = false;
        //    thisMod.Deck1!.Visible = false;
        //    thisMod.Pile1!.Visible = false;
        //    thisMod.Pile2!.Visible = false;
        //    thisMod.ChooseFirstCardsVisible = true;
        //    Visible = true;
        //    IsEnabled = true;
        //}
        public void ClearBoard(IDeckDict<RegularSimpleCard> thisList)
        {
            if (thisList.Count != 4)
                throw new BasicBlankException("The card list must have 4 cards");
            thisList.ForEach(thisCard =>
            {
                thisCard.IsUnknown = true;
            });
            ObjectList.ReplaceRange(thisList);
            //FinishClearing();
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



            //thisMod.Deck1!.Visible = true;
            //thisMod.Pile1!.Visible = true;
            //thisMod.Pile2!.Visible = true;
            //thisMod.ChooseFirstCardsVisible = false;
            //thisMod.KnockedVisible = true;
            //Visible = false;
        }
        //something else has to do the other parts.
    }
}
