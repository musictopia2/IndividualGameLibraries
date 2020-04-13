using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.RegularDeckOfCards;

namespace MonasteryCardGameCP.Data
{
    [SingletonGame]
    public class CustomConfig : ISortCategory
    {
        public EnumSortCategory SortCategory => EnumSortCategory.NumberSuit;
    }
}