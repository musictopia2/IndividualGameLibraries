using BasicGameFramework.Attributes;
using BasicGameFramework.RegularDeckOfCards;
namespace OldMaidCP
{
    [SingletonGame]
    public class CustomConfig : ISortCategory
    {
        public EnumSortCategory SortCategory => EnumSortCategory.NumberSuit;
    }
}