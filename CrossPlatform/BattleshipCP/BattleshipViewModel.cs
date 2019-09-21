using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.MiscProcesses;
using BasicGameFramework.NetworkingClasses.Extensions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
namespace BattleshipCP
{
    public class BattleshipViewModel : BasicMultiplayerVM<BattleshipPlayerItem, BattleshipMainGameClass>
    {
        public BattleshipViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        private bool _ShipDirectionsVisible;
        public bool ShipDirectionsVisible
        {
            get { return _ShipDirectionsVisible; }
            set
            {
                if (SetProperty(ref _ShipDirectionsVisible, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private bool _ShipsHorizontal = true;
        public bool ShipsHorizontal
        {
            get { return _ShipsHorizontal; }
            set
            {
                if (SetProperty(ref _ShipsHorizontal, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private EnumShipList _ShipSelected; // i think it needs to be here.
        public EnumShipList ShipSelected
        {
            get
            {
                return _ShipSelected;
            }
            set
            {
                if (SetProperty(ref _ShipSelected, value) == true)
                {
                }
            }
        }
        public BasicGameCommand<bool>? ShipDirectionCommand { get; set; }
        public BasicGameCommand<Vector>? GameBoardCommand { get; set; } //try vector.
        public BasicGameCommand<EnumShipList>? ChooseShipCommand { get; set; }
        private bool CanEnableShipOptions()
        {
            if (ShipDirectionsVisible == false)
                return false;
            return MainGame!.StatusOfGame == EnumStatusList.PlacingShips;
        }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            ShipDirectionCommand = new BasicGameCommand<bool>(this,
                items => ShipsHorizontal = items,
                items => CanEnableShipOptions(), this, CommandContainer!);
            ChooseShipCommand = new BasicGameCommand<EnumShipList>(this,
                thisShip =>
                {
                    ShipSelected = thisShip;
                    MainGame!.GameBoard1!.HumanWaiting();
                }, items => CanEnableShipOptions(), this, CommandContainer!);
            GameBoardCommand = new BasicGameCommand<Vector>(this, async space =>
            {
                if (MainGame!.StatusOfGame == EnumStatusList.PlacingShips)
                {
                    MainGame.Ships!.PlaceShip(space, ShipsHorizontal);
                    if (MainGame.Ships.IsFinished() == false)
                        return;
                    await MainGame.HumanFinishedPlacingShipsAsync();
                    return;
                }
                MainGame.CurrentSpace = space;
                if (ThisData!.MultiPlayer == true)
                {
                    await ThisNet!.SendMoveAsync(space);
                    CommandContainer!.ManuelFinish = true; //i think this is what i have to do now.
                    MainGame.ThisCheck!.IsEnabled = true; //because has to see what it is.
                    return;
                }
                bool Hits = MainGame.AI!.HasHit(space);
                if (Hits == true)
                    await MainGame.MakeMoveAsync(space, EnumWhatHit.Hit);
                else
                    await MainGame.MakeMoveAsync(space, EnumWhatHit.Miss);
            }, space =>
            {
                if (MainGame!.StatusOfGame == EnumStatusList.None)
                    return false;
                if (MainGame.StatusOfGame == EnumStatusList.PlacingShips)
                {
                    if (MainGame.Ships!.HasSelectedShip() == false)
                    {
                        //ThisMessage.ShowMessageBox("You must choose a ship to place here");
                        return false;
                    }
                    return MainGame.Ships.CanPlaceShip(space, ShipsHorizontal);
                }
                return MainGame.GameBoard1!.CanChooseSpace(space);
            }, this, CommandContainer!);
        }
    }
}