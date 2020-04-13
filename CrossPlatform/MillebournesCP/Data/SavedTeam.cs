using BasicGameFrameworkLibrary.MultiplePilesObservable;
using CommonBasicStandardLibraries.CollectionClasses;
using MillebournesCP.Cards;
namespace MillebournesCP.Data
{
    public class SavedTeam
    {
        public CustomBasicList<MillebournesCardInformation> PreviousList { get; set; } = new CustomBasicList<MillebournesCardInformation>();
        public CustomBasicList<BasicPileInfo<MillebournesCardInformation>> SavedPiles { get; set; } = new CustomBasicList<BasicPileInfo<MillebournesCardInformation>>();
        public CustomBasicList<SafetyInfo> SafetyList { get; set; } = new CustomBasicList<SafetyInfo>();
        public int Wrongs { get; set; }
        public EnumHazardType CurrentHazard { get; set; }
        public bool CurrentSpeed { get; set; }
        public int Miles { get; set; }
        public int TotalScore { get; set; }
        public MillebournesCardInformation? CurrentCard { get; set; }
        public int Number200s { get; set; }
    }
}
