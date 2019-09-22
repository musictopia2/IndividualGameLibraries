using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.BasicControls.SingleCardFrames;
using BaseGPXWindowsAndControlsCore.GameGraphics.GamePieces;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.ColorCards;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UnoCP;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using ts = BasicGameFramework.GameGraphicsCP.Cards.BaseColorCardsCP;
namespace UnoWPF
{
    public class GamePage : MultiPlayerWindow<UnoViewModel, UnoPlayerItem, UnoSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            UnoSaveInfo saveRoot = OurContainer!.Resolve<UnoSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _colorHand!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed);
            _playerHandWPF!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
            _discardGPile!.Init(ThisMod.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.
            _thisColor!.LoadLists(ThisMod.ColorVM!);
            _deckGPile!.Init(ThisMod.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            UnoSaveInfo saveRoot = OurContainer!.Resolve<UnoSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _colorHand!.UpdateList(ThisMod!.PlayerHand1!);
            _playerHandWPF!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            return Task.CompletedTask;
        }
        private BaseDeckWPF<UnoCardInformation, UnoGraphicsCP, CardGraphicsWPF>? _deckGPile;
        private BasePileWPF<UnoCardInformation, UnoGraphicsCP, CardGraphicsWPF>? _discardGPile;

        private ScoreBoardWPF? _thisScore;
        private BaseHandWPF<UnoCardInformation, UnoGraphicsCP, CardGraphicsWPF>? _playerHandWPF;
        private BaseHandWPF<UnoCardInformation, UnoGraphicsCP, CardGraphicsWPF>? _colorHand;
        private EnumPickerWPF<CheckerChoiceCP<EnumColorTypes>,
            CheckerChooserWPF<EnumColorTypes>, EnumColorTypes,
            ColorListChooser<EnumColorTypes>>? _thisColor;

        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            StackPanel colorStack = new StackPanel();
            BasicSetUp(colorStack);
            //anything needed for ui is here.
            _deckGPile = new BaseDeckWPF<UnoCardInformation, UnoGraphicsCP, CardGraphicsWPF>();
            _discardGPile = new BasePileWPF<UnoCardInformation, UnoGraphicsCP, CardGraphicsWPF>();
            _thisScore = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<UnoCardInformation, UnoGraphicsCP, CardGraphicsWPF>();
            _thisColor = new EnumPickerWPF<CheckerChoiceCP<EnumColorTypes>, CheckerChooserWPF<EnumColorTypes>, EnumColorTypes, ColorListChooser<EnumColorTypes>>();
            _thisColor.GraphicsHeight = 250;
            _thisColor.GraphicsWidth = 250;
            _colorHand = new BaseHandWPF<UnoCardInformation, UnoGraphicsCP, CardGraphicsWPF>();
            _colorHand.HandType = HandViewModel<UnoCardInformation>.EnumHandList.Horizontal;
            _colorHand.Margin = new Thickness(3, 3, 3, 3);
            Binding thisBind;
            thisBind = GetVisibleBinding(nameof(UnoViewModel.ColorVisible));
            // has to do to colorstack.
            colorStack.SetBinding(VisibilityProperty, thisBind);
            colorStack.Children.Add(_thisColor);
            colorStack.Children.Add(_colorHand);
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            RoundButton!.HorizontalAlignment = HorizontalAlignment.Center;
            RoundButton!.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton);
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            var endButton = GetGamingButton("End Turn", nameof(UnoViewModel.EndTurnCommand));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            otherStack.Children.Add(endButton);
            var thisBut = GetGamingButton("Uno", nameof(UnoViewModel.UnoCommand));
            thisBind = GetVisibleBinding(nameof(UnoViewModel.UnoVisible));
            thisBut.SetBinding(Button.VisibilityProperty, thisBind);
            otherStack.Children.Add(thisBut);
            thisStack.Children.Add(otherStack);
            _thisScore = new ScoreBoardWPF();
            _thisScore.AddColumn("Cards Left", true, nameof(UnoPlayerItem.ObjectCount)); //very common.
            _thisScore.AddColumn("Total Points", true, nameof(UnoPlayerItem.TotalPoints), rightMargin: 10);
            _thisScore.AddColumn("Previous Points", true, nameof(UnoPlayerItem.PreviousPoints), rightMargin: 10);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(UnoViewModel.NormalTurn));
            firstInfo.AddRow("Next", nameof(UnoViewModel.NextPlayer));
            firstInfo.AddRow("Status", nameof(UnoViewModel.Status));
            MainGrid!.Children.Add(thisStack);
            thisStack.Children.Add(_playerHandWPF);
            thisStack.Children.Add(firstInfo.GetContent);
            thisStack.Children.Add(_thisScore);
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
        }
    }
}