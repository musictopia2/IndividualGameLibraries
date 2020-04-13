using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using LifeCardGameCP.Cards;
using LifeCardGameCP.Logic;
using Newtonsoft.Json;

namespace LifeCardGameCP.Data
{
    public class LifeCardGamePlayerItem : PlayerSingleHand<LifeCardGameCardInformation>
    { //anything needed is here
        private int _Points;

        public int Points
        {
            get { return _Points; }
            set
            {
                if (SetProperty(ref _Points, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public string LifeString { get; set; } = ""; //iffy.
        [JsonIgnore]
        public LifeStoryHand? LifeStory { get; set; }
    }
}
