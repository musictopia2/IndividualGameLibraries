using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using Pinochle2PlayerCP.Data;
using Pinochle2PlayerCP.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pinochle2PlayerXF
{
    public class TestConfig : ITestCardSetUp<Pinochle2PlayerCardInformation, Pinochle2PlayerPlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<Pinochle2PlayerPlayerItem> playerlist, IListShuffler<Pinochle2PlayerCardInformation> decklist)
        {
            //Pinochle2PlayerPlayerItem player = playerlist.GetSelf();
            //for testing i will get 8 eights.
            //player.StartUpList = decklist.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //this is an example.


            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}