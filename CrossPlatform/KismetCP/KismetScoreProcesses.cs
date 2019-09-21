using BasicGameFramework.YahtzeeStyleHelpers;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using SkiaSharpGeneralLibrary.SKExtensions;
using System.Linq;
namespace KismetCP
{
    public class KismetScoreProcesses : IYahtzeeStyle<KismetDice>
    {
        CustomBasicList<string> IYahtzeeStyle<KismetDice>.GetBottomText => new CustomBasicList<string>()
        {
            "2 Pair Same Color", "3 Of A Kind", "Straight (30)",
            "Flush All Same Color (35)", "Full House", "Full House Same Color",
            "4 Of A Kind", "Yarborough", "Kismet (5 Of A Kind)"
        };

        bool IYahtzeeStyle<KismetDice>.HasExceptionFor5Kind => true;

        int IYahtzeeStyle<KismetDice>.BonusAmount(int topScore)
        {
            if (topScore < 63)
                return 0;
            if (topScore <= 70)
                return 35;
            if (topScore <= 77)
                return 55;
            return 75;
        }

        void IYahtzeeStyle<KismetDice>.Extra5OfAKind(ScoresheetVM<KismetDice> thisScore) //i think this is it.
        {
            var firstList = _thisGame.PlayerList.ToCustomBasicList();
            firstList.RemoveSpecificItem(_thisGame.SingleInfo!);
            firstList.ForEach(Items => Items.MissNextTurn = true);
        }

        private readonly YahtzeeViewModel<KismetDice> _thisMod;
        private readonly BasicYahtzeeGame<KismetDice> _thisGame;
        public KismetScoreProcesses(YahtzeeViewModel<KismetDice> thisMod, BasicYahtzeeGame<KismetDice> thisGame)
        {
            _thisMod = thisMod;
            _thisGame = thisGame;
        }

        CustomBasicList<DiceInformation> IYahtzeeStyle<KismetDice>.GetDiceList()
        {
            var firstList = _thisMod.ThisCup!.DiceList.ToCustomBasicList();
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

        private bool HasFlush(ScoresheetVM<KismetDice> thisScore)
        {
            int count = thisScore.DiceList.MaximumDuplicates(Items => Items.Color);
            return count == 5;
        }
        private bool HasTwoPairSameColor(ScoresheetVM<KismetDice> thisScore)
        {
            if (thisScore.HasFullHouse() == true && HasFlush(thisScore) == true)
                return true;
            var tempList = thisScore.DiceList.GroupOrderDescending(Items => Items.Color);
            if (tempList.Count() > 2)
                return false;
            if (tempList.First().Count() < 4)
                return false; //i did that in the old.  if that is not correct, rethink.
            var first = thisScore.DiceList.Where(Items => Items.Color == tempList.First().Key)
                .GroupOrderDescending(Items => Items.Value).ToCustomBasicList();
            if (first.First().Count() == 4)
                return true;
            if (first.First().Count() != 2)
                return false;
            if (first[1].Count() != 2)
                return false;
            return true; //hopefully this works.  if i am wrong, rethink.
        }
        void IYahtzeeStyle<KismetDice>.PopulateBottomScores(ScoresheetVM<KismetDice> thisScore)
        {
            if (HasTwoPairSameColor(thisScore) == true && thisScore.RowList[9].HasFilledIn() == false)
                thisScore.RowList[9].Possible = thisScore.CalculateDiceTotal;
            if (thisScore.HasKind(3) == true && thisScore.RowList[10].HasFilledIn() == false)
                thisScore.RowList[10].Possible = thisScore.CalculateDiceTotal;
            if (thisScore.HasStraight(false) == true && thisScore.RowList[11].HasFilledIn() == false)
                thisScore.RowList[11].Possible = 30;// for now; has to be large.  will later test for small
            if (HasFlush(thisScore) == true && thisScore.RowList[12].HasFilledIn() == false)
                thisScore.RowList[12].Possible = 35;
            if (thisScore.HasFullHouse() == true && thisScore.RowList[13].HasFilledIn() == false)
                thisScore.RowList[13].Possible = thisScore.CalculateDiceTotal + 15;
            if (thisScore.HasFullHouse() == true && HasFlush(thisScore) == true && thisScore.RowList[14].HasFilledIn() == false)
                thisScore.RowList[14].Possible = thisScore.CalculateDiceTotal + 20;
            if (thisScore.HasKind(4) == true && thisScore.RowList[15].HasFilledIn() == false)
                thisScore.RowList[15].Possible = thisScore.CalculateDiceTotal + 25;
            if (thisScore.RowList[16].HasFilledIn() == false)
                thisScore.RowList[16].Possible = thisScore.CalculateDiceTotal;
            if (thisScore.HasAllFive() == true && thisScore.RowList[17].HasFilledIn() == false)
                thisScore.RowList[17].Possible = thisScore.CalculateDiceTotal + 50;// for now; 50  rethink extra yahtzee
        }
    }
}