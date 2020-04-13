using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using HorseshoeCardGameCP.Cards;
using HorseshoeCardGameCP.Data;
using System.Threading.Tasks;

namespace HorseshoeCardGameWPF
{
    public class TestConfig : ITestCardSetUp<HorseshoeCardGameCardInformation, HorseshoeCardGamePlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<HorseshoeCardGamePlayerItem> playerlist, IListShuffler<HorseshoeCardGameCardInformation> decklist)
        {
            //HorseshoeCardGamePlayerItem player = playerlist.GetSelf();
            //for testing i will get 8 eights.
            //player.StartUpList = decklist.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //this is an example.


            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}