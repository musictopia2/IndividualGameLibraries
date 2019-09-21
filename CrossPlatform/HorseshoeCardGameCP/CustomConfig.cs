using BasicGameFramework.Attributes;
using BasicGameFramework.RegularDeckOfCards;
namespace HorseshoeCardGameCP
{
    [SingletonGame]
    public class CustomConfig : ISortCategory
    {
        public EnumSortCategory SortCategory => EnumSortCategory.SuitNumber;
    }
}