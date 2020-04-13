using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.ChooserClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ThreeLetterFunCP.BeginningClasses;
using ThreeLetterFunCP.EventModels;
using static BasicGameFrameworkLibrary.ChooserClasses.ListViewPicker;
namespace ThreeLetterFunCP.ViewModels
{
    [InstanceGame]
    public class CardsPlayerViewModel : Screen, IBlankGameVM, IHandleAsync<CardsChosenEventModel>
    {
        public CardsPlayerViewModel(CommandContainer commandContainer,
            IGamePackageResolver resolver, BasicData basicData,
            ICardsChosenProcesses processes,
            IEventAggregator aggregator
            )
        {
            CommandContainer = commandContainer;
            _basicData = basicData;
            _processes = processes;
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            CardList1 = new ListViewPicker(CommandContainer, resolver);
            CardList1.IndexMethod = EnumIndexMethod.ZeroBased;
            CardList1.ItemSelectedAsync += CardList1_ItemSelectedAsync;
            CustomBasicList<string> thisList = new CustomBasicList<string>() { "4 Cards", "6 Cards", "8 Cards", "10 Cards" };
            CardList1.LoadTextList(thisList);
        }
        public ListViewPicker CardList1;
        public CommandContainer CommandContainer { get; set; }
        public bool CanSubmit()
        {
            if (_basicData.MultiPlayer == false)
            {
                return false; //because single player game if it shows it is only for testing.
            }
            if (_basicData.Client == true)
            {
                return false;
            }
            return HowManyCards > 0;
        }
        [Command(EnumCommandCategory.Plain)] //had to be plain.  otherwise, had to implement another interface.
        public Task SubmitAsync()
        {
            return _processes.CardsChosenAsync(HowManyCards);
        }

        private int _howManyCards;
        private readonly BasicData _basicData;
        private readonly ICardsChosenProcesses _processes;
        private readonly IEventAggregator _aggregator;

        public int HowManyCards
        {
            get { return _howManyCards; }
            set
            {
                if (SetProperty(ref _howManyCards, value)) { }
            }
        }
        //this may require delegates to stop the overflows.
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

        async Task IHandleAsync<CardsChosenEventModel>.HandleAsync(CardsChosenEventModel message)
        {
            HowManyCards = message.HowManyCards;
            SelectGivenValue();
            await SubmitAsync();
        }
        protected override Task TryCloseAsync()
        {
            _aggregator.Unsubscribe(this);
            return base.TryCloseAsync();
        }
    }
}
