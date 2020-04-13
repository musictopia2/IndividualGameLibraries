using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.ChooserClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using MinesweeperCP.Data;
using MinesweeperCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace MinesweeperCP.ViewModels
{
    [InstanceGame]
    //looks like i need to implement the imainscreen if its a sub view model that the main uses.
    public class MinesweeperOpeningViewModel : Screen, ILevelVM, IMainScreen
    {
        //looks like this is what needs to be at the beginning now.
        //not sure if i need to implement other interfaces (?)
        //for sure need level.

        private EnumLevel _levelChosen = EnumLevel.Easy;

        public EnumLevel LevelChosen
        {
            get { return _levelChosen; }
            set
            {
                if (SetProperty(ref _levelChosen, value))
                {
                    //can decide what to do when property changes
                    this.PopulateMinesNeeded();
                    _global.Level = value;
                }

            }
        }

        private int _howManyMinesNeeded;

        public int HowManyMinesNeeded
        {
            get { return _howManyMinesNeeded; }
            set
            {
                if (SetProperty(ref _howManyMinesNeeded, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        //needs this ui part now.
        public ListViewPicker LevelPicker;
        private readonly LevelClass _global;

        public MinesweeperOpeningViewModel(CommandContainer container, IGamePackageResolver resolver, LevelClass global)
        {
            LevelPicker = new ListViewPicker(container, resolver); //hopefully this simple.
            LevelPicker.SelectionMode = ListViewPicker.EnumSelectionMode.SingleItem;
            LevelPicker.IndexMethod = ListViewPicker.EnumIndexMethod.OneBased;
            _global = global;
            //no need for send processes.
            LevelChosen = _global.Level;
            this.PopulateMinesNeeded();
            LevelPicker.ItemSelectedAsync += LevelPicker_ItemSelectedAsync;
            LevelPicker.LoadTextList(new CustomBasicList<string>() { "Easy", "Medium", "Hard" });
            //LevelPicker.SelectSpecificItem(1);
            switch (LevelChosen)
            {
                case EnumLevel.Easy:
                    LevelPicker.SelectSpecificItem(1);
                    break;
                case EnumLevel.Medium:
                    LevelPicker.SelectSpecificItem(2);
                    break;
                case EnumLevel.Hard:
                    LevelPicker.SelectSpecificItem(3);
                    break;
                default:
                    throw new BasicBlankException("Not Supported");
            }
            LevelPicker.IsEnabled = true; //take a risk this time.  not sure why it worked before.
        }

        protected override Task TryCloseAsync()
        {
            //could be iffy (?)
            //we may have to somehow remove the reflection commands (not sure).

            return base.TryCloseAsync();
        }

        private Task LevelPicker_ItemSelectedAsync(int selectedIndex, string selectedText)
        {
            LevelChosen = (EnumLevel)selectedIndex;
            return Task.CompletedTask;
        }

    }
}
