using BaseSolitaireClassesCP.DataClasses;
using BasicGameFramework.Attributes;
using BasicGameFramework.BasicGameDataClasses;
using System;
namespace FreeCellSolitaireCP
{
    [SingletonGame] //i liked this idea.  hopefully works here too.
    public class BasicData : ISolitaireData, IGameInfo
    {
        EnumMoveType ISolitaireData.MoveColumns => EnumMoveType.MoveColumn;
        int ISolitaireData.WasteColumns => 0;
        int ISolitaireData.WasteRows => 0;

        int ISolitaireData.WastePiles => 8; //default is 7.  can change to what it really is.

        int ISolitaireData.Rows => 1;

        int ISolitaireData.Columns => 4;

        bool ISolitaireData.IsKlondike => false;

        int ISolitaireData.CardsNeededWasteBegin => 52;

        int ISolitaireData.CardsNeededMainBegin => 0;

        int ISolitaireData.Deals => -1;

        int ISolitaireData.CardsToDraw => 1;

        bool ISolitaireData.SuitsNeedToMatchForMainPile => true;

        bool ISolitaireData.ShowNextNeededOnMain => false;

        bool ISolitaireData.MainRound => false; //if its round suits; then will be true

        EnumGameType IGameInfo.GameType => EnumGameType.NewGame;

        bool IGameInfo.CanHaveExtraComputerPlayers => throw new NotImplementedException();

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.Solitaire;

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleOnly;

        string IGameInfo.GameName => "Free Cell Solitaire"; //replace with real name.

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 0;

        int IGameInfo.MaxPlayers => 0;
        bool IGameInfo.CanAutoSave => true;
        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyDevice;

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //most solitaire games has to be landscape.
    }
}