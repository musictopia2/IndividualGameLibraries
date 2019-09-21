using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.TestUtilities;
using SkipboCP;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
namespace SkipboWPF
{
    public class TestConfig : ITestCardSetUp<SkipboCardInformation, SkipboPlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<SkipboPlayerItem> playerList, IListShuffler<SkipboCardInformation> deckList)
        {
            SkipboPlayerItem thisPlayer = playerList.GetSelf();
            thisPlayer.StartUpList = new DeckRegularDict<SkipboCardInformation>(); //sample too.
            5.Times(x =>
            {
                thisPlayer.StartUpList.AddRange(deckList.Where(items => items.Number == x).Take(1));
            });
            //for testing i will get 8 eights.
            //ThisPlayer.StartUpList = DeckList.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //this was example with regular deck of card.
            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}