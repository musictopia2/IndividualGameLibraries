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
using BasicGameFrameworkLibrary.Attributes;
using SolitaireBoardGameCP.Data;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
//i think this is the most common things i like to do
namespace SolitaireBoardGameCP.Logic
{
    [SingletonGame]
    public class SolitaireGameEventHandler : ISolitaireBoardEvents
    {
        async Task ISolitaireBoardEvents.PiecePlacedAsync(GameSpace space, SolitaireBoardGameMainGameClass game)
        {
            if (game.IsValidMove(space) == false)
            {
                await UIPlatform.ShowMessageAsync("Illegal Move");
                await game.UnselectPieceAsync(space);
                return;
            }
            await game.MakeMoveAsync(space);
        }

        async Task ISolitaireBoardEvents.PieceSelectedAsync(GameSpace space, SolitaireBoardGameMainGameClass game)
        {
            if (space.Vector.Equals(game.PreviousPiece) == false)
            {
                await game.HightlightSpaceAsync(space);
                return;
            }
            game.SelectUnSelectSpace(space);
        }
    }
}
