using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace RageCardGameCP
{
    [SingletonGame]
    public class SpecificTrickAreaViewModel : SeveralPlayersTrickViewModel<EnumColor, RageCardGameCardInformation, RageCardGamePlayerItem, RageCardGameSaveInfo>
    {

        private readonly RageCardGameMainGameClass _mainGame;
        public SpecificTrickAreaViewModel(IBasicGameVM thisMod) : base(thisMod)
        {
            _mainGame = thisMod.MainContainer!.Resolve<RageCardGameMainGameClass>();
        }
        protected override void PopulateNewCard(RageCardGameCardInformation oldCard, ref RageCardGameCardInformation newCard)
        {
            newCard.Color = oldCard.Color;
            newCard.Value = oldCard.Value;
        }
        public override void ClearBoard()
        {
            base.ClearBoard();
            ShowLeadColor();
        }
        private void ShowLeadColor()
        {
            var tempCard = OrderList.FirstOrDefault(items => items.Color != EnumColor.None);
            if (tempCard == null)
                _mainGame.ThisMod!.Lead = "None";
            else
                _mainGame.ThisMod!.Lead = tempCard.Color.ToString();
        }
        public override void LoadGame()
        {
            base.LoadGame();
            ShowLeadColor();
        }
        protected override async Task AfterPlayCardAsync(RageCardGameCardInformation thisCard)
        {
            if (thisCard.SpecialType != EnumSpecialType.None)
            {
                if (_mainGame.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self && _mainGame.ThisTest!.NoAnimations == false)
                    await _mainGame.Delay!.DelaySeconds(.75);
            }
            if (thisCard.SpecialType == EnumSpecialType.Wild || thisCard.SpecialType == EnumSpecialType.Change)
            {
                await _mainGame.ChooseColorAsync();
                return;
            }
            if (thisCard.SpecialType == EnumSpecialType.Out)
                _mainGame.SaveRoot!.TrumpSuit = EnumColor.None;
            ShowLeadColor();
            await base.AfterPlayCardAsync(thisCard);
        }
        public async Task ColorChosenAsync(EnumColor thisColor)
        {
            var thisCard = OrderList.Last();
            if (thisCard.SpecialType != EnumSpecialType.Wild && thisCard.SpecialType != EnumSpecialType.Change)
                throw new BasicBlankException("Only change or wilds can change colors");
            if (thisColor == EnumColor.None)
                throw new BasicBlankException("Color chosen can't be none");
            _mainGame.ThisMod!.ColorVM!.Visible = false;
            _mainGame.SaveRoot!.Status = EnumStatus.Regular;
            Visible = true; //now set this to visible.
            if (_mainGame.ThisTest!.NoAnimations == false)
                await _mainGame.Delay!.DelaySeconds(.25);
            if (thisCard.SpecialType == EnumSpecialType.Change)
            {
                if (thisColor == _mainGame.SaveRoot.TrumpSuit)
                    throw new BasicBlankException("Must choose a different suit for trump");
                _mainGame.SaveRoot.TrumpSuit = thisColor;
                await base.AfterPlayCardAsync(thisCard);
                return;
            }
            thisCard.Color = thisColor;
            thisCard.Value = 16; //show the highest of the suit.
            await base.AfterPlayCardAsync(thisCard);
        }
        public void LoadColorLists()
        {
            var thisCard = OrderList.Last();
            if (thisCard.SpecialType != EnumSpecialType.Wild && thisCard.SpecialType != EnumSpecialType.Change)
                throw new BasicBlankException("Only change or wilds can change colors");
            if (_mainGame.SaveRoot!.Status != EnumStatus.ChooseColor)
                throw new BasicBlankException("Must be choosing color");
            if (thisCard.SpecialType == EnumSpecialType.Change)
            {
                _mainGame.ThisMod!.ColorVM!.LoadEntireListExcludeOne(_mainGame.SaveRoot.TrumpSuit);
            }
            else
                _mainGame.ThisMod!.ColorVM!.LoadEntireList();
            _mainGame.ThisMod.ColorVM.UnselectAll();
            Visible = false; //this is now not visible because you are loading the lists.
            _mainGame.ThisMod.ColorVisible = true;
            _mainGame.ThisMod.ColorChosen = EnumColor.None;
        }
    }
}