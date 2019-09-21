using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.TestUtilities;
using GolfCardGameCP;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace GolfCardGameWPF
{
    public class TestConfig : ITestCardSetUp<RegularSimpleCard, GolfCardGamePlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<GolfCardGamePlayerItem> PlayerList, IListShuffler<RegularSimpleCard> DeckList)
        {
            GolfCardGamePlayerItem ThisPlayer = PlayerList.GetSelf();
            //for testing i will get 8 eights.
            //ThisPlayer.StartUpList = DeckList.Where(Items => Items.Value == EnumCardValueList.Four).Take(1).ToRegularDeckDict();
            //ThisPlayer.StartUpList.AddRange(DeckList.Where(items => items.Value == EnumCardValueList.Nine).Take(1).ToRegularDeckDict());
            //ThisPlayer.StartUpList.AddRange(DeckList.Where(items => items.Value == EnumCardValueList.King).Take(1).ToRegularDeckDict());
            //ThisPlayer.StartUpList.AddRange(DeckList.Where(items => items.Value == EnumCardValueList.Jack).Take(1).ToRegularDeckDict());


            ThisPlayer.StartUpList = DeckList.Where(Items => Items.Value == EnumCardValueList.Two).Take(2).ToRegularDeckDict();
            ThisPlayer.StartUpList.AddRange(DeckList.Where(items => items.Value == EnumCardValueList.Four).Take(1).ToRegularDeckDict());
            ThisPlayer.StartUpList.AddRange(DeckList.Where(items => items.Value == EnumCardValueList.Six).Take(1).ToRegularDeckDict());

            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}