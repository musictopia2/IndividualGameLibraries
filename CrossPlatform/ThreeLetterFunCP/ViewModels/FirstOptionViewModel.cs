using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.ChooserClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ThreeLetterFunCP.BeginningClasses;
using ThreeLetterFunCP.Data;
using ThreeLetterFunCP.EventModels;
using static BasicGameFrameworkLibrary.ChooserClasses.ListViewPicker;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace ThreeLetterFunCP.ViewModels
{
    [InstanceGame]
    public class FirstOptionViewModel : Screen, IBlankGameVM, IHandleAsync<FirstOptionEventModel>
    {

        //originally was a control view model.  try to not do that this time.
        //since its separate now.

        public ListViewPicker Option1;
        //maybe the view models will process the messages as well.  that could be a good idea.


        public FirstOptionViewModel(CommandContainer commandContainer,
            IGamePackageResolver resolver,
            BasicData basicData,
            IFirstOptionProcesses first,
            IEventAggregator aggregator
            )
        {
            CommandContainer = commandContainer;
            _basicData = basicData;
            _first = first;
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            Option1 = new ListViewPicker(commandContainer, resolver);
            Option1.ItemSelectedAsync += Option1_ItemSelectedAsync;
            CustomBasicList<string> list = new CustomBasicList<string>() { "Beginner", "Advanced" };
            Option1.IndexMethod = EnumIndexMethod.OneBased;
            Option1.LoadTextList(list);
        }
        protected override Task ActivateAsync()
        {
            Option1.ReportCanExecuteChange();
            return base.ActivateAsync();
        }
        protected override Task TryCloseAsync()
        {
            _aggregator.Unsubscribe(this);
            return base.TryCloseAsync();
        }
        private readonly BasicData _basicData;
        private readonly IFirstOptionProcesses _first;
        private readonly IEventAggregator _aggregator;
        private EnumFirstOption _optionChosen = EnumFirstOption.None;

        public EnumFirstOption OptionChosen
        {
            get
            {
                return _optionChosen;
            }

            set
            {
                if (SetProperty(ref _optionChosen, value) == true) { }
            }
        }

        [Command(EnumCommandCategory.Old)]
        public async Task DescriptionAsync()
        {
            await UIPlatform.ShowMessageAsync("The beginner option only allows easy words to be formed.  Plus the beginner also has a choice start out with 4, 6, 8, or 10 cards.  Whoeever gets rid of their cards first by spelling them from the tiles wins." + Constants.vbCrLf + "The advanced option has a choice between allowing easy words or any common 3 letter words.  Also; for the advanced option; all the cards are layed out.  There is a short option in which the first player who spells 5 words correctly wins.  For the regular; once all the cards or tiles are gone; then whoever wins the most tiles wins.  In event of a tie; whoever won it first wins.");
        }
        public bool CanSubmit
        {
            get
            {
                if (_basicData.MultiPlayer == false)
                {
                    return false; //because single player game if it shows it is only for testing.
                }
                if (_basicData.Client == true)
                {
                    return false;
                }
                return OptionChosen != EnumFirstOption.None;
            }
        }
        [Command(EnumCommandCategory.Plain)]
        public Task SubmitAsync()
        {
            return _first.BeginningOptionSelectedAsync(OptionChosen);
        }

        private Task Option1_ItemSelectedAsync(int selectedIndex, string selectedText)
        {
            OptionChosen = (EnumFirstOption)selectedIndex;
            //NotifyOfCanExecuteChange(nameof(CanSubmit)); //try here first before fixing all.
            return Task.CompletedTask;
        }

        async Task IHandleAsync<FirstOptionEventModel>.HandleAsync(FirstOptionEventModel message)
        {
            OptionChosen = await js.DeserializeObjectAsync<EnumFirstOption>(message.Message);
            Option1.SelectSpecificItem((int)OptionChosen);
            await SubmitAsync(); //can just act like you are submitting.
        }

        public CommandContainer CommandContainer { get; set; }
    }
}
