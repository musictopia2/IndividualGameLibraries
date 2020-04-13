using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using FluxxCP.Cards;
using FluxxCP.Containers;
using FluxxCP.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FluxxCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class PlayProcesses : IPlayProcesses
    {
        private readonly FluxxGameContainer _gameContainer;
        private readonly FluxxVMData _model;
        private readonly IAnalyzeProcesses _analyzeProcesses;
        private readonly IDiscardProcesses _discardProcesses;
        private readonly IEmptyTrashProcesses _emptyTrashProcesses;
        private readonly ActionContainer _actionContainer;
        private readonly IShowActionProcesses _showActionProcesses;

        //if discard or empty requires play, then rethink or overflow errors.
        public PlayProcesses(FluxxGameContainer gameContainer,
            FluxxVMData model,
            IAnalyzeProcesses analyzeProcesses,
            IDiscardProcesses discardProcesses,
            IEmptyTrashProcesses emptyTrashProcesses,
            ActionContainer actionContainer,
            IShowActionProcesses showActionProcesses
            )
        {
            _gameContainer = gameContainer;
            _model = model;
            _analyzeProcesses = analyzeProcesses;
            _discardProcesses = discardProcesses;
            _emptyTrashProcesses = emptyTrashProcesses;
            _actionContainer = actionContainer;
            _showActionProcesses = showActionProcesses;
            _gameContainer.PlayCardAsync = PlayCardAsync;

        }
        async Task IPlayProcesses.SendPlayAsync(int deck)
        {
            if (_gameContainer.SingleInfo!.CanSendMessage(_gameContainer.BasicData!))
            {
                await _gameContainer.Network!.SendAllAsync("playcard", deck);
            }
        }

        public async Task PlayCardAsync(int deck)
        {
            _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetWhoPlayer();
            await PlayCardAsync(_gameContainer.DeckList!.GetSpecificItem(deck));
        }


        public async Task PlayCardAsync(FluxxCardInformation card)
        {
            await _model!.ShowPlayCardAsync(card);
            card.IsSelected = false;
            card.Drew = false;
            bool doAgain = false;
            _gameContainer.SaveRoot!.DoAnalyze = false;
            if (_gameContainer!.EverybodyGetsOneList.Count > 0)
                throw new BasicBlankException("Everybody gets one was not finished.  That must be finished before playing another card");
            if (_gameContainer.TempActionHandList.Count > 0)
                throw new BasicBlankException("There are cards left from the temporary action list.  Must finish choosing the order to play the cards.  Then they will play in the order");
            if (_gameContainer.CurrentAction != null)
            {
                if (_gameContainer.CurrentAction.Deck == EnumActionMain.LetsDoThatAgain)
                {
                    doAgain = true;
                    _model.Pile1!.RemoveCardFromPile(card);
                }
            }
            _gameContainer.CurrentAction = null;
            if (_gameContainer.QuePlayList.Count == 0 && doAgain == false)
            {
                _gameContainer.SaveRoot.CardsPlayed++;
                _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetWhoPlayer();
                _gameContainer.RemoveFromHandOnly(card);
            }
            else if (doAgain == false)
            {
                _gameContainer.QuePlayList.RemoveFirstItem();
            }
            _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetWhoPlayer();
            FluxxCardInformation final = FluxxDetailClass.GetNewCard(card.Deck);
            final.Populate(card.Deck);
            switch (card.CardType)
            {
                case EnumCardType.Rule:
                    await PlayRuleAsync((RuleCard)final);
                    break;
                case EnumCardType.Keeper:
                    await PlayKeeperAsync((KeeperCard)final);
                    break;
                case EnumCardType.Goal:
                    await PlayGoalAsync((GoalCard)final);
                    break;
                case EnumCardType.Action:
                    await PlayActionAsync((ActionCard)final);
                    break;
                default:
                    throw new BasicBlankException("Can't figure out which one to play for card type");
            }
        }

        private async Task PlayRuleAsync(RuleCard thisCard)
        {
            if (thisCard.Deck == EnumRuleText.ReverseOrder)
            {
                if (_gameContainer.PlayerList.Count() == 2)
                {
                    _gameContainer.SaveRoot!.AnotherTurn = true;
                    await _gameContainer.AnimatePlayAsync!(thisCard);
                    await _analyzeProcesses.AnalyzeQueAsync();
                    return;
                }
            }
            if (thisCard.Category != EnumRuleCategory.Basic && thisCard.Category != EnumRuleCategory.None)
            {
                var thisCat = thisCard.Category;
                _gameContainer.SaveRoot!.RuleList.RemoveAllOnly(items => items.Category == thisCat);
            }
            _gameContainer.SaveRoot!.RuleList.Add(thisCard);
            _gameContainer.RefreshRules();
            if (_gameContainer!.QuePlayList.Count == 0)
            {
                _analyzeProcesses.AnalyzeRules();
                if (_gameContainer.LeftToDraw > 0)
                {
                    _gameContainer.SaveRoot.DoAnalyze = true;
                    await _gameContainer.DrawAsync!();
                    return;
                }
            }
            await _analyzeProcesses.AnalyzeQueAsync();
        }

        private async Task PlayActionAsync(ActionCard thisCard)
        {
            await _gameContainer.AnimatePlayAsync!(thisCard);
            if (thisCard.Deck == EnumActionMain.LetsSimplify || thisCard.Deck == EnumActionMain.TrashANewRule)
            {
                if (_gameContainer.SaveRoot!.RuleList.Count == 1)
                {
                    await _analyzeProcesses.AnalyzeQueAsync();
                    return;
                }
            }
            else if (thisCard.Deck == EnumActionMain.UseWhatYouTake || thisCard.Deck == EnumActionMain.Taxation)
            {
                var tempList = _gameContainer.PlayerList!.AllPlayersExceptForCurrent();
                if (!tempList.Any(items => items.MainHandList.Count > 0))
                {
                    await _analyzeProcesses.AnalyzeQueAsync();
                    return;
                }
            }
            else if (thisCard.Deck == EnumActionMain.LetsDoThatAgain)
            {
                var tempList = _model!.Pile1!.DiscardList();
                if (!tempList.Any(items => items.CanDoCardAgain() == true))
                {
                    await _analyzeProcesses.AnalyzeQueAsync();
                    return;
                }
            }
            else if (thisCard.Deck == EnumActionMain.TrashAKeeper)
            {
                if (!_gameContainer.PlayerList.Any(items => items.KeeperList.Count > 0))
                {
                    await _analyzeProcesses.AnalyzeQueAsync();
                    return;
                }
            }
            else if (thisCard.Deck == EnumActionMain.ExchangeKeepers)
            {
                if (_gameContainer.SingleInfo!.KeeperList.Count == 0)
                {
                    await _analyzeProcesses.AnalyzeQueAsync();
                    return;
                }
            }
            if (thisCard.Deck == EnumActionMain.ExchangeKeepers || thisCard.Deck == EnumActionMain.StealAKeeper)
            {
                var tempList = _gameContainer.PlayerList!.AllPlayersExceptForCurrent();
                if (!tempList.Any(items => items.KeeperList.Count > 0))
                {
                    await _analyzeProcesses.AnalyzeQueAsync();
                    return;
                }
            }
            _gameContainer!.CurrentAction = thisCard;
            if (_gameContainer.CurrentAction.Category != EnumActionScreen.None)
            {
                if (_gameContainer.CurrentAction.Category == EnumActionScreen.OtherPlayer)
                {
                    var tempList = _gameContainer.PlayerList!.AllPlayersExceptForCurrent();
                    if (tempList.Any(items => items.MainHandList.Count > 0))
                    {
                        do
                        {
                            _gameContainer.OtherTurn = await _gameContainer.PlayerList.CalculateOtherTurnAsync();
                            var tempPlayer = _gameContainer.PlayerList.GetOtherPlayer(); //i think.
                            if (tempPlayer.MainHandList.Count > 0)
                                break;
                            if (_gameContainer.OtherTurn == 0)
                                throw new BasicBlankException("There should have been at least one player with cards for taxation");
                        } while (true);
                        await _gameContainer.ContinueTurnAsync!(); //because of taxation
                        return;
                    }
                }
                if (_gameContainer.CurrentAction.Deck == EnumActionMain.EverybodyGets1 || _gameContainer.CurrentAction.Deck == EnumActionMain.Draw2AndUseEm || _gameContainer.CurrentAction.Deck == EnumActionMain.Draw3Play2OfThem)
                {
                    await DrawTemporaryCardsAsync();
                    return;
                }
                await _analyzeProcesses.AnalyzeQueAsync();
                return;
            }
            _gameContainer.CurrentAction = null;
            switch (thisCard.Deck)
            {
                case EnumActionMain.TakeAnotherTurn:
                    _gameContainer.SaveRoot!.AnotherTurn = true;
                    break;
                case EnumActionMain.ScrambleKeepers:
                    await ScrambleKeepersAsync();
                    break;
                case EnumActionMain.RulesReset:
                    await ResetRulesAsync();
                    break;
                case EnumActionMain.EmptyTheTrash:
                    await _emptyTrashProcesses.EmptyTrashAsync();
                    break;
                case EnumActionMain.DiscardDraw:
                    await DrawDiscardAsync();
                    break;
                case EnumActionMain.NoLimits:
                    await NoLimitsAsync();
                    break;
                case EnumActionMain.Jackpot:
                    _gameContainer.LeftToDraw = _gameContainer.IncreaseAmount() + 3;
                    await _gameContainer.DrawAsync!();
                    break;
                default:
                    break;
            }
            if (thisCard.Deck != EnumActionMain.EmptyTheTrash && thisCard.Deck != EnumActionMain.DiscardDraw && thisCard.Deck != EnumActionMain.Jackpot && thisCard.Deck != EnumActionMain.ScrambleKeepers)
                await _analyzeProcesses.AnalyzeQueAsync();
        }
        private async Task ResetRulesAsync()
        {
            if (_gameContainer.SaveRoot!.RuleList.Count == 0)
            {
                _gameContainer.SaveRoot.RuleList.Clear(); //try this way.
                _gameContainer.SaveRoot.RuleList.Add((RuleCard)_gameContainer.DeckList!.GetSpecificItem(1));
            }
            else
            {
                var tempList = _gameContainer.SaveRoot.RuleList.Where(items => items.Category != EnumRuleCategory.Basic).ToRegularDeckDict();
                await tempList.ForEachAsync(async thisCard =>
                {
                    await _gameContainer.DiscardRuleAsync(thisCard);
                    if (_gameContainer.Test!.NoAnimations == false)
                        await _gameContainer.Delay!.DelaySeconds(.2);
                });
            }
            _gameContainer.RefreshRules();
        }
        private async Task PlayGoalAsync(GoalCard thisCard)
        {
            bool isDouble = _gameContainer!.HasDoubleAgenda();
            if (_gameContainer.SaveRoot!.GoalList.Count == 3)
                throw new BasicBlankException("Needs to remove a goal before it can play a goal");
            if (_gameContainer.SaveRoot.GoalList.Count == 2 && isDouble == false)
                throw new BasicBlankException("Cannot play another goal because 2 goals already and not double agenda");
            if (isDouble == false && _gameContainer.SaveRoot.GoalList.Count == 1)
                await _discardProcesses.DiscardGoalAsync(_gameContainer.SaveRoot.GoalList.Single());
            _gameContainer.SaveRoot.GoalList.Add(thisCard);
            await _analyzeProcesses.AnalyzeQueAsync();
        }
        private async Task PlayKeeperAsync(KeeperCard thisCard)
        {
            if (_gameContainer.OtherTurn > 0)
                throw new BasicBlankException("Can't be otherturn for playing a keeper.  If its playing first card random; will set OtherTurn to 0 first");
            _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetWhoPlayer();
            _gameContainer.SingleInfo.KeeperList.Add(thisCard);
            await _analyzeProcesses.AnalyzeQueAsync(); //should not be a need to analyze keeper anymore because observable.
        }

        public async Task PlayRandomCardAsync(int deck, int player)
        {
            var tempPlayer = _gameContainer.PlayerList![player];
            var thisCard = tempPlayer.MainHandList.GetSpecificItem(deck);
            await PlayRandomCardAsync(thisCard, player);
        }

        public async Task PlayRandomCardAsync(FluxxCardInformation thisCard, int player)
        {
            _actionContainer.SetUpGoals(); //its setting up goals now.
            if (_gameContainer.OtherTurn > 0)
            {
                _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetOtherPlayer();
                if (_gameContainer.SingleInfo.PlayerCategory != EnumPlayerCategory.Self)
                    await _showActionProcesses.ChoseOtherCardSelectedAsync(thisCard.Deck);
                _gameContainer.SingleInfo = _gameContainer.PlayerList.GetWhoPlayer();
                _gameContainer.OtherTurn = 0;
                await PlayCardAsync(thisCard);
                return;
            }
            _gameContainer!.CurrentAction = null;
            var tempPlayer = _gameContainer.PlayerList![player];
            tempPlayer.MainHandList.RemoveObjectByDeck(thisCard.Deck);
            _gameContainer.SingleInfo = _gameContainer.PlayerList.GetWhoPlayer();
            _gameContainer.QuePlayList.Add(thisCard);
            await _analyzeProcesses.AnalyzeQueAsync();
        }

        async Task IPlayProcesses.PlayRandomCardAsync(int deck)
        {
            if (_gameContainer.SingleInfo!.CanSendMessage(_gameContainer.BasicData!))
                await _gameContainer.Network!.SendAllAsync("firstrandom", deck);
            _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetWhoPlayer();
            await PlayRandomCardAsync(deck, _gameContainer.WhoTurn);
        }

        async Task IPlayProcesses.PlayUseTakeAsync(int deck, int player)
        {
            if (_gameContainer.SingleInfo!.CanSendMessage(_gameContainer.BasicData!))
            {
                CustomBasicList<int> newList = new CustomBasicList<int> { deck, player };
                await _gameContainer.Network!.SendAllAsync("usetake", newList);
            }
            await PlayRandomCardAsync(deck, player);
        }
        //if i need discardrule by name, rethink.
        

        private async Task ScrambleKeepersAsync()
        {
            if (_gameContainer.PlayerList.Count(items => items.KeeperList.Count > 0) < 2)
            {
                await _analyzeProcesses.AnalyzeQueAsync();
                return;
            }
            if (_gameContainer.BasicData!.MultiPlayer == true && _gameContainer.SingleInfo!.CanSendMessage(_gameContainer.BasicData) == false)
            {
                _gameContainer.Check!.IsEnabled = true;
                return;
            }
            _gameContainer.PlayerList!.ScrambleKeepers();
            if (_gameContainer.BasicData.MultiPlayer)
            {
                var thisList = _gameContainer.PlayerList.Where(items => items.KeeperList.Count > 0).Select(temps => temps.KeeperList.Select(fins => (int)fins.Deck).ToCustomBasicList().ToCustomBasicList());
                await _gameContainer.Network!.SendAllAsync("scramblekeepers", thisList.ToCustomBasicList());
            }
            await _analyzeProcesses.AnalyzeQueAsync();
        }
       

        private async Task DrawDiscardAsync()
        {
            _gameContainer.LeftToDraw = _gameContainer.SingleInfo!.MainHandList.Count;
            var tempList = _gameContainer.SingleInfo.MainHandList.ToRegularDeckDict();
            await tempList.ForEachAsync(async thisCard => await _discardProcesses.DiscardFromHandAsync(thisCard));
            _gameContainer.AllNewCards = true;
            await _gameContainer.DrawAsync!();
        }

        private async Task DrawTemporaryCardsAsync()
        {
            _gameContainer.DoDrawTemporary = true;
            if (_gameContainer!.TempActionHandList.Count > 0)
                throw new BasicBlankException("There are already cards left to choose from.  Should have not Drawn Temporary Cards");
            int extras = _gameContainer.IncreaseAmount();
            switch (_gameContainer.CurrentAction!.Deck)
            {
                case EnumActionMain.EverybodyGets1:
                    if (extras == 0)
                        _gameContainer.LeftToDraw = _gameContainer.PlayerList.Count();
                    else
                        _gameContainer.LeftToDraw = _gameContainer.PlayerList.Count() * 2;
                    break;
                case EnumActionMain.Draw2AndUseEm:
                    _gameContainer.LeftToDraw = 2 + extras;
                    break;
                case EnumActionMain.Draw3Play2OfThem:
                    _gameContainer.LeftToDraw = 3 + extras;
                    break;
                default:
                    break;
            }
            await _gameContainer.DrawAsync!();
        }
        private async Task NoLimitsAsync()
        {
            var tempList = _gameContainer.SaveRoot!.RuleList.Where(items => items.Category == EnumRuleCategory.Hand || items.Category == EnumRuleCategory.Keeper).ToCustomBasicList();
            await tempList.ForEachAsync(async thisRule =>
            {
                await _gameContainer.DiscardRuleAsync(thisRule);
                if (_gameContainer.Test!.NoAnimations == false)
                    await _gameContainer.Delay!.DelaySeconds(.1);
            });
            _gameContainer.RefreshRules();
        }
    }
}