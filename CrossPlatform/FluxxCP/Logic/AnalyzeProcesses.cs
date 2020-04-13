using BasicGameFrameworkLibrary.Attributes;
using CommonBasicStandardLibraries.Exceptions;
using FluxxCP.Cards;
using FluxxCP.Containers;
using FluxxCP.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FluxxCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class AnalyzeProcesses : IAnalyzeProcesses
    {
        private readonly FluxxGameContainer _gameContainer;

        public AnalyzeProcesses(FluxxGameContainer gameContainer)
        {
            _gameContainer = gameContainer;
        }
        async Task IAnalyzeProcesses.AnalyzeQueAsync()
        {
            if (_gameContainer.AnimatePlayAsync == null)
            {
                throw new BasicBlankException("Nobody is animating plays.  Rethink");
            }
            if (_gameContainer!.QuePlayList.Count == 0 || _gameContainer.TempActionHandList.Count > 0 || _gameContainer.CurrentAction != null)
            {
                if (_gameContainer.TempActionHandList.Count == 0 && _gameContainer.CurrentAction == null)
                {
                    await _gameContainer.SaveRoot!.SavedActionData.TempDiscardList.ForEachAsync(async thisDiscard =>
                    {
                        var thisCard = _gameContainer.DeckList!.GetSpecificItem(thisDiscard);
                        await _gameContainer.AnimatePlayAsync(thisCard);
                    });
                    _gameContainer.SaveRoot.SavedActionData.TempDiscardList.Clear();
                }
                _gameContainer.SaveRoot!.DoAnalyze = false;
                await _gameContainer.ContinueTurnAsync!.Invoke();
                return;
            }
            AnalyzeRules();
            if (_gameContainer.LeftToDraw > 0)
            {
                await _gameContainer.DrawAsync!.Invoke();
                return;
            }
            var tempList = _gameContainer.PlayerList!.AllPlayersExceptForCurrent();
            if (tempList.Any(items => items.ObeyedRulesWhenNotYourTurn() == false))
            {
                _gameContainer.SaveRoot!.DoAnalyze = false; //try false since somebody has rules to obey.
                await _gameContainer.ContinueTurnAsync!.Invoke();
                return;
            }
            _gameContainer.OtherTurn = 0;
            _gameContainer.SingleInfo = _gameContainer.PlayerList.GetWhoPlayer();
            if (_gameContainer.NeedsToRemoveGoal())
            {
                _gameContainer.SaveRoot!.DoAnalyze = false;
                await _gameContainer.ContinueTurnAsync!.Invoke();
                return;
            }
            int wins = _gameContainer.WhoWonGame();
            if (wins > 0)
            {
                _gameContainer.SingleInfo = _gameContainer.PlayerList[wins];
                await _gameContainer.ShowWinAsync!.Invoke();
                return;
            }
            _gameContainer.SaveRoot!.DoAnalyze = true;
            if (_gameContainer.SaveStateAsync == null)
            {
                throw new BasicBlankException("Nobody is saving state.  Rethink");
            }
            await _gameContainer.SaveStateAsync();
            if (_gameContainer.PlayCardAsync == null)
            {
                throw new BasicBlankException("Nobody is playing card.  Rethink");
            }
            await _gameContainer.PlayCardAsync(_gameContainer.QuePlayList.First().Deck); //possible warning.
        }

        private void AnalyzeRules()
        {
            int extraAmount = _gameContainer!.IncreaseAmount();
            int possibleBonus = extraAmount + 1;
            if (_gameContainer.SaveRoot!.RuleList.Any(items => items.Deck == EnumRuleText.PoorBonus))
            {
                if (_gameContainer.SaveRoot.DrawBonus < possibleBonus && _gameContainer.HasFewestKeepers())
                    _gameContainer.SaveRoot.DrawBonus = possibleBonus;
            }
            if (_gameContainer.SaveRoot.RuleList.Any(items => items.Deck == EnumRuleText.RichBonus))
            {
                if (_gameContainer.SaveRoot.PlayBonus < possibleBonus && _gameContainer.HasMostKeepers())
                    _gameContainer.SaveRoot.PlayBonus = possibleBonus;
            }
            RuleCard thisRule;
            thisRule = _gameContainer.SaveRoot.RuleList.Where(items => items.Category == EnumRuleCategory.Draw).SingleOrDefault();
            if (thisRule == null)
            {
                _gameContainer.SaveRoot.DrawRules = possibleBonus;
            }
            else
            {
                if (thisRule.HowMany == 0)
                    throw new BasicBlankException("Cannot be 0");
                _gameContainer.SaveRoot.DrawRules = extraAmount + thisRule.HowMany;
            }
            thisRule = _gameContainer.SaveRoot.RuleList.Where(items => items.Category == EnumRuleCategory.Play).SingleOrDefault();
            if (thisRule == null)
            {
                _gameContainer.SaveRoot.PlayLimit = possibleBonus;
            }
            else if (thisRule.HowMany == -1)
            {
                _gameContainer.SaveRoot.PlayLimit = -1; //means unlimited.
            }
            else
            {
                if (thisRule.HowMany == 0)
                    throw new BasicBlankException("Cannot be 0");
                _gameContainer.SaveRoot.PlayLimit = extraAmount + thisRule.HowMany;
            }
            thisRule = _gameContainer.SaveRoot.RuleList.Where(items => items.Category == EnumRuleCategory.Hand).SingleOrDefault();
            if (thisRule == null)
            {
                _gameContainer.SaveRoot.HandLimit = -1; //means unlimited
            }
            else
            {
                _gameContainer.SaveRoot.HandLimit = extraAmount + thisRule.HowMany;
            }
            thisRule = _gameContainer.SaveRoot.RuleList.Where(items => items.Category == EnumRuleCategory.Keeper).SingleOrDefault();
            if (thisRule == null)
            {
                _gameContainer.SaveRoot.KeeperLimit = -1; //means unlimited
            }
            else
            {
                _gameContainer.SaveRoot.KeeperLimit = extraAmount + thisRule.HowMany;
            }
            if (_gameContainer.SaveRoot.PlayLimit == -1)
                _gameContainer.SaveRoot.PlaysLeft = _gameContainer.SingleInfo!.MainHandList.Count;
            else
                _gameContainer.SaveRoot.PlaysLeft = _gameContainer.SaveRoot.PlayLimit + _gameContainer.SaveRoot.PlayBonus - _gameContainer.SaveRoot.CardsPlayed;
            if (_gameContainer.SaveRoot.PlaysLeft < 0)
                _gameContainer.SaveRoot.PlaysLeft = 0;
            if (_gameContainer.SaveRoot.PlaysLeft > _gameContainer.SingleInfo!.MainHandList.Count)
                _gameContainer.SaveRoot.PlaysLeft = _gameContainer.SingleInfo.MainHandList.Count;
            _gameContainer.LeftToDraw = ExtraCardsToDraw();
            if (_gameContainer.LeftToDraw > 0)
                _gameContainer.SaveRoot.CardsDrawn += _gameContainer.LeftToDraw;
        }
        private int ExtraCardsToDraw()
        {
            int howMany = _gameContainer.SaveRoot!.PreviousBonus + _gameContainer.SaveRoot.DrawRules + _gameContainer.SaveRoot.DrawBonus - _gameContainer.SaveRoot.CardsDrawn;
            if (howMany <= 0)
                return 0;
            return howMany;
        }
        void IAnalyzeProcesses.AnalyzeRules()
        {
            AnalyzeRules();
        }
    }
}
