using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using DutchBlitzCP.Data;
using DutchBlitzCP.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DutchBlitzXF
{
    public class TestConfig : ITestCardSetUp<DutchBlitzCardInformation, DutchBlitzPlayerItem>
    {
        public Task SetUpTestHandsAsync(PlayerCollection<DutchBlitzPlayerItem> playerlist, IListShuffler<DutchBlitzCardInformation> decklist)
        {
            //DutchBlitzPlayerItem player = playerlist.GetSelf();
            //for testing i will get 8 eights.
            //player.StartUpList = decklist.Where(Items => Items.Value == EnumCardValueList.Eight).Take(2).ToRegularDeckDict();
            //this is an example.


            //can be anything you want.
            return Task.CompletedTask;
        }
    }
}