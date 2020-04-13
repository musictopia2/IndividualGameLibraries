using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using GalaxyCardGameCP.Cards;
using GalaxyCardGameCP.Data;
using GalaxyCardGameCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace GalaxyCardGameCP.ViewModels
{
    [InstanceGame]
    public class GalaxyCardGameMainViewModel : TrickCardGamesVM<GalaxyCardGameCardInformation, EnumSuitList>
    {
        private readonly GalaxyCardGameMainGameClass _mainGame; //if we don't need, delete.
        private readonly GalaxyCardGameVMData _model;

        public GalaxyCardGameMainViewModel(CommandContainer commandContainer,
            GalaxyCardGameMainGameClass mainGame,
            GalaxyCardGameVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _mainGame = mainGame;
            _model = viewModel;
            _model.Deck1.NeverAutoDisable = true;
            _mainGame.PlayerList.ForEach(player =>
            {
                if (player.PlanetHand == null)
                {
                    throw new BasicBlankException("No planet hand.  Rethink");
                }
                if (player.Moons == null)
                {
                    throw new BasicBlankException("No moons.  Rethink");
                }
                player.PlanetHand.SendEnableProcesses(this, (() =>
                {
                    if (player.PlayerCategory != EnumPlayerCategory.Self)
                        return false;
                    if (mainGame.SaveRoot!.GameStatus != EnumGameStatus.PlaceSets)
                        return false;
                    return mainGame.HasAutomaticPlanet() || player.PlanetHand.HandList.Count == 0;
                }));
                player.Moons.SendEnableProcesses(this, (() =>
                {
                    if (player.PlayerCategory != EnumPlayerCategory.Self)
                        return false;
                    return player.CanEnableMoon();
                }));

                commandContainer.ExecutingChanged += CommandContainer_ExecutingChanged;

            });
            _model.TrickArea1.SendEnableProcesses(this, (() => _mainGame.CanEnableTrickAreas)); //if any other trick taking games has speciali processes, has to do here.
        }
        protected override Task TryCloseAsync()
        {
            CommandContainer.ExecutingChanged += CommandContainer_ExecutingChanged;
            return base.TryCloseAsync();
        }
        private void CommandContainer_ExecutingChanged()
        {
            if (_mainGame!.SaveRoot!.GameStatus == EnumGameStatus.PlaceSets)
                _model.PlayerHand1!.AutoSelect = HandObservable<GalaxyCardGameCardInformation>.EnumAutoType.SelectAsMany;
            else
                _model.PlayerHand1!.AutoSelect = HandObservable<GalaxyCardGameCardInformation>.EnumAutoType.SelectOneOnly; //try this way.
        }
        public override bool CanEndTurn()
        {
            return _mainGame!.CanEndTurn();
        }
        protected override bool CanEnableDeck()
        {
            return false; //otherwise, can't compile.
        }

        protected override bool CanEnablePile1()
        {
            return false; //otherwise, can't compile.
        }

        protected override async Task ProcessDiscardClickedAsync()
        {
            //if we have anything, will be here.
            await Task.CompletedTask;
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
        public bool CanMoon => _mainGame!.SingleInfo!.CanEnableMoon();
        [Command(EnumCommandCategory.Game)]
        public async Task MoonAsync()
        {
            var thisList = _model.PlayerHand1.ListSelectedObjects();
            if (_mainGame!.HasValidMoon(thisList) == false)
            {
                await UIPlatform.ShowMessageAsync("Invalid Moon");
                return;
            }
            if (_mainGame.BasicData!.MultiPlayer)
            {
                var tempList = thisList.GetDeckListFromObjectList();
                await _mainGame.Network!.SendAllAsync("newmoon", tempList);
            }
            await _mainGame.PlayNewMoonAsync(thisList);
        }

    }
}