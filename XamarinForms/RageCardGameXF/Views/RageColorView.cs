using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using BasicGamingUIXFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.GameGraphics.GamePieces;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using RageCardGameCP.Cards;
using RageCardGameCP.Data;
using RageCardGameCP.ViewModels;
using Xamarin.Forms;

namespace RageCardGameXF.Views
{
    public class RageColorView : CustomControlBase
    {
        public RageColorView(RageCardGameVMData model, RageCardGameGameContainer gameContainer)
        {
            StackLayout stack = new StackLayout();
            BaseHandXF<RageCardGameCardInformation, RageCardGameGraphicsCP, CardGraphicsXF> hand = new BaseHandXF<RageCardGameCardInformation, RageCardGameGraphicsCP, CardGraphicsXF>();
            ScoreBoardXF score = new ScoreBoardXF();
            RageCardGameMainView.PopulateScores(score);
            SimpleLabelGridXF details = new SimpleLabelGridXF();
            details.AddRow("Trump", nameof(RageColorViewModel.TrumpSuit));
            details.AddRow("Lead", nameof(RageColorViewModel.Lead));
            EnumPickerXF<CheckerChoiceCP<EnumColor>, CheckerChooserXF<EnumColor>, EnumColor> piece = new EnumPickerXF<CheckerChoiceCP<EnumColor>, CheckerChooserXF<EnumColor>, EnumColor>();
            stack.Children.Add(piece);
            stack.Children.Add(hand);
            stack.Children.Add(details.GetContent);
            stack.Children.Add(score);
            Content = stack;
            score!.LoadLists(gameContainer.SaveRoot.PlayerList);
            hand!.LoadList(model.PlayerHand1!, "");
            piece.LoadLists(model.Color1);
        }

    }
}
