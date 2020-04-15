using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using RoundsCardGameCP.Data;
using RoundsCardGameCP.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoundsCardGameXF
{
    public class TestConfig : ITestCardSetUp<RoundsCardGameCardInformation, RoundsCardGamePlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<RoundsCardGamePlayerItem> playerlist, IListShuffler<RoundsCardGameCardInformation> decklist)
        {
            //RoundsCardGamePlayerItem player = playerlist.GetSelf();
            //for testing i will get 8 eights.
            //player.StartUpList = decklist.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //this is an example.


            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}