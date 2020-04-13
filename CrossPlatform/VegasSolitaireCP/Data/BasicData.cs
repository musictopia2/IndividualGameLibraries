using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.SolitaireClasses.DataClasses;

namespace VegasSolitaireCP.Data
{
    [SingletonGame]
    public class BasicData : IGameInfo, ISolitaireData
    {
        EnumMoveType ISolitaireData.MoveColumns => EnumMoveType.MoveColumn; //this is default.  can change to what i want.

        int ISolitaireData.WasteColumns => 0;
        int ISolitaireData.WasteRows => 0;

        int ISolitaireData.WastePiles => 7; //default is 7.  can change to what it really is.

        int ISolitaireData.Rows => 1;

        int ISolitaireData.Columns => 4;

        bool ISolitaireData.IsKlondike => true;

        int ISolitaireData.CardsNeededWasteBegin => 28;

        int ISolitaireData.CardsNeededMainBegin => 0;

        int ISolitaireData.Deals => 1;

        int ISolitaireData.CardsToDraw => 1;

        bool ISolitaireData.SuitsNeedToMatchForMainPile => true;

        bool ISolitaireData.ShowNextNeededOnMain => false;
        bool ISolitaireData.MainRound => false; //if its round suits; then will be true

        EnumGameType IGameInfo.GameType => EnumGameType.NewGame;

        bool IGameInfo.CanHaveExtraComputerPlayers => false;

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.Solitaire;

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleOnly;

        string IGameInfo.GameName => "Vegas Solitaire"; //replace with real name.

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 0;

        int IGameInfo.MaxPlayers => 0;

        bool IGameInfo.CanAutoSave => false;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyDevice;

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //most solitaire games has to be landscape.
    }
}
