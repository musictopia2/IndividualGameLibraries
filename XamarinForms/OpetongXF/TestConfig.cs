using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.TestUtilities;
using OpetongCP;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace OpetongXF
{
    public class TestConfig : ITestCardSetUp<RegularRummyCard, OpetongPlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<OpetongPlayerItem> PlayerList, IListShuffler<RegularRummyCard> DeckList)
        {
            OpetongPlayerItem ThisPlayer = PlayerList.GetSelf();
            //for testing i will get 8 eights.
            ThisPlayer.StartUpList = DeckList.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}