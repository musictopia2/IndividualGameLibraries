using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using SkipboCP.Cards;
using SkipboCP.Data;
using System.Threading.Tasks;

namespace SkipboWPF
{
    public class TestConfig : ITestCardSetUp<SkipboCardInformation, SkipboPlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<SkipboPlayerItem> playerlist, IListShuffler<SkipboCardInformation> decklist)
        {
            //SkipboPlayerItem player = playerlist.GetSelf();
            //for testing i will get 8 eights.
            //player.StartUpList = decklist.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //this is an example.


            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}