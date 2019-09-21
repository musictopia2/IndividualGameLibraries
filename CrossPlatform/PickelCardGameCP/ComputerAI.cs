using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
namespace PickelCardGameCP
{
    public static class ComputerAI
    {
        public static int HowManyToBid(PickelCardGameMainGameClass mainGame)
        {
            if (mainGame.CanPass() == false)
                return mainGame.ThisMod!.Bid1!.NumberToChoose();
            return mainGame.ThisMod!.Bid1!.NumberToChoose(false);
        }
        public static EnumSuitList SuitToCall(PickelCardGameMainGameClass mainGame)
        {
            return mainGame.ThisMod!.Suit1!.ItemToChoose();
        }
        public static int CardToPlay(PickelCardGameMainGameClass mainGame)
        {
            var newList = mainGame.SingleInfo!.MainHandList.Where(items => mainGame.IsValidMove(items.Deck) == true).ToCustomBasicList();
            return newList.GetRandomItem().Deck;
        }
    }
}