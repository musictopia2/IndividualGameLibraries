using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.RegularDeckOfCards;

namespace ChinazoCP.Data
{
    [SingletonGame]
    public class CustomConfig : ISortCategory
    {
        public EnumSortCategory SortCategory => EnumSortCategory.NumberSuit;
    }
}