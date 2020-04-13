using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using SkuckCardGameCP.Cards;
using SkuckCardGameCP.Data;
using System.Threading.Tasks;

namespace SkuckCardGameWPF
{
    public class TestConfig : ITestCardSetUp<SkuckCardGameCardInformation, SkuckCardGamePlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<SkuckCardGamePlayerItem> playerlist, IListShuffler<SkuckCardGameCardInformation> decklist)
        {
            //SkuckCardGamePlayerItem player = playerlist.GetSelf();
            //for testing i will get 8 eights.
            //player.StartUpList = decklist.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //this is an example.


            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}