using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BowlingDiceGameCP;
using CommonBasicStandardLibraries.Exceptions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
namespace BowlingDiceGameWPF
{
    public class GamePage : MultiPlayerWindow<BowlingDiceGameViewModel, BowlingDiceGamePlayerItem, BowlingDiceGameSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
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
        private DiceListWPF? _diceGraphicsBoard;
        private BowlingCompleteScoresheetWPF? _thisBoard;
        private BowlingDiceGameMainGameClass? _mainGame;
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel();
            BasicSetUp();
            Button thisBut;
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            _thisBoard = new BowlingCompleteScoresheetWPF();
            thisStack.Children.Add(_thisBoard);
            StackPanel tempStack = new StackPanel();
            tempStack.Orientation = Orientation.Horizontal;
            thisBut = GetGamingButton("Roll", nameof(BowlingDiceGameViewModel.RollCommand));
            _diceGraphicsBoard = new DiceListWPF();
            tempStack.Children.Add(_diceGraphicsBoard);
            tempStack.Children.Add(thisBut); // the roll dice should be on the right side
            thisStack.Children.Add(tempStack); // hopefully thissimple
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(BowlingDiceGameViewModel.NormalTurn));
            firstInfo.AddRow("Frame", nameof(BowlingDiceGameViewModel.WhatFrame)); // i think so we know what frame its on.
            firstInfo.AddRow("Status", nameof(BowlingDiceGameViewModel.Status));
            thisStack.Children.Add(firstInfo.GetContent);
            MainGrid!.Children.Add(thisStack);
            thisBut = GetGamingButton("Continue", nameof(BowlingDiceGameViewModel.ContinueTurnCommand));
            thisBut.HorizontalAlignment = HorizontalAlignment.Left;
            thisStack.Children.Add(thisBut);
            var endButton = GetGamingButton("End Turn", nameof(BowlingDiceGameViewModel.EndTurnCommand));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
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