using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using CommonBasicStandardLibraries.CollectionClasses;
using FluxxCP.Cards;
using FluxxCP.Containers;
using System.Threading.Tasks;

namespace FluxxCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class FinalRuleProcesses : IFinalRuleProcesses
    {
        private readonly FluxxGameContainer _gameContainer;
        private readonly IAnalyzeProcesses _processes;

        public FinalRuleProcesses(FluxxGameContainer gameContainer, IAnalyzeProcesses processes)
        {
            _gameContainer = gameContainer;
            _processes = processes;
        }
        async Task IFinalRuleProcesses.SimplifyRulesAsync(CustomBasicList<int> thisList)
        {
            if (_gameContainer.SingleInfo!.CanSendMessage(_gameContainer.BasicData!))
            {
                await _gameContainer.Network!.SendAllAsync("simplifyrules", thisList);
            }
            DeckRegularDict<RuleCard> newList = new DeckRegularDict<RuleCard>();
            thisList.ForEach(index =>
            {
                newList.Add(_gameContainer.SaveRoot!.RuleList[index + 1]);
            });
            await newList.ForEachAsync(async thisRule =>
            {
                await _gameContainer.DiscardRuleAsync(thisRule);
            });
            if (thisList.Count > 0)
                _gameContainer.RefreshRules();
            _gameContainer!.CurrentAction = null;
            await _processes.AnalyzeQueAsync();
        }

        async Task IFinalRuleProcesses.TrashNewRuleAsync(int index)
        {
            if (_gameContainer.SingleInfo!.CanSendMessage(_gameContainer.BasicData!))
                await _gameContainer.Network!.SendAllAsync("trashnewrule", index);
            var thisRule = _gameContainer.SaveRoot!.RuleList[index + 1];
            await _gameContainer.DiscardRuleAsync(thisRule);
            _gameContainer.RefreshRules();
            _gameContainer!.CurrentAction = null;
            await _processes.AnalyzeQueAsync();
        }
    }
}