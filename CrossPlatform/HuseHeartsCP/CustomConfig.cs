using BasicGameFramework.Attributes;
using BasicGameFramework.RegularDeckOfCards;
namespace HuseHearts
{
    [SingletonGame]
    public class CustomConfig : ISortCategory
    {
        public EnumSortCategory SortCategory => EnumSortCategory.SuitNumber;
    }
}