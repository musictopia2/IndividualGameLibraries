using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.TestUtilities;
using HitTheDeckCP;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace HitTheDeckWPF
{
    public class TestConfig : ITestCardSetUp<HitTheDeckCardInformation, HitTheDeckPlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<HitTheDeckPlayerItem> playerList, IListShuffler<HitTheDeckCardInformation> deckList)
        {
            HitTheDeckPlayerItem thisPlayer = playerList.Where(items => items.PlayerCategory == EnumPlayerCategory.OtherHuman).Single(); //assume 2 player game.
            //HitTheDeckPlayerItem thisPlayer = playerList.GetSelf();
            thisPlayer.StartUpList = new DeckRegularDict<HitTheDeckCardInformation>(); //sample too.
            thisPlayer.StartUpList.AddRange(deckList.Where(items => items.CardType == EnumTypeList.Flip).Take(1));
            thisPlayer.StartUpList.AddRange(deckList.Where(items => items.CardType == EnumTypeList.Cut).Take(1));
            thisPlayer.StartUpList.AddRange(deckList.Where(items => items.CardType == EnumTypeList.Draw4).Take(1));
            //for testing i will get 8 eights.
            //ThisPlayer.StartUpList = DeckList.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //this was example with regular deck of card.
            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}