using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using PickelCardGameCP.Data;
using PickelCardGameCP.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PickelCardGameXF
{
    public class TestConfig : ITestCardSetUp<PickelCardGameCardInformation, PickelCardGamePlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<PickelCardGamePlayerItem> playerlist, IListShuffler<PickelCardGameCardInformation> decklist)
        {
            //PickelCardGamePlayerItem player = playerlist.GetSelf();
            //for testing i will get 8 eights.
            //player.StartUpList = decklist.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //this is an example.


            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}