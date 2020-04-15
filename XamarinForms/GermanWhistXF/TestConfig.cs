using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using GermanWhistCP.Data;
using GermanWhistCP.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GermanWhistXF
{
    public class TestConfig : ITestCardSetUp<GermanWhistCardInformation, GermanWhistPlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<GermanWhistPlayerItem> playerlist, IListShuffler<GermanWhistCardInformation> decklist)
        {
            //GermanWhistPlayerItem player = playerlist.GetSelf();
            //for testing i will get 8 eights.
            //player.StartUpList = decklist.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //this is an example.


            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}