using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.RegularDeckOfCards;

namespace GolfCardGameCP.Data
{
    [SingletonGame]
    public class CustomConfig : ISortCategory
    {
        public EnumSortCategory SortCategory => EnumSortCategory.SuitNumber;
    }
}