using BasicGameFrameworkLibrary.DrawableListsViewModels;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using CommonBasicStandardLibraries.Exceptions;
using GolfCardGameCP.Data;
using System.Threading.Tasks;

namespace GolfCardGameCP.CustomPiles
{
    public class GolfHand : GameBoardObservable<RegularSimpleCard>
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
            _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetSelf();
            if (_gameContainer.SingleInfo.MainHandList.Count != 2)
                throw new BasicBlankException("There has to be just 2 cards here");
            _gameContainer.SingleInfo.MainHandList.ForEach(thisCard =>
            {
                thisCard.IsSelected = false;
                thisCard.IsUnknown = false;
            });
            ObjectList.ReplaceRange(_gameContainer.SingleInfo.MainHandList);
            Visible = true;
        }
        private readonly GolfCardGameGameContainer _gameContainer;
        public GolfHand(GolfCardGameGameContainer gameContainer) : base(gameContainer.Command)
        {
            IsEnabled = false;
            Text = "Your Hand";
            Rows = 1;
            Columns = 2;
            HasFrame = true;
            _gameContainer = gameContainer;
            Visible = false; //i think.  if i am wrong, rethink.
        }
        protected override async Task ClickProcessAsync(RegularSimpleCard payLoad)
        {
            int index = ObjectList.IndexOf(payLoad);
            if (_gameContainer.ChangeHandAsync == null)
            {
                throw new BasicBlankException("Nobody is handling change hand async.  Rethink");
            }
            await _gameContainer.ChangeHandAsync.Invoke(index);
        }
    }
}