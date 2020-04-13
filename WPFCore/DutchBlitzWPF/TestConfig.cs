using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using DutchBlitzCP.Cards;
using DutchBlitzCP.Data;
using System.Threading.Tasks;

namespace DutchBlitzWPF
{
    public class TestConfig : ITestCardSetUp<DutchBlitzCardInformation, DutchBlitzPlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<DutchBlitzPlayerItem> playerlist, IListShuffler<DutchBlitzCardInformation> decklist)
        {
            //DutchBlitzPlayerItem player = playerlist.GetSelf();
            //for testing i will get 8 eights.
            //player.StartUpList = decklist.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //this is an example.


            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}