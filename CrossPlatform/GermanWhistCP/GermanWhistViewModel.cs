using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace GermanWhistCP
{
    public class GermanWhistViewModel : TrickGamesVM<EnumSuitList, GermanWhistCardInformation, GermanWhistPlayerItem, GermanWhistMainGameClass>
    {
        public GermanWhistViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableDeck()
        {
            return false; //otherwise, can't compile.
        }
        protected override bool CanEnablePile1()
        {
            return false; //otherwise, can't compile.
        }
        protected override async Task ProcessDiscardClickedAsync()
        {
            //if we have anything, will be here.
            await Task.CompletedTask;
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.  most trick taking games does not reshuffle.
            PlayerHand1!.Maximum = 13;
            Deck1!.DeckStyle = DeckViewModel<GermanWhistCardInformation>.EnumStyleType.AlwaysKnown;
        }
    }
}