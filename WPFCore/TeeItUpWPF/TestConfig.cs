using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.TestUtilities;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using TeeItUpCP;
namespace TeeItUpWPF
{
    public class TestConfig : ITestCardSetUp<TeeItUpCardInformation, TeeItUpPlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<TeeItUpPlayerItem> playerList, IListShuffler<TeeItUpCardInformation> deckList)
        {
            TeeItUpPlayerItem thisPlayer = playerList.Last();
            thisPlayer.StartUpList = new DeckRegularDict<TeeItUpCardInformation>(); //sample too.
            thisPlayer.StartUpList.AddRange(deckList.Where(items => items.Points == 1).Take(1));
            thisPlayer.StartUpList.AddRange(deckList.Where(items => items.Points == 2).Take(1));
            thisPlayer.StartUpList.AddRange(deckList.Where(items => items.Points == 3).Take(1));
            thisPlayer.StartUpList.AddRange(deckList.Where(items => items.Points == 4).Take(1));
            thisPlayer.StartUpList.AddRange(deckList.Where(items => items.Points == -5).Take(1));
            thisPlayer = playerList.First();
            thisPlayer.StartUpList.AddRange(deckList.Where(items => items.Points == 5).Take(1));
            thisPlayer.StartUpList.AddRange(deckList.Where(items => items.Points == 6).Take(1));
            thisPlayer.StartUpList.AddRange(deckList.Where(items => items.Points == 7).Take(1));
            thisPlayer.StartUpList.AddRange(deckList.Where(items => items.Points == 8).Take(1));
            thisPlayer.StartUpList.AddRange(deckList.Where(items => items.Points == 9).Take(1));
            thisPlayer.StartUpList.AddRange(deckList.Where(items => items.IsMulligan == true).Take(1));

            return Task.CompletedTask;
        }
    }
}