using BasicGameFramework.Attributes;
using BasicGameFramework.RegularDeckOfCards;
namespace GoFishCP
{
    [SingletonGame]
    public class CustomConfig : ISortCategory
    {
        public EnumSortCategory SortCategory => EnumSortCategory.NumberSuit;
    }
}