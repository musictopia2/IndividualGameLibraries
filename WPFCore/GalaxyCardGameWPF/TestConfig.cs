using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using GalaxyCardGameCP.Cards;
using GalaxyCardGameCP.Data;
using System.Linq;
using System.Threading.Tasks;

namespace GalaxyCardGameWPF
{
    public class TestConfig : ITestCardSetUp<GalaxyCardGameCardInformation, GalaxyCardGamePlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<GalaxyCardGamePlayerItem> playerlist, IListShuffler<GalaxyCardGameCardInformation> decklist)
        {
            GalaxyCardGamePlayerItem player = playerlist.GetSelf();
            //for testing i will get 8 eights.
            //player.StartUpList = decklist.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //this is an example.
            player.StartUpList.AddRange(decklist.Where(x => x.Value == EnumCardValueList.HighAce));

            player.StartUpList.AddRange(decklist.Where(x => x.Value == EnumCardValueList.Five && x.Suit == EnumSuitList.Clubs));
            player.StartUpList.AddRange(decklist.Where(x => x.Value == EnumCardValueList.Eight));

            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}