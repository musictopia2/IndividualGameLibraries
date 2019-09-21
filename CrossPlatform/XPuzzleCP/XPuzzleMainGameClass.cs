using BasicGameFramework.Attributes;
using BasicGameFramework.CommonInterfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace XPuzzleCP
{
    [SingletonGame]
    public class XPuzzleMainGameClass
    {
        internal XPuzzleViewModel ThisMod;
        private readonly ISaveSinglePlayerClass _thisState;
        internal XPuzzleSaveInfo SaveRoot;
        public XPuzzleMainGameClass(XPuzzleViewModel thisMod, ISaveSinglePlayerClass thisState)
        {
            ThisMod = thisMod;
            _thisState = thisState;
            SaveRoot = new XPuzzleSaveInfo();
        }

        private bool _opened;
        internal bool GameGoing;
        public async Task NewGameAsync()
        {
            GameGoing = true;
            if (_opened == false)
            {
                _opened = true;
                if (await _thisState.CanOpenSavedSinglePlayerGameAsync())
                {
                    await RestoreGameAsync();
                    return;
                }
            }
        }
        private async Task RestoreGameAsync()
        {
            SaveRoot = await _thisState.RetrieveSinglePlayerGameAsync<XPuzzleSaveInfo>();

        }
        public async Task ShowWinAsync()
        {
            GameGoing = false;
            await ThisMod.ShowGameMessageAsync("You Win");
            ThisMod.NewGameVisible = true;
            await _thisState.DeleteSinglePlayerGameAsync();
        }
    }
}