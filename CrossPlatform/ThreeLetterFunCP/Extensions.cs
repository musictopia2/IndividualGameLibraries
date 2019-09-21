using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using SkiaSharp;
using System;
namespace ThreeLetterFunCP
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
        public static void RemoveTiles(this CustomBasicList<TileInformation> thisList, ThreeLetterFunViewModel thisMod)
        {
            thisList.RemoveRange(0, 2);
            thisMod.TileBoard1!.UpdateBoard(); //i think
        }
        public static void RemoveTiles(this CustomBasicList<TileInformation> thisList)
        {
            thisList.RemoveRange(0, 2);
        }
        public static void TakeTurns(this PlayerCollection<ThreeLetterFunPlayerItem> playerList)
        {
            playerList.ForEach(ThisPlayer =>
            {
                ThisPlayer.ClearTurn();
            });
        }
    }
}