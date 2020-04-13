using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using RookCP.Data;
using RookCP.Logic;
using System.Threading.Tasks;

namespace RookCP.ViewModels
{
    [InstanceGame]
    public class RookNestViewModel : Screen, IBlankGameVM, ISubmitText
    {
        private readonly RookVMData _model;
        private readonly INestProcesses _processes;
        public string Text => "Choose Nest Cards"; //to show same on both xamarin forms and wpf
        public RookNestViewModel(CommandContainer commandContainer,
            RookVMData model,
            INestProcesses processes
            )
        {
            CommandContainer = commandContainer;
            _model = model;
            _processes = processes;
        }
        public CommandContainer CommandContainer { get; set; }
        [Command(EnumCommandCategory.Plain)]
        public async Task NestAsync()
        {
            var thisList = _model.PlayerHand1!.ListSelectedObjects();
            if (thisList.Count != 5)
            {
                await UIPlatform.ShowMessageAsync("Sorry, you must choose 5 cards to throw away");
                return;
            }
            await _processes!.ProcessNestAsync(thisList);
        }

    }
}