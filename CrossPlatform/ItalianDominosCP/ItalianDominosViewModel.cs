using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Dominos;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
namespace ItalianDominosCP
{
    public class ItalianDominosViewModel : DominoGamesVM<SimpleDominoInfo, ItalianDominosPlayerItem, ItalianDominosMainGameClass>
    {
        private int _NextNumber;
        public int NextNumber
        {
            get { return _NextNumber; }
            set
            {
                if (SetProperty(ref _NextNumber, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private int _UpTo;
        public int UpTo
        {
            get { return _UpTo; }
            set
            {
                if (SetProperty(ref _UpTo, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public ItalianDominosViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableBoneYard()
        {
            return !MainGame!.SingleInfo!.DrewYet;
        }
        public BasicGameCommand? PlayCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit();
            PlayerHand1!.Maximum = 8; //i think.
            PlayerHand1.AutoSelect = HandViewModel<SimpleDominoInfo>.EnumAutoType.SelectOneOnly;
            PlayCommand = new BasicGameCommand(this, async items =>
            {
                int Deck = PlayerHand1.ObjectSelected();
                if (Deck == 0)
                {
                    await ShowGameMessageAsync("You must choose one domino to play");
                    return;
                }
                await MainGame!.PlayDominoAsync(Deck);

            }, Items => true, this, CommandContainer!);
        }
    }
}