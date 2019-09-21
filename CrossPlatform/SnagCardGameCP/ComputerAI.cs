using BasicGameFramework.Extensions;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
namespace SnagCardGameCP
{
    public static class ComputerAI
    {
        public static int CardToPlay(SnagCardGameMainGameClass mainGame)
        {
            if (mainGame.SaveRoot!.FirstCardPlayed == false)
            {
                var thisCol = mainGame.ThisMod!.Bar1!.PossibleList;
                if (thisCol.Count == 0)
                    throw new BasicBlankException("No cards left from bar");
                if (thisCol.Count > 2)
                    throw new BasicBlankException("There cannot be more than 2 cards to choose from for bar for computer player");
                return thisCol.GetRandomItem().Deck;
            }
            var possibleList = mainGame.SingleInfo!.MainHandList.Where(Items => mainGame.IsValidMove(Items.Deck)).ToRegularDeckDict();
            if (possibleList.Count == 0)
                throw new BasicBlankException("There cannot be 0 cards left");
            return possibleList.GetRandomItem().Deck;
        }
        public static int CardToTake(SnagCardGameMainGameClass mainGame)
        {
            var thisList = mainGame.TrickArea1!.ListCardsLeft();
            return thisList.GetRandomItem().Deck;
        }
    }
}
