using BasicGameFramework.Attributes;
using BasicGameFramework.RegularDeckOfCards;
namespace Rummy500CP
{
    [SingletonGame]
    public class CustomConfig : ISortCategory
    {
        public EnumSortCategory SortCategory => EnumSortCategory.NumberSuit;
    }
}