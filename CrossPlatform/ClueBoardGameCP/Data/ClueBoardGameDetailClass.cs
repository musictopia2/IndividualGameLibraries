using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
namespace ClueBoardGameCP.Data
{
    [SingletonGame]
    public class ClueBoardGameDetailClass : IGameInfo, IPlayerNeeds
    {
        EnumGameType IGameInfo.GameType => EnumGameType.NewGame;

        bool IGameInfo.CanHaveExtraComputerPlayers => false;

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked; //computer ai had too many problems now.

        string IGameInfo.GameName => "Clue Board Game";

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 2;

        int IGameInfo.MaxPlayers => 6;

        bool IGameInfo.CanAutoSave => false;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.LargeDevices; //default to smallest but can change as needed.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Portrait; //looks like most games are landscape.  can change to what is needed though.
        int IPlayerNeeds.PlayersNeeded => 6; //default to portrait but can change to what is needed.
    }
}
