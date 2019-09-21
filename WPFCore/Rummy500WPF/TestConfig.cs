using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.TestUtilities;
using Rummy500CP;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace Rummy500WPF
{
    //public class TestConfig : ITestCardSetUp<RegularSimpleCard, Rummy500PlayerItem>
    //{
    //    public Task SetUpTestHandsAsync(PlayerCollection<Rummy500PlayerItem> PlayerList, IListShuffler<RegularSimpleCard> DeckList)
    //    {
    //        Rummy500PlayerItem ThisPlayer = PlayerList.GetSelf();
    //        //for testing i will get 8 eights.
    //        ThisPlayer.StartUpList = DeckList.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
    //        //can be anything you want.
    //        return Task.CompletedTask;
    //    }
    //}
}