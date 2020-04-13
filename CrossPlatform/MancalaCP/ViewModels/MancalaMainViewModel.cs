using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.TestUtilities;
using MancalaCP.Logic;
namespace MancalaCP.ViewModels
{
    [InstanceGame]
    public class MancalaMainViewModel : BasicMultiplayerMainVM
    {

        public MancalaMainViewModel(CommandContainer commandContainer,
            MancalaMainGameClass mainGame,
            IViewModelData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {

        }
        //anything else needed is here.
        private string _instructions = "";
        [VM]
        public string Instructions
        {
            get { return _instructions; }
            set
            {
                if (SetProperty(ref _instructions, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private int _piecesAtStart;
        [VM]
        public int PiecesAtStart
        {
            get { return _piecesAtStart; }
            set
            {
                if (SetProperty(ref _piecesAtStart, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _piecesLeft;
        [VM]
        public int PiecesLeft
        {
            get { return _piecesLeft; }
            set
            {
                if (SetProperty(ref _piecesLeft, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
    }
}