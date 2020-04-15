using BasicXFControlsAndPages.MVVMFramework.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace FroggiesXF.Views
{
    public class FroggiesTestView : CustomControlBase
    {
        public FroggiesTestView()
        {
            TestControl test = new TestControl();

            Content = test;
        }
    }
}