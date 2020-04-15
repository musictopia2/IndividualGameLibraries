using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using Phase10CP.Cards;
using Phase10CP.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phase10XF
{
    public class TestConfig : ITestCardSetUp<Phase10CardInformation, Phase10PlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<Phase10PlayerItem> playerlist, IListShuffler<Phase10CardInformation> decklist)
        {
            //Phase10PlayerItem player = playerlist.GetSelf();
            //for testing i will get 8 eights.
            //player.StartUpList = decklist.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //this is an example.


            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}