using BasicGameFramework.Attributes;
using BasicGameFramework.RegularDeckOfCards;
namespace FourSuitRummyCP
{
    [SingletonGame]
    public class CustomConfig : ISortCategory
    {
        public EnumSortCategory SortCategory => EnumSortCategory.NumberSuit;
    }
}