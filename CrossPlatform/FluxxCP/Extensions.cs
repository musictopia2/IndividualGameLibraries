using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
namespace FluxxCP
{
    public static class Extensions
    {
        public static void ScrambleKeepers(this PlayerCollection<FluxxPlayerItem> playerList)
        {
            var thisList = playerList.Where(items => items.KeeperList.Count > 0).Select(items => items.KeeperList.Count).ToCustomBasicList();
            if (thisList.Count < 2)
                throw new BasicBlankException("Cannot scramble the keepers");
            var firstKeeperTempList = playerList.Select(items => items.KeeperList).ToCustomBasicList();
            DeckRegularDict<KeeperCard> output = new DeckRegularDict<KeeperCard>();
            firstKeeperTempList.ForEach(thisItem => output.AddRange(thisItem));
            output.ShuffleList();
            if (output.Count != thisList.Sum(items => items))
                throw new BasicBlankException("Numbers don't match");
            playerList.ForEach(thisPlayer =>
            {
                int nums = thisList[thisPlayer.Id - 1]; //because 0 based
                thisPlayer.KeeperList.ReplaceRange(output.Take(nums));
                if (thisPlayer.KeeperList.Count != thisList[thisPlayer.Id - 1])
                    throw new BasicBlankException("Numbers don't match");
                output = output.Skip(nums).ToRegularDeckDict();
            });
        }
        public static DeckRegularDict<FluxxCardInformation> GetFluxxCardListFromDeck(this CustomBasicList<int> cardList, FluxxMainGameClass mainGame)
        {
            DeckRegularDict<FluxxCardInformation> output = new DeckRegularDict<FluxxCardInformation>();
            cardList.ForEach(thisDeck =>
            {
                output.Add(mainGame.DeckList!.GetSpecificItem(thisDeck));
            });
            return output;
        }
        public static int RulesThatCanBeDiscarded<T>(this CustomBasicList<T> ruleList)
        {
            int counts = ruleList.Count;
            if (counts <= 1)
                return 1;
            return counts / 2;
        }
    }
}