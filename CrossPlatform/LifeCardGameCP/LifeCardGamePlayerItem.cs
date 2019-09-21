using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using Newtonsoft.Json;
namespace LifeCardGameCP
{
    public class LifeCardGamePlayerItem : PlayerSingleHand<LifeCardGameCardInformation>
    {
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