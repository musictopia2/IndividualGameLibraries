using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using XactikaCP.Cards;
using XactikaCP.Data;
using System.Threading.Tasks;

namespace XactikaWPF
{
    public class TestConfig : ITestCardSetUp<XactikaCardInformation, XactikaPlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<XactikaPlayerItem> playerlist, IListShuffler<XactikaCardInformation> decklist)
        {
            //XactikaPlayerItem player = playerlist.GetSelf();
            //for testing i will get 8 eights.
            //player.StartUpList = decklist.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //this is an example.


            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}