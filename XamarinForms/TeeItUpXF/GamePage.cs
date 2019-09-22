using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.BasicControls.SingleCardFrames;
using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using TeeItUpCP;
using Xamarin.Forms;
namespace TeeItUpXF
{
    public class GamePage : MultiPlayerPage<TeeItUpViewModel, TeeItUpPlayerItem, TeeItUpSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            TeeItUpSaveInfo saveRoot = OurContainer!.Resolve<TeeItUpSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _discardGPile!.Init(ThisMod!.Pile1!, ""); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.
            _deckGPile!.Init(ThisMod.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _otherPile!.Init(ThisMod.Pile2!, "");
            _otherPile.StartAnimationListener("otherpile");
            CustomBasicList<TeeItUpPlayerItem> thisList;
            if (ThisData!.MultiPlayer == true)
                thisList = _mainGame!.PlayerList!.GetAllPlayersStartingWithSelf();
            else
                thisList = _mainGame!.PlayerList.ToCustomBasicList();
            thisList.ForEach(thisPlayer =>
            {
                CardBoardXF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsXF> thisControl = new CardBoardXF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsXF>();
                _boardList!.Add(thisControl);
                thisControl.LoadList(thisPlayer.PlayerBoard!, "");
                _boardStack!.Children.Add(thisControl);
            });
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            TeeItUpSaveInfo saveRoot = OurContainer!.Resolve<TeeItUpSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _deckGPile!.UpdateDeck(ThisMod!.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            CustomBasicList<TeeItUpPlayerItem> thisList;
            if (ThisData!.MultiPlayer == true)
                thisList = _mainGame!.PlayerList!.GetAllPlayersStartingWithSelf();
            else
                thisList = _mainGame!.PlayerList.ToCustomBasicList();
            int x = 0;
            _otherPile!.UpdatePile(ThisMod.Pile2!);
            thisList.ForEach(thisPlayer =>
            {
                CardBoardXF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsXF> thisControl = _boardList![x];
                thisControl.UpdateList(thisPlayer.PlayerBoard!);
                x++;
            });
            return Task.CompletedTask;
        }
        private BaseDeckXF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsXF>? _deckGPile;
        private BasePileXF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsXF>? _discardGPile;
        private ScoreBoardXF? _thisScore;
        private BasePileXF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsXF>? _otherPile;
        private StackLayout? _boardStack;
        private TeeItUpMainGameClass? _mainGame;
        private CustomBasicList<CardBoardXF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsXF>>? _boardList;
        protected override async Task AfterGameButtonAsync()
        {
            _mainGame = OurContainer!.Resolve<TeeItUpMainGameClass>();
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckXF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsXF>();
            _discardGPile = new BasePileXF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsXF>();
            _thisScore = new ScoreBoardXF();
            _otherPile = new BasePileXF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsXF>();
            _boardStack = new StackLayout();
            _boardStack.Orientation = StackOrientation.Horizontal;
            _boardList = new CustomBasicList<CardBoardXF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsXF>>();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            RoundButton!.HorizontalOptions = LayoutOptions.Center;
            RoundButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton);
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            otherStack.Children.Add(_otherPile);
            thisStack.Children.Add(otherStack);
            _thisScore = new ScoreBoardXF();
            _thisScore.AddColumn("Went Out", true, nameof(TeeItUpPlayerItem.WentOut), useTrueFalse: true);
            _thisScore.AddColumn("Previous Score", true, nameof(TeeItUpPlayerItem.PreviousScore));
            _thisScore.AddColumn("Total Score", true, nameof(TeeItUpPlayerItem.TotalScore));
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(TeeItUpViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(TeeItUpViewModel.Status));
            firstInfo.AddRow("Round", nameof(TeeItUpViewModel.Round));
            firstInfo.AddRow("Instructions", nameof(TeeItUpViewModel.Instructions));
            otherStack.Children.Add(_thisScore);
            otherStack.Children.Add(firstInfo.GetContent);
            thisStack.Children.Add(_boardStack);
            MainGrid!.Children.Add(thisStack);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            _otherPile.Margin = new Thickness(5, 5, 5, 5);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<TeeItUpViewModel>();
            OurContainer!.RegisterType<DeckViewModel<TeeItUpCardInformation>>(true);
            OurContainer.RegisterType<BasicGameLoader<TeeItUpPlayerItem, TeeItUpSaveInfo>>();
            OurContainer.RegisterType<GenericCardShuffler<TeeItUpCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, TeeItUpDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, CustomProportion>("");
            //anything else that needs to be registered will be here.

        }
    }
}