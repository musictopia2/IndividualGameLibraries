using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using FluxxCP.Cards;
using FluxxCP.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace FluxxWPF
{
    public class TestConfig : ITestCardSetUp<FluxxCardInformation, FluxxPlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<FluxxPlayerItem> playerlist, IListShuffler<FluxxCardInformation> decklist)
        {
            FluxxPlayerItem player = playerlist.GetSelf();
            //for testing i will get 8 eights.
            //player.StartUpList = decklist.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //this is an example.
            player.StartUpList.AddRange(decklist.Where(x => x.Deck == 76));
            
            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}