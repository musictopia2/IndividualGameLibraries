using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.TestUtilities;
using MonasteryCardGameCP;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace MonasteryCardGameWPF
{
    public class TestConfig : ITestCardSetUp<MonasteryCardInfo, MonasteryCardGamePlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<MonasteryCardGamePlayerItem> PlayerList, IListShuffler<MonasteryCardInfo> DeckList)
        {
            MonasteryCardGamePlayerItem ThisPlayer = PlayerList.GetSelf();
            ThisPlayer.StartUpList.AddRange(DeckList.Where(Items => Items.Value == EnumCardValueList.LowAce).Take(2));
            ThisPlayer.StartUpList.AddRange(DeckList.Where(items => items.Value == EnumCardValueList.Three).Take(1));
            ThisPlayer.StartUpList.AddRange(DeckList.Where(items => items.Value == EnumCardValueList.Queen).Take(1));
            ThisPlayer.StartUpList.AddRange(DeckList.Where(Items => Items.Value == EnumCardValueList.Five).Take(2));
            ThisPlayer.StartUpList.AddRange(DeckList.Where(Items => Items.Value == EnumCardValueList.Seven).Take(2));
            ThisPlayer.StartUpList.AddRange(DeckList.Where(Items => Items.Value == EnumCardValueList.Four).Take(1));
            ThisPlayer.StartUpList.AddRange(DeckList.Where(Items => Items.Value == EnumCardValueList.Six).Take(1));
            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}