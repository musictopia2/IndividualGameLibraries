using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
namespace RollEmCP
{
    public static class ComputerAI
    {
        private struct ComboInfo
        {
            public int Number1;
            public int Number2;
        }
        public static CustomBasicList<int> NumberList(GameBoardProcesses gameBoard1)
        {
            CustomBasicList<int> output = new CustomBasicList<int>();
            var thisList = gameBoard1.GetNumberList();
            var newList = ComboList(thisList, gameBoard1);
            if (newList.Count == 0)
                return new CustomBasicList<int>();
            var thisCombo = newList.GetRandomItem();
            if (thisCombo.Number1 == 0)
                throw new BasicBlankException("The first number at least has to be filled out");
            if (gameBoard1.GetDiceTotal != (thisCombo.Number1 + thisCombo.Number2))
                throw new BasicBlankException("This was not even correct");
            output.Add(thisCombo.Number1);
            if (thisCombo.Number2 > 0)
                output.Add(thisCombo.Number2);
            return output;
        }
        private static CustomBasicList<ComboInfo> ComboList(CustomBasicList<int> thisList, GameBoardProcesses gameBoard1)
        {
            CustomBasicList<ComboInfo> output;
            int totals = gameBoard1.GetDiceTotal;
            output = thisList.Where(Items => Items == totals).Select(Items => new ComboInfo { Number1 = Items, Number2 = 0 }).ToCustomBasicList();
            int x;
            var loopTo = thisList.Count - 1;
            for (x = 0; x <= loopTo; x++)
            {
                var loopTo1 = thisList.Count - 1;
                for (var y = x + 1; y <= loopTo1; y++)
                {
                    if ((thisList[x] + thisList[y]) == totals)
                    {
                        var ThisCombo = new ComboInfo();
                        ThisCombo.Number1 = thisList[x];
                        ThisCombo.Number2 = thisList[y];
                        output.Add(ThisCombo);
                    }
                }
            }
            return output;
        }
    }
}