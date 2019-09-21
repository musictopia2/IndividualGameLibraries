using BasicGameFramework.Attributes;
using BasicGameFramework.RegularDeckOfCards;
namespace Spades2Player
{
    [SingletonGame]
    public class CustomConfig : ISortCategory
    {
        public EnumSortCategory SortCategory => EnumSortCategory.SuitNumber;
    }
}