﻿using Android.Support.V7.Widget;
using Android.Views;
using System;

namespace SpotyPie.Helpers
{
    public static class RV_click
    {
        public static void SetItemClickListener(this RecyclerView rv, Action<RecyclerView, int, View> action)
        {
            rv.AddOnChildAttachStateChangeListener(new AttachStateChangeListener(rv, action));
        }
    }

    public class AttachStateChangeListener : Java.Lang.Object, RecyclerView.IOnChildAttachStateChangeListener
    {
        private RecyclerView mRecyclerview;
        private Action<RecyclerView, int, View> mAction;

        public AttachStateChangeListener(RecyclerView rv, Action<RecyclerView, int, View> action) : base()
        {
            mRecyclerview = rv;
            mAction = action;
        }

        public void OnChildViewAttachedToWindow(View view)
        {
            view.Click += View_Click;
        }

        private void View_Click(object sender, EventArgs e)
        {
            mAction.Invoke(mRecyclerview, mRecyclerview.GetChildViewHolder(((View)sender)).AdapterPosition, ((View)sender));
        }

        public void OnChildViewDetachedFromWindow(View view)
        {
            view.Click -= View_Click;
        }
    }
}