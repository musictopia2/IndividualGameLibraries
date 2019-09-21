using AndyCristinaGamePackageCP.CommonProportionClasses;
using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Interfaces;
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
using SkipboCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
namespace SkipboXF
{
    public class GamePage : MultiPlayerPage<SkipboViewModel, SkipboPlayerItem, SkipboSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            SkipboMainGameClass mainGame = OurContainer!.Resolve<SkipboMainGameClass>();
            _thisScore!.LoadLists(mainGame.SaveRoot!.PlayerList);
            _playerHand!.LoadList(ThisMod!.PlayerHand1!, ""); // i think
            _deckGPile!.Init(ThisMod.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _publicGraphics!.Init(mainGame.PublicPiles!, ""); // i think
            _publicGraphics.StartAnimationListener("public");
            foreach (var thisPlayer in mainGame.PlayerList!)
            {
                StackLayout thisStack = new StackLayout();
                thisStack.Orientation = StackOrientation.Horizontal;
                BasicMultiplePilesXF<SkipboCardInformation, SkipboGraphicsCP, CardGraphicsXF> discardGraphics = new BasicMultiplePilesXF<SkipboCardInformation, SkipboGraphicsCP, CardGraphicsXF>();
                discardGraphics.Init(thisPlayer.DiscardPiles!, "");
                discardGraphics.StartAnimationListener("discard" + thisPlayer.NickName);
                thisStack.Children.Add(discardGraphics);
                BasicMultiplePilesXF<SkipboCardInformation, SkipboGraphicsCP, CardGraphicsXF> stockGraphics = new BasicMultiplePilesXF<SkipboCardInformation, SkipboGraphicsCP, CardGraphicsXF>();
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
            SkipboMainGameClass mainGame = OurContainer!.Resolve<SkipboMainGameClass>();
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
        private BaseDeckXF<SkipboCardInformation, SkipboGraphicsCP, CardGraphicsXF>? _deckGPile;
        private ScoreBoardXF? _thisScore;
        private BaseHandXF<SkipboCardInformation, SkipboGraphicsCP, CardGraphicsXF>? _playerHand;
        private Grid? _pileGrid;
        private readonly CustomBasicList<BasicMultiplePilesXF<SkipboCardInformation, SkipboGraphicsCP, CardGraphicsXF>> _discardList = new CustomBasicList<BasicMultiplePilesXF<SkipboCardInformation, SkipboGraphicsCP, CardGraphicsXF>>();
        private readonly CustomBasicList<BasicMultiplePilesXF<SkipboCardInformation, SkipboGraphicsCP, CardGraphicsXF>> _stockList = new CustomBasicList<BasicMultiplePilesXF<SkipboCardInformation, SkipboGraphicsCP, CardGraphicsXF>>();
        private BasicMultiplePilesXF<SkipboCardInformation, SkipboGraphicsCP, CardGraphicsXF>? _publicGraphics;
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckXF<SkipboCardInformation, SkipboGraphicsCP, CardGraphicsXF>();
            _thisScore = new ScoreBoardXF();
            _playerHand = new BaseHandXF<SkipboCardInformation, SkipboGraphicsCP, CardGraphicsXF>();
            _pileGrid = new Grid();
            _publicGraphics = new BasicMultiplePilesXF<SkipboCardInformation, SkipboGraphicsCP, CardGraphicsXF>();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_publicGraphics);
            thisStack.Children.Add(otherStack);
            _thisScore = new ScoreBoardXF();
            _thisScore.AddColumn("In Stock", false, nameof(SkipboPlayerItem.InStock));
            int x;
            for (x = 1; x <= 4; x++) //has to change for flinch.
            {
                var thisStr = "Discard" + x;
                _thisScore.AddColumn(thisStr, false, thisStr);
            }
            _thisScore.AddColumn("Stock Left", false, nameof(SkipboPlayerItem.StockLeft));
            _thisScore.AddColumn("Cards Left", false, nameof(SkipboPlayerItem.ObjectCount)); //very common.
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("RS Cards", nameof(SkipboViewModel.CardsToShuffle));
            firstInfo.AddRow("Turn", nameof(SkipboViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(SkipboViewModel.Status));
            MainGrid!.Children.Add(thisStack);
            thisStack.Children.Add(_playerHand);
            thisStack.Children.Add(_pileGrid);
            thisStack.Children.Add(firstInfo.GetContent);
            thisStack.Children.Add(_thisScore);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<SkipboViewModel>();
            OurContainer!.RegisterType<DeckViewModel<SkipboCardInformation>>(true);
            OurContainer.RegisterType<BasicGameLoader<SkipboPlayerItem, SkipboSaveInfo>>();
            OurContainer.RegisterType<GenericCardShuffler<SkipboCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, SkipboDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>("");
        }
    }
}