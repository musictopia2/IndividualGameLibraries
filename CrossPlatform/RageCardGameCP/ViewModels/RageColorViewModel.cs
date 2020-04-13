using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using RageCardGameCP.Data;
using System.ComponentModel;
using System.Threading.Tasks;

namespace RageCardGameCP.ViewModels
{
    [InstanceGame]
    public class RageColorViewModel : Screen, IBlankGameVM
    {
        private readonly RageCardGameVMData _model;

        public RageColorViewModel(CommandContainer commandContainer, RageCardGameVMData model)
        {
            CommandContainer = commandContainer;
            _model = model;
            Lead = _model.Lead;
            TrumpSuit = _model.TrumpSuit;
            _model.PropertyChanged += Model_PropertyChanged;
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Lead))
            {
                Lead = _model.Lead;
            }
            if (e.PropertyName == nameof(TrumpSuit))
            {
                TrumpSuit = _model.TrumpSuit;
            }
        }

        protected override Task TryCloseAsync()
        {
            _model.PropertyChanged -= Model_PropertyChanged;
            return base.TryCloseAsync();
        }

        public CommandContainer CommandContainer { get; set; }

        private EnumColor _trumpSuit;

        public EnumColor TrumpSuit
        {
            get { return _trumpSuit; }
            set
            {
                if (SetProperty(ref _trumpSuit, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private string _lead = "";

        public string Lead
        {
            get { return _lead; }
            set
            {
                if (SetProperty(ref _lead, value))
                {
                    //can decide what to do when property changes
                }

            }
        }


    }
}
