using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using TileRummyCP.Data;
using TileRummyCP.Logic;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace TileRummyCP.ViewModels
{
    [InstanceGame]
    public class TileRummyMainViewModel : BasicMultiplayerMainVM
    {
        private readonly TileRummyMainGameClass _mainGame; //if we don't need, delete.
        private readonly TileRummyVMData _model;
        public TileRummyMainViewModel(CommandContainer commandContainer,
            TileRummyMainGameClass mainGame,
            TileRummyVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _mainGame = mainGame;
            _model = viewModel;
			_model.TempSets.Init(this);
			_model.TempSets.ClearBoard(); 
            _model.Pool1.DrewTileAsync = DrewTileAsync;
            _model.MainSetsClickedAsync = MainSets1_SetClickedAsync;
            _model.TempSetsClickedAsync = TempSets_SetClickedAsync;
        }

        public override bool CanEndTurn()
        {
            bool didPlay = _model.MainSets1!.PlayedAtLeastOneFromHand();
            return didPlay == true || _model.Pool1!.HasDrawn() || _model.Pool1.HasTiles() == false;
        }

        public override async Task EndTurnAsync()
        {
            var thisCol = _mainGame!.GetSelectedList();
            if (thisCol.Count > 0)
            {
                await UIPlatform.ShowMessageAsync("Must either use the tiles selected or unselect the tiles before ending turn");
                return;
            }
            bool didPlay;
            bool valids;
            didPlay = _model.MainSets1!.PlayedAtLeastOneFromHand();
            valids = _mainGame.ValidPlay();
            if (_mainGame.BasicData!.MultiPlayer)
            {
                SendCustom thisEnd = new SendCustom();
                thisEnd.DidPlay = didPlay;
                thisEnd.ValidSets = valids;
                await _mainGame.Network!.SendAllAsync("endcustom", thisEnd); //i think
            }
            await _mainGame.EndTurnAsync(didPlay, valids);
        }


        private bool _isProcessing;

        private async Task DrewTileAsync(TileInfo thisTile)
        {
            if (_mainGame.BasicData.MultiPlayer)
            {
                SendDraw thisDraw = new SendDraw();
                thisDraw.Deck = thisTile.Deck;
                thisDraw.FromEnd = false;
                await _mainGame.Network!.SendAllAsync("drewtile", thisDraw);
            }
            await _mainGame!.DrawTileAsync(thisTile, false);
        }
        private async Task MainSets1_SetClickedAsync(int setNumber, int section, int deck)
        {
            if (setNumber == 0)
                throw new BasicBlankException("If the set is 0, rethinking is required");
            var thisSet = _model.MainSets1!.GetIndividualSet(setNumber);
            var thisCol = _mainGame!.GetSelectedList();
            if (thisCol.Count == 0)
            {
                if (deck == 0 || thisSet.HandList.Count < 2)
                {
                    if (_mainGame.BasicData!.MultiPlayer)
                        await _mainGame.Network!.SendAllAsync("removeentireset", setNumber);
                    await _mainGame.RemoveEntireSetAsync(setNumber);
                    return;
                }
                if (_mainGame.BasicData!.MultiPlayer)
                {
                    SendSet thisSend = new SendSet();
                    thisSend.Index = setNumber;
                    thisSend.Tile = deck;
                    await _mainGame.Network!.SendAllAsync("removeonefromset", thisSend);
                }
                await _mainGame.RemoveTileFromSetAsync(setNumber, deck);
                return;
            }
            if (thisCol.Count > 1)
            {
                await UIPlatform.ShowMessageAsync("Can only add one tile to the set at a time");
                return;
            }
            var thisTile = thisCol.First();
            var newPos = thisSet.PositionToPlay(thisTile, section);
            if (_mainGame.BasicData!.MultiPlayer)
            {
                SendSet finSend = new SendSet();
                finSend.Index = setNumber;
                finSend.Position = newPos;
                finSend.Tile = thisTile.Deck;
                await _mainGame.Network!.SendAllAsync("addtoset", finSend);
            }
            if (_model.TempSets!.HasObject(thisTile.Deck))
                _model.TempSets.RemoveObject(thisTile.Deck);
            else
                _mainGame.SingleInfo!.MainHandList.RemoveObjectByDeck(thisTile.Deck);
            await _mainGame.AddToSetAsync(setNumber, thisTile, newPos);
        }
        private Task TempSets_SetClickedAsync(int index)
        {
            if (_isProcessing == true)
                return Task.CompletedTask;
            _isProcessing = true;
            var tempList = _model.PlayerHand1!.ListSelectedObjects(true);
            _model.TempSets!.AddCards(index, tempList);
            _isProcessing = false;
            return Task.CompletedTask;
        }
        #region "Command Processes"
        public bool CanCreateFirstSets => !_mainGame.SingleInfo!.InitCompleted;
        [Command(EnumCommandCategory.Game)]
        public async Task CreateFirstSetsAsync()
        {
            bool rets = _mainGame!.HasInitialSets(out CustomBasicList<TempInfo> thisList);
            if (rets == false)
            {
                await UIPlatform.ShowMessageAsync("Sorry, you do not have the initial set needed.  The point values has to be at least 30");
                return;
            }
            CustomBasicList<string> newList = new CustomBasicList<string>();
            await thisList.ForEachAsync(async thisTemp =>
            {
                if (_mainGame.BasicData!.MultiPlayer == true)
                {
                    var tempList = thisTemp.CardList.GetDeckListFromObjectList();
                    SendCreateSet thisSend = new SendCreateSet();
                    thisSend.CardList = tempList;
                    thisSend.WhatSet = thisTemp.WhatSet;
                    var thisStr = await js.SerializeObjectAsync(thisSend);
                    newList.Add(thisStr);
                }
                _model.TempSets.ClearBoard(thisTemp.TempSet);
                await _mainGame.CreateNewSetAsync(thisTemp, true);
            });
            if (_mainGame.BasicData!.MultiPlayer)
                await _mainGame.Network!.SendSeveralSetsAsync(newList, "laiddowninitial");
            await _mainGame.InitialCompletedAsync();
        }
        public bool CanCreateNewSet => _mainGame.SingleInfo!.InitCompleted;
        [Command(EnumCommandCategory.Game)]
        public async Task CreateNewSetAsync()
        {
            var thisCol = _mainGame!.GetSelectedList();
            if (thisCol.Count < 3)
            {
                await UIPlatform.ShowMessageAsync("You must have at least 3 tiles in order to create a set");
                return;
            }
            _mainGame.HasValidSet(thisCol, out int firstNumber, out int secondNumber);
            TempInfo thisTemp = new TempInfo();
            thisTemp.CardList = thisCol;
            thisTemp.FirstNumber = firstNumber;
            thisTemp.SecondNumber = secondNumber;
            if (thisTemp.FirstNumber == -1)
                thisTemp.WhatSet = EnumWhatSets.Kinds;
            else
                thisTemp.WhatSet = EnumWhatSets.Runs;
            if (thisTemp.FirstNumber == -1)
                thisTemp.WhatSet = EnumWhatSets.Kinds;
            else
                thisTemp.WhatSet = EnumWhatSets.Runs;
            if (_mainGame.BasicData!.MultiPlayer)
            {
                SendCreateSet thisSend = new SendCreateSet()
                {
                    CardList = thisCol.GetDeckListFromObjectList(),
                    FirstNumber = thisTemp.FirstNumber,
                    SecondNumber = thisTemp.SecondNumber,
                    WhatSet = thisTemp.WhatSet
                };
                await _mainGame.Network!.SendAllAsync("createnewset", thisSend);
            }
            thisCol.ForEach(thisTile =>
            {
                if (_model.TempSets.HasObject(thisTile.Deck))
                    _model.TempSets.RemoveObject(thisTile.Deck);
                else
                    _mainGame.SingleInfo!.MainHandList.RemoveObjectByDeck(thisTile.Deck);
            });
            await _mainGame.CreateNewSetAsync(thisTemp, false);
        }
        public bool CanUndoMove => _mainGame.SaveRoot.DidExpand;
        [Command(EnumCommandCategory.Game)]
        public async Task UndoMoveAsync()
        {
            if (_mainGame.BasicData!.MultiPlayer)
                await _mainGame.Network!.SendAllAsync("undomove");
            await _mainGame.UndoMoveAsync();
        }
        #endregion
    }
}