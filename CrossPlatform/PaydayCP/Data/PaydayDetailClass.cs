using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
namespace PaydayCP.Data
{
    [SingletonGame]
    public class PaydayDetailClass : IGameInfo
    {
        EnumGameType IGameInfo.GameType => EnumGameType.NewGame;

        bool IGameInfo.CanHaveExtraComputerPlayers => true;

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.Either; //most board games are pass and play.  if exceptions, then change.

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;

        string IGameInfo.GameName => "Payday"; //put in the name of the game here.

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 2;

        int IGameInfo.MaxPlayers => 6;
        bool IGameInfo.CanAutoSave => true;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.LargeDevices; //default to smallest but can change as needed.

        //attempt to have this as landscape instead of portrait to make it closer to layout on desktop (?)
        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //default to portrait but can change to what is needed.
    }
}
