using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using HuseHeartsCP.Cards;
using HuseHeartsCP.Data;
using System.Threading.Tasks;

namespace HuseHeartsWPF
{
    public class TestConfig : ITestCardSetUp<HuseHeartsCardInformation, HuseHeartsPlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<HuseHeartsPlayerItem> playerlist, IListShuffler<HuseHeartsCardInformation> decklist)
        {
            //HuseHeartsPlayerItem player = playerlist.GetSelf();
            //for testing i will get 8 eights.
            //player.StartUpList = decklist.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //this is an example.


            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}