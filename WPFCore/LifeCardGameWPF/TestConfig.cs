using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using LifeCardGameCP.Cards;
using LifeCardGameCP.Data;
using System.Threading.Tasks;

namespace LifeCardGameWPF
{
    public class TestConfig : ITestCardSetUp<LifeCardGameCardInformation, LifeCardGamePlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<LifeCardGamePlayerItem> playerlist, IListShuffler<LifeCardGameCardInformation> decklist)
        {
            //LifeCardGamePlayerItem player = playerlist.GetSelf();
            //for testing i will get 8 eights.
            //player.StartUpList = decklist.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //this is an example.


            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}