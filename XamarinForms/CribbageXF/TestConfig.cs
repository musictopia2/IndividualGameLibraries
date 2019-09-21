using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.TestUtilities;
using CribbageCP;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace CribbageXF
{
    public class TestConfig : ITestCardSetUp<CribbageCard, CribbagePlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<CribbagePlayerItem> PlayerList, IListShuffler<CribbageCard> DeckList)
        {
            CribbagePlayerItem ThisPlayer = PlayerList.GetSelf();
            //for testing i will get 8 eights.
            ThisPlayer.StartUpList = DeckList.Where(Items => Items.Value == EnumCardValueList.LowAce).Take(1).ToRegularDeckDict();
            ThisPlayer.StartUpList.AddRange(DeckList.Where(items => items.Value == EnumCardValueList.Two).Take(1));
            ThisPlayer.StartUpList.AddRange(DeckList.Where(items => items.Value == EnumCardValueList.Three).Take(1));
            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}