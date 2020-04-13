using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.ColorCards;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.SpecializedGameTypes.RummyClasses;
using Phase10CP.Cards;
using Phase10CP.Data;
using System.Linq;

namespace Phase10CP.SetClasses
{
    public class PhaseSet : SetInfo<EnumColorTypes, EnumColorTypes, Phase10CardInformation, SavedSet>
    {
        private EnumWhatSets _whatSet;
        //private readonly BasicData _thisData;
        public void CreateSet(IDeckDict<Phase10CardInformation> thisCol, EnumWhatSets whatType)
        {
            _whatSet = whatType;
            thisCol.ForEach(items =>
            {
                items.Drew = false;
                items.IsSelected = false;
            });
            if (_whatSet != EnumWhatSets.Runs)
            {
                HandList.ReplaceRange(thisCol);
                return;
            }
            DeckRegularDict<Phase10CardInformation> tempList = thisCol.ToRegularDeckDict();
            DeckRegularDict<Phase10CardInformation> wildCol = thisCol.Where(items => items.CardCategory == EnumCardCategory.Wild).ToRegularDeckDict();
            thisCol.KeepConditionalItems(items => items.CardCategory == EnumCardCategory.None);
            int firstNum = thisCol.First().Number;
            int whatFirst = firstNum;
            int lastNum = thisCol.Last().Number;
            int x;
            var loopTo = thisCol.Count;
            Phase10CardInformation thisCard;
            for (x = 2; x <= loopTo; x++)
            {
                firstNum += 1;
                thisCard = thisCol[x - 1];
                if (thisCard.Number != firstNum)
                {
                    thisCard = wildCol.First();
                    thisCard.Number = firstNum; // will put back when new round (?)
                    wildCol.RemoveSpecificItem(thisCard);
                    x -= 1;
                }
            }
            if (wildCol.Count > 0)
            {
                lastNum += 1;
                for (x = lastNum; x <= 11; x++)
                {
                    if (wildCol.Count == 0)
                        break;
                    thisCard = wildCol.First();
                    thisCard.Number = x;
                    wildCol.RemoveSpecificItem(thisCard);
                }
                whatFirst -= 1;
                for (x = whatFirst; x >= 2; x += -1)
                {
                    if (wildCol.Count == 0)
                        break;
                    thisCard = wildCol.First();
                    thisCard.Number = x;
                    wildCol.RemoveSpecificItem(thisCard);
                }
            }
            var Fins = tempList.OrderBy(Items => Items.Number);
            HandList.ReplaceRange(Fins);
        }
        public void AddCard(Phase10CardInformation thisCard, int position)
        {
            thisCard.IsSelected = false;
            thisCard.Drew = false;
            if (position == 1)
            {
                if (_whatSet == EnumWhatSets.Runs && thisCard.CardCategory == EnumCardCategory.Wild)
                    thisCard.Number = HandList.First().Number - 1;
                HandList.InsertBeginning(thisCard);
            }
            else
            {
                if (_whatSet == EnumWhatSets.Runs && thisCard.CardCategory == EnumCardCategory.Wild)
                    thisCard.Number = HandList.Last().Number + 1;
                HandList.Add(thisCard);
            }
        }
        public int PositionToPlay(Phase10CardInformation thisCard, int position)
        {
            if (thisCard.CardCategory == EnumCardCategory.Skip)
                return 0;
            if (thisCard.CardCategory == EnumCardCategory.Wild)
            {
                if (_whatSet != EnumWhatSets.Runs)
                    return position;
                if (HandList.Count == 12)
                    return 0; //this can only contain 12 at the most period.
                if (HandList.First().Number == 1)
                    return 2;
                if (HandList.Last().Number == 12)
                    return 1;
                return position;
            }
            if (_whatSet == EnumWhatSets.Colors)
            {
                EnumColorTypes thisColor = HandList.First(Items => Items.CardCategory == EnumCardCategory.None).Color;
                if (thisColor == thisCard.Color)
                    return position;
                return 0; //you can't expand because wrong color.
            }
            if (_whatSet == EnumWhatSets.Kinds)
            {
                int thisNumber = HandList.First(Items => Items.CardCategory == EnumCardCategory.None).Number;
                if (thisNumber == thisCard.Number)
                    return position;
                return 0; //can't expand because wrong number.
            }
            int popNumber = HandList.First().Number;
            int bottomNumber = HandList.Last().Number;
            if (thisCard.Number == popNumber - 1)
                return 1;
            if (thisCard.Number == bottomNumber + 1)
                return 2;
            return 0;
        }

        public PhaseSet(CommandContainer command) : base(command)
        {
            CanExpandRuns = true;
        }
        protected override bool IsRun()
        {
            return _whatSet == EnumWhatSets.Runs;
        }
        protected override bool CanClickMainBoard()
        {
            return true; //maybe its okay now because of how behavior changed even for desktop now because of onuithread.
        }
        public override void LoadSet(SavedSet currentItem)
        {
            HandList = currentItem.CardList.ToObservableDeckDict();
            _whatSet = currentItem.WhatSet;
        }
        public override SavedSet SavedSet()
        {
            SavedSet output = new SavedSet();
            output.CardList = HandList.ToRegularDeckDict();
            output.WhatSet = _whatSet;
            return output;
        }
    }
}