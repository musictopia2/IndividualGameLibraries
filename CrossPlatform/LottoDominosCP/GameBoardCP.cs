using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Dominos;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.Extensions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace LottoDominosCP
{
    [SingletonGame]
    public class GameBoardCP : GameBoardViewModel<SimpleDominoInfo>
    {
        private readonly LottoDominosViewModel _thisMod;
        private readonly IAsyncDelayer _thisDelay;
        public GameBoardCP(LottoDominosViewModel thisMod) : base(thisMod)
        {
            _thisMod = thisMod;
            _thisDelay = thisMod.MainContainer!.Resolve<IAsyncDelayer>();
            Columns = 7;
            Rows = 4;
        }
        protected override async Task ClickProcessAsync(SimpleDominoInfo payLoad)
        {
            await _thisMod.DominoClickedAsync(payLoad);
        }
        public void ClearPieces()
        {
            _thisMod.DominosList!.ForEach(items =>
            {
                items.IsUnknown = true;
            });
            ObjectList.ReplaceRange(_thisMod.DominosList);
        }
        public DeckRegularDict<SimpleDominoInfo> GetVisibleList()
        {
            return ObjectList.Where(Items => Items.Visible).ToRegularDeckDict();
        }
        public async Task ShowDominoAsync(int deck)
        {
            SimpleDominoInfo thisDomino = ObjectList.GetSpecificItem(deck);
            thisDomino.IsUnknown = false;
            await _thisDelay.DelaySeconds(2);
            thisDomino.IsUnknown = true;
        }
        public void MakeInvisible(int deck)
        {
            SimpleDominoInfo thisDomino = ObjectList.GetSpecificItem(deck);
            thisDomino.Visible = false;
        }
    }
}