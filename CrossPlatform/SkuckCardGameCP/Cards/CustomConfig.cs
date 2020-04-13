using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.RegularDeckOfCards;

namespace SkuckCardGameCP.Cards
{
    //if you don't need, remove.
    [SingletonGame]
    public class CustomConfig : ISortCategory
    {
        public EnumSortCategory SortCategory => EnumSortCategory.SuitNumber;
    }
}