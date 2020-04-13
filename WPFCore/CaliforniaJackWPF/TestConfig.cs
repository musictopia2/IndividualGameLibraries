using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using CaliforniaJackCP.Cards;
using CaliforniaJackCP.Data;
using System.Threading.Tasks;

namespace CaliforniaJackWPF
{
    public class TestConfig : ITestCardSetUp<CaliforniaJackCardInformation, CaliforniaJackPlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<CaliforniaJackPlayerItem> playerlist, IListShuffler<CaliforniaJackCardInformation> decklist)
        {
            //CaliforniaJackPlayerItem player = playerlist.GetSelf();
            //for testing i will get 8 eights.
            //player.StartUpList = decklist.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //this is an example.


            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}