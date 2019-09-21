using BasicGameFramework.BasicDrawables.Dictionary;
using CommonBasicStandardLibraries.CollectionClasses;
namespace YahtzeeHandsDownCP
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