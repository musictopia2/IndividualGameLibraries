using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.TestUtilities;
using ConcentrationCP;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace ConcentrationWPF
{
    public class TestConfig : ITestCardSetUp<RegularSimpleCard, ConcentrationPlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<ConcentrationPlayerItem> PlayerList, IListShuffler<RegularSimpleCard> DeckList)
        {
            ConcentrationPlayerItem ThisPlayer = PlayerList.GetSelf();
            //for testing i will get 8 eights.
            ThisPlayer.StartUpList = DeckList.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}