using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using YahtzeeHandsDownCP.Cards;
using YahtzeeHandsDownCP.Data;
using System.Threading.Tasks;

namespace YahtzeeHandsDownWPF
{
    public class TestConfig : ITestCardSetUp<YahtzeeHandsDownCardInformation, YahtzeeHandsDownPlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<YahtzeeHandsDownPlayerItem> playerlist, IListShuffler<YahtzeeHandsDownCardInformation> decklist)
        {
            //YahtzeeHandsDownPlayerItem player = playerlist.GetSelf();
            //for testing i will get 8 eights.
            //player.StartUpList = decklist.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //this is an example.


            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}