using LifeBoardGameCP.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LifeBoardGameCP.Logic
{
    internal static class PopulatePlayerProcesses
    {
        internal static void ObtainLife(LifeBoardGamePlayerItem thisPlayer)
        {
            thisPlayer.TilesCollected++;
        }

        internal static void FillInfo(LifeBoardGamePlayerItem thisPlayer)
        {
            var career1 = CareerChosen(thisPlayer, out string career2);
            thisPlayer.Career1 = career1;
            thisPlayer.Career2 = career2;
            thisPlayer.HouseName = thisPlayer.GetHouseName();
        }
        public static string CareerChosen(LifeBoardGamePlayerItem thisPlayer, out string secondCareer)
        {
            secondCareer = ""; //until proven.
            var tempList = thisPlayer.GetCareerList();
            string output;
            var thisCareer = tempList.SingleOrDefault(items => items.Career == EnumCareerType.Teacher);
            if (thisCareer != null && thisCareer.Deck > 0)
            {
                output = "Teacher";
                var newCareer = tempList.SingleOrDefault(items => items.Career != EnumCareerType.Teacher);
                if (newCareer != null && newCareer.Deck > 0)
                    secondCareer = newCareer.Career.ToString();
                return output;
            }
            if (tempList.Count == 0)
                return "";
            return tempList.Single().Career.ToString();
        }
    }
}
