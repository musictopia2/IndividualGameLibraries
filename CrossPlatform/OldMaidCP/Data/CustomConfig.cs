using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using System;
using System.Collections.Generic;
using System.Text;

namespace OldMaidCP.Data
{
    [SingletonGame]
    public class CustomConfig : ISortCategory
    {
        public EnumSortCategory SortCategory => EnumSortCategory.NumberSuit;
    }
}