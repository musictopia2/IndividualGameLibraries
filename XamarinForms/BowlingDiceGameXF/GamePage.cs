using AndyCristinaGamePackageCP.CommonProportionClasses;
using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BowlingDiceGameCP;
using CommonBasicStandardLibraries.Exceptions;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
namespace BowlingDiceGameXF
{
    public class GamePage : MultiPlayerPage<BowlingDiceGameViewModel, BowlingDiceGamePlayerItem, BowlingDiceGameSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }

        public override Task HandleAsync(LoadEventModel message)
        {
            if (ThisMod!.WhatFrame == 0)
                throw new BasicBlankException("Frame cannot be 0");
            _mainGame = OurContainer!.Resolve<BowlingDiceGameMainGameClass>(); //i think do here would be best.
            _thisBoard!.LoadPlayerScores();
            _diceGraphicsBoard!.LoadBoard(_mainGame!.DiceBoard!.DiceList);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            if (ThisMod!.WhatFrame == 0)
                throw new BasicBlankException("Frame cannot be 0");
            _diceGraphicsBoard!.SaveBoard(_mainGame!.DiceBoard!.DiceList);
            _thisBoard!.SavedScores();
            return Task.CompletedTask;
        }
        private DiceListXF? _diceGraphicsBoard;
        private BowlingCompleteScoresheetXF? _thisBoard;
        private BowlingDiceGameMainGameClass? _mainGame;
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout();
            BasicSetUp();
            Button thisBut;
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            _thisBoard = new BowlingCompleteScoresheetXF();
            thisStack.Children.Add(_thisBoard);
            StackLayout tempStack = new StackLayout();
            tempStack.Orientation = StackOrientation.Horizontal;
            thisBut = GetGamingButton("Roll", nameof(BowlingDiceGameViewModel.RollCommand));
            _diceGraphicsBoard = new DiceListXF();
            tempStack.Children.Add(_diceGraphicsBoard);
            tempStack.Children.Add(thisBut); // the roll dice should be on the right side
            thisStack.Children.Add(tempStack); // hopefully thissimple
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(BowlingDiceGameViewModel.NormalTurn));
            firstInfo.AddRow("Frame", nameof(BowlingDiceGameViewModel.WhatFrame)); // i think so we know what frame its on.
            firstInfo.AddRow("Status", nameof(BowlingDiceGameViewModel.Status));
            thisStack.Children.Add(firstInfo.GetContent);
            MainGrid!.Children.Add(thisStack);
            thisBut = GetSmallerButton("Continue", nameof(BowlingDiceGameViewModel.ContinueTurnCommand));
            thisBut.HorizontalOptions = LayoutOptions.Start;
            thisStack.Children.Add(thisBut);
            var endButton = GetSmallerButton("End Turn", nameof(BowlingDiceGameViewModel.EndTurnCommand));
            endButton.HorizontalOptions = LayoutOptions.Start;
            thisStack.Children.Add(endButton);
            AddRestoreCommand(thisStack);
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<BowlingDiceGamePlayerItem, BowlingDiceGameSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterNonSavedClasses<BowlingDiceGameViewModel>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>();
        }
    }
}