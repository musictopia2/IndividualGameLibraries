using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using RackoCP.Cards;
using RackoCP.Data;
using System.Threading.Tasks;

namespace RackoWPF
{
    public class TestConfig : ITestCardSetUp<RackoCardInformation, RackoPlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<RackoPlayerItem> playerlist, IListShuffler<RackoCardInformation> decklist)
        {
            //RackoPlayerItem player = playerlist.GetSelf();
            //for testing i will get 8 eights.
            //player.StartUpList = decklist.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //this is an example.


            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}