using BasicGameFramework.CommandClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.Exceptions;
using Newtonsoft.Json;
namespace SorryCardGameCP
{
    public class SorryCardGamePlayerItem : PlayerSingleHand<SorryCardGameCardInformation>, IPlayerBoardGame<EnumColorChoices>
    {
        private EnumColorChoices _Color = EnumColorChoices.None;
        public EnumColorChoices Color
        {
            get { return _Color; }
            set
            {
                if (SetProperty(ref _Color, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public bool DidChooseColor => Color != EnumColorChoices.None;
        public void Clear()
        {
            Color = EnumColorChoices.None;
        }
        public bool OtherTurn { get; set; }
        private int _HowManyAtHome;
        public int HowManyAtHome
        {
            get { return _HowManyAtHome; }
            set
            {
                if (SetProperty(ref _HowManyAtHome, value))
                {
                    //can decide what to do when property changes
                    if (value > 4)
                        throw new BasicBlankException("There can't ever be more than 4 at home.");
                    if (value < 0)
                        throw new BasicBlankException("There can't ever be less than 0 at home.");
                }
            }
        }
        [JsonIgnore]
        public BasicGameCommand? ClickCommand { get; set; }
        public void LoadCommand(SorryCardGameMainGameClass thisGame)
        {
            if (ClickCommand != null)
                return; //i think
            ClickCommand = new BasicGameCommand(thisGame.ThisMod!, async items =>
            {
                if (thisGame.ThisData!.MultiPlayer == true)
                    await thisGame.ThisNet!.SendAllAsync("sorryplayer", Id);
                await thisGame.SorryPlayerAsync(Id);
            }, items =>
            {
                if (thisGame.DidChooseColors == false)
                    return false;
                if (PlayerCategory == EnumPlayerCategory.Self)
                    return false; //you can't even click on your own pile.
                if (thisGame.SaveRoot!.GameStatus != EnumGameStatus.ChoosePlayerToSorry)
                    return false;
                return HowManyAtHome > 0;
            }, thisGame.ThisMod!, thisGame.ThisMod!.CommandContainer!);
        }
    }
}