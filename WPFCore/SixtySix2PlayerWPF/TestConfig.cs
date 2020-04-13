using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using SixtySix2PlayerCP.Cards;
using SixtySix2PlayerCP.Data;
using System.Threading.Tasks;

namespace SixtySix2PlayerWPF
{
    public class TestConfig : ITestCardSetUp<SixtySix2PlayerCardInformation, SixtySix2PlayerPlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<SixtySix2PlayerPlayerItem> playerlist, IListShuffler<SixtySix2PlayerCardInformation> decklist)
        {
            //SixtySix2PlayerPlayerItem player = playerlist.GetSelf();
            //for testing i will get 8 eights.
            //player.StartUpList = decklist.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //this is an example.


            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}