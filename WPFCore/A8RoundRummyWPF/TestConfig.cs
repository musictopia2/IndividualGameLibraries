using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using A8RoundRummyCP.Cards;
using A8RoundRummyCP.Data;
using System.Threading.Tasks;

namespace A8RoundRummyWPF
{
    public class TestConfig : ITestCardSetUp<A8RoundRummyCardInformation, A8RoundRummyPlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<A8RoundRummyPlayerItem> playerlist, IListShuffler<A8RoundRummyCardInformation> decklist)
        {
            //A8RoundRummyPlayerItem player = playerlist.GetSelf();
            //for testing i will get 8 eights.
            //player.StartUpList = decklist.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //this is an example.


            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}