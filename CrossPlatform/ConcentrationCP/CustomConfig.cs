using BasicGameFramework.Attributes;
using BasicGameFramework.RegularDeckOfCards;
namespace ConcentrationCP
{
    [SingletonGame]
    public class CustomConfig : ISortCategory
    {
        public EnumSortCategory SortCategory => EnumSortCategory.SuitNumber;
    }
}