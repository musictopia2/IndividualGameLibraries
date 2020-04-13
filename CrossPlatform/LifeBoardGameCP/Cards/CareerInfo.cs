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
using LifeBoardGameCP.Data;
//i think this is the most common things i like to do
namespace LifeBoardGameCP.Cards
{
    public class CareerInfo : LifeBaseCard
    {
        public CareerInfo()
        {
            CardCategory = EnumCardCategory.Career;
        }
        private EnumCareerType _career;
        public EnumCareerType Career
        {
            get { return _career; }
            set
            {
                if (SetProperty(ref _career, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private string _title = "";
        public string Title
        {
            get { return _title; }
            set
            {
                if (SetProperty(ref _title, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private EnumPayScale _scale1;
        public EnumPayScale Scale1
        {
            get { return _scale1; }
            set
            {
                if (SetProperty(ref _scale1, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private EnumPayScale _scale2;
        public EnumPayScale Scale2
        {
            get { return _scale2; }
            set
            {
                if (SetProperty(ref _scale2, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private bool _degreeRequired;
        public bool DegreeRequired
        {
            get { return _degreeRequired; }
            set
            {
                if (SetProperty(ref _degreeRequired, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private string _description = "";
        public string Description
        {
            get { return _description; }
            set
            {
                if (SetProperty(ref _description, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
    }
}
