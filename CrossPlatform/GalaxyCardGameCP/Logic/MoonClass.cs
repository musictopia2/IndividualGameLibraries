using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.SpecializedGameTypes.RummyClasses;
using CommonBasicStandardLibraries.Exceptions;
using GalaxyCardGameCP.Cards;
using GalaxyCardGameCP.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalaxyCardGameCP.Logic
{
    public class MoonClass : SetInfo<EnumSuitList, EnumColorList, GalaxyCardGameCardInformation, SavedSet>
    {
        public EnumWhatSets WhatSet { get; set; }
        public MoonClass(CommandContainer command) : base(command) { }
        public void CreateNewMoon(IDeckDict<GalaxyCardGameCardInformation> thisCol, EnumWhatSets whatSet)
        {
            WhatSet = whatSet;
            if (WhatSet == 0)
                throw new BasicBlankException("WhatSet cannot be 0 when creating a set");
            thisCol = thisCol.OrderBy(items => items.Value).ToRegularDeckDict();
            thisCol.ForEach(thisCard =>
            {
                thisCard.Drew = false;
                thisCard.IsSelected = false;
            });
            HandList.ReplaceRange(thisCol);
        }
        public void AddCard(GalaxyCardGameCardInformation thisCard)
        {
            GalaxyCardGameCardInformation originalCard = HandList.First();
            thisCard.Drew = false;
            thisCard.IsSelected = false;
            if (WhatSet == EnumWhatSets.Kinds)
            {
                HandList.Add(thisCard);
                return;
            }
            if ((int)thisCard.Value == (int)originalCard.Value - 1)
                HandList.InsertBeginning(thisCard);
            else
                HandList.Add(thisCard);
        }
        public bool CanExpand(GalaxyCardGameCardInformation thisCard)
        {
            var originalCard = HandList.First();
            if (WhatSet == EnumWhatSets.Kinds)
            {
                return thisCard.Value == originalCard.Value;
            }
            if (originalCard.Suit != thisCard.Suit)
                return false;
            if ((int)thisCard.Value == (int)originalCard.Value - 1)
                return true;
            originalCard = HandList.Last();
            return (int)thisCard.Value == (int)originalCard.Value + 1;
        }
        public override void LoadSet(SavedSet Object)
        {
            WhatSet = Object.WhatSet;
            HandList.ReplaceRange(Object.CardList);
        }
        public override SavedSet SavedSet()
        {
            SavedSet output = new SavedSet();
            output.WhatSet = WhatSet;
            output.CardList = HandList.ToRegularDeckDict();
            return output;
        }
    }
}
