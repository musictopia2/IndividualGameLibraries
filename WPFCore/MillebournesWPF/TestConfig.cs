using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.TestUtilities;
using MillebournesCP;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace MillebournesWPF
{
    public class TestConfig : ITestCardSetUp<MillebournesCardInformation, MillebournesPlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<MillebournesPlayerItem> playerList, IListShuffler<MillebournesCardInformation> deckList)
        {
            //thisPlayer.StartUpList = new DeckRegularDict<MillebournesCardInformation>(); //sample too.
            //thisPlayer.StartUpList.AddRange(deckList.Where(items => items.CardName == "").Take(1));
            var thisPlayer = playerList.Where(items => items.PlayerCategory == EnumPlayerCategory.OtherHuman).Single();
            thisPlayer.StartUpList.AddRange(deckList.Where(items => items.CompleteCategory == EnumCompleteCategories.SpeedLimit).Take(1));
            //for testing i will get 8 eights.
            //ThisPlayer.StartUpList = DeckList.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //this was example with regular deck of card.
            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}