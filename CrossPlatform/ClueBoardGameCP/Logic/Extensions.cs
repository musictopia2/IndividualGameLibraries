using ClueBoardGameCP.Data;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System;
using System.Collections.Generic;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace ClueBoardGameCP.Logic
{
    public static class Extensions
    {
        public static void PlaceWeaponsInRooms(this Dictionary<int, WeaponInfo> thisList)
        {
            if (thisList.Count != 6)
                throw new BasicBlankException("There has to be 6 weapons");
            CustomBasicList<int> tempList = GetIntegerList(1, 9);
            tempList.ShuffleList();
            int x = 0;
            foreach (var thisWeapon in thisList.Values)
            {
                thisWeapon.Room = tempList[x];
                x++;
            }
        }
        public static void PlayerChoseColor(this Dictionary<int, CharacterInfo> thisList, ClueBoardGamePlayerItem thisPlayer)
        {
            int id = thisPlayer.Id;
            foreach (var thisCharacter in thisList.Values)
            {
                if (thisCharacter.MainColor.Equals(thisPlayer.Color.ToColor()))
                {
                    thisCharacter.Player = id;
                    return;
                }
            }
            throw new BasicBlankException("No color for player");
        }
    }
}
