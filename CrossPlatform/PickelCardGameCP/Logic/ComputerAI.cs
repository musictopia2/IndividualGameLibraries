using BasicGameFrameworkLibrary.RegularDeckOfCards;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using PickelCardGameCP.Data;
using System.Linq;

namespace PickelCardGameCP.Logic
{
    public static class ComputerAI
    {
        public static int HowManyToBid(PickelCardGameVMData model, IBidProcesses processes)
        {
            if (processes.CanPass() == false)
                return model.Bid1!.NumberToChoose();
            return model.Bid1!.NumberToChoose(false);
        }
        public static EnumSuitList SuitToCall(PickelCardGameVMData model)
        {
            return model.Suit1!.ItemToChoose();
        }
        public static int CardToPlay(PickelCardGameMainGameClass mainGame)
        {
            var newList = mainGame.SingleInfo!.MainHandList.Where(items => mainGame.IsValidMove(items.Deck) == true).ToCustomBasicList();
            return newList.GetRandomItem().Deck;
        }
    }
}