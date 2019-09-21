using BasicGameFramework.Attributes;
using BasicGameFramework.RegularDeckOfCards;
namespace Opetong
{
    [SingletonGame]
    public class CustomConfig : ISortCategory
    {
        public EnumSortCategory SortCategory => EnumSortCategory.NumberSuit;
    }
}