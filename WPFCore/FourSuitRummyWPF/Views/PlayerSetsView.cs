using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using FourSuitRummyCP.Data;
using FourSuitRummyCP.ViewModels;
using System.Threading.Tasks;
using System.Windows.Controls;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace FourSuitRummyWPF.Views
{
    public class PlayerSetsView : UserControl, IUIView
    {
        private readonly FourSuitRummyGameContainer _gameContainer;
        private readonly MainRummySetsWPF<EnumSuitList, EnumColorList,
                RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>,
                SetInfo, SavedSet> _main;
        public PlayerSetsView(FourSuitRummyGameContainer gameContainer)
        {
            _main = new MainRummySetsWPF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>, SetInfo, SavedSet>();
            _main.Height = 700;
            _main.Width = 400;
            Content = _main;
            _gameContainer = gameContainer;
        }

        Task IUIView.TryActivateAsync()
        {
            FourSuitRummyPlayerItem player;
            if (Name == nameof(FourSuitRummyMainViewModel.YourSetsScreen))
            {
                //yours.
                player = _gameContainer.PlayerList!.GetSelf();
            }
            else if (Name == nameof(FourSuitRummyMainViewModel.OpponentSetsScreen))
            {
                //opponents.
                player = _gameContainer.PlayerList!.GetOnlyOpponent();
            }
            else
            {
                throw new BasicBlankException("Unable to figure out player for loading sets screens.  Rethink");
            }
            _main.Init(player.MainSets!, ts.TagUsed);
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            return Task.CompletedTask;
        }
    }
}
