using CommonBasicStandardLibraries.CollectionClasses;

namespace MonasteryCardGameCP.Data
{
    public class MissionList
    {
        public string Description { get; set; } = "";
        public CustomBasicList<SetInfo> MissionSets { get; set; } = new CustomBasicList<SetInfo>();
    }
}