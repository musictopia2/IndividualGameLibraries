using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.Extensions;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
namespace MonasteryCardGameCP
{
    public class RummyClass
    {
        public DeckRegularDict<MonasteryCardInfo> EntireList = new DeckRegularDict<MonasteryCardInfo>();
        private readonly MonasteryCardGameMainGameClass _mainGame;
        public RummyClass(MonasteryCardGameMainGameClass mainGame)
        {
            _mainGame = mainGame;
            EntireList = _mainGame.DeckList!.ToRegularDeckDict(); //in this case, no need for init.
            if (EntireList.Count == 0)
                throw new BasicBlankException("Cannot have an empty list.  Rethink");
        }
        private DeckRegularDict<MonasteryCardInfo> PopulateTempCol(DeckRegularDict<MonasteryCardInfo> thisCol, out DeckRegularDict<MonasteryCardInfo> aceList)
        {
            aceList = thisCol.Where(items => items.Value == EnumCardValueList.LowAce || items.Value == EnumCardValueList.HighAce).ToRegularDeckDict();
            DeckRegularDict<MonasteryCardInfo> output = thisCol.ToRegularDeckDict();
            output.RemoveGivenList(aceList, System.Collections.Specialized.NotifyCollectionChangedAction.Remove);
            return output; //hopefully this works.
        }
        public bool IsDoubleRun(DeckRegularDict<MonasteryCardInfo> thisCol)
        {
            if (thisCol.Count < 6)
                return false; //must have at least 3 cards to even be considered for this.
            int counts = thisCol.Count;
            if (counts.IsNumberOdd() == true)
                return false; //has to be even number to even be considered.
            var tempCol = PopulateTempCol(thisCol, out DeckRegularDict<MonasteryCardInfo> aceList);
            int y;
            int z = 0;
            int q = 0;
            int currentNumber = 0;
            int diffs;
            int previousNumber = 0;
            int removes = 0;
            MonasteryCardInfo thisCard;
            do
            {
                for (y = 1; y <= 2; y++)
                {
                    z++;
                    q++;
                    if (q > thisCol.Count)
                        return true; //if went far enough, then its fine.
                    if (z > thisCol.Count)
                    {
                        diffs = thisCol.Count - tempCol.Count;
                        return aceList.Count + removes == diffs;
                    }
                    thisCard = tempCol[z - 1];
                    if (q == 1)
                    {
                        currentNumber = (int)thisCard.Value;
                        previousNumber = (int)thisCard.Value;
                    }
                    if (y == 2)
                    {
                        if ((int)thisCard.Value > currentNumber) //see if it matches
                        {
                            if (aceList.Count == 0)
                                return false;
                            z--; //because needs to look again
                            if (currentNumber + 1 == (int)thisCard.Value)
                            {
                                removes++;
                                aceList.RemoveFirstItem(); //because we had an ace.
                            }
                            else if (currentNumber + 2 == (int)thisCard.Value)
                            {
                                if (aceList.Count < 3)
                                    return false;//because not enough aces
                                3.Times(x =>
                                {
                                    removes++;
                                    aceList.RemoveFirstItem();
                                });
                                q += 2;
                                previousNumber = (int)thisCard.Value; //because it has to add 2.
                            }
                            else if (currentNumber + 3 == (int)thisCard.Value)
                            {
                                if (aceList.Count < 5)
                                    return false;
                                5.Times(x =>
                                {
                                    removes++;
                                    aceList.RemoveFirstItem();
                                });
                                q += 4;
                                previousNumber = (int)thisCard.Value;
                            }
                            else if (currentNumber + 4 == (int)thisCard.Value)
                            {
                                if (aceList.Count < 7)
                                    return false;
                                7.Times(x =>
                                {
                                    removes++;
                                    aceList.RemoveFirstItem();
                                });
                                q += 6;
                                previousNumber = (int)thisCard.Value;
                            }
                            else
                                return false;
                        }
                    }
                    else if (q > 1)
                    {
                        currentNumber = (int)thisCard.Value;
                        diffs = currentNumber - previousNumber - 1;
                        if (diffs > 0)
                        {
                            z--;
                            if (diffs == 1)
                            {
                                if (aceList.Count < 2)
                                    return false;
                                2.Times(x =>
                                {
                                    removes++;
                                    aceList.RemoveFirstItem();
                                });
                                q++;
                            }
                            else if (diffs == 2)
                            {
                                if (aceList.Count < 4)
                                    return false; //i think had to remove 4 and not 2.
                                4.Times(x =>
                                {
                                    removes++;
                                    aceList.RemoveFirstItem();
                                });
                                q += 3;
                            }
                            else if (diffs == 3)
                            {
                                if (aceList.Count < 6)
                                    return false; //i think had to remove 4 and not 2.
                                6.Times(x =>
                                {
                                    removes++;
                                    aceList.RemoveFirstItem();
                                });
                                q += 5;
                            }
                            else if (diffs == 4)
                            {
                                if (aceList.Count < 8)
                                    return false; //i think had to remove 4 and not 2.
                                8.Times(x =>
                                {
                                    removes++;
                                    aceList.RemoveFirstItem();
                                });
                                q += 7;
                            }
                        }
                    }
                    previousNumber = currentNumber;
                }
            } while (true);
        }
        public bool IsRun(DeckRegularDict<MonasteryCardInfo> thisCol, EnumRunType needType, int required)
        {
            if (thisCol.Count < required)
                return false;
            if (needType == EnumRunType.Suit)
            {
                if (thisCol.GroupBy(items => items.Suit).Count() > 1)
                    return false;
            }
            if (needType == EnumRunType.Color)
            {
                if (thisCol.GroupBy(items => items.Color).Count() > 1)
                    return false;
            }
            var tempCol = PopulateTempCol(thisCol, out DeckRegularDict<MonasteryCardInfo> aceList);
            if (tempCol.Count == 0)
                return true; //if all aces, then fine at this point.
            int howMany = tempCol.GroupBy(items => items.Value).Count();
            if (howMany != tempCol.Count)
                return false;
            int currentNumber = 0;
            int diffs;
            int previousNumber = 0;
            int x = 0;
            foreach (var thisCard in tempCol)
            {
                x++;
                currentNumber = (int)thisCard.Value;
                if (x == 1)
                    previousNumber = (int)thisCard.Value;
                else
                {
                    diffs = currentNumber - previousNumber - 1;
                    if (diffs > 0)
                    {
                        if (aceList.Count < diffs)
                            return false;
                        diffs.Times(y => aceList.RemoveFirstItem());
                    }
                }
                previousNumber = (int)thisCard.Value;
            }
            return true;
        }
        public bool IsColor(DeckRegularDict<MonasteryCardInfo> thisCol, int required)
        {
            if (thisCol.Count < required)
                return false;
            return thisCol.GroupBy(items => items.Color).Count() == 1;
        }
        public bool IsKind(DeckRegularDict<MonasteryCardInfo> thisCol, bool needColor, int required)
        {
            if (thisCol.Count < required)
                return false;
            if (needColor)
            {
                if (thisCol.GroupBy(items => items.Color).Count() > 1)
                    return false;
            }
            var tempCol = PopulateTempCol(thisCol, out DeckRegularDict<MonasteryCardInfo> aceList);
            if (tempCol.Count == 0)
                return true;
            return tempCol.GroupBy(items => items.Value).Count() == 1;
        }
        public bool IsEvenOdd(DeckRegularDict<MonasteryCardInfo> thisCol)
        {
            if (thisCol.Count < 9)
                return false;
            var tempCol = PopulateTempCol(thisCol, out DeckRegularDict<MonasteryCardInfo> aceList);
            if (tempCol.Count == 0)
                return true; //if you somehow was able to get 9 aces, then its okay.
            int thisValue = (int)tempCol.First().Value;
            bool isOdd = thisValue.IsNumberOdd();
            return tempCol.All(items =>
            {
                thisValue = (int)items.Value;
                return thisValue.IsNumberOdd() == isOdd;
            });
        }
        public bool IsSuit(DeckRegularDict<MonasteryCardInfo> thisCol, int required)
        {
            if (thisCol.Count < required)
                return false;
            return thisCol.GroupBy(items => items.Suit).Count() == 1;
        }
        public DeckRegularDict<MonasteryCardInfo> DoubleRunList(DeckRegularDict<MonasteryCardInfo> thisCol)
        {
            return thisCol; //has to do this way now.
        }
        public DeckRegularDict<MonasteryCardInfo> RunList(DeckRegularDict<MonasteryCardInfo> thisCol, EnumRunType runType)
        {
            var tempCol = PopulateTempCol(thisCol, out DeckRegularDict<MonasteryCardInfo> aceList);
            DeckRegularDict<MonasteryCardInfo> output = new DeckRegularDict<MonasteryCardInfo>();
            var thisCard = tempCol.First();
            int minNumber = (int)thisCard.Value;
            int maxNumber = minNumber + thisCol.Count - 1;
            if (maxNumber > 13)
            {
                thisCard = tempCol.Last();
                maxNumber = (int)thisCard.Value;
                minNumber = maxNumber - thisCol.Count + 1;
            }
            EnumSuitList suitNeeded;
            if (runType == EnumRunType.Color)
            {
                if (thisCard.Color == EnumColorList.Red)
                    suitNeeded = EnumSuitList.Diamonds;
                else
                    suitNeeded = EnumSuitList.Clubs;
            }
            else if (runType == EnumRunType.Suit)
                suitNeeded = thisCard.Suit;
            else
                suitNeeded = EnumSuitList.Diamonds;
            int currentNum = minNumber;
            thisCol.Count.Times(x =>
            {
                if (tempCol.Any(items => (int)items.Value == currentNum) == false)
                {
                    thisCard = EntireList.First(items => items.Suit == suitNeeded && (int)items.Value == currentNum);
                    var nextCard = new MonasteryCardInfo();
                    nextCard.Populate(thisCard.Deck);
                    nextCard.Temp = thisCard.Deck; //this too i think.
                    nextCard.Deck = aceList.First().Deck;//this is what i had to do now.
                    aceList.RemoveFirstItem(); //has to use the deck of the ace to stop the id problems.
                    output.Add(nextCard);
                }
                else
                {
                    thisCard = tempCol.Single(items => (int)items.Value == currentNum);
                    tempCol.RemoveSpecificItem(thisCard);
                    output.Add(thisCard);
                }
                currentNum++;
            });
            return output;
        }
        public DeckRegularDict<MonasteryCardInfo> KindList(DeckRegularDict<MonasteryCardInfo> thisCol, bool needColor)
        {
            var tempCol = PopulateTempCol(thisCol, out DeckRegularDict<MonasteryCardInfo> aceList);
            DeckRegularDict<MonasteryCardInfo> output = new DeckRegularDict<MonasteryCardInfo>();
            if (aceList.Count == 0)
                return thisCol;
            if (tempCol.Count == 0)
                return aceList;
            var thisCard = tempCol.First();
            int numberNeeded = (int)thisCard.Value;
            EnumSuitList suitNeeded;
            if (needColor)
            {
                if (thisCard.Color == EnumColorList.Red)
                    suitNeeded = EnumSuitList.Diamonds;
                else
                    suitNeeded = EnumSuitList.Clubs;
            }
            else
                suitNeeded = EnumSuitList.Diamonds;
            aceList.Count.Times(x =>
            {
                thisCard = EntireList.First(items => items.Suit == suitNeeded && (int)items.Value == numberNeeded);
                var nextCard = new MonasteryCardInfo();
                nextCard.Populate(thisCard.Deck); //needs to clone it.
                nextCard.Temp = thisCard.Deck;
                nextCard.Deck = aceList[x - 1].Deck;//this is what i had to do now.
                tempCol.Add(nextCard);
            });
            return tempCol;
        }
        public DeckRegularDict<MonasteryCardInfo> EvenOddList(DeckRegularDict<MonasteryCardInfo> thisCol)
        {
            var tempCol = PopulateTempCol(thisCol, out DeckRegularDict<MonasteryCardInfo> aceList);
            DeckRegularDict<MonasteryCardInfo> output = new DeckRegularDict<MonasteryCardInfo>();
            if (aceList.Count == 0)
                return thisCol;
            if (tempCol.Count == 0)
                return aceList;
            var thisCard = tempCol.First();
            int numberNeeded = (int)thisCard.Value;
            //i like the idea that even if aces are there it will hide them even in this case.
            aceList.Count.Times(x =>
            {
                thisCard = EntireList.First(items => items.Suit == EnumSuitList.Diamonds && (int)items.Value == numberNeeded);
                var nextCard = new MonasteryCardInfo();
                nextCard.Populate(thisCard.Deck);
                nextCard.Temp = thisCard.Deck;
                nextCard.Deck = aceList[x - 1].Deck; //to stop the linking problems.
                tempCol.Add(nextCard);
            });
            return tempCol;
        }
    }
}