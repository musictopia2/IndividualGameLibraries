using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;;
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
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using MillebournesCP;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
namespace MillebournesXF
{
    public class GamePage : MultiPlayerPage<MillebournesViewModel, MillebournesPlayerItem, MillebournesSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            MillebournesSaveInfo saveRoot = OurContainer!.Resolve<MillebournesSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHand!.LoadList(ThisMod!.PlayerHand1!, ""); // i think
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
            _playerHand!.UpdateList(ThisMod!.PlayerHand1!);
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
            _pileGrid!.Children.Clear();
            int x = 0;
            _mainGame.TeamList.ForEach(thisTeam =>
            {
                x++;
                Grid tempGrid = new Grid();
                tempGrid.Margin = new Thickness(0, 0, 0, 5);
                Label ThisLabel = new Label();
                if (ScreenUsed == EnumScreen.SmallPhone)
                {
                    ThisLabel.FontSize = 8;
                    AddPixelRow(tempGrid, 12);
                    AddPixelRow(tempGrid, 68);
                }
                else
                {
                    AddAutoRows(tempGrid, 1);
                    AddLeftOverRow(tempGrid, 1);
                    if (ScreenUsed == EnumScreen.SmallTablet)
                        ThisLabel.FontSize = 12;
                    else
                        ThisLabel.FontSize = 20;
                }
                AddAutoColumns(tempGrid, 2);
                ThisLabel.Text = thisTeam.Text;
                ThisLabel.TextColor = Color.Aqua;
                ThisLabel.FontAttributes = FontAttributes.Bold;
                AddControlToGrid(tempGrid, ThisLabel, 0, 0);
                ThisLabel.HorizontalOptions = LayoutOptions.Center; // try this
                BasicMultiplePilesXF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsXF> thisDis = new BasicMultiplePilesXF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsXF>();
                thisDis.Init(thisTeam.CardPiles, "");
                thisDis.StartAnimationListener("team" + thisTeam.TeamNumber);
                AddControlToGrid(tempGrid, thisDis, 1, 0);
                SafetiesXF thisS = new SafetiesXF();
                thisS.Init(thisTeam, _mainGame);
                _disList.Add(thisDis);
                if (x == 1)
                {
                    AddControlToGrid(tempGrid, thisS, 0, 1);
                    Grid.SetRowSpan(thisS, 2);
                }
                else
                    AddControlToGrid(tempGrid, thisS, 1, 1);
                AddControlToGrid(_pileGrid, tempGrid, x - 1, 0);
            });
        }
        private BaseDeckXF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsXF>? _deckGPile;
        private BasePileXF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsXF>? _discardGPile;
        private ScoreBoardXF? _thisScore;
        private BaseHandXF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsXF>? _playerHand;
        private BasePileXF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsXF>? _newCard;
        private Grid? _pileGrid;
        private MillebournesMainGameClass? _mainGame;
        private readonly CustomBasicList<BasicMultiplePilesXF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsXF>> _disList = new CustomBasicList<BasicMultiplePilesXF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsXF>>();
        protected override async Task AfterGameButtonAsync()
        {
            _mainGame = OurContainer!.Resolve<MillebournesMainGameClass>();
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckXF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsXF>();
            _discardGPile = new BasePileXF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsXF>();
            _thisScore = new ScoreBoardXF();
            _playerHand = new BaseHandXF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsXF>();
            _newCard = new BasePileXF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsXF>();
            ScrollView thisScroll = new ScrollView();
            thisScroll.Orientation = ScrollOrientation.Vertical;
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            RoundButton!.HorizontalOptions = LayoutOptions.Center;
            RoundButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton);
            StackLayout summaryStack = new StackLayout();
            summaryStack.Orientation = StackOrientation.Horizontal;
            if (ScreenUsed == EnumScreen.SmallPhone)
                thisScroll.Content = summaryStack;
            summaryStack.Children.Add(thisStack);
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_newCard);
            var thisBut = GetGamingButton("Coupe Foure", nameof(MillebournesViewModel.CoupeCommand));
            var thisBind = new Binding(nameof(MillebournesViewModel.CoupeVisible));
            thisBut.SetBinding(IsVisibleProperty, thisBind);
            otherStack.Children.Add(thisBut);
            thisStack.Children.Add(otherStack);
            _thisScore.AddColumn("Team", true, nameof(MillebournesPlayerItem.Team), rightMargin: 10);
            _thisScore.AddColumn("Miles", true, nameof(MillebournesPlayerItem.Miles), rightMargin: 10);
            _thisScore.AddColumn("Other Points", true, nameof(MillebournesPlayerItem.OtherPoints), rightMargin: 10);
            _thisScore.AddColumn("Total Points", true, nameof(MillebournesPlayerItem.TotalPoints), rightMargin: 10);
            _thisScore.AddColumn("# 200s", true, nameof(MillebournesPlayerItem.Number200s), rightMargin: 10);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(MillebournesViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(MillebournesViewModel.Status));
            if (ScreenUsed == EnumScreen.SmallPhone)
                MainGrid!.Children.Add(thisScroll);
            else
                MainGrid!.Children.Add(summaryStack);
            thisStack.Children.Add(_playerHand);
            thisStack.Children.Add(firstInfo.GetContent);
            thisStack.Children.Add(_thisScore);
            _pileGrid = new Grid();
            AddAutoColumns(_pileGrid, 1);
            3.Times(x =>
            {
                AddLeftOverRow(_pileGrid, 1);
            });
            summaryStack.Children.Add(_pileGrid);
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
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(""); //decided to risk standard proportions this time.
        }
    }
}