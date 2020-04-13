using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.ChooserClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ThreeLetterFunCP.BeginningClasses;
using ThreeLetterFunCP.EventModels;
using static BasicGameFrameworkLibrary.ChooserClasses.ListViewPicker;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.

namespace ThreeLetterFunCP.ViewModels
{
    [InstanceGame]
    public class AdvancedOptionsViewModel : Screen, IBlankGameVM, IHandleAsync<AdvancedSettingsEventModel>
    {
        public AdvancedOptionsViewModel(CommandContainer commandContainer,
            IGamePackageResolver resolver, 
            BasicData basicData, 
            IAdvancedProcesses processes,
            IEventAggregator aggregator
            )
        {
            CommandContainer = commandContainer;
            _basicData = basicData;
            _processes = processes;
            _aggregator = aggregator;
            aggregator.Subscribe(this);
            Game1 = new ListViewPicker(commandContainer, resolver);
            Easy1 = new ListViewPicker(commandContainer, resolver);
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
        public CommandContainer CommandContainer { get; set; }
        public ListViewPicker Game1;
        public ListViewPicker Easy1;

        private bool _shortGame;

        public bool ShortGame
        {
            get { return _shortGame; }
            set
            {
                if (SetProperty(ref _shortGame, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private bool _easyWords;
        private readonly BasicData _basicData;
        private readonly IAdvancedProcesses _processes;
        private readonly IEventAggregator _aggregator;

        public bool EasyWords
        {
            get { return _easyWords; }
            set
            {
                if (SetProperty(ref _easyWords, value))
                {
                    //can decide what to do when property changes
                }

            }
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
                return true; //it chooses something at default.
            }
        }
        [Command(EnumCommandCategory.Plain)]
        public Task SubmitAsync()
        {
            return _processes.ChoseAdvancedOptions(EasyWords, ShortGame);
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
        //this may require delegates.  because otherwise, overflow will most likely result.
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

        async Task IHandleAsync<AdvancedSettingsEventModel>.HandleAsync(AdvancedSettingsEventModel message)
        {
            AdvancedSettingModel model = await js.DeserializeObjectAsync<AdvancedSettingModel>(message.Message);
            EasyWords = model.IsEasy;
            ShortGame = model.ShortGame;
            SelectSpecificOptions();
            await SubmitAsync();
        }
        protected override Task TryCloseAsync()
        {
            _aggregator.Unsubscribe(this);
            return base.TryCloseAsync();
        }
    }
}