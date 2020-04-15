using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using MillebournesCP.Data;
using MillebournesCP.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillebournesXF
{
    public class TestConfig : ITestCardSetUp<MillebournesCardInformation, MillebournesPlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<MillebournesPlayerItem> playerlist, IListShuffler<MillebournesCardInformation> decklist)
        {
            //MillebournesPlayerItem player = playerlist.GetSelf();
            //for testing i will get 8 eights.
            //player.StartUpList = decklist.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //this is an example.


            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}