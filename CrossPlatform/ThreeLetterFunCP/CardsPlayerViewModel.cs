using BasicGameFramework.Attributes;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommandClasses;
using BasicGameFramework.MiscViewModels;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using static BasicGameFramework.ChooserClasses.ListViewPicker;
namespace ThreeLetterFunCP
{
    [SingletonGame] //try this.
    public class CardsPlayerViewModel : SimpleControlViewModel
    {
        private int _HowManyCards;
        public int HowManyCards
        {
            get { return _HowManyCards; }
            set
            {
                if (SetProperty(ref _HowManyCards, value)) { }
            }
        }
        public PlainCommand SubmitCommand { get; set; }
        public ListViewPicker CardList1; //not sure if it needs property.
        public CardsPlayerViewModel(IBasicGameVM thisMod, IFirstOptions firstOptions) : base(thisMod)
        {
            CardList1 = new ListViewPicker(thisMod);
            CardList1.IndexMethod = EnumIndexMethod.ZeroBased;
            CardList1.ItemSelectedAsync += CardList1_ItemSelectedAsync;
            SubmitCommand = new PlainCommand(async items => await firstOptions.CardsChosenAsync(HowManyCards),
                Items =>
                {
                    if (IsEnabled == false)
                        return false;
                    if (HowManyCards == 0)
                        return false;
                    return true;
                }, thisMod, thisMod.CommandContainer!);

            CustomBasicList<string> thisList = new CustomBasicList<string>() { "4 Cards", "6 Cards", "8 Cards", "10 Cards" };
            CardList1.LoadTextList(thisList);
        }
        public void SelectGivenValue()
        {
            int index;
            if (HowManyCards == 4)
                index = 0;
            else if (HowManyCards == 6)
                index = 1;
            else if (HowManyCards == 8)
                index = 2;
            else if (HowManyCards == 10)
                index = 3;
            else
                throw new BasicBlankException("Nothing found.");
            CardList1.SelectSpecificItem(index);
        }
        private Task CardList1_ItemSelectedAsync(int selectedIndex, string selectedText)
        {
            var temps = selectedText.Replace(" Cards", "");
            HowManyCards = int.Parse(temps); // otherwise, error.
            return Task.CompletedTask;
        }
        protected override void EnableChange() //hopefully nothing is needed here.
        {
            SubmitCommand.ReportCanExecuteChange(); //did have to manually be done here too.
        }
        protected override void PrivateEnableAlways() { }

        protected override void VisibleChange()
        {
            CardList1.Visible = Visible;
        }
    }
}