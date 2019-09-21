using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.Extensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
namespace A8RoundRummyCP
{
    public static class RummyExtensions
    {
        private static A8RoundRummyCardInformation GetLastCard(this IDeckDict<A8RoundRummyCardInformation> originalList, IDeckDict<A8RoundRummyCardInformation> newList)
        {
            if (originalList.Count != 8 && newList.Count != 7)
                throw new BasicBlankException("The original list must have 8 cards and the new list must have 7 cards");
            return originalList.Single(ThisCard =>
            {
                return newList.ObjectExist(ThisCard.Deck) == false;
            });
        }
        public static bool HasRummy(this IDeckDict<A8RoundRummyCardInformation> cardList, A8RoundRummyMainGameClass mainGame) //you have to send one more argument
        {
            if (cardList.Count != 8)
                throw new BasicBlankException("Must have 8 cards in order to have the rummy");
            mainGame.LastCard = null;
            mainGame.LastSuccessful = false;
            RoundClass currentRound = mainGame.SaveRoot!.RoundList.First();
            int wilds = cardList.Count(Items => Items.CardType == EnumCardType.Wild);
            int reverses = cardList.Count(Items => Items.CardType == EnumCardType.Reverse);
            EnumCardShape shapeUsed;
            EnumColor colorUsed;
            if (reverses > 1)
                return false; //can't have more than one reverse in your hand.
            if (currentRound.Category == EnumCategory.Colors && currentRound.Rummy == EnumRummyType.Regular)
            {
                var blueList = cardList.Where(Items => Items.Color == EnumColor.Blue || Items.CardType == EnumCardType.Wild).Take(7).ToRegularDeckDict();
                var redList = cardList.Where(Items => Items.Color == EnumColor.Red || Items.CardType == EnumCardType.Wild).Take(7).ToRegularDeckDict();
                if (blueList.Count < 7 && redList.Count < 7) //has to be and, not or.
                    return false;
                mainGame.LastSuccessful = true;
                if (blueList.Count == 7)
                    mainGame.LastCard = cardList.GetLastCard(blueList);
                else
                    mainGame.LastCard = cardList.GetLastCard(redList);
                return true;
            }
            DeckRegularDict<A8RoundRummyCardInformation> tempList;
            if (currentRound.Category == EnumCategory.Shapes && currentRound.Rummy == EnumRummyType.Regular)
            {
                var firstList = cardList.Where(Items => Items.CardType == EnumCardType.Regular).GroupBy(Items => Items.Shape).ToCustomBasicList();
                if (firstList.Count > 2)
                    return false;
                if (firstList.Count > 1 && reverses > 0)
                    return false;
                if (firstList.Count > 1)
                {
                    if (firstList.First().Count() > 1 && firstList.Last().Count() > 1)
                        return false;
                    if (firstList.First().Count() > 1)
                    {
                        shapeUsed = firstList.First().Key;
                    }
                    else
                    {
                        shapeUsed = firstList.Last().Key;
                    }
                }
                else
                {
                    shapeUsed = firstList.First().Key;
                }
                tempList = cardList.Where(Items => Items.Shape == shapeUsed || Items.CardType == EnumCardType.Wild).Take(7).ToRegularDeckDict();
                mainGame.LastSuccessful = true;
                mainGame.LastCard = cardList.GetLastCard(tempList);
                return true;
            }
            if (currentRound.Category == EnumCategory.Both && currentRound.Rummy == EnumRummyType.Regular)
            {
                var firstList = cardList.Where(Items => Items.CardType == EnumCardType.Regular).GroupBy(Items => new { Items.Color, Items.Shape }).ToCustomBasicList(); //iffy
                if (firstList.Count > 2)
                    return false;
                if (firstList.Count > 1 && reverses > 0)
                    return false;
                if (firstList.Count > 1)
                {
                    if (firstList.First().Count() > 1 && firstList.Last().Count() > 1)
                        return false;
                    if (firstList.First().Count() > 1)
                    {
                        shapeUsed = firstList.First().Key.Shape;
                        colorUsed = firstList.First().Key.Color;
                    }
                    else
                    {
                        shapeUsed = firstList.Last().Key.Shape;
                        colorUsed = firstList.Last().Key.Color;
                    }
                }
                else
                {
                    shapeUsed = firstList.First().Key.Shape;
                    colorUsed = firstList.First().Key.Color;
                }
                tempList = cardList.Where(Items => Items.CardType == EnumCardType.Wild || (Items.Color == colorUsed && Items.Shape == shapeUsed)).Take(7).ToRegularDeckDict();
                mainGame.LastSuccessful = true;
                mainGame.LastCard = cardList.GetLastCard(tempList);
                return true;
            }
            if (currentRound.Rummy == EnumRummyType.Kinds)
            {
                var firstList = cardList.Where(Items => Items.CardType == EnumCardType.Regular).GroupBy(Items => Items.Value).ToCustomBasicList();
                int numberUsed;
                if (firstList.Count > 2)
                    return false;
                if (firstList.Count > 1 && reverses > 0)
                    return false;
                if (firstList.Count > 1)
                {
                    if (firstList.First().Count() > 1 && firstList.Last().Count() > 1)
                        return false;
                    if (firstList.First().Count() > 1)
                    {
                        numberUsed = firstList.First().Key;
                    }
                    else
                    {
                        numberUsed = firstList.Last().Key;
                    }
                }
                else
                {
                    numberUsed = firstList.First().Key;
                }
                tempList = cardList.Where(Items => Items.Value == numberUsed || Items.CardType == EnumCardType.Wild).Take(7).ToRegularDeckDict();
                mainGame.LastSuccessful = true;
                mainGame.LastCard = cardList.GetLastCard(tempList);
                return true;
            }
            if (currentRound.Rummy != EnumRummyType.Straight)
                throw new BasicBlankException("Not Supported");
            var straightList = cardList.Where(Items => Items.CardType == EnumCardType.Regular).OrderBy(Items => Items.Value).ToRegularDeckDict();
            switch (currentRound.Category)
            {
                case EnumCategory.None:
                    var nexts = straightList.GroupBy(Items => Items.Value).ToCustomBasicList();
                    var finList = nexts.Where(Items => Items.Count() > 1).ToCustomBasicList();
                    if (finList.Count == 0)
                    {
                        tempList = cardList.Where(Items => Items.CardType != EnumCardType.Reverse).Take(7).ToRegularDeckDict();
                        mainGame.LastSuccessful = true;
                        mainGame.LastCard = cardList.GetLastCard(tempList);
                        return true;
                    }
                    if (reverses == 1)
                        return false;
                    if (finList.Count > 1)
                        return false;
                    mainGame.LastSuccessful = true;
                    mainGame.LastCard = finList.Single().First(); //hopefully can take the first one.
                    return true;
                case EnumCategory.Colors:
                    return cardList.IsStraightColorOnly(straightList, mainGame, reverses);
                case EnumCategory.Shapes:
                    return cardList.IsStraightShapeOnly(straightList, mainGame, reverses);
                case EnumCategory.Both:
                    return cardList.IsStraightBoth(straightList, mainGame, reverses);
                default:
                    throw new BasicBlankException("Not Supported");
            }
        }
        private static bool IsStraightColorOnly(this IDeckDict<A8RoundRummyCardInformation> cardList, IDeckDict<A8RoundRummyCardInformation> straightList, A8RoundRummyMainGameClass mainGame, int reverses)
        {
            var firstList = straightList.GroupBy(Items => Items.Color).ToCustomBasicList();
            if (firstList.Count > 2)
                throw new BasicBlankException("Can only have 2 colors at the most");
            var nextFirst = firstList.First().GroupBy(Items => Items.Value).ToCustomBasicList();
            var finList = nextFirst.Where(Items => Items.Count() > 1).ToCustomBasicList();
            DeckRegularDict<A8RoundRummyCardInformation> tempList;
            EnumColor colorUsed;
            if (firstList.Count == 1)
            {
                if (finList.Count == 0)
                {
                    tempList = cardList.Where(Items => Items.CardType != EnumCardType.Reverse).Take(7).ToRegularDeckDict();
                    mainGame.LastSuccessful = true;
                    mainGame.LastCard = cardList.GetLastCard(tempList);
                    return true;
                }
                if (reverses == 1)
                    return false;
                if (finList.Count > 1)
                    return false;
                mainGame.LastSuccessful = true;
                mainGame.LastCard = finList.Single().First(); //i think first.
                return true;
            }
            if (reverses == 1)
                return false;
            var nextSecond = firstList.Last().GroupBy(Items => Items.Value).ToCustomBasicList();
            if (nextSecond.Count > 1 && nextFirst.Count > 1)
                return false;
            if (nextSecond.Count > 1)
            {
                colorUsed = firstList.Last().Key;
                finList = nextSecond.Where(Items => Items.Count() > 1).ToCustomBasicList();
            }
            else
            {
                colorUsed = firstList.First().Key;
            }
            if (finList.Count == 0)
            {
                tempList = cardList.Where(Items => Items.Color == colorUsed || Items.CardType == EnumCardType.Wild).Take(7).ToRegularDeckDict();
                mainGame.LastSuccessful = true;
                mainGame.LastCard = cardList.GetLastCard(tempList);
                return true;
            }
            if (finList.Count > 1)
                return false;
            mainGame.LastSuccessful = true;
            mainGame.LastCard = finList.Single().First();
            return true;
        }
        private static bool IsStraightShapeOnly(this IDeckDict<A8RoundRummyCardInformation> cardList, IDeckDict<A8RoundRummyCardInformation> straightList, A8RoundRummyMainGameClass mainGame, int reverses)
        {
            var firstList = straightList.GroupBy(Items => Items.Shape).ToCustomBasicList();
            if (firstList.Count > 2)
                return false;
            var nextFirst = firstList.First().GroupBy(Items => Items.Value).ToCustomBasicList();
            var finList = nextFirst.Where(Items => Items.Count() > 1).ToCustomBasicList();
            DeckRegularDict<A8RoundRummyCardInformation> tempList;
            EnumCardShape shapeUsed;
            if (firstList.Count == 1)
            {
                if (finList.Count == 0)
                {
                    tempList = cardList.Where(Items => Items.CardType != EnumCardType.Reverse).Take(7).ToRegularDeckDict();
                    mainGame.LastSuccessful = true;
                    mainGame.LastCard = cardList.GetLastCard(tempList);
                    return true;
                }
                if (reverses == 1)
                    return false;
                if (finList.Count > 1)
                    return false;
                mainGame.LastSuccessful = true;
                mainGame.LastCard = finList.Single().First(); //hopefully this simple.
                return true;
            }
            if (reverses == 1)
                return false;
            var nextSecond = firstList.Last().GroupBy(Items => Items.Value).ToCustomBasicList();
            if (nextSecond.Count > 1 && nextFirst.Count > 1)
                return false;
            if (nextSecond.Count > 1)
            {
                shapeUsed = firstList.Last().Key;
                finList = nextSecond.Where(Items => Items.Count() > 1).ToCustomBasicList();
            }
            else
            {
                shapeUsed = firstList.First().Key;
            }
            if (finList.Count == 0)
            {
                tempList = cardList.Where(Items => Items.Shape == shapeUsed || Items.CardType == EnumCardType.Wild).Take(7).ToRegularDeckDict();
                mainGame.LastSuccessful = true;
                mainGame.LastCard = cardList.GetLastCard(tempList);
                return true;
            }
            if (finList.Count > 1)
                return false;
            mainGame.LastSuccessful = true;
            mainGame.LastCard = finList.Single().First();
            return true;
        }
        private static bool IsStraightBoth(this IDeckDict<A8RoundRummyCardInformation> cardList, IDeckDict<A8RoundRummyCardInformation> straightList, A8RoundRummyMainGameClass mainGame, int reverses)
        {
            var firstList = straightList.GroupBy(Items => new { Items.Color, Items.Shape }).ToCustomBasicList();
            if (firstList.Count > 2)
                return false;
            var nextFirst = firstList.First().GroupBy(Items => Items.Value).ToCustomBasicList();
            var finList = nextFirst.Where(Items => Items.Count() > 1).ToCustomBasicList();
            DeckRegularDict<A8RoundRummyCardInformation> tempList;
            EnumCardShape shapeUsed;
            EnumColor colorUsed;
            if (firstList.Count == 1)
            {
                if (finList.Count == 0)
                {
                    tempList = cardList.Where(Items => Items.CardType != EnumCardType.Reverse).Take(7).ToRegularDeckDict();
                    mainGame.LastSuccessful = true;
                    mainGame.LastCard = cardList.GetLastCard(tempList);
                    return true;
                }
                if (reverses == 1)
                    return false;
                if (finList.Count > 1)
                    return false;
                mainGame.LastSuccessful = true;
                mainGame.LastCard = finList.Single().First(); //hopefully this simple.
                return true;
            }
            if (reverses == 1)
                return false;
            var nextSecond = firstList.Last().GroupBy(Items => Items.Value).ToCustomBasicList();
            if (nextSecond.Count > 1 && nextFirst.Count > 1)
                return false;
            if (nextSecond.Count > 1)
            {
                shapeUsed = firstList.Last().Key.Shape;
                colorUsed = firstList.Last().Key.Color;
                finList = nextSecond.Where(Items => Items.Count() > 1).ToCustomBasicList();
            }
            else
            {
                shapeUsed = firstList.First().Key.Shape;
                colorUsed = firstList.First().Key.Color;
            }
            if (finList.Count == 0)
            {
                tempList = cardList.Where(Items => Items.CardType == EnumCardType.Wild || (Items.Shape == shapeUsed && Items.Color == colorUsed)).Take(7).ToRegularDeckDict();
                mainGame.LastSuccessful = true;
                mainGame.LastCard = cardList.GetLastCard(tempList);
                return true;
            }
            if (finList.Count > 1)
                return false;
            mainGame.LastSuccessful = true;
            mainGame.LastCard = finList.Single().First();
            return true;
        }
        public static bool GuaranteedVictory(this IDeckDict<A8RoundRummyCardInformation> cardList, A8RoundRummyMainGameClass mainGame)
        {
            mainGame.LastSuccessful = false;
            mainGame.LastCard = null;
            if (mainGame.SaveRoot!.RoundList.Last().Rummy != EnumRummyType.Kinds)
                return false; //only last round has this.
            if (cardList.Count(Items => Items.CardType != EnumCardType.Regular) > 1)
                return false;
            var tempList = cardList.Where(Items => Items.CardType == EnumCardType.Regular).GroupBy(Items => Items.Value).ToCustomBasicList();
            if (tempList.Count == 1)
            {
                if (tempList.First().Count() >= 7)
                {
                    var finList = tempList.Single().Take(7).ToRegularDeckDict();
                    mainGame.LastSuccessful = true;
                    mainGame.LastCard = cardList.GetLastCard(finList);
                    return true;
                }
                return false;
            }
            if (tempList.Count > 2)
                return false;
            if (tempList.First().Count() < 7 && tempList.Last().Count() < 7)
                return false;
            if (tempList.First().Count() >= 7)
            {
                var finList = tempList.First().Take(7).ToRegularDeckDict();
                mainGame.LastSuccessful = true;
                mainGame.LastCard = cardList.GetLastCard(finList);
                return true;
            }
            else
            {
                if (tempList.Last().Count() < 7)
                    throw new BasicBlankException("Had to have 7");
                var finList = tempList.Last().Take(7).ToRegularDeckDict();
                mainGame.LastSuccessful = true;
                mainGame.LastCard = cardList.GetLastCard(finList);
                return true;
            }
        }
    }
}