using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.Extensions;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace RookCP
{
    public static class ComputerAI
    {
        public static async Task CardToBidAsync(RookMainGameClass mainGame)
        {
            if (await mainGame.CanPassAsync() == false)
            {
                mainGame.ThisMod!.BidChosen = mainGame.ThisMod.Bid1!.NumberToChoose();
                return;
            }
            mainGame.ThisMod!.BidChosen = mainGame.ThisMod.Bid1!.NumberToChoose(false);
        }
        public static void ColorToCall(RookMainGameClass mainGame)
        {
            var thisColor = mainGame.ThisMod!.Color1!.ItemToChoose();
            mainGame.ThisMod.ColorChosen = thisColor;
        }
        public static int CardToPlay(RookMainGameClass mainGame)
        {
            DeckRegularDict<RookCardInformation> newList;
            if (mainGame.SaveRoot!.DummyPlay)
                newList = mainGame.ThisMod!.Dummy1!.HandList.Where(items => mainGame.IsValidMove(items.Deck)).ToRegularDeckDict();
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