using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
namespace HuseHeartsCP
{
    public static class Extensions
    {
        public static int WhoShotMoon(this PlayerCollection<HuseHeartsPlayerItem> thisList)
        {
            var firstList = thisList.Where(Items => Items.HadPoints == true).ToCustomBasicList();
            if (firstList.Count == 1)
                return firstList.Single().Id;
            return 0;
        }
    }
}