﻿using System;
using Android.Content;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;

namespace SpotyPie.Player
{
    public class SpotyPieViewPager : ViewPager
    {
        private bool Enabled = false;

        public SpotyPieViewPager(Context context) : base(context)
        {
        }

        protected SpotyPieViewPager(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public SpotyPieViewPager(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public override void SetCurrentItem(int item, bool smoothScroll)
        {
            base.SetCurrentItem(item, smoothScroll);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            return this.Enabled && base.OnTouchEvent(e);
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            return this.Enabled && base.OnInterceptTouchEvent(ev);
        }

        public void Enable(bool enableStatus)
        {
            this.Enabled = enableStatus;
        }
    }
}