using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using FlinchCP.Cards;
using FlinchCP.Data;
using System.Threading.Tasks;

namespace FlinchWPF
{
    public class TestConfig : ITestCardSetUp<FlinchCardInformation, FlinchPlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<FlinchPlayerItem> playerlist, IListShuffler<FlinchCardInformation> decklist)
        {
            //FlinchPlayerItem player = playerlist.GetSelf();
            //for testing i will get 8 eights.
            //player.StartUpList = decklist.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //this is an example.


            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}