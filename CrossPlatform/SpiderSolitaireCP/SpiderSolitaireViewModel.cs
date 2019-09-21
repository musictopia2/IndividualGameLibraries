using BaseSolitaireClassesCP.MainClasses;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.DIContainers;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace SpiderSolitaireCP
{
    public class SpiderSolitaireViewModel : SolitaireMainViewModel<SpiderSolitaireSaveInfo>
    {
        public SpiderSolitaireViewModel(ISimpleUI tempUI, IGamePackageResolver tempC) : base(tempUI, tempC) { }
        public int LevelChosen { get; set; } = 1; //1 is easiest.
        public ListViewPicker? Levels1;

        public override void Init()
        {
            base.Init();
            CanStartNewGameImmediately = false;
            Levels1 = new ListViewPicker(this);
            Levels1.IndexMethod = ListViewPicker.EnumIndexMethod.OneBased;
            Levels1.Visible = true;
            var thisList = new CustomBasicList<string>() { "1 Suit", "2 Suits", "4 Suits" };
            Levels1.LoadTextList(thisList);
            Levels1.ItemSelectedAsync += Levels1_ItemSelectedAsync;
            Levels1.SelectSpecificItem(1);
        }

        private Task Levels1_ItemSelectedAsync(int SelectedIndex, string SelectedText)
        {
            LevelChosen = SelectedIndex;
            return Task.CompletedTask;
        }
    }
}