using AndyCristinaGamePackageCP.CommonProportionClasses;
using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.MultipleFrameContainers;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.BasicControls.SingleCardFrames;
using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using CommonBasicStandardLibraries.CollectionClasses; //just in case i want to use the new custom classes.
using FlinchCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
namespace FlinchWPF
{
    public class GamePage : MultiPlayerWindow<FlinchViewModel, FlinchPlayerItem, FlinchSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            FlinchMainGameClass mainGame = OurContainer!.Resolve<FlinchMainGameClass>();
            _thisScore!.LoadLists(mainGame.SaveRoot!.PlayerList);
            _playerHandWPF!.LoadList(ThisMod!.PlayerHand1!, ""); // i think
            _deckGPile!.Init(ThisMod.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _publicGraphics!.Init(mainGame.PublicPiles!); // i think
            _publicGraphics.StartAnimationListener("public");
            foreach (var thisPlayer in mainGame.PlayerList!)
            {
                StackPanel thisStack = new StackPanel();
                thisStack.Orientation = Orientation.Horizontal;
                BasicMultiplePilesWPF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsWPF> discardGraphics = new BasicMultiplePilesWPF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsWPF>();
                discardGraphics.Init(thisPlayer.DiscardPiles!, "");
                discardGraphics.StartAnimationListener("discard" + thisPlayer.NickName);
                thisStack.Children.Add(discardGraphics);
                BasicMultiplePilesWPF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsWPF> stockGraphics = new BasicMultiplePilesWPF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsWPF>();
                stockGraphics.Init(thisPlayer.StockPile!.StockFrame, ""); // i think
                stockGraphics.StartAnimationListener("stock" + thisPlayer.NickName);
                thisStack.Children.Add(stockGraphics);
                _stockList.Add(stockGraphics);
                _discardList.Add(discardGraphics);
                _pileGrid!.Children.Add(thisStack);
            }
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            FlinchMainGameClass mainGame = OurContainer!.Resolve<FlinchMainGameClass>();
            _thisScore!.UpdateLists(mainGame.SaveRoot!.PlayerList);
            _playerHandWPF!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _publicGraphics!.UpdateLists(mainGame.PublicPiles!);
            int x = 0;
            foreach (var thisPlayer in mainGame.PlayerList!)
            {
                var stockGraphics = _stockList[x];
                stockGraphics.UpdateLists(thisPlayer.StockPile!.StockFrame);
                var discardGraphics = _discardList[x];
                discardGraphics.UpdateLists(thisPlayer.DiscardPiles!);
                x++;
            }
            return Task.CompletedTask;
        }
        private BaseDeckWPF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsWPF>? _deckGPile;
        private ScoreBoardWPF? _thisScore;
        private BaseHandWPF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsWPF>? _playerHandWPF;
        private PublicPilesWPF? _publicGraphics;
        private Grid? _pileGrid;
        private readonly CustomBasicList<BasicMultiplePilesWPF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsWPF>> _discardList = new CustomBasicList<BasicMultiplePilesWPF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsWPF>>();
        private readonly CustomBasicList<BasicMultiplePilesWPF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsWPF>> _stockList = new CustomBasicList<BasicMultiplePilesWPF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsWPF>>();
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            //anything needed for ui is here.
            _deckGPile = new BaseDeckWPF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsWPF>();
            _thisScore = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsWPF>();
            _publicGraphics = new PublicPilesWPF();
            _publicGraphics.Width = 700;
            _publicGraphics.Height = 500;
            _pileGrid = new Grid();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_publicGraphics);
            StackPanel tempStack = new StackPanel();
            var thisLabel = GetDefaultLabel();
            thisLabel.Text = "Cards To Reshuffle";
            tempStack.Children.Add(thisLabel);
            otherStack.Children.Add(tempStack);
            thisLabel = GetDefaultLabel();
            thisLabel.SetBinding(TextBlock.TextProperty, nameof(FlinchViewModel.CardsToShuffle));
            tempStack.Children.Add(thisLabel);
            _thisScore = new ScoreBoardWPF();
            _thisScore.AddColumn("In Stock", false, nameof(FlinchPlayerItem.InStock));
            int x;
            for (x = 1; x <= 5; x++) //has to change for flinch.
            {
                var thisStr = "Discard" + x;
                _thisScore.AddColumn(thisStr, false, thisStr);
            }
            _thisScore.AddColumn("Stock Left", false, nameof(FlinchPlayerItem.StockLeft));
            _thisScore.AddColumn("Cards Left", false, nameof(FlinchPlayerItem.ObjectCount)); //very common.
            otherStack.Children.Add(_thisScore); // i think
            thisStack.Children.Add(otherStack);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(FlinchViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(FlinchViewModel.Status));
            MainGrid!.Children.Add(thisStack);
            thisStack.Children.Add(_playerHandWPF);
            Button endButton = GetGamingButton("End Turn", nameof(FlinchViewModel.EndTurnCommand));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            endButton.VerticalAlignment = VerticalAlignment.Top;
            otherStack.Children.Add(endButton);
            thisStack.Children.Add(_pileGrid);
            otherStack.Children.Add(firstInfo.GetContent);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            AddRestoreCommand(otherStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<FlinchViewModel>();
            OurContainer!.RegisterType<DeckViewModel<FlinchCardInformation>>(true);
            OurContainer.RegisterType<BasicGameLoader<FlinchPlayerItem, FlinchSaveInfo>>();
            OurContainer.RegisterType<GenericCardShuffler<FlinchCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, FlinchDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>("");
            //anything else that needs to be registered will be here.

        }
    }
}