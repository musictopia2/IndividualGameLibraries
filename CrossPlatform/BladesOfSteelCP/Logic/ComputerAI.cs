using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BladesOfSteelCP.Data;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;

namespace BladesOfSteelCP.Logic
{
    [SingletonGame] //so it can be automatic.
    [AutoReset]
    public class ComputerAI
    {
        private readonly BladesOfSteelGameContainer _gameContainer;
        private readonly BladesOfSteelVMData _model;

        public ComputerAI(BladesOfSteelGameContainer gameContainer, BladesOfSteelVMData model)
        {
            _gameContainer = gameContainer;
            _model = model;
        }

        private DeckRegularDict<RegularSimpleCard> CardsForFirstDefense()
        {
            if (_gameContainer.GetDefenseStage == null)
            {
                throw new BasicBlankException("Nobody is handling get defense stage for computer ai.  Rethink");
            }
            var thisList = _gameContainer.SingleInfo!.MainHandList.PossibleCombinations(EnumColorList.Black);
            var firstItem = thisList.OrderByDescending(items => _gameContainer.GetDefenseStage(items)).ThenByDescending(items => items.Sum(temps => (int)temps.Value)).Take(1).Single();
            return new DeckRegularDict<RegularSimpleCard>(firstItem);
        }
        private DeckRegularDict<RegularSimpleCard> CardsToAddDefense()
        {
            if (_gameContainer.GetDefenseStage == null)
            {
                throw new BasicBlankException("Nobody is handling get defense stage for computer ai.  Rethink");
            }
            if (_gameContainer.SingleInfo!.DefenseList.Count == 0)
                throw new BasicBlankException("Should have used the CardsForFirstDefense because 0 cards left");
            int maxs = 3 - _gameContainer.SingleInfo.DefenseList.Count;
            var thisList = _gameContainer.SingleInfo.MainHandList.PossibleCombinations(EnumColorList.Black, maxs);
            if (thisList.Count == 0)
                throw new BasicBlankException("Must be at least one combination.  Otherwise; would be attacking");
            var firstItem = thisList.OrderByDescending(items => _gameContainer.GetDefenseStage(items)).ThenByDescending(items => items.Sum(temps => (int)temps.Value)).Take(1).Single();
            return new DeckRegularDict<RegularSimpleCard>(firstItem);
        }
        private bool NeedsToRemoveDefenseCards()
        {
            if (_gameContainer.GetDefenseStage == null)
            {
                throw new BasicBlankException("Nobody is handling get defense stage for computer ai.  Rethink");
            }
            var handCombo = _gameContainer.SingleInfo!.MainHandList.PossibleCombinations(EnumColorList.Black);
            var boardCombo = _gameContainer.SingleInfo.DefenseList.PossibleCombinations(EnumColorList.Black);
            var handList = handCombo.OrderByDescending(items => _gameContainer.GetDefenseStage(items)).ThenByDescending(items => items.Sum(temps => (int)temps.Value)).Take(1).Single();
            var boardList = boardCombo.OrderByDescending(items => _gameContainer.GetDefenseStage(items)).ThenByDescending(items => items.Sum(temps => (int)temps.Value)).Take(1).Single();
            var handLevel = _gameContainer.GetDefenseStage(handList);
            var boardLevel = _gameContainer.GetDefenseStage(boardList);
            return handLevel > boardLevel || handList.Sum(items => (int)items.Value) > boardList.Sum(items => (int)items.Value);
        }
        private DeckRegularDict<RegularSimpleCard> CardsForAttack()
        {
            if (_gameContainer.GetAttackStage == null)
            {
                throw new BasicBlankException("Nobody is handling get attack stage for computer ai.  Rethink");
            }
            var possibleList = _gameContainer.SingleInfo!.MainHandList.PossibleCombinations(EnumColorList.Red);
            var thisItem = possibleList.OrderByDescending(items => _gameContainer.GetAttackStage(items)).ThenByDescending(items => items.DistinctCount(temps => temps.Value)).Take(1).Single(); //hopefully using distinct count works.
            return new DeckRegularDict<RegularSimpleCard>(thisItem);
        }
        public (EnumFirstStep firstStep, DeckRegularDict<RegularSimpleCard> cardList) GetFirstMove()
        {
            if (_gameContainer.SingleInfo!.MainHandList.Count != 6)
                throw new BasicBlankException("Must have exactly 6 cards in hand when figuring out the first move");
            bool exists = _gameContainer.SingleInfo.MainHandList.Any(items => items.Color == EnumColorList.Black);
            if (_gameContainer.SingleInfo.DefenseList.Count == 0 && exists == true)
                return (EnumFirstStep.PlayDefense, CardsForFirstDefense());
            int counts = _gameContainer.SingleInfo.MainHandList.Count(items => items.Color == EnumColorList.Red);
            if (counts > 1)
                return (EnumFirstStep.PlayAttack, CardsForAttack());
            if (NeedsToRemoveDefenseCards() == true)
                return (EnumFirstStep.PlayDefense, CardsForFirstDefense());
            if (_gameContainer.SingleInfo.DefenseList.Count == 3)
                return (EnumFirstStep.ThrowAwayAllCards, null!);
            return (EnumFirstStep.PlayDefense, CardsToAddDefense());
        }
        public (EnumDefenseStep DefenseStep, DeckRegularDict<RegularSimpleCard> CardList) CardsForDefense()
        {
            if (_gameContainer.GetDefenseStage == null)
            {
                throw new BasicBlankException("Nobody is handling get defense stage for computer ai.  Rethink");
            }
            var possibleList = _gameContainer.SingleInfo!.MainHandList.PossibleCombinations(EnumColorList.Black);
            possibleList.KeepConditionalItems(items => _model.MainDefense1!.CanAddDefenseCards(items) == true);
            var newList = _gameContainer.SingleInfo.DefenseList.PossibleCombinations(EnumColorList.Black);
            newList.KeepConditionalItems(items => _model.MainDefense1!.CanAddDefenseCards(items) == true);
            if (possibleList.Count == 0 && newList.Count == 0)
                return (EnumDefenseStep.Pass, null!);
            var handList = possibleList.OrderByDescending(items => _gameContainer.GetDefenseStage(items)).ThenByDescending(items => items.DistinctCount(temps => temps.Value)).Take(1).SingleOrDefault();
            var tempDefenseList = newList.OrderByDescending(items => _gameContainer.GetDefenseStage(items)).ThenByDescending(items => items.DistinctCount(temps => temps.Value)).Take(1).SingleOrDefault();
            if (possibleList.Count == 0)
                return (EnumDefenseStep.Board, new DeckRegularDict<RegularSimpleCard>(tempDefenseList));
            if (newList.Count == 0)
                return (EnumDefenseStep.Hand, new DeckRegularDict<RegularSimpleCard>(handList));
            var handLevel = _gameContainer.GetDefenseStage(handList);
            var boardLevel = _gameContainer.GetDefenseStage(tempDefenseList);
            if (handLevel < boardLevel || handList.Sum(items => (int)items.Value) <= tempDefenseList.Sum(items => (int)items.Value))
                return (EnumDefenseStep.Hand, handList.ToRegularDeckDict());
            return (EnumDefenseStep.Board, tempDefenseList.ToRegularDeckDict());
        }
    }
}
