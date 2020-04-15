using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using BasicXFControlsAndPages.MVVMFramework.ViewLinkersPlusBinders;
using CommonBasicStandardLibraries.Exceptions;
using FourSuitRummyCP.Data;
using FourSuitRummyCP.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace FourSuitRummyXF.Views
{
    public class PlayerSetsView : CustomControlBase
    {
        private readonly FourSuitRummyGameContainer _gameContainer;
        private readonly MainRummySetsXF<EnumSuitList, EnumColorList,
                RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>,
                SetInfo, SavedSet> _main;
        public PlayerSetsView(FourSuitRummyGameContainer gameContainer)
        {

            _main = new MainRummySetsXF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>, SetInfo, SavedSet>();
            _main.WidthRequest = 600;

            Content = _main;
            _gameContainer = gameContainer;
        }
        protected override Task TryActivateAsync()
        {
            FourSuitRummyPlayerItem player;
            string name;
            name = this.GetName();
            if (name == "")
            {
                throw new BasicBlankException("Name was still blank.   Rethink");
            }
            if (name == nameof(FourSuitRummyMainViewModel.YourSetsScreen))
            {
                player = _gameContainer.PlayerList!.GetSelf();
            }
            else if (name == nameof(FourSuitRummyMainViewModel.OpponentSetsScreen) || name == "")
            {
                player = _gameContainer.PlayerList!.GetOnlyOpponent();
            }
            else
            {
                throw new BasicBlankException("Unable to figure out player for loading sets screens.  Rethink");
            }


            _main.Init(player.MainSets!, ts.TagUsed);
            return Task.CompletedTask;
        }
    }
}
