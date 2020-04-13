using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using RageCardGameCP.Cards;
using RageCardGameCP.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RageCardGameWPF
{
    public class TestConfig : ITestCardSetUp<RageCardGameCardInformation, RageCardGamePlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<RageCardGamePlayerItem> playerlist, IListShuffler<RageCardGameCardInformation> decklist)
        {
            RageCardGamePlayerItem player = playerlist.GetSelf();
            //for testing i will get 8 eights.
            //player.StartUpList = decklist.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //this is an example.
            player.StartUpList.AddRange(decklist.Where(x => x.Color == EnumColor.Purple && x.Value == 15).Take(1).ToRegularDeckDict());
            
            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}