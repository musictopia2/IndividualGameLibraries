using BasicGameFramework.Attributes;
using BasicGameFramework.BasicGameDataClasses;
namespace AccordianSolitaireCP
{
    [SingletonGame]
    public class AccordianSolitaireDetailClass : IGameInfo
    {
        EnumGameType IGameInfo.GameType => EnumGameType.NewGame;

        bool IGameInfo.CanHaveExtraComputerPlayers => false;

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.Solitaire;

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleOnly;

        string IGameInfo.GameName => "Accordian Solitaire";

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 0;

        int IGameInfo.MaxPlayers => 0;

        bool IGameInfo.CanAutoSave => true; //if a game can't be saved, then mark false
        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.DesktopOnly; //for now, i am forced to desktop only because the reloading is too slow.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //this one for sure landscape.
    }
}