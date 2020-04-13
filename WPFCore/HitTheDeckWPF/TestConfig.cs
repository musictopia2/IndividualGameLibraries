using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using HitTheDeckCP.Cards;
using HitTheDeckCP.Data;
using System.Threading.Tasks;

namespace HitTheDeckWPF
{
    public class TestConfig : ITestCardSetUp<HitTheDeckCardInformation, HitTheDeckPlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<HitTheDeckPlayerItem> playerlist, IListShuffler<HitTheDeckCardInformation> decklist)
        {
            //HitTheDeckPlayerItem player = playerlist.GetSelf();
            //for testing i will get 8 eights.
            //player.StartUpList = decklist.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //this is an example.


            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}