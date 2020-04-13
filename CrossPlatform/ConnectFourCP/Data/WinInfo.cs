using CommonBasicStandardLibraries.CollectionClasses;

namespace ConnectFourCP.Data
{
    public class WinInfo
    {
        public CustomBasicList<SpaceInfoCP> WinList { get; set; } = new CustomBasicList<SpaceInfoCP>();
        public bool IsDraw { get; set; }
    }
}