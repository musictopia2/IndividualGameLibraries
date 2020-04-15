using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using HorseshoeCardGameCP.Data;
using HorseshoeCardGameCP.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HorseshoeCardGameXF
{
    public class TestConfig : ITestCardSetUp<HorseshoeCardGameCardInformation, HorseshoeCardGamePlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<HorseshoeCardGamePlayerItem> playerlist, IListShuffler<HorseshoeCardGameCardInformation> decklist)
        {
            //HorseshoeCardGamePlayerItem player = playerlist.GetSelf();
            //for testing i will get 8 eights.
            //player.StartUpList = decklist.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //this is an example.


            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}