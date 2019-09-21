using BasicGameFramework.Attributes;
using BasicGameFramework.BasicGameDataClasses;
namespace RollEmCP
{
    [SingletonGame]
    public class RollEmDetailClass : IGameInfo
    {
        EnumGameType IGameInfo.GameType => EnumGameType.NewGame; //mostly new games.

        bool IGameInfo.CanHaveExtraComputerPlayers => false; //had too many bugs and not worth spending time to fix.

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.Either; //most dice games allow either.  if i am wrong, change.

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;

        string IGameInfo.GameName => "Roll Em"; //put in the name of the game here.

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 2;

        int IGameInfo.MaxPlayers => 6; //most dice games allow more players.  if i am wrong, change.

        bool IGameInfo.CanAutoSave => true;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyDevice; //default to smallest but can change as needed.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Portrait; //looks like most games are landscape.  can change to what is needed though.
    }
}
