using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.Extensions;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace HuseHeartsCP
{
    public class HuseHeartsViewModel : TrickGamesVM<EnumSuitList, HuseHeartsCardInformation, HuseHeartsPlayerItem, HuseHeartsMainGameClass>
        , ITrickDummyHand<EnumSuitList, HuseHeartsCardInformation>
    {
        public HandViewModel<HuseHeartsCardInformation>? Dummy1;
        public HandViewModel<HuseHeartsCardInformation>? Blind1;

        private int _RoundNumber;

        public int RoundNumber
        {
            get { return _RoundNumber; }
            set
            {
                if (SetProperty(ref _RoundNumber, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public void ShowVisibleChange()
        {
            OnPropertyChanged(nameof(TrickVisible));
            OnPropertyChanged(nameof(MoonVisible));
            OnPropertyChanged(nameof(PassingVisible));
            GameStatusChange();
        }
        public void GameStatusChange()
        {
            if (MainGame!.SaveRoot!.GameStatus == EnumStatus.Passing)
                PlayerHand1!.AutoSelect = HandViewModel<HuseHeartsCardInformation>.EnumAutoType.SelectAsMany;
            else
                PlayerHand1!.AutoSelect = HandViewModel<HuseHeartsCardInformation>.EnumAutoType.SelectOneOnly;
        }
        public HuseHeartsViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
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
            await Task.CompletedTask;
        }
        public BasicGameCommand<EnumMoonOptions>? MoonOptionsCommand { get; set; }
        public BasicGameCommand? PassCardsCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.  most trick taking games does not reshuffle.
            MoonOptionsCommand = new BasicGameCommand<EnumMoonOptions>(this, async option =>
            {
                switch (option)
                {
                    case EnumMoonOptions.GiveSelfMinus:
                        if (ThisData!.MultiPlayer == true)
                            await ThisNet!.SendAllAsync("takepointsaway");
                        await MainGame!.GiveSelfMinusPointsAsync();
                        break;
                    case EnumMoonOptions.GiveEverybodyPlus:
                        if (ThisData!.MultiPlayer == true)
                            await ThisNet!.SendAllAsync("givepointseverybodyelse");
                        await MainGame!.GiveEverybodyElsePointsAsync();
                        break;
                    default:
                        throw new BasicBlankException("Not Supported");
                }
            }, Items => MoonVisible, this, CommandContainer!);
            PassCardsCommand = new BasicGameCommand(this, async items =>
            {
                int cardsSelected = PlayerHand1!.HowManySelectedObjects;
                if (cardsSelected != 3)
                {
                    await ShowGameMessageAsync("Must pass 3 cards");
                    return;
                }
                var tempList = PlayerHand1.ListSelectedObjects().GetDeckListFromObjectList();
                if (ThisData!.MultiPlayer == true)
                    await ThisNet!.SendAllAsync("passcards", tempList);
                await MainGame!.CardsPassedAsync(tempList);
            }, Items => PassingVisible, this, CommandContainer!);
            PopulateCustomControls(); //can take a risk (no guarantees)
        }
        internal void PopulateCustomControls()
        {
            Blind1 = new HandViewModel<HuseHeartsCardInformation>(this);
            Blind1.SendEnableProcesses(this, () => false); //you can't even enable this one.
            Blind1.Text = "Blind";
            Dummy1 = new HandViewModel<HuseHeartsCardInformation>(this);
            Dummy1.SendEnableProcesses(this, () =>
            {
                if (MainGame!.SaveRoot!.GameStatus != EnumStatus.Normal)
                    return false;
                return MainGame!.TrickArea1!.FromDummy;
            });
            Blind1.Maximum = 4;
            Dummy1.Visible = true;
            Dummy1.Text = "Dummy Hand";
            Dummy1.AutoSelect = HandViewModel<HuseHeartsCardInformation>.EnumAutoType.SelectOneOnly;
        }
        public bool TrickVisible => MainGame!.SaveRoot!.GameStatus == EnumStatus.Normal;
        public bool MoonVisible => MainGame!.SaveRoot!.GameStatus == EnumStatus.ShootMoon;
        public bool PassingVisible => MainGame!.SaveRoot!.GameStatus == EnumStatus.Passing;
        public override bool CanEnableAlways()
        {
            return true;
        }
        protected override bool AlwaysEnableHand()
        {
            return false;
        }
        protected override bool CanEnableHand()
        {
            if (MainGame!.SaveRoot!.GameStatus == EnumStatus.Passing)
                return true;
            if (MainGame.SaveRoot.GameStatus == EnumStatus.Normal)
            {
                if (MainGame!.TrickArea1!.FromDummy == true)
                    return false;
                return true;
            }
            return false;
        }
        public DeckObservableDict<HuseHeartsCardInformation> GetCurrentHandList()
        {
            if (MainGame!.TrickArea1!.FromDummy == true)
                return Dummy1!.HandList;
            else
                return MainGame!.SingleInfo!.MainHandList;
        }

        public int CardSelected()
        {
            if (MainGame!.TrickArea1!.FromDummy == true)
                return Dummy1!.ObjectSelected();
            else if (MainGame!.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
                throw new BasicBlankException("Only self can show card selected.  If I am wrong, rethink");
            return PlayerHand1!.ObjectSelected();
        }
        public void RemoveCard(int deck)
        {
            if (MainGame!.TrickArea1!.FromDummy == true)
                Dummy1!.HandList.RemoveObjectByDeck(deck);
            else
                MainGame.SingleInfo!.MainHandList.RemoveObjectByDeck(deck); //because computer player does this too.
        }
    }
}