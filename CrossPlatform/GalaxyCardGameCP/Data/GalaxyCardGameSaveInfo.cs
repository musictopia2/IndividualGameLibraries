using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using GalaxyCardGameCP.Cards;
using GalaxyCardGameCP.Logic;
namespace GalaxyCardGameCP.Data
{
    [SingletonGame]
    public class GalaxyCardGameSaveInfo : BasicSavedTrickGamesClass<EnumSuitList, GalaxyCardGameCardInformation, GalaxyCardGamePlayerItem>
    { //anything needed for autoresume is here.
        public EnumGameStatus GameStatus { get; set; }
        private INewWinCard? _winUI;
        private GalaxyCardGameCardInformation _winningCard = new GalaxyCardGameCardInformation();
        public GalaxyCardGameCardInformation WinningCard
        {
            get { return _winningCard; }
            set
            {
                if (SetProperty(ref _winningCard, value))
                {
                    if (_winUI == null)
                        return;
                    _winUI.ShowNewCard();
                }
            }
        }
        public void LoadWin(INewWinCard winUI) //iffy.
        {
            _winUI = winUI;
        }
        public int LastWin { get; set; }
    }
}