using CommonBasicStandardLibraries.CollectionClasses;
namespace TicTacToeCP.Data
{
    public class WinInfo
    {

        public CustomBasicList<SpaceInfoCP> WinList { get; set; } = new CustomBasicList<SpaceInfoCP>();
        public bool IsDraw { get; set; }
        public EnumWinCategory Category { get; set; }
    }
}