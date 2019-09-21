using BasicGameFramework.Attributes;
using BasicGameFramework.RegularDeckOfCards;
namespace GalaxyCardGame
{
    [SingletonGame]
    public class CustomConfig : ISortCategory
    {
        public EnumSortCategory SortCategory => EnumSortCategory.SuitNumber;
    }
}