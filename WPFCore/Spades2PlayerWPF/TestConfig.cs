using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.TestUtilities;
using Spades2PlayerCP;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace Spades2PlayerWPF
{
    public class TestConfig : ITestCardSetUp<Spades2PlayerCardInformation, Spades2PlayerPlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<Spades2PlayerPlayerItem> playerList, IListShuffler<Spades2PlayerCardInformation> deckList)
        {
            Spades2PlayerPlayerItem thisPlayer = playerList.GetSelf();
            thisPlayer.StartUpList = new DeckRegularDict<Spades2PlayerCardInformation>(); //sample too.
            //for testing i will get 8 eights.
            //ThisPlayer.StartUpList = DeckList.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //this was example with regular deck of card.
            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}