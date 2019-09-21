using BasicGameFramework.Attributes;
using BasicGameFramework.RegularDeckOfCards;
namespace DummyRummyCP
{
    [SingletonGame]
    public class CustomConfig : ISortCategory
    {
        public EnumSortCategory SortCategory => EnumSortCategory.NumberSuit;
    }
}