using BasicGameFramework.Attributes;
using BasicGameFramework.RegularDeckOfCards;
namespace ChinazoCP
{
    [SingletonGame]
    public class CustomConfig : ISortCategory
    {
        public EnumSortCategory SortCategory => EnumSortCategory.NumberSuit;
    }
}