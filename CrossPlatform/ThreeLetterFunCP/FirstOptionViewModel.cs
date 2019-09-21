using BasicGameFramework.Attributes;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommandClasses;
using BasicGameFramework.MiscViewModels;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using static BasicGameFramework.ChooserClasses.ListViewPicker;
namespace ThreeLetterFunCP
{
    [SingletonGame] //try this.
    public class FirstOptionViewModel : SimpleControlViewModel
    {

        private EnumFirstOption _OptionChosen = EnumFirstOption.None;
        public EnumFirstOption OptionChosen
        {
            get
            {
                return _OptionChosen;
            }

            set
            {
                if (SetProperty(ref _OptionChosen, value) == true) { }
            }
        }
        public ListViewPicker Option1;
        public ControlCommand DescriptionCommand { get; set; }
        public PlainCommand SubmitCommand { get; set; }

        public FirstOptionViewModel(IBasicGameVM thisMod, IFirstOptions firstOptions) : base(thisMod)
        {
            Option1 = new ListViewPicker(thisMod);
            Option1.ItemSelectedAsync += Option1_ItemSelectedAsync;
            CustomBasicList<string> ThisList = new CustomBasicList<string>() { "Beginner", "Advanced" };
            Option1.IndexMethod = EnumIndexMethod.OneBased;
            Option1.LoadTextList(ThisList);
            DescriptionCommand = new ControlCommand(this,
                async items => await thisMod.ShowGameMessageAsync("The beginner option only allows easy words to be formed.  Plus the beginner also has a choice start out with 4, 6, 8, or 10 cards.  Whoeever gets rid of their cards first by spelling them from the tiles wins." + Constants.vbCrLf + "The advanced option has a choice between allowing easy words or any common 3 letter words.  Also; for the advanced option; all the cards are layed out.  There is a short option in which the first player who spells 5 words correctly wins.  For the regular; once all the cards or tiles are gone; then whoever wins the most tiles wins.  In event of a tie; whoever won it first wins."),
                thisMod, thisMod.CommandContainer!);
            SubmitCommand = new PlainCommand(async items => await firstOptions.BeginningOptionSelectedAsync(OptionChosen),
                items =>
                {
                    if (OptionChosen == EnumFirstOption.None)
                        return false;
                    return IsEnabled;
                }, thisMod, thisMod.CommandContainer!);
        }
        private Task Option1_ItemSelectedAsync(int selectedIndex, string selectedText)
        {
            OptionChosen = (EnumFirstOption)selectedIndex;
            return Task.CompletedTask;
        }
        protected override void EnableChange()
        {
            DescriptionCommand.ReportCanExecuteChange(); //try this.
            SubmitCommand.ReportCanExecuteChange(); //maybe this too.
        }
        protected override void PrivateEnableAlways() { }
        protected override void VisibleChange()
        {
            Option1.Visible = Visible;
        }
    }
}