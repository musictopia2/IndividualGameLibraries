using BasicGameFramework.Attributes;
using BasicGameFramework.RegularDeckOfCards;
namespace MonasteryCardGameCP
{
    [SingletonGame]
    public class CustomConfig : ISortCategory
    {
        public EnumSortCategory SortCategory => EnumSortCategory.NumberSuit;
    }
}