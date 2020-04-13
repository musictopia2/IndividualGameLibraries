using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using SorryCardGameCP.Cards;
using SorryCardGameCP.Data;
using System.Threading.Tasks;

namespace SorryCardGameWPF
{
    public class TestConfig : ITestCardSetUp<SorryCardGameCardInformation, SorryCardGamePlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<SorryCardGamePlayerItem> playerlist, IListShuffler<SorryCardGameCardInformation> decklist)
        {
            //SorryCardGamePlayerItem player = playerlist.GetSelf();
            //for testing i will get 8 eights.
            //player.StartUpList = decklist.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //this is an example.


            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}