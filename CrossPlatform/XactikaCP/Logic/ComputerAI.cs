using BasicGameFrameworkLibrary.Extensions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Linq;
using XactikaCP.Data;

namespace XactikaCP.Logic
{
    public static class ComputerAI
    {
        public static int HowManyToBid(XactikaVMData model)
        {
            return model.Bid1.NumberToChoose();
        }
        public static int CardToPlay(XactikaMainGameClass mainGame)
        {
            var newList = mainGame!.SingleInfo!.MainHandList.Where(items => mainGame.IsValidMove(items.Deck)).ToRegularDeckDict();
            return newList.GetRandomItem().Deck;
        }
        public static EnumShapes GetShapeChosen()
        {
            CustomBasicList<int> tempList = new CustomBasicList<int> { 1, 2, 3, 4 };
            int output = tempList.GetRandomItem();
            return (EnumShapes)output;
        }
    }
}
