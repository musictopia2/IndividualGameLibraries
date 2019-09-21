using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.TestUtilities;
using FluxxCP;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace FluxxXF
{
    public class TestConfig : ITestCardSetUp<FluxxCardInformation, FluxxPlayerItem>
    {
        Task ITestCardSetUp<FluxxCardInformation, FluxxPlayerItem>.SetUpTestHandsAsync(PlayerCollection<FluxxPlayerItem> PlayerList, IListShuffler<FluxxCardInformation> DeckList)
        {
            var firstPlayer = PlayerList.First();
            var secondPlayer = PlayerList.Last();

            //firstPlayer.StartUpList = DeckList.Where(items => items.CardType == EnumCardType.Keeper).Take(1).ToRegularDeckDict();
            //secondPlayer.StartUpList = DeckList.Where(items => items.CardType == EnumCardType.Keeper).Skip(1).Take(1).ToRegularDeckDict();
            //firstPlayer.StartUpList.AddRange(DeckList.Where(items => items.Deck == 5 || items.Deck == 81 || items.Deck == 13));
            firstPlayer.StartUpList.AddRange(DeckList.Where(items => items.Deck == 9));
            return Task.CompletedTask;
        }
    }
}