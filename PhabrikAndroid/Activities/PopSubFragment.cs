using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Phabrik.AndroidApp
{
    public class PopSubFragment : Android.Support.V4.App.Fragment
    {
        public PointOfPresenceFragment parent { get; set; }
        
    }
}