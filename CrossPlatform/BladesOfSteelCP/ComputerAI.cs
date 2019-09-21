using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.Extensions;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
namespace BladesOfSteelCP
{
    public enum EnumFirstStep
    {
        ThrowAwayAllCards = 0,
        PlayAttack = 1,
        PlayDefense = 2
    }
    public enum EnumDefenseStep
    {
        Pass = 0,
        Hand = 1,
        Board = 2
    }
    [SingletonGame] //so it can be automatic.
    public class ComputerAI
    {
        private readonly BladesOfSteelMainGameClass _mainGame;
        public ComputerAI(BladesOfSteelMainGameClass mainGame)
        {
            _mainGame = mainGame;
        }

        private DeckRegularDict<RegularSimpleCard> CardsForFirstDefense()
        {
            var thisList = _mainGame.SingleInfo!.MainHandList.PossibleCombinations(EnumColorList.Black);
            var firstItem = thisList.OrderByDescending(items => _mainGame.GetDefenseStage(items)).ThenByDescending(items => items.Sum(temps => (int)temps.Value)).Take(1).Single();
            return new DeckRegularDict<RegularSimpleCard>(firstItem);
        }
        private DeckRegularDict<RegularSimpleCard> CardsToAddDefense()
        {
            if (_mainGame.SingleInfo!.DefenseList.Count == 0)
                throw new BasicBlankException("Should have used the CardsForFirstDefense because 0 cards left");
            int maxs = 3 - _mainGame.SingleInfo.DefenseList.Count;
            var thisList = _mainGame.SingleInfo.MainHandList.PossibleCombinations(EnumColorList.Black, maxs);
            if (thisList.Count == 0)
                throw new BasicBlankException("Must be at least one combination.  Otherwise; would be attacking");
            var firstItem = thisList.OrderByDescending(items => _mainGame.GetDefenseStage(items)).ThenByDescending(items => items.Sum(temps => (int)temps.Value)).Take(1).Single();
            return new DeckRegularDict<RegularSimpleCard>(firstItem);
        }
        private bool NeedsToRemoveDefenseCards()
        {
            var handCombo = _mainGame.SingleInfo!.MainHandList.PossibleCombinations(EnumColorList.Black);
            var boardCombo = _mainGame.SingleInfo.DefenseList.PossibleCombinations(EnumColorList.Black);
            var handList = handCombo.OrderByDescending(items => _mainGame.GetDefenseStage(items)).ThenByDescending(items => items.Sum(temps => (int)temps.Value)).Take(1).Single();
            var boardList = boardCombo.OrderByDescending(items => _mainGame.GetDefenseStage(items)).ThenByDescending(items => items.Sum(temps => (int)temps.Value)).Take(1).Single();
            var handLevel = _mainGame.GetDefenseStage(handList);
            var boardLevel = _mainGame.GetDefenseStage(boardList);
            return handLevel > boardLevel || handList.Sum(items => (int)items.Value) > boardList.Sum(items => (int)items.Value);
        }
        private DeckRegularDict<RegularSimpleCard> CardsForAttack()
        {
            var possibleList = _mainGame.SingleInfo!.MainHandList.PossibleCombinations(EnumColorList.Red);
            var thisItem = possibleList.OrderByDescending(items => _mainGame.GetAttackStage(items)).ThenByDescending(items => items.DistinctCount(temps => temps.Value)).Take(1).Single(); //hopefully using distinct count works.
            return new DeckRegularDict<RegularSimpleCard>(thisItem);
        }
        public (EnumFirstStep firstStep, DeckRegularDict<RegularSimpleCard> cardList) GetFirstMove()
        {
            if (_mainGame.SingleInfo!.MainHandList.Count != 6)
                throw new BasicBlankException("Must have exactly 6 cards in hand when figuring out the first move");
            bool exists = _mainGame.SingleInfo.MainHandList.Any(items => items.Color == EnumColorList.Black);
            if (_mainGame.SingleInfo.DefenseList.Count == 0 && exists == true)
                return (EnumFirstStep.PlayDefense, CardsForFirstDefense());
            int counts = _mainGame.SingleInfo.MainHandList.Count(items => items.Color == EnumColorList.Red);
            if (counts > 1)
                return (EnumFirstStep.PlayAttack, CardsForAttack());
            if (NeedsToRemoveDefenseCards() == true)
                return (EnumFirstStep.PlayDefense, CardsForFirstDefense());
            if (_mainGame.SingleInfo.DefenseList.Count == 3)
                return (EnumFirstStep.ThrowAwayAllCards, null!);
            return (EnumFirstStep.PlayDefense, CardsToAddDefense());
        }
        public (EnumDefenseStep DefenseStep, DeckRegularDict<RegularSimpleCard> CardList) CardsForDefense()
        {
            var possibleList = _mainGame.SingleInfo!.MainHandList.PossibleCombinations(EnumColorList.Black);
            possibleList.KeepConditionalItems(items => _mainGame.ThisMod!.MainDefense1!.CanAddDefenseCards(items) == true);
            var newList = _mainGame.SingleInfo.DefenseList.PossibleCombinations(EnumColorList.Black);
            newList.KeepConditionalItems(items => _mainGame.ThisMod!.MainDefense1!.CanAddDefenseCards(items) == true);
            if (possibleList.Count == 0 && newList.Count == 0)
                return (EnumDefenseStep.Pass, null!);
            var handList = possibleList.OrderByDescending(items => _mainGame.GetDefenseStage(items)).ThenByDescending(items => items.DistinctCount(temps => temps.Value)).Take(1).SingleOrDefault();
            var tempDefenseList = newList.OrderByDescending(items => _mainGame.GetDefenseStage(items)).ThenByDescending(items => items.DistinctCount(temps => temps.Value)).Take(1).SingleOrDefault();
            if (possibleList.Count == 0)
                return (EnumDefenseStep.Board, new DeckRegularDict<RegularSimpleCard>(tempDefenseList));
            if (newList.Count == 0)
                return (EnumDefenseStep.Hand, new DeckRegularDict<RegularSimpleCard>(handList));
            var handLevel = _mainGame.GetDefenseStage(handList);
            var boardLevel = _mainGame.GetDefenseStage(tempDefenseList);
            if (handLevel < boardLevel || handList.Sum(items => (int)items.Value) <= tempDefenseList.Sum(items => (int)items.Value))
                return (EnumDefenseStep.Hand, handList.ToRegularDeckDict());
            return (EnumDefenseStep.Board, tempDefenseList.ToRegularDeckDict());
        }
    }
}