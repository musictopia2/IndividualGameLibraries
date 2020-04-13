using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using CommonBasicStandardLibraries.CollectionClasses;
using YahtzeeHandsDownCP.Cards;

namespace YahtzeeHandsDownCP.Logic
{
    public static class Extensions
    {
        public static CustomBasicList<ICard> GetInterfaceList(this IDeckDict<YahtzeeHandsDownCardInformation> thisList)
        {
            CustomBasicList<ICard> output = new CustomBasicList<ICard>();
            output.AddRange(thisList);
            return output;
        }
    }
}