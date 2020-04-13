using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using FiveCrownsCP.Cards;
using FiveCrownsCP.Data;
using System.Threading.Tasks;

namespace FiveCrownsWPF
{
    public class TestConfig : ITestCardSetUp<FiveCrownsCardInformation, FiveCrownsPlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<FiveCrownsPlayerItem> playerlist, IListShuffler<FiveCrownsCardInformation> decklist)
        {
            //FiveCrownsPlayerItem player = playerlist.GetSelf();
            //for testing i will get 8 eights.
            //player.StartUpList = decklist.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //this is an example.


            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}