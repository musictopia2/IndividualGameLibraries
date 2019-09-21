using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.TestUtilities;
using BladesOfSteelCP;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BladesOfSteelXF
{
    public class TestConfig : ITestCardSetUp<RegularSimpleCard, BladesOfSteelPlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<BladesOfSteelPlayerItem> PlayerList, IListShuffler<RegularSimpleCard> DeckList)
        {
            BladesOfSteelPlayerItem ThisPlayer = PlayerList.GetSelf();
            //for testing i will get 8 eights.
            ThisPlayer.StartUpList = DeckList.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}