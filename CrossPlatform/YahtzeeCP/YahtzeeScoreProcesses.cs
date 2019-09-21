using BasicGameFramework.Dice;
using BasicGameFramework.YahtzeeStyleHelpers;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using SkiaSharp;
namespace YahtzeeCP
{
    public class YahtzeeScoreProcesses : IYahtzeeStyle<SimpleDice>
    {
        CustomBasicList<string> IYahtzeeStyle<SimpleDice>.GetBottomText => new CustomBasicList<string>()
        {
            "Three Of A Kind", "Four Of A Kind", "Full House (25)", "Small Straight (30)",
            "Large Straight (40)", "Yahtzee", "Chance"
        };

        bool IYahtzeeStyle<SimpleDice>.HasExceptionFor5Kind => false;

        int IYahtzeeStyle<SimpleDice>.BonusAmount(int topScore) //done.
        {
            if (topScore >= 63)
                return 35;
            return 0;
        }

        void IYahtzeeStyle<SimpleDice>.Extra5OfAKind(ScoresheetVM<SimpleDice> thisScore) //done.
        {
            if (thisScore.RowList[14].PointsObtained.HasValue == true)
            {
                if (thisScore.RowList[14].PointsObtained == 0)
                    return;
            }
            thisScore.RowList[14].PointsObtained += 100;

        }
        private readonly YahtzeeViewModel<SimpleDice> _thisMod;
        public YahtzeeScoreProcesses(YahtzeeViewModel<SimpleDice> thisMod)
        {
            _thisMod = thisMod;
        }




        CustomBasicList<DiceInformation> IYahtzeeStyle<SimpleDice>.GetDiceList()
        {
            var firstList = _thisMod.ThisCup!.DiceList.ToCustomBasicList();
            CustomBasicList<DiceInformation> output = new CustomBasicList<DiceInformation>();
            firstList.ForEach(items =>
            {
                DiceInformation ThisDice = new DiceInformation();
                ThisDice.Value = items.Value;
                ThisDice.Color = SKColors.Black;
                output.Add(ThisDice);
            });
            return output;
        }

        void IYahtzeeStyle<SimpleDice>.PopulateBottomScores(ScoresheetVM<SimpleDice> thisScore)
        {
            if (thisScore.HasKind(3) == true && thisScore.RowList[9].HasFilledIn() == false)
                thisScore.RowList[9].Possible = thisScore.CalculateDiceTotal;
            if (thisScore.HasKind(4) == true && thisScore.RowList[10].HasFilledIn() == false)
                thisScore.RowList[10].Possible = thisScore.CalculateDiceTotal;
            if (thisScore.HasFullHouse() == true && thisScore.RowList[11].HasFilledIn() == false)
                thisScore.RowList[11].Possible = 25;
            if (thisScore.HasStraight(true) == true && thisScore.RowList[12].HasFilledIn() == false)
                thisScore.RowList[12].Possible = 30;
            if (thisScore.HasStraight(false) == true && thisScore.RowList[13].HasFilledIn() == false)
                thisScore.RowList[13].Possible = 40;
            if (thisScore.HasAllFive() == true && thisScore.RowList[14].HasFilledIn() == false)
                thisScore.RowList[14].Possible = 50;
            if (thisScore.RowList[15].HasFilledIn() == false)
                thisScore.RowList[15].Possible = thisScore.CalculateDiceTotal;
        }
    }
}