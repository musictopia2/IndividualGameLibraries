using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.Extensions;
using CommonBasicStandardLibraries.Exceptions;
using RookCP.Cards;
using RookCP.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RookCP.Logic
{
    public static class ComputerAI
    {
        public static async Task CardToBidAsync(RookVMData model, IBidProcesses processes)
        {
            if (await processes.CanPassAsync() == false)
            {
                model.BidChosen = model.Bid1!.NumberToChoose();
                return;
            }
            model.BidChosen = model.Bid1!.NumberToChoose(false);
        }
        public static void ColorToCall(RookVMData model)
        {
            var thisColor = model.Color1!.ItemToChoose();
            model.ColorChosen = thisColor;
        }
        public static int CardToPlay(RookMainGameClass mainGame, RookVMData model)
        {
            DeckRegularDict<RookCardInformation> newList;
            if (mainGame.SaveRoot!.DummyPlay)
                newList = model.Dummy1!.HandList.Where(items => mainGame.IsValidMove(items.Deck)).ToRegularDeckDict();
            else
                newList = mainGame.SingleInfo!.MainHandList.Where(items => mainGame.IsValidMove(items.Deck)).ToRegularDeckDict();
            if (newList.Count == 0)
                throw new BasicBlankException("There must be at least one card it can play for computer player");
            return newList.GetRandomItem().Deck;
        }
        public static DeckRegularDict<RookCardInformation> CardsToRemove(RookMainGameClass mainGame)
        {
            return mainGame.SingleInfo!.MainHandList.GetRandomList(false, 5).ToRegularDeckDict();
        }
    }
}
