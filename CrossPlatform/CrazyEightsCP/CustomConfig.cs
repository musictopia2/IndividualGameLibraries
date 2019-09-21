using BasicGameFramework.Attributes;
using BasicGameFramework.RegularDeckOfCards;
namespace CrazyEights
{
    [SingletonGame]
    public class CustomConfig : ISortCategory
    {
        public EnumSortCategory SortCategory => EnumSortCategory.SuitNumber;
    }
}
