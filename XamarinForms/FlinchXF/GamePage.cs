using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.MultipleFrameContainers;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.BasicControls.SingleCardFrames;
using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using FlinchCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
namespace FlinchXF
{
    public class GamePage : MultiPlayerPage<FlinchViewModel, FlinchPlayerItem, FlinchSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            FlinchMainGameClass mainGame = OurContainer!.Resolve<FlinchMainGameClass>();
            _thisScore!.LoadLists(mainGame.SaveRoot!.PlayerList);
            _playerHand!.LoadList(ThisMod!.PlayerHand1!, ""); // i think
            _deckGPile!.Init(ThisMod.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _publicGraphics!.Init(mainGame.PublicPiles!); // i think
            _publicGraphics.StartAnimationListener("public");
            foreach (var thisPlayer in mainGame.PlayerList!)
            {
                StackLayout thisStack = new StackLayout();
                thisStack.Orientation = StackOrientation.Horizontal;
                BasicMultiplePilesXF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsXF> discardGraphics = new BasicMultiplePilesXF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsXF>();
                discardGraphics.Init(thisPlayer.DiscardPiles!, "");
                discardGraphics.StartAnimationListener("discard" + thisPlayer.NickName);
                thisStack.Children.Add(discardGraphics);
                BasicMultiplePilesXF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsXF> stockGraphics = new BasicMultiplePilesXF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsXF>();
                stockGraphics.Init(thisPlayer.StockPile!.StockFrame, ""); // i think
                stockGraphics.StartAnimationListener("stock" + thisPlayer.NickName);
                if (thisPlayer.PlayerCategory != EnumPlayerCategory.Self)
                    thisStack.InputTransparent = true;
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
            _playerHand!.UpdateList(ThisMod!.PlayerHand1!);
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
        private BaseDeckXF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsXF>? _deckGPile;
        private ScoreBoardXF? _thisScore;
        private BaseHandXF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsXF>? _playerHand;
        private PublicPilesXF? _publicGraphics;
        private Grid? _pileGrid;
        private readonly CustomBasicList<BasicMultiplePilesXF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsXF>> _discardList = new CustomBasicList<BasicMultiplePilesXF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsXF>>();
        private readonly CustomBasicList<BasicMultiplePilesXF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsXF>> _stockList = new CustomBasicList<BasicMultiplePilesXF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsXF>>();
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckXF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsXF>();
            _thisScore = new ScoreBoardXF();
            _playerHand = new BaseHandXF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsXF>();
            _publicGraphics = new PublicPilesXF();
            _pileGrid = new Grid();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(_publicGraphics);
            Grid tempGrid = new Grid();
            AddLeftOverColumn(tempGrid, 60);
            AddLeftOverColumn(tempGrid, 40);
            thisStack.Children.Add(tempGrid);
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_playerHand);
            StackLayout tempStack = new StackLayout();
            tempStack.Children.Add(otherStack);
            tempStack.Children.Add(_pileGrid);
            AddControlToGrid(tempGrid, tempStack, 0, 0);
            Button endButton = GetGamingButton("End Turn", nameof(FlinchViewModel.EndTurnCommand));
            endButton.HorizontalOptions = LayoutOptions.Start;
            tempStack.Children.Add(endButton);
            tempStack = new StackLayout();
            AddControlToGrid(tempGrid, tempStack, 0, 1);
            _thisScore = new ScoreBoardXF();
            _thisScore.AddColumn("In Stock", false, nameof(FlinchPlayerItem.InStock));
            int x;
            for (x = 1; x <= 5; x++) //has to change for flinch.
            {
                var thisStr = "Discard" + x;
                _thisScore.AddColumn(thisStr, false, thisStr);
            }
            _thisScore.AddColumn("Stock Left", false, nameof(FlinchPlayerItem.StockLeft));
            _thisScore.AddColumn("Cards Left", false, nameof(FlinchPlayerItem.ObjectCount)); //very common.
            tempStack.Children.Add(_thisScore);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("RS Cards", nameof(FlinchViewModel.CardsToShuffle));
            firstInfo.AddRow("Turn", nameof(FlinchViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(FlinchViewModel.Status));
            MainGrid!.Children.Add(thisStack);
            tempStack.Children.Add(firstInfo.GetContent);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
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
        }
    }
}