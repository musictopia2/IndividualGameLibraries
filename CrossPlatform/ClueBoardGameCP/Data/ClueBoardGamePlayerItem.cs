using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using ClueBoardGameCP.Cards;
using Newtonsoft.Json;

namespace ClueBoardGameCP.Data
{
    public class ClueBoardGamePlayerItem : PlayerBoardGame<EnumColorChoice>, IPlayerObject<CardInfo>
    { //anything needed is here
        [JsonIgnore]
        public override bool DidChooseColor => Color != EnumColorChoice.None;

        public override void Clear()
        {
            Color = EnumColorChoice.None;
        }
        public DeckObservableDict<CardInfo> MainHandList { get; set; } = new DeckObservableDict<CardInfo>();
        public override bool CanStartInGame
        {
            get
            {
                if (PlayerCategory != EnumPlayerCategory.Computer)
                    return true;
                if (NickName.StartsWith("Computeridle"))
                    return false;
                return true;
            }
        }
        [JsonIgnore]
        public DeckRegularDict<CardInfo> StartUpList { get; set; } = new DeckRegularDict<CardInfo>();
    }
}