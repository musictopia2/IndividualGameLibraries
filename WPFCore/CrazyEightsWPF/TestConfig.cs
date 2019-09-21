using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.TestUtilities;
using CrazyEightsCP;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace CrazyEightsWPF
{
    public class TestConfig : ITestCardSetUp<RegularSimpleCard, CrazyEightsPlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<CrazyEightsPlayerItem> PlayerList, IListShuffler<RegularSimpleCard> DeckList)
        {
            CrazyEightsPlayerItem ThisPlayer = PlayerList.GetSelf();
            //for testing i will get 8 eights.
            ThisPlayer.StartUpList = DeckList.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}