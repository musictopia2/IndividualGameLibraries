using BasicGameFramework.Attributes;
using BasicGameFramework.RegularDeckOfCards;
namespace GermanWhist
{
    [SingletonGame]
    public class CustomConfig : ISortCategory
    {
        public EnumSortCategory SortCategory => EnumSortCategory.SuitNumber;
    }
}