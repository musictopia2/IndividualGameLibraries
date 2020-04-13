using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using FroggiesCP.Data;
using FroggiesCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace FroggiesCP.ViewModels
{
    [InstanceGame]
    public class FroggiesMainViewModel : Screen, IBasicEnableProcess, IBlankGameVM, IAggregatorContainer
    {
        private readonly IEventAggregator _aggregator;
        private readonly FroggiesMainGameClass _mainGame;

        public int StartingFrogs { get; set; }

        private int _numberOfFrogs;

        public int NumberOfFrogs
        {
            get { return _numberOfFrogs; }
            set
            {
                if (SetProperty(ref _numberOfFrogs, value))
                {
                    //can decide what to do when property changes
                }

            }
        }


        private int _movesLeft;

        public int MovesLeft
        {
            get { return _movesLeft; }
            set
            {
                if (SetProperty(ref _movesLeft, value))
                {
                    //can decide what to do when property changes

                }

            }
        }

        [Command(EnumCommandCategory.Plain)]
        public async Task MakeMoveAsync(LilyPadCP lily)
        {
            if (lily == null)
            {
                throw new BasicBlankException("Lily cannot be null. Rethink");
            }
            await _mainGame.ProcessLilyClickAsync(lily, this);
        }

        [Command(EnumCommandCategory.Plain)]
        public async Task RedoAsync()
        {
            await _mainGame.RedoAsync();
            MovesLeft = _mainGame.MovesLeft();
        }
        //will never show game over.

        public FroggiesMainViewModel(IEventAggregator aggregator,
            CommandContainer commandContainer,
            IGamePackageResolver resolver,
            LevelClass level
            )
        {
            _aggregator = aggregator;
            StartingFrogs = level.NumberOfFrogs;
            NumberOfFrogs = level.NumberOfFrogs;
            CommandContainer = commandContainer;
            _mainGame = resolver.ReplaceObject<FroggiesMainGameClass>(); //hopefully this works.  means you have to really rethink.
        }

        public CommandContainer CommandContainer { get; set; }

        IEventAggregator IAggregatorContainer.Aggregator => _aggregator;

        public bool CanEnableBasics()
        {
            return true; //because maybe you can't enable it.
        }
        protected override async Task ActivateAsync()
        {
            await base.ActivateAsync();
            await _mainGame.NewGameAsync(this);
        }
    }
}
