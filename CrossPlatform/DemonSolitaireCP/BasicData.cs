using BaseSolitaireClassesCP.DataClasses;
using BasicGameFramework.Attributes;
using BasicGameFramework.BasicGameDataClasses;
using System;
namespace DemonSolitaireCP
{
    [SingletonGame] //i liked this idea.  hopefully works here too.
    public class BasicData : ISolitaireData, IGameInfo
    {
        EnumMoveType ISolitaireData.MoveColumns => EnumMoveType.MoveColumn; //this is default.  can change to what i want.

        int ISolitaireData.WasteColumns => 0;
        int ISolitaireData.WasteRows => 0;

        int ISolitaireData.WastePiles => 4; //default is 7.  can change to what it really is.

        int ISolitaireData.Rows => 1;

        int ISolitaireData.Columns => 4;

        bool ISolitaireData.IsKlondike => false;

        int ISolitaireData.CardsNeededWasteBegin => 4;

        int ISolitaireData.CardsNeededMainBegin => 1;

        int ISolitaireData.Deals => -1;

        int ISolitaireData.CardsToDraw => 3;

        bool ISolitaireData.SuitsNeedToMatchForMainPile => true;

        bool ISolitaireData.ShowNextNeededOnMain => false;

        bool ISolitaireData.MainRound => true; //if its round suits; then will be true

        EnumGameType IGameInfo.GameType => EnumGameType.NewGame;

        bool IGameInfo.CanHaveExtraComputerPlayers => throw new NotImplementedException();

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.Solitaire;

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleOnly;

        string IGameInfo.GameName => "Demon Solitaire"; //replace with real name.

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 0;

        int IGameInfo.MaxPlayers => 0;

        bool IGameInfo.CanAutoSave => true;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet; //not enough room for phone unfortunately.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //most solitaire games has to be landscape.
    }
}