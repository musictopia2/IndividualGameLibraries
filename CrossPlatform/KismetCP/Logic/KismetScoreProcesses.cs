using BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.Containers;
using BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.Data;
using BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.Logic;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using KismetCP.Data;
using SkiaSharpGeneralLibrary.SKExtensions;
using System.Linq;
namespace KismetCP.Logic
{
    public class KismetScoreProcesses : IYahtzeeStyle
    {
        private readonly YahtzeeGameContainer<KismetDice> _gameContainer;
        private readonly ScoreContainer _scoreContainer;
        private readonly YahtzeeVMData<KismetDice> _model;

        CustomBasicList<string> IYahtzeeStyle.GetBottomText => new CustomBasicList<string>()
        {
            "2 Pair Same Color", "3 Of A Kind", "Straight (30)",
            "Flush All Same Color (35)", "Full House", "Full House Same Color",
            "4 Of A Kind", "Yarborough", "Kismet (5 Of A Kind)"
        };

        bool IYahtzeeStyle.HasExceptionFor5Kind => true;

        int IYahtzeeStyle.BonusAmount(int topScore)
        {
            if (topScore < 63)
                return 0;
            if (topScore <= 70)
                return 35;
            if (topScore <= 77)
                return 55;
            return 75;
        }

        void IYahtzeeStyle.Extra5OfAKind() //i think this is it.
        {
            var firstList = _gameContainer.PlayerList!.ToCustomBasicList();
            firstList.RemoveSpecificItem(_gameContainer.SingleInfo!);
            firstList.ForEach(Items => Items.MissNextTurn = true);
        }

        public KismetScoreProcesses(YahtzeeGameContainer<KismetDice> gameContainer,
            ScoreContainer scoreContainer,
            YahtzeeVMData<KismetDice> model
            )
        {
            _gameContainer = gameContainer;
            _scoreContainer = scoreContainer;
            _model = model;
        }



        CustomBasicList<DiceInformation> IYahtzeeStyle.GetDiceList()
        {
            var firstList = _model.Cup!.DiceList.ToCustomBasicList();
            CustomBasicList<DiceInformation> output = new CustomBasicList<DiceInformation>();
            firstList.ForEach(items =>
            {
                DiceInformation thisDice = new DiceInformation();
                thisDice.Value = items.Value;
                thisDice.Color = items.DotColor.ToSKColor(); //i think
                output.Add(thisDice);
            });
            return output;
        }

        private bool HasFlush()
        {
            int count = _scoreContainer.DiceList.MaximumDuplicates(Items => Items.Color);
            return count == 5;
        }
        private bool HasTwoPairSameColor()
        {
            if (_scoreContainer.HasFullHouse() == true && HasFlush() == true)
                return true;
            var tempList = _scoreContainer.DiceList.GroupOrderDescending(Items => Items.Color);
            if (tempList.Count() > 2)
                return false;
            if (tempList.First().Count() < 4)
                return false; //i did that in the old.  if that is not correct, rethink.
            var first = _scoreContainer.DiceList.Where(Items => Items.Color == tempList.First().Key)
                .GroupOrderDescending(Items => Items.Value).ToCustomBasicList();
            if (first.First().Count() == 4)
                return true;
            if (first.First().Count() != 2)
                return false;
            if (first[1].Count() != 2)
                return false;
            return true; //hopefully this works.  if i am wrong, rethink.
        }
        void IYahtzeeStyle.PopulateBottomScores()
        {
            if (HasTwoPairSameColor() == true && _scoreContainer.RowList[9].HasFilledIn() == false)
                _scoreContainer.RowList[9].Possible = _scoreContainer.CalculateDiceTotal();
            if (_scoreContainer.HasKind(3) == true && _scoreContainer.RowList[10].HasFilledIn() == false)
                _scoreContainer.RowList[10].Possible = _scoreContainer.CalculateDiceTotal();
            if (_scoreContainer.HasStraight(false) == true && _scoreContainer.RowList[11].HasFilledIn() == false)
                _scoreContainer.RowList[11].Possible = 30;// for now; has to be large.  will later test for small
            if (HasFlush() == true && _scoreContainer.RowList[12].HasFilledIn() == false)
                _scoreContainer.RowList[12].Possible = 35;
            if (_scoreContainer.HasFullHouse() == true && _scoreContainer.RowList[13].HasFilledIn() == false)
                _scoreContainer.RowList[13].Possible = _scoreContainer.CalculateDiceTotal() + 15;
            if (_scoreContainer.HasFullHouse() == true && HasFlush() == true && _scoreContainer.RowList[14].HasFilledIn() == false)
                _scoreContainer.RowList[14].Possible = _scoreContainer.CalculateDiceTotal() + 20;
            if (_scoreContainer.HasKind(4) == true && _scoreContainer.RowList[15].HasFilledIn() == false)
                _scoreContainer.RowList[15].Possible = _scoreContainer.CalculateDiceTotal() + 25;
            if (_scoreContainer.RowList[16].HasFilledIn() == false)
                _scoreContainer.RowList[16].Possible = _scoreContainer.CalculateDiceTotal();
            if (_scoreContainer.HasAllFive() == true && _scoreContainer.RowList[17].HasFilledIn() == false)
                _scoreContainer.RowList[17].Possible = _scoreContainer.CalculateDiceTotal() + 50;
        }
    }
}