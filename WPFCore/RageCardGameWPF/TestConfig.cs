using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.TestUtilities;
using RageCardGameCP;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace RageCardGameWPF
{
    public class TestConfig : ITestCardSetUp<RageCardGameCardInformation, RageCardGamePlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<RageCardGamePlayerItem> playerList, IListShuffler<RageCardGameCardInformation> deckList)
        {
            RageCardGamePlayerItem thisPlayer = playerList.GetSelf();
            thisPlayer.StartUpList = new DeckRegularDict<RageCardGameCardInformation>(); //sample too.
            //for testing i will get 8 eights.
            //ThisPlayer.StartUpList = DeckList.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //this was example with regular deck of card.
            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}