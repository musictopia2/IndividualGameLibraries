using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using Spades2PlayerCP.Cards;
using Spades2PlayerCP.Data;
using System.Threading.Tasks;

namespace Spades2PlayerWPF
{
    public class TestConfig : ITestCardSetUp<Spades2PlayerCardInformation, Spades2PlayerPlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<Spades2PlayerPlayerItem> playerlist, IListShuffler<Spades2PlayerCardInformation> decklist)
        {
            //Spades2PlayerPlayerItem player = playerlist.GetSelf();
            //for testing i will get 8 eights.
            //player.StartUpList = decklist.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //this is an example.


            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}