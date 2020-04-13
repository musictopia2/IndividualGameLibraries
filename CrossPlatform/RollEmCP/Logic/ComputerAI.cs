using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
//i think this is the most common things i like to do
namespace RollEmCP.Logic
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
                        var combo = new ComboInfo();
                        combo.Number1 = thisList[x];
                        combo.Number2 = thisList[y];
                        output.Add(combo);
                    }
                }
            }
            return output;
        }
    }
}
