using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.Extensions;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace GolfCardGameCP
{
    public class GolfHand : GameBoardViewModel<RegularSimpleCard>
    {
        public void ChangeCard(int selected, RegularSimpleCard thisCard)
        {
            var tempList = ObjectList.ToRegularDeckDict();
            if (selected == 1)
            {
                tempList.RemoveLastItem();
                tempList.Add(thisCard);
            }
            else
            {
                tempList.RemoveFirstItem();
                tempList.InsertBeginning(thisCard);
            }
            ObjectList.ReplaceRange(tempList);
        }
        public void ClearBoard()
        {
            _mainGame.SingleInfo = _mainGame.PlayerList!.GetSelf();
            if (_mainGame.SingleInfo.MainHandList.Count != 2)
                throw new BasicBlankException("There has to be just 2 cards here");
            _mainGame.SingleInfo.MainHandList.ForEach(thisCard =>
            {
                thisCard.IsSelected = false;
                thisCard.IsUnknown = false;
            });
            ObjectList.ReplaceRange(_mainGame.SingleInfo.MainHandList);
            Visible = true;
        }
        private readonly GolfCardGameMainGameClass _mainGame;
        public GolfHand(IBasicGameVM thisMod) : base(thisMod)
        {
            IsEnabled = false;
            Text = "Your Hand";
            Rows = 1;
            Columns = 2;
            HasFrame = true;
            _mainGame = thisMod.MainContainer!.Resolve<GolfCardGameMainGameClass>();
            Visible = false; //i think.  if i am wrong, rethink.
        }
        protected override async Task ClickProcessAsync(RegularSimpleCard ThisObject)
        {
            int Index = ObjectList.IndexOf(ThisObject);
            await _mainGame.ChangeHandAsync(Index);
        }
    }
}