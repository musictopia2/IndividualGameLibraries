using BasicGameFramework.Attributes;
using BasicGameFramework.RegularDeckOfCards;
namespace GolfCardGameCP
{
    [SingletonGame]
    public class CustomConfig : ISortCategory
    {
        public EnumSortCategory SortCategory => EnumSortCategory.SuitNumber;
    }
}