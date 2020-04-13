using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommonInterfaces;
using XPuzzleCP.Data;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.BasicEventModels;

namespace XPuzzleCP.Logic
{
    [InstanceGame]
    //it may be best to do as instance for now on.
    //well see how this goes (?)

    //since this is brand new, may require lots of rethinking on the templates.

    public class XPuzzleMainGameClass : IAggregatorContainer
    {
        //internal XPuzzleViewModel ThisMod;
        private readonly ISaveSinglePlayerClass _thisState;
        internal XPuzzleSaveInfo _saveRoot;
        public XPuzzleMainGameClass(ISaveSinglePlayerClass thisState, IEventAggregator aggregator)
        {
            _thisState = thisState;
            Aggregator = aggregator;
            _saveRoot = new XPuzzleSaveInfo();
        }

        private bool _opened;
        internal bool _gameGoing;

        public IEventAggregator Aggregator { get; }

        public async Task NewGameAsync()
        {
            _gameGoing = true;
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
            _saveRoot = await _thisState.RetrieveSinglePlayerGameAsync<XPuzzleSaveInfo>();

        }
        public async Task ShowWinAsync()
        {
            _gameGoing = false;
            await UIPlatform.ShowMessageAsync("You Win");
            //ThisMod.NewGameVisible = true;
            await _thisState.DeleteSinglePlayerGameAsync();
            //send message to show win.
            await this.SendGameOverAsync();

        }
    }
}
