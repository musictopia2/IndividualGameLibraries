using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.ChooserClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.Conductors;
using MastermindCP.Data;
using MastermindCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace MastermindCP.ViewModels
{
    [InstanceGame]
    public class MastermindMainViewModel : ConductorSingle<object>, IBasicEnableProcess, IBlankGameVM, IAggregatorContainer
    {
        private readonly IEventAggregator _aggregator;
        private readonly MastermindMainGameClass _mainGame;
        private readonly GlobalClass _global;

        public int LevelChosen { get; set; }

        public SimpleEnumPickerVM<EnumColorPossibilities, CirclePieceCP<EnumColorPossibilities>> Color1;

        public bool CanGiveUp
        {
            get
            {
                if (_global.Solution == null)
                {
                    return false;
                }
                return _global.Solution.Count > 0;
            }
        }
        [Command(EnumCommandCategory.Plain)]
        public async Task GiveUpAsync()
        {
            await _mainGame.GiveUpAsync();
        }
        public bool CanAccept => MainBoard.DidFillInGuess();
        [Command(EnumCommandCategory.Plain)]
        public async Task AcceptAsync()
        {
            await MainBoard.SubmitGuessAsync();
        }

        

        public MastermindMainViewModel(IEventAggregator aggregator, CommandContainer commandContainer,
            IGamePackageResolver resolver,
            LevelClass level
            )
        {
            _aggregator = aggregator;
            //looks like can't just ask for the mainboard.  because if you do, then when i change the global class, they have the old one (wrong)
            LevelChosen = level.LevelChosen;
            CommandContainer = commandContainer;
            _global = resolver.ReplaceObject<GlobalClass>(); //looks like global has to be replaced before game.  otherwise, reference is broken.
            CustomColorClass colorClass = new CustomColorClass(_global);
            //hopefully won't cause another problem (?)
            _mainGame = resolver.ReplaceObject<MastermindMainGameClass>(); //hopefully this works.  means you have to really rethink.



            MainBoard = resolver.Resolve<GameBoardViewModel>(); //by this time, it will have the proper global class.
            //ActiveViewModel = MainBoard;
            Color1 = new SimpleEnumPickerVM<EnumColorPossibilities, CirclePieceCP<EnumColorPossibilities>>(commandContainer, colorClass);
            Color1.AutoSelectCategory = EnumAutoSelectCategory.AutoEvent;
            Color1.IsEnabled = true;
            Color1.ItemClickedAsync += Color1_ItemClickedAsync;
        }

        private Task Color1_ItemClickedAsync(EnumColorPossibilities piece)
        {
            MainBoard.SelectedColorForCurrentGuess(piece);
            return Task.CompletedTask;
        }

        public CommandContainer CommandContainer { get; set; }

        IEventAggregator IAggregatorContainer.Aggregator => _aggregator;

        private GameBoardViewModel MainBoard { get; set; }

        public bool CanEnableBasics()
        {
            return true; //because maybe you can't enable it.
        }
        protected override async Task ActivateAsync()
        {
            await base.ActivateAsync();
            await LoadScreenAsync(MainBoard); //i think i forgot this too.
            await _mainGame.NewGameAsync(MainBoard);
            Color1.LoadEntireList();
            await _aggregator.SendLoadAsync();
        }
    }
}
