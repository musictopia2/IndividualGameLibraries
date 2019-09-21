using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
namespace ConcentrationCP
{
    public static class ComputerAI
    {
        public static int CardToTry(ConcentrationMainGameClass mainGame)
        {
            DeckRegularDict<RegularSimpleCard> computerList = mainGame.SaveRoot!.ComputerList;
            DeckRegularDict<RegularSimpleCard> output;
            if (computerList.Count == 0)
            {
                output = mainGame.ThisMod!.GameBoard1!.CardsLeft();
                return output.GetRandomItem().Deck;
            }
            output = PairToTry(computerList);
            if (output.Count == 0)
            {
                output = mainGame.ThisMod!.GameBoard1!.CardsLeft();
                return output.GetRandomItem().Deck;
            }

            bool rets1 = mainGame.ThisMod!.GameBoard1!.WasSelected(output.First().Deck);
            bool rets2 = mainGame.ThisMod.GameBoard1.WasSelected(output.Last().Deck);
            if (rets1 == true && rets2 == true)
                throw new BasicBlankException("Both items cannot be selected");
            if (rets1 == true)
                return output.Last().Deck;
            return output.First().Deck;
        }
        private static DeckRegularDict<RegularSimpleCard> PairToTry(DeckRegularDict<RegularSimpleCard> computerList)
        {
            DeckRegularDict<RegularSimpleCard> output = new DeckRegularDict<RegularSimpleCard>();
            foreach (var firstCard in computerList)
            {
                foreach (var secondCard in computerList)
                {
                    if (firstCard.Deck != secondCard.Deck)
                    {
                        if (firstCard.Value == secondCard.Value)
                        {
                            output.Add(firstCard);
                            output.Add(secondCard);
                            return output;
                        }
                    }
                }
            }
            return output;
        }
    }
}