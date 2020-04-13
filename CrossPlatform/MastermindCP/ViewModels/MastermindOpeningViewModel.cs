using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.ChooserClasses;
using MastermindCP.Data;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
//i think this is the most common things i like to do
namespace MastermindCP.ViewModels
{
    [InstanceGame]
    public class MastermindOpeningViewModel : Screen, IBlankGameVM
    {
        public int LevelChosen { get; set; } = 3;
        public CommandContainer CommandContainer { get; set; }

        public ListViewPicker LevelPicker;
        private readonly LevelClass _global;
        [Command(EnumCommandCategory.Old)]
        public async Task LevelInformationAsync()
        {
            await UIPlatform.ShowMessageAsync("Level 1 has 4 possible colors that are only used once" + Constants.vbCrLf + "Level 2 has 4 possible colors that can be used more than once" + Constants.vbCrLf + "Level 3 has 6 possible colors that can only be used once" + Constants.vbCrLf + "Level 4 has 6 possible colors that can be used more than once" + Constants.vbCrLf + "Level 5 has 8 possible colors that can be used once" + Constants.vbCrLf + "Level 6 has 8 possible colors that can be used more than once");
        }

        public MastermindOpeningViewModel(CommandContainer container, IGamePackageResolver resolver, LevelClass global)
        {
            LevelPicker = new ListViewPicker(container, resolver); //hopefully this simple.
            LevelPicker.SelectionMode = ListViewPicker.EnumSelectionMode.SingleItem;
            LevelPicker.IndexMethod = ListViewPicker.EnumIndexMethod.OneBased;
            _global = global;
            CommandContainer = container;
            LevelChosen = _global.LevelChosen;
            LevelPicker.LoadTextList(new CustomBasicList<string>() { "Level 1", "Level 2", "Level 3", "Level 4", "Level 5", "Level 6" });
            LevelPicker.ItemSelectedAsync += LevelPicker_ItemSelectedAsync;
            LevelPicker.SelectSpecificItem(LevelChosen);
            LevelPicker.IsEnabled = true;
        }

        private Task LevelPicker_ItemSelectedAsync(int selectedIndex, string selectedText)
        {
            LevelChosen = selectedIndex;
            _global.LevelChosen = selectedIndex;
            return Task.CompletedTask;
        }


    }
}
