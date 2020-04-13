using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using RookCP.Cards;
using RookCP.Data;
using System.Threading.Tasks;

namespace RookWPF
{
    public class TestConfig : ITestCardSetUp<RookCardInformation, RookPlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<RookPlayerItem> playerlist, IListShuffler<RookCardInformation> decklist)
        {
            //RookPlayerItem player = playerlist.GetSelf();
            //for testing i will get 8 eights.
            //player.StartUpList = decklist.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //this is an example.


            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}