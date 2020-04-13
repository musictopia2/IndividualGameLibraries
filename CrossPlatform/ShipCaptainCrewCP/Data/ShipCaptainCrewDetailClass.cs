using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
namespace ShipCaptainCrewCP.Data
{
    [SingletonGame]
    public class ShipCaptainCrewDetailClass : IGameInfo
    {
        EnumGameType IGameInfo.GameType => EnumGameType.NewGame; //mostly new games.

        bool IGameInfo.CanHaveExtraComputerPlayers => true;

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.Either; //most dice games allow either.  if i am wrong, change.
        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;
        //looks like i can't have / in the name since it uses that to decide how to save.
        //can change if i ever put into a database.
        string IGameInfo.GameName => "Ship Captain Crew"; //put in the name of the game here.

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 2;

        int IGameInfo.MaxPlayers => 10; //most dice games allow more players.  if i am wrong, change.

        bool IGameInfo.CanAutoSave => true;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyDevice; //default to smallest but can change as needed.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Portrait; //default to portrait but can change to what is needed.
    }
}