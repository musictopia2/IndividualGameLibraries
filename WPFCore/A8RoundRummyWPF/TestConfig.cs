using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.TestUtilities;
using A8RoundRummyCP;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
namespace A8RoundRummyWPF
{
    public class TestConfig : ITestCardSetUp<A8RoundRummyCardInformation, A8RoundRummyPlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<A8RoundRummyPlayerItem> playerList, IListShuffler<A8RoundRummyCardInformation> deckList)
        {
            A8RoundRummyPlayerItem thisPlayer = playerList.GetSelf();
            thisPlayer.StartUpList = new DeckRegularDict<A8RoundRummyCardInformation>(); //sample too.
            7.Times(x =>
            {
                if (x == 7)
                    thisPlayer.StartUpList.AddRange(deckList.Where(items => items.CardType == EnumCardType.Wild).Take(1));
                else
                    thisPlayer.StartUpList.AddRange(deckList.Where(items => items.Value == x).Take(1));
            });
            return Task.CompletedTask;
        }
    }
}