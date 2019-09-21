using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using BasicGameFramework.RegularDeckOfCards;
namespace GalaxyCardGameCP
{
    [SingletonGame]
    public class GalaxyCardGameSaveInfo : BasicSavedTrickGamesClass<EnumSuitList, GalaxyCardGameCardInformation, GalaxyCardGamePlayerItem>
    { //anything needed for autoresume is here.
        public EnumGameStatus GameStatus { get; set; }
        private INewWinCard? _winUI;
        private GalaxyCardGameCardInformation _WinningCard = new GalaxyCardGameCardInformation();
        public GalaxyCardGameCardInformation WinningCard
        {
            get { return _WinningCard; }
            set
            {
                if (SetProperty(ref _WinningCard, value))
                {
                    if (_winUI == null)
                        return;
                    _winUI.ShowNewCard();
                }
            }
        }
        public void LoadWin(INewWinCard winUI)
        {
            _winUI = winUI;
        }
        public int LastWin { get; set; }
    }
}