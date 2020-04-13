using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.Containers;
using BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.Data;
using BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.Logic;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using SkiaSharp;
namespace YahtzeeCP.Logic
{
    public class YahtzeeScoreProcesses : IYahtzeeStyle
    {
        private readonly ScoreContainer _scoreContainer;
        private readonly YahtzeeVMData<SimpleDice> _model;

        public YahtzeeScoreProcesses(ScoreContainer scoreContainer,
            YahtzeeVMData<SimpleDice> model
            )
        {
            _scoreContainer = scoreContainer;
            _model = model;
        }
        CustomBasicList<string> IYahtzeeStyle.GetBottomText => new CustomBasicList<string>()
        {
            "Three Of A Kind", "Four Of A Kind", "Full House (25)", "Small Straight (30)",
            "Large Straight (40)", "Yahtzee", "Chance"
        };
        bool IYahtzeeStyle.HasExceptionFor5Kind => false;

        int IYahtzeeStyle.BonusAmount(int topScore)
        {
            if (topScore >= 63)
            {
                return 35;
            }
            return 0;
        }

        void IYahtzeeStyle.Extra5OfAKind()
        {


            if (_scoreContainer.RowList[14].PointsObtained.HasValue == true)
            {
                if (_scoreContainer.RowList[14].PointsObtained == 0)
                {
                    return;
                }
            }
            _scoreContainer.RowList[14].PointsObtained += 100;
        }

        CustomBasicList<DiceInformation> IYahtzeeStyle.GetDiceList()
        {
            var firstList = _model.Cup!.DiceList.ToCustomBasicList();
            CustomBasicList<DiceInformation> output = new CustomBasicList<DiceInformation>();
            firstList.ForEach(items =>
            {
                DiceInformation dice = new DiceInformation();
                dice.Value = items.Value;
                dice.Color = SKColors.Black;
                output.Add(dice);
            });
            return output;
        }

        void IYahtzeeStyle.PopulateBottomScores()
        {
            if (_scoreContainer.HasKind(3) == true && _scoreContainer.RowList[9].HasFilledIn() == false)
                _scoreContainer.RowList[9].Possible = _scoreContainer.CalculateDiceTotal();
            if (_scoreContainer.HasKind(4) == true && _scoreContainer.RowList[10].HasFilledIn() == false)
                _scoreContainer.RowList[10].Possible = _scoreContainer.CalculateDiceTotal();
            if (_scoreContainer.HasFullHouse() == true && _scoreContainer.RowList[11].HasFilledIn() == false)
                _scoreContainer.RowList[11].Possible = 25;
            if (_scoreContainer.HasStraight(true) == true && _scoreContainer.RowList[12].HasFilledIn() == false)
                _scoreContainer.RowList[12].Possible = 30;
            if (_scoreContainer.HasStraight(false) == true && _scoreContainer.RowList[13].HasFilledIn() == false)
                _scoreContainer.RowList[13].Possible = 40;
            if (_scoreContainer.HasAllFive() == true && _scoreContainer.RowList[14].HasFilledIn() == false)
                _scoreContainer.RowList[14].Possible = 50;
            if (_scoreContainer.RowList[15].HasFilledIn() == false)
                _scoreContainer.RowList[15].Possible = _scoreContainer.CalculateDiceTotal();
        }
    }
}
