using BasicGameFramework.GameBoardCollections;
using BasicGameFramework.MiscProcesses;
using CommonBasicStandardLibraries.MVVMHelpers;
namespace BingoCP
{
    public class BingoItem : ObservableObject, IBasicSpace
    {
        public Vector Vector { get; set; }


        private int _WhatValue;

        public int WhatValue
        {
            get { return _WhatValue; }
            set
            {
                if (SetProperty(ref _WhatValue, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private string _Letter = "";

        public string Letter
        {
            get { return _Letter; }
            set
            {
                if (SetProperty(ref _Letter, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private bool _DidGet;

        public bool DidGet
        {
            get { return _DidGet; }
            set
            {
                if (SetProperty(ref _DidGet, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        public void ClearSpace()
        {
            DidGet = false;
        }

        public bool IsFilled()
        {
            return DidGet;
        }
    }
}