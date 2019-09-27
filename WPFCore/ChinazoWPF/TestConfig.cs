using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.TestUtilities;
using ChinazoCP;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace ChinazoWPF
{
    public class TestConfig : ITestCardSetUp<ChinazoCard, ChinazoPlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<ChinazoPlayerItem> playerList, IListShuffler<ChinazoCard> deckList)
        {
            ChinazoPlayerItem thisPlayer = playerList.GetSelf();
            //for testing i will get 8 eights.
            thisPlayer.StartUpList.AddRange(deckList.Where(items => items.Value == EnumCardValueList.Eight).Take(3));
            thisPlayer.StartUpList.AddRange(deckList.Where(items => items.Value == EnumCardValueList.Joker).Take(2));
            thisPlayer.StartUpList.AddRange(deckList.Where(items => items.Value == EnumCardValueList.HighAce && items.Suit == EnumSuitList.Spades).Take(1));
            thisPlayer.StartUpList.AddRange(deckList.Where(items => items.Value == EnumCardValueList.Two && items.Suit == EnumSuitList.Spades).Take(1));
            thisPlayer.StartUpList.AddRange(deckList.Where(items => items.Value == EnumCardValueList.Three && items.Suit == EnumSuitList.Spades).Take(1));
            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}