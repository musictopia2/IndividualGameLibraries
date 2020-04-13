using CommonBasicStandardLibraries.CollectionClasses;

namespace CousinRummyCP.Data
{
    public class SetList
    {
        public string Description { get; set; } = "";
        public CustomBasicList<SetInfo> PhaseSets = new CustomBasicList<SetInfo>();
    }
}