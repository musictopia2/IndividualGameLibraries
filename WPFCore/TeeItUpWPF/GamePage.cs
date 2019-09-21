using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
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
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses; //just in case i want to use the new custom classes.
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TeeItUpCP;
namespace TeeItUpWPF
{
    public class GamePage : MultiPlayerWindow<TeeItUpViewModel, TeeItUpPlayerItem, TeeItUpSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
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
                CardBoardWPF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsWPF> thisControl = new CardBoardWPF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsWPF>();
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
                CardBoardWPF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsWPF> thisControl = _boardList![x];
                thisControl.UpdateList(thisPlayer.PlayerBoard!);
                x++;
            });
            return Task.CompletedTask;
        }
        private BaseDeckWPF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsWPF>? _deckGPile;
        private BasePileWPF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsWPF>? _discardGPile;
        private BasePileWPF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsWPF>? _otherPile;
        private ScoreBoardWPF? _thisScore;
        private StackPanel? _boardStack;
        private TeeItUpMainGameClass? _mainGame;
        private CustomBasicList<CardBoardWPF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsWPF>>? _boardList;
        protected async override void AfterGameButton()
        {
            _mainGame = OurContainer!.Resolve<TeeItUpMainGameClass>();
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckWPF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsWPF>();
            _discardGPile = new BasePileWPF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsWPF>();
            _thisScore = new ScoreBoardWPF();
            _otherPile = new BasePileWPF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsWPF>();
            _boardStack = new StackPanel();
            _boardStack.Orientation = Orientation.Horizontal;
            _boardList = new CustomBasicList<CardBoardWPF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsWPF>>();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            RoundButton!.HorizontalAlignment = HorizontalAlignment.Center;
            RoundButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton);
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            otherStack.Children.Add(_otherPile);
            thisStack.Children.Add(otherStack);
            _thisScore = new ScoreBoardWPF();
            _thisScore.AddColumn("Went Out", true, nameof(TeeItUpPlayerItem.WentOut), useTrueFalse: true);
            _thisScore.AddColumn("Previous Score", true, nameof(TeeItUpPlayerItem.PreviousScore));
            _thisScore.AddColumn("Total Score", true, nameof(TeeItUpPlayerItem.TotalScore));
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
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
        }
    }
}