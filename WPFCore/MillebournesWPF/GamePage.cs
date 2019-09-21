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
using CommonBasicStandardLibraries.Exceptions;
using MillebournesCP;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
namespace MillebournesWPF
{
    public class GamePage : MultiPlayerWindow<MillebournesViewModel, MillebournesPlayerItem, MillebournesSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            MillebournesSaveInfo saveRoot = OurContainer!.Resolve<MillebournesSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHandWPF!.LoadList(ThisMod!.PlayerHand1!, ""); // i think
            _discardGPile!.Init(ThisMod.Pile2!, ""); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.
            _deckGPile!.Init(ThisMod.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _newCard!.Init(ThisMod.Pile1!, "");
            SetUpTeamPiles();
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            MillebournesSaveInfo saveRoot = OurContainer!.Resolve<MillebournesSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHandWPF!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile2!);
            _newCard!.UpdatePile(ThisMod.Pile1!);
            _disList.ForEach(thisD => thisD.Unregister());
            _disList.Clear();
            SetUpTeamPiles();
            return Task.CompletedTask;
        }
        private void SetUpTeamPiles()
        {
            _mainGame!.SingleInfo = _mainGame.PlayerList!.GetSelf();
            if (_mainGame.SingleInfo.MainHandList.Any(items => items.CompleteCategory == EnumCompleteCategories.None))
                throw new BasicBlankException("Cannot have category of none.  Rethink");
            _pileStack!.Children.Clear();
            _mainGame.TeamList.ForEach(thisTeam =>
            {
                Grid tempGrid = new Grid();
                tempGrid.Margin = new Thickness(0, 0, 0, 20);
                AddAutoRows(tempGrid, 2);
                AddAutoColumns(tempGrid, 2);
                TextBlock ThisLabel = new TextBlock();
                ThisLabel.Text = thisTeam.Text;
                ThisLabel.Foreground = Brushes.Aqua;
                ThisLabel.FontWeight = FontWeights.Bold;
                AddControlToGrid(tempGrid, ThisLabel, 0, 0);
                Grid.SetColumnSpan(ThisLabel, 2);
                ThisLabel.HorizontalAlignment = HorizontalAlignment.Center; // try this
                BasicMultiplePilesWPF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsWPF> thisDis = new BasicMultiplePilesWPF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsWPF>();
                thisDis.Init(thisTeam.CardPiles, "");
                thisDis.StartAnimationListener("team" + thisTeam.TeamNumber);
                AddControlToGrid(tempGrid, thisDis, 1, 0);
                SafetiesWPF thisS = new SafetiesWPF();
                thisS.Init(thisTeam, _mainGame);
                _disList.Add(thisDis);
                AddControlToGrid(tempGrid, thisS, 1, 1);
                _pileStack.Children.Add(tempGrid);
            });
        }
        private BaseDeckWPF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsWPF>? _deckGPile;
        private BasePileWPF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsWPF>? _discardGPile;
        private ScoreBoardWPF? _thisScore;
        private BaseHandWPF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsWPF>? _playerHandWPF;
        private BasePileWPF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsWPF>? _newCard;
        private StackPanel? _pileStack;
        private MillebournesMainGameClass? _mainGame;
        private readonly CustomBasicList<BasicMultiplePilesWPF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsWPF>> _disList = new CustomBasicList<BasicMultiplePilesWPF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsWPF>>();
        protected async override void AfterGameButton() //warning.  iffy because of new game class.
        {
            _mainGame = OurContainer!.Resolve<MillebournesMainGameClass>();
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckWPF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsWPF>();
            _discardGPile = new BasePileWPF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsWPF>();
            _thisScore = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsWPF>();
            _newCard = new BasePileWPF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsWPF>();
            _pileStack = new StackPanel();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            RoundButton!.HorizontalAlignment = HorizontalAlignment.Center;
            RoundButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton);
            StackPanel summaryStack = new StackPanel();
            summaryStack.Orientation = Orientation.Horizontal;
            summaryStack.Children.Add(thisStack);
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_newCard);
            var thisBut = GetGamingButton("Coupe Foure", nameof(MillebournesViewModel.CoupeCommand));
            var thisBind = GetVisibleBinding(nameof(MillebournesViewModel.CoupeVisible));
            thisBut.SetBinding(VisibilityProperty, thisBind);
            otherStack.Children.Add(thisBut);
            thisStack.Children.Add(otherStack);
            _thisScore.AddColumn("Team", true, nameof(MillebournesPlayerItem.Team), rightMargin: 10);
            _thisScore.AddColumn("Miles", true, nameof(MillebournesPlayerItem.Miles), rightMargin: 10);
            _thisScore.AddColumn("Other Points", true, nameof(MillebournesPlayerItem.OtherPoints), rightMargin: 10);
            _thisScore.AddColumn("Total Points", true, nameof(MillebournesPlayerItem.TotalPoints), rightMargin: 10);
            _thisScore.AddColumn("# 200s", true, nameof(MillebournesPlayerItem.Number200s), rightMargin: 10);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(MillebournesViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(MillebournesViewModel.Status));
            MainGrid!.Children.Add(summaryStack);
            thisStack.Children.Add(_playerHandWPF);
            thisStack.Children.Add(firstInfo.GetContent);
            thisStack.Children.Add(_thisScore);
            summaryStack.Children.Add(_pileStack);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _newCard.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<MillebournesViewModel>();
            OurContainer!.RegisterType<DeckViewModel<MillebournesCardInformation>>(true);
            OurContainer.RegisterType<BasicGameLoader<MillebournesPlayerItem, MillebournesSaveInfo>>();
            OurContainer.RegisterType<GenericCardShuffler<MillebournesCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, MillebournesDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, CustomProportion>("");
        }
    }
}