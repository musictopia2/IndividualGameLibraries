using BasicGameFramework.Attributes;
using BasicGameFramework.RegularDeckOfCards;
namespace RoundsCardGame
{
    [SingletonGame]
    public class CustomConfig : ISortCategory
    {
        public EnumSortCategory SortCategory => EnumSortCategory.SuitNumber;
    }
}