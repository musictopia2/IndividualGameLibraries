using BasicGameFramework.MultiplePilesViewModels;
using CommonBasicStandardLibraries.CollectionClasses;
namespace MillebournesCP
{
    public class SafetyInfo
    {
        public string SafetyName { get; set; } = "";
        public bool WasCoupe { get; set; }
    }
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
    public class TempData
    {
        public int Team { get; set; }
        public SavedTeam? SavedData { get; set; }
    }
    public class SendPlay
    {
        public int Team { get; set; }
        public int Deck { get; set; }
        public int Player { get; set; }
        public EnumPileType Pile { get; set; }
    }
    public struct CoupeInfo
    {
        public int Player;
        public int Card; // this is the card used for the play
    }
}