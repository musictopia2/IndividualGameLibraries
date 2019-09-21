using BasicGameFramework.Attributes;
using BasicGameFramework.RegularDeckOfCards;
namespace Pinochle2Player
{
    [SingletonGame]
    public class CustomConfig : ISortCategory
    {
        public EnumSortCategory SortCategory => EnumSortCategory.NumberSuit;
    }
}