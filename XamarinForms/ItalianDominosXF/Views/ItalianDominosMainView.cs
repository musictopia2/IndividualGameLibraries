using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.Dominos;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.GameGraphics.Dominos;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using ItalianDominosCP.Data;
using ItalianDominosCP.ViewModels;
using SkiaSharp;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Dominos.DominosCP;

namespace ItalianDominosXF.Views
{
    public class ItalianDominosMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly ItalianDominosVMData _model;
        private readonly BoneYardXF<SimpleDominoInfo, ts, DominosXF<SimpleDominoInfo>,
            DominosBasicShuffler<SimpleDominoInfo>> _bone;
        private readonly BaseHandXF<SimpleDominoInfo, ts, DominosXF<SimpleDominoInfo>> _playerHandXF;
        private readonly ScoreBoardXF _score;
        public ItalianDominosMainView(IEventAggregator aggregator,
            TestOptions test,
            ItalianDominosVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);
            StackLayout mainStack = new StackLayout();

            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(ItalianDominosMainViewModel.RestoreScreen));
            }


            _bone = new BoneYardXF<SimpleDominoInfo, ts, DominosXF<SimpleDominoInfo>, DominosBasicShuffler<SimpleDominoInfo>>();
            _playerHandXF = new BaseHandXF<SimpleDominoInfo, ts, DominosXF<SimpleDominoInfo>>();
            _score = new ScoreBoardXF();
            _bone.HeightRequest = 130;
            _bone.WidthRequest = 600;
            mainStack.Children.Add(_bone);
            mainStack.Children.Add(_playerHandXF);

            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(ItalianDominosMainViewModel.NormalTurn));
            firstInfo.AddRow("Up To", nameof(ItalianDominosMainViewModel.UpTo));
            firstInfo.AddRow("Next #", nameof(ItalianDominosMainViewModel.NextNumber));
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            var thisBut = GetGamingButton("Play", nameof(ItalianDominosMainViewModel.PlayAsync));
            otherStack.Children.Add(thisBut);
            _score.AddColumn("Total Score", true, nameof(ItalianDominosPlayerItem.TotalScore), rightMargin: 10);
            _score.AddColumn("Dominos Left", true, nameof(ItalianDominosPlayerItem.ObjectCount), rightMargin: 10); // if not important, can just comment
            _score.AddColumn("Drew Yet", true, nameof(ItalianDominosPlayerItem.DrewYet), useTrueFalse: true);
            otherStack.Children.Add(firstInfo.GetContent);
            otherStack.Children.Add(_score);
            mainStack.Children.Add(otherStack); // this may be missing as well.

            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            ItalianDominosSaveInfo save = cons!.Resolve<ItalianDominosSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandXF!.LoadList(_model.PlayerHand1, ts.TagUsed);
            _model.BoneYard.MaxSize = new SKSize(550, 250);
            _model.BoneYard.ScatterPieces();
            _bone!.LoadList(_model.BoneYard, ts.TagUsed);

            return this.RefreshBindingsAsync(_aggregator);
        }


        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
        }
        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            return Task.CompletedTask;
        }
    }
}
