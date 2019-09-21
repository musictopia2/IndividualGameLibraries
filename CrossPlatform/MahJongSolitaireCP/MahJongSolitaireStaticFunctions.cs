using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace MahJongSolitaireCP
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