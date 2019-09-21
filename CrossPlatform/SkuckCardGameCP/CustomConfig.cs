using BasicGameFramework.Attributes;
using BasicGameFramework.RegularDeckOfCards;
namespace SkuckCardGame
{
    [SingletonGame]
    public class CustomConfig : ISortCategory
    {
        public EnumSortCategory SortCategory => EnumSortCategory.SuitNumber;
    }
}