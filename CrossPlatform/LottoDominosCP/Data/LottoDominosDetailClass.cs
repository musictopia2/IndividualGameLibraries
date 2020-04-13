using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
namespace LottoDominosCP.Data
{
    [SingletonGame]
    public class LottoDominosDetailClass : IGameInfo
    {
        EnumGameType IGameInfo.GameType => EnumGameType.NewGame;

        bool IGameInfo.CanHaveExtraComputerPlayers => true;

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.Either;

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;

        string IGameInfo.GameName => "Lotto Dominos";

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 2;

        int IGameInfo.MaxPlayers => 7;

        bool IGameInfo.CanAutoSave => true;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyDevice;

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape;
    }
}
