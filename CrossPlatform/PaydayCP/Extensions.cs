using BasicGameFramework.BasicDrawables.Dictionary;
using System.Linq;
namespace PaydayCP
{
    public static class Extensions
    {
        public static DeckRegularDict<C> GetMailOrDealList<C>(this IDeckDict<CardInformation> tempList, EnumCardCategory whichType) where C : CardInformation, new()
        {

            var firstList = tempList.Where(items => items.CardCategory == whichType);
            DeckRegularDict<C> output = new DeckRegularDict<C>();
            foreach (var thisItem in firstList)
            {
                output.Add((C)thisItem);
            }
            return output;
        }
    }
}