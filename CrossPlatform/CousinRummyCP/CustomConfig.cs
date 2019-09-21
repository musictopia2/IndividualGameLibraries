using BasicGameFramework.Attributes;
using BasicGameFramework.RegularDeckOfCards;
namespace CousinRummyCP
{
    [SingletonGame]
    public class CustomConfig : ISortCategory
    {
        public EnumSortCategory SortCategory => EnumSortCategory.NumberSuit;
    }
}