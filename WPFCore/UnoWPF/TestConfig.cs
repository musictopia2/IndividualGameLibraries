using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using UnoCP.Cards;
using UnoCP.Data;
using System.Threading.Tasks;

namespace UnoWPF
{
    public class TestConfig : ITestCardSetUp<UnoCardInformation, UnoPlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<UnoPlayerItem> playerlist, IListShuffler<UnoCardInformation> decklist)
        {
            //UnoPlayerItem player = playerlist.GetSelf();
            //for testing i will get 8 eights.
            //player.StartUpList = decklist.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //this is an example.


            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}