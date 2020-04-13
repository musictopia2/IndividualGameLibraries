using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using MonopolyCardGameCP.Cards;
using MonopolyCardGameCP.Data;
using System.Threading.Tasks;

namespace MonopolyCardGameWPF
{
    public class TestConfig : ITestCardSetUp<MonopolyCardGameCardInformation, MonopolyCardGamePlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<MonopolyCardGamePlayerItem> playerlist, IListShuffler<MonopolyCardGameCardInformation> decklist)
        {
            //MonopolyCardGamePlayerItem player = playerlist.GetSelf();
            //for testing i will get 8 eights.
            //player.StartUpList = decklist.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //this is an example.


            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}