using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
namespace PyramidSolitaireCP.Data
{
    [SingletonGame]
    public class PyramidSolitaireDetailClass : IGameInfo
    {
        EnumGameType IGameInfo.GameType => EnumGameType.NewGame;

        bool IGameInfo.CanHaveExtraComputerPlayers => false;

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.Solitaire;

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleOnly;

        string IGameInfo.GameName => "Pyramid Solitaire";

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 0;

        int IGameInfo.MaxPlayers => 0;


        bool IGameInfo.CanAutoSave => false; //can't be saved because of how it saved positioning.

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyDevice; //default to smallest but can change as needed.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //default to portrait but can change to what is needed.
    }
}
