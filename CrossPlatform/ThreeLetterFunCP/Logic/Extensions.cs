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
using SkiaSharp;
using ThreeLetterFunCP.Data;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
//i think this is the most common things i like to do
namespace ThreeLetterFunCP.Logic
{
    internal static class Extensions
    {
        private static readonly CustomBasicList<string> _vowelList = new CustomBasicList<string>() { "A", "E", "I", "O", "U" };
        public static SKColor GetColorOfLetter(this string thisLetter)
        {
            if (_vowelList.Count != 5)
                throw new Exception("Must have 5 vowels");
            if (_vowelList.Exists(x => x == thisLetter))
                return SKColors.Red;
            return SKColors.Black;
        }
        public static void RemoveTiles(this IDeckDict<ThreeLetterFunCardData> thisList)
        {
            foreach (var thisCard in thisList)
                thisCard.ClearTiles();
        }
        public static void RemoveTiles(this CustomBasicList<TileInformation> thisList, ThreeLetterFunVMData model)
        {
            thisList.RemoveRange(0, 2);
            model.TileBoard1!.UpdateBoard(); //i think
        }
        public static void RemoveTiles(this CustomBasicList<TileInformation> thisList)
        {
            thisList.RemoveRange(0, 2);
        }
        public static void TakeTurns(this PlayerCollection<ThreeLetterFunPlayerItem> playerList)
        {
            playerList.ForEach(player =>
            {
                player.ClearTurn();
            });
        }
    }
}
