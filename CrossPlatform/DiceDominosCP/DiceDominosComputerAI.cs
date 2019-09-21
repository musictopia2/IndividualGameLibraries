using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.Dominos;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
namespace DiceDominosCP
{
    [SingletonGame]
    public class DiceDominosComputerAI
    {
        private readonly DeckRegularDict<SimpleDominoInfo> _possibleList; //maybe we don't need observable since no ui with it.
        private int HowMany(int whatNumber)
        {

            int nums = default;
            foreach (var thisDomino in _possibleList)
            {
                if (thisDomino.FirstNum == whatNumber || thisDomino.SecondNum == whatNumber || (whatNumber == 6 && thisDomino.FirstNum == 0) || (whatNumber == 6 && thisDomino.SecondNum == 0))
                    nums += 1;
            }
            return nums;
        }
        public int Move()
        {
            DeckRegularDict<SimpleDominoInfo> newList = new DeckRegularDict<SimpleDominoInfo>();
            foreach (var thisDomino in _possibleList)
            {
                if (_gameBoard1.IsValidMove(thisDomino.Deck) == true)
                    newList.Add(thisDomino);
            }
            if (newList.Count == 0)
                return 0;
            return newList.GetRandomItem().Deck;
        }
        public int DominoToHold()
        {
            int firstNumber;
            int secondNumber;
            firstNumber = HowMany(_gameBoard1.DiceValue(1));
            secondNumber = HowMany(_gameBoard1.DiceValue(2));
            if (firstNumber > secondNumber)
                return 1;
            if (secondNumber > firstNumber)
                return 2;
            return _rs.GetRandomNumber(2);
        }
        readonly RandomGenerator _rs; //decided to not do unit testing with this one.
        private readonly GameBoardCP _gameBoard1;
        public DiceDominosComputerAI(GameBoardCP gameBoard1)
        {
            _gameBoard1 = gameBoard1;
            _rs = new RandomGenerator();
            _possibleList = gameBoard1.GetVisibleList();
        }
    }
}