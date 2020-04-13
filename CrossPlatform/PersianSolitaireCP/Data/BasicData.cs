using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.SolitaireClasses.DataClasses;
namespace PersianSolitaireCP.Data
{
    [SingletonGame]
    public class BasicData : IGameInfo, ISolitaireData
    {
        EnumMoveType ISolitaireData.MoveColumns => EnumMoveType.MoveColumn; //this is default.  can change to what i want.

        int ISolitaireData.WasteColumns => 0;
        int ISolitaireData.WasteRows => 0;

        int ISolitaireData.WastePiles => 8; //default is 7.  can change to what it really is.

        int ISolitaireData.Rows => 4;

        int ISolitaireData.Columns => 2;

        bool ISolitaireData.IsKlondike => false;

        int ISolitaireData.CardsNeededWasteBegin => 64;

        int ISolitaireData.CardsNeededMainBegin => 0;

        int ISolitaireData.Deals => 3;

        int ISolitaireData.CardsToDraw => 1;

        bool ISolitaireData.SuitsNeedToMatchForMainPile => true;

        bool ISolitaireData.ShowNextNeededOnMain => false;

        bool ISolitaireData.MainRound => false; //if its round suits; then will be true

        EnumGameType IGameInfo.GameType => EnumGameType.NewGame;

        bool IGameInfo.CanHaveExtraComputerPlayers => false;

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.Solitaire;

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleOnly;

        string IGameInfo.GameName => "Persian Solitaire"; //replace with real name.

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 0;

        int IGameInfo.MaxPlayers => 0;

        bool IGameInfo.CanAutoSave => true;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyDevice;

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //most solitaire games has to be landscape.
    }
}
