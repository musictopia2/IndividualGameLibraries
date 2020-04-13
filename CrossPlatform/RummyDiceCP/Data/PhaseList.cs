using CommonBasicStandardLibraries.CollectionClasses;

namespace RummyDiceCP.Data
{
    public class PhaseList
    {
        public string Description = "";
        public CustomBasicList<SetInfo> PhaseSets = new CustomBasicList<SetInfo>();
    }
}
