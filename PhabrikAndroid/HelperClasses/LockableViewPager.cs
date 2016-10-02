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
using Android.Support.V4.View;
using Android.Support.V4.App;

namespace Phabrik.AndroidApp
{
    [Register("com.eweware.phabrik.Phabrik.AndroidApp.LockedableViewpager")]
    public class LockableViewPager : ViewPager
    {
        public LockableViewPager(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        private void Initialize()
        {
        }

        public bool SwipeLocked { get; set; }

        public override bool OnTouchEvent(MotionEvent e)
        {
            return !SwipeLocked && base.OnTouchEvent(e);
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            return !SwipeLocked && base.OnInterceptTouchEvent(ev);
        }

        public override bool CanScrollHorizontally(int direction)
        {
            return !SwipeLocked && base.CanScrollHorizontally(direction);
        }
    }
}
