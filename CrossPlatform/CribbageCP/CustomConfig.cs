using BasicGameFramework.Attributes;
using BasicGameFramework.RegularDeckOfCards;
namespace CribbageCP
{
    [SingletonGame]
    public class CustomConfig : ISortCategory
    {
        public EnumSortCategory SortCategory => EnumSortCategory.NumberSuit;
    }
}