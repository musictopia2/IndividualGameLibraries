using BasicGameFramework.Attributes;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommandClasses;
using BasicGameFramework.MiscViewModels;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using static BasicGameFramework.ChooserClasses.ListViewPicker;
namespace ThreeLetterFunCP
{
    [SingletonGame] //try this.
    public class AdvancedOptionsViewModel : SimpleControlViewModel
    {

        private bool _ShortGame;

        public bool ShortGame
        {
            get { return _ShortGame; }
            set
            {
                if (SetProperty(ref _ShortGame, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private bool _EasyWords;

        public bool EasyWords
        {
            get { return _EasyWords; }
            set
            {
                if (SetProperty(ref _EasyWords, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        //needs to be public because the other players need to know.
        public ListViewPicker Game1;
        public ListViewPicker Easy1;

        public ControlCommand SubmitCommand { get; set; }

        public void SelectSpecificOptions()
        {
            if (EasyWords == true)
                Easy1.SelectSpecificItem(1);
            else
                Easy1.SelectSpecificItem(2);
            if (ShortGame == true)
                Game1.SelectSpecificItem(1);
            else
                Game1.SelectSpecificItem(2);
        }

        public AdvancedOptionsViewModel(IBasicGameVM thisMod, IFirstOptions firstOptions) : base(thisMod)
        {
            SubmitCommand = new ControlCommand(this, async Items => await firstOptions.ChoseAdvancedOptions(EasyWords, ShortGame),
                thisMod, thisMod.CommandContainer!);
            Game1 = new ListViewPicker(thisMod);
            Easy1 = new ListViewPicker(thisMod);
            Game1.IndexMethod = EnumIndexMethod.OneBased;
            Easy1.IndexMethod = EnumIndexMethod.OneBased;
            var thisList = new CustomBasicList<string>() { "Easy Words", "Any Words" };
            Easy1.LoadTextList(thisList);
            thisList = new CustomBasicList<string>() { "Short Game", "Regular Game" };
            Game1.LoadTextList(thisList);
            SelectSpecificOptions();
            Game1.ItemSelectedAsync += Game1_ItemSelectedAsync;
            Easy1.ItemSelectedAsync += Easy1_ItemSelectedAsync;
        }

        private Task Easy1_ItemSelectedAsync(int selectedIndex, string selectedText)
        {
            if (selectedIndex == 1)
                EasyWords = true;
            else
                EasyWords = false;
            return Task.CompletedTask;
        }

        private Task Game1_ItemSelectedAsync(int selectedIndex, string selectedText)
        {
            if (selectedIndex == 1)
                ShortGame = true;
            else
                ShortGame = false;
            return Task.CompletedTask;
        }

        protected override void EnableChange()
        {
            SubmitCommand.ReportCanExecuteChange();
        }

        protected override void PrivateEnableAlways() { }

        protected override void VisibleChange()
        {
            Game1.Visible = Visible;
            Easy1.Visible = Visible;
        }
    }
}