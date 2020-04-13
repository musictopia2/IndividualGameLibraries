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
using MahJongSolitaireCP.Data;
//i think this is the most common things i like to do
namespace MahJongSolitaireCP.Logic
{
    public static class MahJongSolitaireStaticFunctions
    {

        private static async Task<CustomBasicList<BoardInfo>> GetPreviousListAsync(CustomBasicList<BoardInfo> BoardList)
        {
            string ThisStr = await js.SerializeObjectAsync(BoardList);
            return await js.DeserializeObjectAsync<CustomBasicList<BoardInfo>>(ThisStr);
        }

        public static async Task SaveMoveAsync(MahJongSolitaireSaveInfo Games)
        {
            Games.PreviousList = await GetPreviousListAsync(Games.BoardList);
        }

    }
}
