using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.SpecializedGameTypes.RummyClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace TileRummyCP
{
    public class TileRummyViewModel : BasicMultiplayerVM<TileRummyPlayerItem, TileRummyMainGameClass>
    {
        public PoolCP? Pool1;
        public TempSetsViewModel<EnumColorType, EnumColorType, TileInfo>? TempSets;
        public MainSets? MainSets1;
        public TileHand? PlayerHand1;
        protected override async Task EndTurnProcess()
        {
            //has to do more with the main game class first.
            var thisCol = MainGame!.GetSelectedList();
            if (thisCol.Count > 0)
            {
                await ShowGameMessageAsync("Must either use the tiles selected or unselect the tiles before ending turn");
                return;
            }
            bool didPlay;
            bool valids;
            didPlay = MainSets1!.PlayedAtLeastOneFromHand();
            valids = MainGame.ValidPlay();
            if (ThisData!.MultiPlayer)
            {
                SendCustom thisEnd = new SendCustom();
                thisEnd.DidPlay = didPlay;
                thisEnd.ValidSets = valids;
                await ThisNet!.SendAllAsync("endcustom", thisEnd); //i think
            }
            await MainGame.EndTurnAsync(didPlay, valids);
        }
        public override bool CanEndTurn()
        {
            bool didPlay = MainSets1!.PlayedAtLeastOneFromHand();
            return didPlay == true || Pool1!.HasDrawn() || Pool1.HasTiles() == false;
        }
        public async Task DrewTileAsync(TileInfo thisTile)
        {
            if (ThisData!.MultiPlayer)
            {
                SendDraw thisDraw = new SendDraw();
                thisDraw.Deck = thisTile.Deck;
                thisDraw.FromEnd = false;
                await ThisNet!.SendAllAsync("drewtile", thisDraw);
            }
            await MainGame!.DrawTileAsync(thisTile, false);
        }
        public TileRummyViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        public BasicGameCommand? CreateFirstSetsCommand { get; set; }
        public BasicGameCommand? CreateNewSetCommand { get; set; }
        public BasicGameCommand? UndoMoveCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            TempSets = new TempSetsViewModel<EnumColorType, EnumColorType, TileInfo>();
            TempSets.HowManySets = 4;
            TempSets.Init(this);
            TempSets.SetClickedAsync += TempSets_SetClickedAsync;
            MainSets1 = new MainSets(this);
            Pool1 = new PoolCP(this);
            MainSets1.SendEnableProcesses(this, () => MainGame!.SingleInfo!.InitCompleted);
            PlayerHand1 = new TileHand(this);
            PlayerHand1.Visible = true;
            MainSets1.SetClickedAsync += MainSets1_SetClickedAsync;
            CreateFirstSetsCommand = new BasicGameCommand(this, async items =>
            {
                bool rets = MainGame!.HasInitialSets(out CustomBasicList<TempInfo> thisList);
                if (rets == false)
                {
                    await ShowGameMessageAsync("Sorry, you do not have the initial set needed.  The point values has to be at least 30");
                    return;
                }
                CustomBasicList<string> newList = new CustomBasicList<string>();
                await thisList.ForEachAsync(async thisTemp =>
                {
                    if (ThisData!.MultiPlayer == true)
                    {
                        var tempList = thisTemp.CardList.GetDeckListFromObjectList();
                        SendCreateSet thisSend = new SendCreateSet();
                        thisSend.CardList = tempList;
                        thisSend.WhatSet = thisTemp.WhatSet;
                        var thisStr = await js.SerializeObjectAsync(thisSend);
                        newList.Add(thisStr);
                    }
                    TempSets.ClearBoard(thisTemp.TempSet);
                    await MainGame.CreateNewSetAsync(thisTemp, true);
                });
                if (ThisData!.MultiPlayer)
                    await ThisNet!.SendSeveralSetsAsync(newList, "laiddowninitial");
                await MainGame.InitialCompletedAsync();

            }, items => !MainGame!.SingleInfo!.InitCompleted, this, CommandContainer!);
            CreateNewSetCommand = new BasicGameCommand(this, async items =>
            {
                var thisCol = MainGame!.GetSelectedList();
                if (thisCol.Count < 3)
                {
                    await ShowGameMessageAsync("You must have at least 3 tiles in order to create a set");
                    return;
                }
                MainGame.HasValidSet(thisCol, out int firstNumber, out int secondNumber);
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
                if (ThisData!.MultiPlayer)
                {
                    SendCreateSet thisSend = new SendCreateSet()
                    {
                        CardList = thisCol.GetDeckListFromObjectList(),
                        FirstNumber = thisTemp.FirstNumber,
                        SecondNumber = thisTemp.SecondNumber,
                        WhatSet = thisTemp.WhatSet
                    };
                    await ThisNet!.SendAllAsync("createnewset", thisSend);
                }
                thisCol.ForEach(thisTile =>
                {
                    if (TempSets.HasObject(thisTile.Deck))
                        TempSets.RemoveObject(thisTile.Deck);
                    else
                        MainGame.SingleInfo!.MainHandList.RemoveObjectByDeck(thisTile.Deck);
                });
                await MainGame.CreateNewSetAsync(thisTemp, false);
            }, items => MainGame!.SingleInfo!.InitCompleted, this, CommandContainer!);
            UndoMoveCommand = new BasicGameCommand(this, async items =>
            {
                if (ThisData!.MultiPlayer)
                    await ThisNet!.SendAllAsync("undomove");
                await MainGame!.UndoMoveAsync();
            }, items => MainGame!.SaveRoot!.DidExpand, this, CommandContainer!);
        }
        private async Task MainSets1_SetClickedAsync(int setNumber, int section, int deck)
        {
            if (setNumber == 0)
                throw new BasicBlankException("If the set is 0, rethinking is required");
            var thisSet = MainSets1!.GetIndividualSet(setNumber);
            var thisCol = MainGame!.GetSelectedList();
            if (thisCol.Count == 0)
            {
                if (deck == 0 || thisSet.HandList.Count < 2)
                {
                    if (ThisData!.MultiPlayer)
                        await ThisNet!.SendAllAsync("removeentireset", setNumber);
                    await MainGame.RemoveEntireSetAsync(setNumber);
                    return;
                }
                if (ThisData!.MultiPlayer)
                {
                    SendSet thisSend = new SendSet();
                    thisSend.Index = setNumber;
                    thisSend.Tile = deck;
                    await ThisNet!.SendAllAsync("removeonefromset", thisSend);
                }
                await MainGame.RemoveTileFromSetAsync(setNumber, deck);
                return;
            }
            if (thisCol.Count > 1)
            {
                await ShowGameMessageAsync("Can only add one tile to the set at a time");
                return;
            }
            var thisTile = thisCol.First();
            var newPos = thisSet.PositionToPlay(thisTile, section);
            if (ThisData!.MultiPlayer)
            {
                SendSet finSend = new SendSet();
                finSend.Index = setNumber;
                finSend.Position = newPos;
                finSend.Tile = thisTile.Deck;
                await ThisNet!.SendAllAsync("addtoset", finSend);
            }
            if (TempSets!.HasObject(thisTile.Deck))
                TempSets.RemoveObject(thisTile.Deck);
            else
                MainGame.SingleInfo!.MainHandList.RemoveObjectByDeck(thisTile.Deck);
            await MainGame.AddToSetAsync(setNumber, thisTile, newPos);
        }
        private bool _isProcessing;
        private Task TempSets_SetClickedAsync(int index)
        {
            if (_isProcessing == true)
                return Task.CompletedTask;
            _isProcessing = true;
            var tempList = PlayerHand1!.ListSelectedObjects(true);
            TempSets!.AddCards(index, tempList);
            _isProcessing = false;
            return Task.CompletedTask;
        }
    }
}