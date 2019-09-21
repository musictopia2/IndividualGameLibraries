using BasicGameFramework.Attributes;
using BasicGameFramework.RegularDeckOfCards;
namespace SixtySix2Player
{
    [SingletonGame]
    public class CustomConfig : ISortCategory
    {
        public EnumSortCategory SortCategory => EnumSortCategory.SuitNumber;
    }
}