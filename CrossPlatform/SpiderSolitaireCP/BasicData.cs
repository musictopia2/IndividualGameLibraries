using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BaseSolitaireClassesCP.DataClasses;
using BasicGameFramework.Attributes;
using BasicGameFramework.BasicGameDataClasses;
namespace SpiderSolitaireCP
{
    [SingletonGame] //i liked this idea.  hopefully works here too.
    public class BasicData : ISolitaireData, IGameInfo
    {
        EnumMoveType ISolitaireData.MoveColumns => EnumMoveType.MoveColumn; //this is default.  can change to what i want.
        int ISolitaireData.WasteColumns => 0;
        int ISolitaireData.WasteRows => 0;

        int ISolitaireData.WastePiles => 10; //default is 7.  can change to what it really is.

        int ISolitaireData.Rows => 4;

        int ISolitaireData.Columns => 2;

        bool ISolitaireData.IsKlondike => false;

        int ISolitaireData.CardsNeededWasteBegin => 40;

        int ISolitaireData.CardsNeededMainBegin => 0;

        int ISolitaireData.Deals => 1;

        int ISolitaireData.CardsToDraw => 1;

        bool ISolitaireData.SuitsNeedToMatchForMainPile => true;

        bool ISolitaireData.ShowNextNeededOnMain => false;

        bool ISolitaireData.MainRound => false; //if its round suits; then will be true

        EnumGameType IGameInfo.GameType => EnumGameType.NewGame;

        bool IGameInfo.CanHaveExtraComputerPlayers => throw new NotImplementedException();

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.Solitaire;

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleOnly;

        string IGameInfo.GameName => "Spider Solitaire"; //replace with real name.

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 0;

        int IGameInfo.MaxPlayers => 0;

        bool IGameInfo.CanAutoSave => false;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyDevice;

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //most solitaire games has to be landscape.
    }
}