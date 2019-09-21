using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Collections.Generic;
using System.Linq;
namespace PokerCP
{
    public class ScoreInfo
    {
        public DeckRegularDict<PokerCardInfo>? CardList;

        public bool IsRoyalFlush()
        {
            if (CardList.Any(items => items.Value == EnumCardValueList.HighAce) == false)
                return false;
            return IsStraightFlush();
        }
        public bool IsStraightFlush()
        {
            return IsFlush() && IsStraight();
        }
        public bool IsFlush()
        {
            return CardList.GroupBy(items => items.Suit).Count() == 1;
        }
        public bool Kinds(int howMany)
        {
            var thisList = CardList.GroupOrderDescending(items => items.Value).ToCustomBasicList();
            return thisList.Any(items => items.Count() == howMany); //hopefully this simple now.
        }
        public bool IsFullHouse()
        {
            var thisList = CardList.GroupOrderDescending(items => items.Value).ToCustomBasicList();
            if (thisList.Count != 2)
                return false;
            return thisList.First().Count() == 3;
        }
        public bool IsStraight()
        {
            bool acess;
            acess = HasAce();
            // Dim x As Integer
            int y;
            IEnumerable<PokerCardInfo> sortList;
            bool straightSoFar;
            int currentNumber;
            int previousNumber;
            for (y = 1; y <= 2; y++)
            {
                if (y == 1)
                    sortList = from Cards in CardList
                               orderby Cards.Value
                               select Cards;
                else
                    sortList = from Cards in CardList
                               orderby Cards.SecondNumber
                               select Cards;
                straightSoFar = true;
                currentNumber = 0;
                previousNumber = 0;
                foreach (var thisCard in sortList)
                {
                    if (previousNumber == 0)
                    {
                        if (y == 1)
                            previousNumber = (int)thisCard.Value;
                        else
                            previousNumber = (int)thisCard.SecondNumber;
                    }
                    else if (y == 1)
                        currentNumber = (int)thisCard.Value;
                    else
                        currentNumber = (int)thisCard.SecondNumber;
                    if ((currentNumber > 0) & (previousNumber > 0))
                    {
                        if ((previousNumber + 1) != currentNumber)
                        {
                            straightSoFar = false;
                            break; // exit this section
                        }
                    }
                }
                if (straightSoFar == true)
                    return true;
                if (acess == false)
                    return false;
            }
            return false; // i think
        }
        public bool MultiPair()
        {
            var thisList = CardList.GroupOrderDescending(items => items.Value).ToCustomBasicList();
            if (thisList.Count() != 3)
                return false;
            return thisList.First().Count() == 2 && thisList[1].Count() == 2;
        }
        public bool HasAce() //ace is high on this one.
        {
            return CardList.Any(items => items.Value == EnumCardValueList.HighAce);
        }
    }
}