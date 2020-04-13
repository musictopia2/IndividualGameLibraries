using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Dominos;
using BasicGameFrameworkLibrary.DrawableListsViewModels;
using BasicGameFrameworkLibrary.Extensions;
using CommonBasicStandardLibraries.Exceptions;
using LottoDominosCP.Data;
using System;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace LottoDominosCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class GameBoardCP : GameBoardObservable<SimpleDominoInfo>
    {
        private readonly IAsyncDelayer _delayer;
        private readonly LottoDominosVMData _model;

        public GameBoardCP(CommandContainer container,
            IAsyncDelayer delayer,
            LottoDominosVMData model
            ) : base(container)
        {
            Columns = 7;
            Rows = 4;
            _delayer = delayer;
            _model = model;
            Text = "Dominos"; //just do here since its inherited anyways.
            Visible = true; //hopefully no problem because if the view model is not shown, this won't show up anyways.
        }

        internal Func<int, Task>? MakeMoveAsync { get; set; }

        protected override Task ClickProcessAsync(SimpleDominoInfo domino)
        {
            //the iffy part is the click processes.
            if (MakeMoveAsync == null)
            {
                throw new BasicBlankException("Make move was never populated to run.  Rethink");
            }
            return MakeMoveAsync.Invoke(domino.Deck);
        }

        public void ClearPieces()
        {
            _model.DominosList!.ForEach(items =>
            {
                items.IsUnknown = true;
            });
            ObjectList.ReplaceRange(_model.DominosList);
        }
        public DeckRegularDict<SimpleDominoInfo> GetVisibleList()
        {
            return ObjectList.Where(Items => Items.Visible).ToRegularDeckDict();
        }
        public async Task ShowDominoAsync(int deck)
        {
            SimpleDominoInfo thisDomino = ObjectList.GetSpecificItem(deck);
            thisDomino.IsUnknown = false;
            await _delayer.DelaySeconds(2);
            thisDomino.IsUnknown = true;
        }
        public void MakeInvisible(int deck)
        {
            SimpleDominoInfo thisDomino = ObjectList.GetSpecificItem(deck);
            thisDomino.Visible = false;
        }
    }
}
