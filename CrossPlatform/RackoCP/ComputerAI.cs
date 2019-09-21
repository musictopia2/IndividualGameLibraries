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
namespace RackoCP
{
    public static class ComputerAI
    {
        public static int PickUp(RackoMainGameClass MainGame)
        {
            return ComputerUse(MainGame, MainGame.ThisMod.Pile1.GetCardInfo().Deck);
        }
        private static int ComputerUse(RackoMainGameClass MainGame, int CardNum)
        {
            int MaxDiffs;
            MaxDiffs = MainGame.PlayerList.Count() + 2;
            int PossNum = 0;
            int PreviousCard = 0;
            int PrevPossNum = 0;
            MainGame.SingleInfo.MainHandList.ForEach(ThisCard =>
            {
                PossNum += MaxDiffs;
                if (ThisCard.Value > PrevPossNum && ThisCard.Value < PossNum && ThisCard.WillKeep == false)
                    ThisCard.WillKeep = true;
                PrevPossNum = PossNum;
            });
            PossNum = 0;
            PrevPossNum = 0;
            int x = 0;
            foreach (var ThisCard in MainGame.SingleInfo.MainHandList)
            {
                x++;
                PossNum += MaxDiffs;
                if (ThisCard.WillKeep == false && CardNum >= PossNum && CardNum <= PrevPossNum)
                    return x;
                if (ThisCard.WillKeep == false && PreviousCard > PossNum && CardNum <= PossNum)
                    return x;
                PrevPossNum = PossNum;
                PreviousCard = ThisCard.Value;
            }
            return 0;
        }
        public static int CardToPlay(RackoMainGameClass MainGame)
        {
            return ComputerUse(MainGame, MainGame.ThisMod.Pile2.GetCardInfo().Deck);
        }
    }
}
