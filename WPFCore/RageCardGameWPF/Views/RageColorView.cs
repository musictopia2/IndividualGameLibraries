using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using BasicGamingUIWPFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.GameGraphics.GamePieces;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using RageCardGameCP.Cards;
using RageCardGameCP.Data;
using RageCardGameCP.ViewModels;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RageCardGameWPF.Views
{
    public class RageColorView : UserControl, IUIView
    {
        public RageColorView(RageCardGameVMData model, RageCardGameGameContainer gameContainer)
        {
            StackPanel stack = new StackPanel();
            BaseHandWPF<RageCardGameCardInformation, RageCardGameGraphicsCP, CardGraphicsWPF> hand = new BaseHandWPF<RageCardGameCardInformation, RageCardGameGraphicsCP, CardGraphicsWPF>();
            ScoreBoardWPF score = new ScoreBoardWPF();
            RageCardGameMainView.PopulateScores(score);
            SimpleLabelGrid details = new SimpleLabelGrid();
            details.AddRow("Trump", nameof(RageColorViewModel.TrumpSuit));
            details.AddRow("Lead", nameof(RageColorViewModel.Lead));
            EnumPickerWPF<CheckerChoiceCP<EnumColor>, CheckerChooserWPF<EnumColor>, EnumColor> piece = new EnumPickerWPF<CheckerChoiceCP<EnumColor>, CheckerChooserWPF<EnumColor>, EnumColor>();
            stack.Children.Add(piece);
            stack.Children.Add(hand);
            stack.Children.Add(details.GetContent);
            stack.Children.Add(score);
            Content = stack;
            score!.LoadLists(gameContainer.SaveRoot.PlayerList);
            hand!.LoadList(model.PlayerHand1!, "");
            piece.LoadLists(model.Color1);
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
