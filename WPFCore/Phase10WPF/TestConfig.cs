using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.TestUtilities;
using Phase10CP;
using System.Linq;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace Phase10WPF
{
    public class TestConfig : ITestCardSetUp<Phase10CardInformation, Phase10PlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<Phase10PlayerItem> playerList, IListShuffler<Phase10CardInformation> deckList)
        {
            Phase10PlayerItem thisPlayer = playerList.GetSelf();
            thisPlayer.StartUpList = new DeckRegularDict<Phase10CardInformation>(); //sample too.
            thisPlayer.StartUpList.AddRange(deckList.Where(items => items.Number == 12).Take(5));

            //7.Times(x => thisPlayer.StartUpList.AddRange(deckList.Where(items => items.Number == x).Take(1)));
            thisPlayer.StartUpList.AddRange(deckList.Where(items => items.Number == 9).Take(3));
            thisPlayer.Phase = 9;
            thisPlayer.StartUpList.AddRange(deckList.Where(items => items.Number == 2).Take(2));
            //for testing i will get 8 eights.
            //ThisPlayer.StartUpList = DeckList.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //this was example with regular deck of card.
            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}