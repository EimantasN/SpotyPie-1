﻿using Android.App;
using Android.Support.V7.Widget;
using Android.Widget;
using Mobile_Api;
using Mobile_Api.Models;
using Mobile_Api.Models.Enums;
using SpotyPie.Base;
using SpotyPie.RecycleView;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpotyPie
{
    public class Browse : FragmentBase
    {
        //main_rv
        private RvList<Songs> RvData { get; set; }

        public override int LayoutId { get; set; } = Resource.Layout.browse_layout;

        protected override void InitView()
        {
            if (RvData == null)
            {
                var rvBase = new BaseRecycleView<Songs>(this, Resource.Id.main_rv);
                RvData = rvBase.Setup(RecycleView.Enums.LayoutManagers.Linear_vertical);
                rvBase.DisableScroolNested();
            }
            Task.Run(() => PopulateData());
        }

        public async Task PopulateData()
        {
            await Task.Run(() => LoadSongs());
        }

        public async Task LoadSongs()
        {
            try
            {
                List<Songs> data;
                RvData.AddList(new List<Songs>() { null });
                //data.AddRange(await GetService().GetRecent<Songs>());
                //RvData.AddList(data);

                //data.AddRange(await GetService().GetPopular<Songs>());
                //RvData.AddList(data);
            }
            catch (Exception e)
            {
                Application.SynchronizationContext.Post(_ =>
                {
                    Toast.MakeText(this.Context, e.Message, ToastLength.Long).Show();
                }, null);
            }
        }

        public override void ForceUpdate()
        {
            Task.Run(() => PopulateData());
        }
    }
}