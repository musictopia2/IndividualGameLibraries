using AndyCristinaGamePackageCP.ExtensionClasses;
using AndyCristinaGamePackageXF;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.ChoicePickers;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.BasicControls.SingleCardFrames;
using BaseGPXPagesAndControlsXF.GameGraphics.GamePieces;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.ColorCards;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using UnoCP;
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using ts = BasicGameFramework.GameGraphicsCP.Cards.BaseColorCardsCP;
namespace UnoXF
{
    public class GamePage : MultiPlayerPage<UnoViewModel, UnoPlayerItem, UnoSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            UnoSaveInfo saveRoot = OurContainer!.Resolve<UnoSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _thisColor!.LoadLists(ThisMod!.ColorVM!);
            _discardGPile!.Init(ThisMod.Pile1!, ts.TagUsed); // may have to be here (well see)
            _playerHand!.LoadList(ThisMod.PlayerHand1!, ts.TagUsed); // i think
            _colorHand!.LoadList(ThisMod.PlayerHand1!, ts.TagUsed);
            _discardGPile.StartListeningDiscardPile(); // its the main one.
            _deckGPile!.Init(ThisMod.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            UnoSaveInfo saveRoot = OurContainer!.Resolve<UnoSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _colorHand!.UpdateList(ThisMod!.PlayerHand1!);
            _playerHand!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            return Task.CompletedTask;
        }
        private BaseDeckXF<UnoCardInformation, UnoGraphicsCP, CardGraphicsXF>? _deckGPile;
        private BasePileXF<UnoCardInformation, UnoGraphicsCP, CardGraphicsXF>? _discardGPile;
        private ScoreBoardXF? _thisScore;
        private BaseHandXF<UnoCardInformation, UnoGraphicsCP, CardGraphicsXF>? _playerHand;
        private BaseHandXF<UnoCardInformation, UnoGraphicsCP, CardGraphicsXF>? _colorHand;
        private EnumPickerXF<CheckerChoiceCP<EnumColorTypes>,
            CheckerChooserXF<EnumColorTypes>, EnumColorTypes,
            ColorListChooser<EnumColorTypes>>? _thisColor;
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            StackLayout colorStack = new StackLayout();
            BasicSetUp(colorStack);
            //anything needed for ui is here.
            _deckGPile = new BaseDeckXF<UnoCardInformation, UnoGraphicsCP, CardGraphicsXF>();
            _discardGPile = new BasePileXF<UnoCardInformation, UnoGraphicsCP, CardGraphicsXF>();
            _thisScore = new ScoreBoardXF();
            _playerHand = new BaseHandXF<UnoCardInformation, UnoGraphicsCP, CardGraphicsXF>();
            _thisColor = new EnumPickerXF<CheckerChoiceCP<EnumColorTypes>, CheckerChooserXF<EnumColorTypes>, EnumColorTypes, ColorListChooser<EnumColorTypes>>();
            _colorHand = new BaseHandXF<UnoCardInformation, UnoGraphicsCP, CardGraphicsXF>();
            colorStack.SetBinding(IsVisibleProperty, new Binding(nameof(UnoViewModel.ColorVisible)));
            colorStack.Children.Add(_thisColor);
            colorStack.Children.Add(_colorHand);
            RoundButton!.HorizontalOptions = LayoutOptions.Center;
            RoundButton!.VerticalOptions = LayoutOptions.Center;
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton);
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            var endButton = GetGamingButton("End Turn", nameof(UnoViewModel.EndTurnCommand));
            endButton.HorizontalOptions = LayoutOptions.Start;
            otherStack.Children.Add(endButton);
            var thisBut = GetGamingButton("Uno", nameof(UnoViewModel.UnoCommand));
            thisBut.SetBinding(IsVisibleProperty, new Binding(nameof(UnoViewModel.UnoVisible)));
            otherStack.Children.Add(thisBut);
            thisStack.Children.Add(otherStack);
            _thisScore = new ScoreBoardXF();
            _thisScore.AddColumn("Cards Left", true, nameof(UnoPlayerItem.ObjectCount)); //very common.
            _thisScore.AddColumn("Total Points", true, nameof(UnoPlayerItem.TotalPoints), rightMargin: 10);
            _thisScore.AddColumn("Previous Points", true, nameof(UnoPlayerItem.PreviousPoints), rightMargin: 10);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(UnoViewModel.NormalTurn));
            firstInfo.AddRow("Next", nameof(UnoViewModel.NextPlayer));
            firstInfo.AddRow("Status", nameof(UnoViewModel.Status));

            MainGrid!.Children.Add(thisStack);
            thisStack.Children.Add(_playerHand);
            thisStack.Children.Add(firstInfo.GetContent);

            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            thisStack.Children.Add(otherStack);
            otherStack.Children.Add(_thisScore);
            //this is only a starting point.


            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterCommonMiscCards<UnoViewModel, UnoCardInformation>(ts.TagUsed);
            OurContainer!.RegisterType<BasicGameLoader<UnoPlayerItem, UnoSaveInfo>>();
            OurContainer.RegisterType<ColorCardsShuffler<UnoCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, Phase10UnoDeck>();
            OurContainer.RegisterType<StandardPickerSizeClass>();
        }
    }
}