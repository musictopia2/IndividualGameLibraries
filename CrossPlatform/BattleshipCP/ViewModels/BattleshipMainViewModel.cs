using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.NetworkingClasses.Extensions;
using BasicGameFrameworkLibrary.TestUtilities;
using BattleshipCP.Data;
using BattleshipCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BattleshipCP.ViewModels
{
    [InstanceGame]
    public class BattleshipMainViewModel : BasicMultiplayerMainVM
    {
        private readonly BattleshipMainGameClass _mainGame; //if we don't need, delete.
        private readonly BattleshipVMData _viewModel;
        private readonly BasicData _basicData;
        private readonly BattleshipComputerAI? _ai;

        public BattleshipMainViewModel(CommandContainer commandContainer,
            BattleshipMainGameClass mainGame,
            BattleshipVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _mainGame = mainGame;
            _viewModel = viewModel;
            _basicData = basicData;
            if (_basicData.MultiPlayer == false)
            {
                _ai = resolver.Resolve<BattleshipComputerAI>();
            }
        }
        //anything else needed is here.
        private EnumShipList _shipSelected; // i think it needs to be here.
        [VM]
        public EnumShipList ShipSelected
        {
            get
            {
                return _shipSelected;
            }
            set
            {
                if (SetProperty(ref _shipSelected, value) == true)
                {
                }
            }
        }
        private bool _shipDirectionsVisible;
        [VM]
        public bool ShipDirectionsVisible
        {
            get { return _shipDirectionsVisible; }
            set
            {
                if (SetProperty(ref _shipDirectionsVisible, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private bool _shipsHorizontal = true;
        public bool ShipsHorizontal
        {
            get { return _shipsHorizontal; }
            set
            {
                if (SetProperty(ref _shipsHorizontal, value))
                {
                    //can decide what to do when property changes
                }
            }
        }


        private bool CanEnableShipOptions()
        {
            if (ShipDirectionsVisible == false)
                return false;
            return _mainGame!.StatusOfGame == EnumStatusList.PlacingShips;
        }
        public bool CanShipDirection() => CanEnableShipOptions();
        [Command(EnumCommandCategory.Game)]
        public void ShipDirection(bool horizontal)
        {
            ShipsHorizontal = horizontal;
        }
        public bool CanMakeMove(Vector space)
        {
            if (_mainGame.StatusOfGame == EnumStatusList.None)
            {
                return false;
            }
            if (_mainGame.StatusOfGame == EnumStatusList.PlacingShips)
            {
                if (_mainGame.Ships.HasSelectedShip() == false)
                {
                    return false;
                }
                return _mainGame.Ships.CanPlaceShip(space, ShipsHorizontal);
            }
            return _mainGame.GameBoard1.CanChooseSpace(space);
        }
        [Command(EnumCommandCategory.Game)]
        public async Task MakeMoveAsync(Vector space)
        {
            if (_mainGame!.StatusOfGame == EnumStatusList.PlacingShips)
            {
                _mainGame.Ships!.PlaceShip(space, ShipsHorizontal);
                if (_mainGame.Ships.IsFinished() == false)
                    return;
                await _mainGame.HumanFinishedPlacingShipsAsync();
                return;
            }
            _mainGame.CurrentSpace = space;
            if (_basicData!.MultiPlayer == true)
            {
                await _mainGame.Network!.SendMoveAsync(space);
                CommandContainer!.ManuelFinish = true; //i think this is what i have to do now.
                _mainGame.Check!.IsEnabled = true; //because has to see what it is.
                return;
            }
            bool Hits = _ai!.HasHit(space);
            if (Hits == true)
                await _mainGame.MakeMoveAsync(space, EnumWhatHit.Hit);
            else
                await _mainGame.MakeMoveAsync(space, EnumWhatHit.Miss);
        }
        public bool CanChooseShip()
        {
            return CanEnableShipOptions();
        }
        [Command(EnumCommandCategory.Game)]
        public void ChooseShip(EnumShipList ship)
        {
            _viewModel.ShipSelected = ship; //looks like has to be put to that model which will notify this as well.
            _mainGame.GameBoard1.HumanWaiting();
        }
    }
}