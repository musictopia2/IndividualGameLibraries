using BasicGameFramework.Attributes;
using BasicGameFramework.BasicGameDataClasses;
namespace ItalianDominosCP
{
    [SingletonGame]
    public class ItalianDominosDetailClass : IGameInfo
    {
        EnumGameType IGameInfo.GameType => EnumGameType.NewGame;

        bool IGameInfo.CanHaveExtraComputerPlayers => true;

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;

        string IGameInfo.GameName => "Italian Dominos";

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 3;

        int IGameInfo.MaxPlayers => 5;

        bool IGameInfo.CanAutoSave => false;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyDevice; //default to smallest but can change as needed.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //looks like most games are landscape.  can change to what is needed though.
    }
}