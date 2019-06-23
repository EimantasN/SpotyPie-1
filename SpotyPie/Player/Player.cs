﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Media;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Mobile_Api.Models;
using Realms;
using SpotyPie.Base;
using SpotyPie.Database.Helpers;
using SpotyPie.Enums;
using SpotyPie.Music;
using SpotyPie.Music.Helpers;
using SpotyPie.Player.Interfaces;
using SpotyPie.Services;
using SpotyPie.Services.Binders;
using SpotyPie.Services.Interfaces;
using Square.Picasso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotyPie.Player
{
    [Activity(Label = "SpotyPie", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Icon = "@drawable/logo_spotify", Theme = "@style/Theme.SpotyPie")]
    public class Player : ActivityBase, View.IOnTouchListener, Playback.ICallback, IServiceConnection
    {
        private bool IsBinded = false;
        public override NavigationColorState NavigationBtnColorState { get; set; } = NavigationColorState.Player;

        private int ViewLoadState = 0;

        private bool Bound { get; set; } = false;

        //private MusicService MusicService;

        private IServiceConnection ServiceConnection;

        private string CurrentPlayerImage { get; set; }

        protected float mLastPosY;
        protected static int newsId = 0;
        protected const int OffsetContainer = 250;
        protected int FragmentWidth = 0;

        public int CurrentState { get; set; } = 1;
        public override int LayoutId { get; set; } = Resource.Layout.player;
        public override LayoutScreenState ScreenState { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        TimeSpan CurrentTime = new TimeSpan(0, 0, 0, 0);
        TimeSpan TotalSongTime = new TimeSpan(0, 0, 0, 0);
        bool saved_to_songs = false;

        public ImageButton HidePlayerButton;
        public ImageButton PlayToggle;

        ImageButton NextSong;
        ImageButton PreviewSong;

        public TextView CurretSongTimeText;
        TextView TotalSongTimeText;

        //public ImageView ImgHolder;
        public TextView PlayerSongName;
        public TextView PlayerArtistName;
        public TextView PlayerPlaylistName;

        ImageButton SongListButton;

        SeekBar SongTimeSeekBar;

        ImageButton Repeat;

        ImageButton Shuffle;

        bool Shuffle_state = false;

        ImageView Save_to_songs;

        private bool SeekActive = false;

        ViewPager pager;

        private int LastPosition = 0;

        protected override void InitView()
        {
            ServiceConnection = this;

            pager = FindViewById<ViewPager>(Resource.Id.img_holder);
            ImageAdapter adapter = new ImageAdapter(this.ApplicationContext, this);
            pager.Adapter = adapter;
            pager.PageSelected += Pager_PageSelected;

            SongListButton = FindViewById<ImageButton>(Resource.Id.song_list);
            SongListButton.Click += SongListButton_Click;

            NextSong = FindViewById<ImageButton>(Resource.Id.next_song);
            NextSong.Click += NextSong_Click;
            PreviewSong = FindViewById<ImageButton>(Resource.Id.preview_song);
            PreviewSong.Click += PreviewSong_Click;

            Repeat = FindViewById<ImageButton>(Resource.Id.repeat);
            Repeat.Click += Repeat_Click;
            Shuffle = FindViewById<ImageButton>(Resource.Id.shuffle);
            Shuffle.Click += Shuffle_Click;
            Save_to_songs = FindViewById<ImageView>(Resource.Id.save_to_songs);
            Save_to_songs.Click += Save_to_songs_Click;

            //ImgHolder = FindViewById<ImageView>(Resource.Id.img_holder);
            PlayerSongName = FindViewById<TextView>(Resource.Id.song_name);
            PlayerSongName.Selected = true;
            PlayerArtistName = FindViewById<TextView>(Resource.Id.artist_name);
            PlayerArtistName.Selected = true;
            PlayerPlaylistName = FindViewById<TextView>(Resource.Id.playlist_name);

            CurretSongTimeText = FindViewById<TextView>(Resource.Id.current_song_time);
            CurretSongTimeText.Text = "00:00";
            TotalSongTimeText = FindViewById<TextView>(Resource.Id.total_song_time);
            TotalSongTimeText.Visibility = ViewStates.Invisible;

            HidePlayerButton = FindViewById<ImageButton>(Resource.Id.back_button);
            PlayToggle = FindViewById<ImageButton>(Resource.Id.play_stop);

            //if (GetState().IsPlaying)
            //{
            //    PlayToggle.SetImageResource(Resource.Drawable.pause);
            //    PlayToggle.Tag = Resource.Drawable.pause;
            //}
            //else
            //{
            //    PlayToggle.SetImageResource(Resource.Drawable.play_button);
            //    PlayToggle.Tag = Resource.Drawable.play_button;
            //}

            HidePlayerButton.Click += HidePlayerButton_Click;
            PlayToggle.Click += PlayToggle_Click;
            Repeat_Click(null, null);
            Shuffle_Click(null, null);

            FragmentWidth = Resources.DisplayMetrics.WidthPixels;

            SongTimeSeekBar = FindViewById<SeekBar>(Resource.Id.seekBar);

            SongTimeSeekBar.StartTrackingTouch += SongTimeSeekBar_StartTrackingTouch;
            SongTimeSeekBar.StopTrackingTouch += SongTimeSeekBar_StopTrackingTouch;
            SongTimeSeekBar.ProgressChanged += SongTimeSeekBar_ProgressChanged;
        }

        private void Pager_PageSelected(object sender, ViewPager.PageSelectedEventArgs e)
        {
            if (LastPosition == 0 && e.Position == 0)
            {
                Task.Run(() => RunOnUiThread(() => SendBroadcast(new Intent("com.spotypie.adnroid.musicservice.play"))));
                
            }
            else if (LastPosition < e.Position) //Moved foward
            {
                LastPosition = e.Position;
                Task.Run(() => RunOnUiThread(() => SendBroadcast(new Intent("com.spotypie.adnroid.musicservice.next"))));
                Task.Run(() => RunOnUiThread(() => SongLoadStarted()));
            }
            else if (LastPosition > e.Position)
            {
                LastPosition = e.Position;
                Task.Run(() => RunOnUiThread(() => SendBroadcast(new Intent("com.spotypie.adnroid.musicservice.prev"))));
                Task.Run(() => RunOnUiThread(() => SongLoadStarted()));

            }
            //Toast.MakeText(this.BaseContext, $"index {e.Position}", ToastLength.Short).Show();
            //pager.Adapter.NotifyDataSetChanged();
        }

        public void SongLoadStarted()
        {
            CurretSongTimeText.Text = "00:00";
            if (!SeekActive && SongTimeSeekBar != null)
                SongTimeSeekBar.Progress = 0;
            SongTimeSeekBar.Enabled = false;
            PlayToggle.SetImageResource(Resource.Drawable.play_loading);
            PlayToggle.Tag = Resource.Drawable.play_loading;
        }

        public void SkipToNext()
        {
            throw new NotImplementedException();
        }

        public void SkipToPrevious()
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        protected override void OnResume()
        {
            base.OnResume();
            //if (!IsMyServiceRunning(typeof(Music.MusicService)))
            //{
            //    this.Activity.StartService(new Intent(this.Activity, typeof(Music.MusicService)));
            //}

            //Intent intent = new Intent(this.Activity, typeof(MusicService));
            //Activity.BindService(intent, this.ServiceConnection, Bind.AutoCreate);

            //ImgHolder.SetOnTouchListener(this);

            BindService(new Intent(this, typeof(Music.MusicService)), ServiceConnection, Bind.AutoCreate);
        }

        protected override void OnDestroy()
        {
            //ImgHolder.SetOnTouchListener(null);

            if (IsBinded)
            {
                //GetActivity().UnbindService(ServiceConnection);
            }
            base.OnDestroy();
        }

        public override void LoadFragment(dynamic switcher, string jsonModel = null)
        {
            switch (switcher)
            {
                case Enums.Activitys.Player.CurrentSongList:
                    GetFManager().SetCurrentFragment(new PlayerSongList());
                    return;
            }
        }

        public override int GetParentView(bool Player = false)
        {
            return Resource.Id.parent_view;
        }

        #region Player events
        private void SongListButton_Click(object sender, EventArgs ee)
        {
            LoadFragmentInner(Enums.Activitys.Player.CurrentSongList, screen: Enums.LayoutScreenState.FullScreen);
        }

        public void NextSongPlayer()
        {
            Task.Run((Action)(() =>
            {
                RunOnUiThread(() =>
                {
                    //this.ImgHolder.TranslationX = FragmentWidth * -1;
                    //this.ImgHolder?.Animate().TranslationX(0);
                });
            }));
        }

        public void PrevSongPlayer()
        {
            Task.Run((Action)(() =>
            {
                RunOnUiThread(() =>
                {
                    //this.ImgHolder.TranslationX = FragmentWidth;
                    //this.ImgHolder?.Animate().TranslationX(0);
                });
            }));
        }

        private void HidePlayerButton_Click(object sender, EventArgs e)
        {
        }

        private void PreviewSong_Click(object sender, EventArgs e)
        {
            SongLoadStarted();
            SendBroadcast(new Intent("com.spotypie.adnroid.musicservice.prev"));
        }

        private void NextSong_Click(object sender, EventArgs e)
        {
            SongLoadStarted();
            SendBroadcast(new Intent("com.spotypie.adnroid.musicservice.next"));
        }

        private void PlayToggle_Click(object sender, EventArgs e)
        {
            int tag = (int)PlayToggle.Tag;
            if (tag != Resource.Drawable.play_loading)
                SongLoadStarted();
            else
                return;

            if (tag == Resource.Drawable.play_button)
            {
                SendBroadcast(new Intent("com.spotypie.adnroid.musicservice.play"));
            }
            else if (tag == Resource.Drawable.pause)
            {
                SendBroadcast(new Intent("com.spotypie.adnroid.musicservice.pause"));
            }
        }

        public void PlayerPrepared(int duration)
        {
            Task.Run(() =>
            {
                RunOnUiThread(() =>
                {
                    //TotalSongTime = new TimeSpan(0, 0, (int)MusicService.MusicPlayer.Duration / 1000);
                    //string totalTime = TotalSongTime.Minutes + ":" + (TotalSongTime.Seconds > 9 ? TotalSongTime.Seconds.ToString() : "0" + TotalSongTime.Seconds);
                    //TotalSongTimeText.Text = totalTime;
                    //TotalSongTimeText.Visibility = ViewStates.Visible;
                    //PlayToggle.SetImageResource(Resource.Drawable.pause);
                    //PlayToggle.Tag = Resource.Drawable.pause;
                });
            });
        }

        public void Music_play()
        {
            //Task.Run(async () =>
            //{
            //    while (MusicService?.MusicPlayer == null)
            //        await Task.Delay(250);

            //    Activity.RunOnUiThread(() =>
            //    {
            //        PlayToggle.SetImageResource(Resource.Drawable.play_loading);
            //        PlayToggle.Tag = Resource.Drawable.play_loading;
            //    });
            //});
        }

        public void Music_pause()
        {
            PlayToggle.SetImageResource(Resource.Drawable.play_button);
            PlayToggle.Tag = Resource.Drawable.play_button;
        }

        public void SetSeekBarProgress(int progress, string text)
        {
            Task.Run(() =>
            {
                RunOnUiThread(() =>
                {
                    CurretSongTimeText.Text = text;
                    if (!SeekActive && SongTimeSeekBar != null)
                        SongTimeSeekBar.Progress = progress;
                });
            });
        }

        public void SongEnded()
        {
            Task.Run(() =>
            {
                RunOnUiThread(() =>
                {
                    //CurretSongTimeText.Text = "00:00";
                    //SongTimeSeekBar.Progress = 0;
                });
            });
        }

        public void SongStopped()
        {
            Task.Run(() =>
            {
                RunOnUiThread(() =>
                {
                    PlayToggle.SetImageResource(Resource.Drawable.play_button);
                    PlayToggle.Tag = Resource.Drawable.play_button;
                });
            });
        }

        //This method must be call then song is setted to refresh main UI view
        public void SongLoadStarted(List<Songs> newSongList, int position)
        {
        }

        

        public void SongLoadEnded()
        {
            Task.Run(() =>
            {
                RunOnUiThread(() =>
                {
                    PlayToggle.SetImageResource(Resource.Drawable.pause);
                    PlayToggle.Tag = Resource.Drawable.pause;
                });
            });
        }

        private void SongTimeSeekBar_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
        }

        private void SongTimeSeekBar_StopTrackingTouch(object sender, SeekBar.StopTrackingTouchEventArgs e)
        {
            Intent intent = new Intent("com.spotypie.adnroid.musicservice.seek");
            intent.PutExtra("PLAYER_SEEK", e.SeekBar.Progress.ToString());
            SendBroadcast(intent);
            SeekActive = false;
        }

        private void SongTimeSeekBar_StartTrackingTouch(object sender, SeekBar.StartTrackingTouchEventArgs e)
        {
            SeekActive = true;
        }

        private void Repeat_Click(object sender, EventArgs e)
        {
            //if (MusicService == null) return;
            //switch (MusicService.Repeat_state)
            //{
            //    case 0:
            //        {
            //            Repeat.SetImageResource(Resource.Drawable.repeat);
            //            MusicService.Repeat_state = 1;
            //            if (MusicService != null) MusicService.MusicPlayer.Looping = true;
            //            break;
            //        }
            //    case 1:
            //        {
            //            Repeat.SetImageResource(Resource.Drawable.repeat_once);
            //            MusicService.Repeat_state = 2;
            //            break;
            //        }
            //    case 2:
            //        {
            //            Repeat.SetImageResource(Resource.Drawable.repeat_off);
            //            MusicService.Repeat_state = 0;
            //            break;
            //        }
            //}
        }

        private void Shuffle_Click(object sender, EventArgs e)
        {
            if (Shuffle_state)
                Shuffle.SetImageResource(Resource.Drawable.shuffle_disabled);
            else
                Shuffle.SetImageResource(Resource.Drawable.shuffle_variant);


            Shuffle_state = !Shuffle_state;
        }

        private void Save_to_songs_Click(object sender, EventArgs e)
        {
            if (saved_to_songs)
                Save_to_songs.SetImageResource(Resource.Drawable.check);
            else
                Save_to_songs.SetImageResource(Resource.Drawable.@checked);


            saved_to_songs = !saved_to_songs;
        }

        #endregion

        #region Lisiners

        public bool OnTouch(View v, MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    mLastPosY = e.GetX();
                    return true;

                case MotionEventActions.Up:
                    var metrics = Resources.DisplayMetrics;
                    var widthInDp = metrics.HeightPixels;

                    var transXExit = v.TranslationX;
                    if (v.TranslationX > 0)
                    {
                        if (transXExit < (FragmentWidth / 3))
                        {
                            v.Animate().TranslationX(0);
                            return true;
                        }
                        else
                        {
                            v.Animate().TranslationX(FragmentWidth);
                            PreviewSong_Click(null, null);
                            return true;
                        }
                    }
                    else
                    {
                        if (Math.Abs(transXExit) < (FragmentWidth / 3))
                        {
                            v.Animate().TranslationX(0);
                            return true;
                        }
                        else
                        {
                            v.Animate().TranslationX(FragmentWidth);
                            NextSong_Click(null, null);
                            return true;
                        }
                    }

                case MotionEventActions.Move:
                    var proc = 90 * v.TranslationX / (FragmentWidth - OffsetContainer);
                    //Debug.Print(proc.ToString());
                    if (proc > 90) proc = 90;
                    var currentPosition = e.GetX();
                    var deltX = mLastPosY - currentPosition;

                    var transX = v.TranslationX;
                    transX -= deltX;

                    v.TranslationX = transX;
                    //v.Animate().TranslationY(transY);
                    return true;

                default:
                    v.Animate().TranslationX(0);
                    return v.OnTouchEvent(e);
            }
        }

        #endregion

        #region MediaSession Callback

        public void OnDurationChanged(int miliseconds)
        {
            RunOnUiThread(() =>
            {
                TotalSongTimeText.Text = new TimeSpan(0, 0, 0, 0, miliseconds).ToString(@"mm\:ss");
                TotalSongTimeText.Visibility = ViewStates.Visible;
            });
        }

        public void OnPositionChanged(int miliseconds, TimeSpan currentTime)
        {
            RunOnUiThread(() =>
            {
                SongTimeSeekBar.Progress = miliseconds;
                CurretSongTimeText.Text = currentTime.ToString(@"mm\:ss");
                //Toast.MakeText(this.Context, $"{miliseconds} {currentTime.ToString(@"mm\:ss")}", ToastLength.Short).Show();
            });
        }

        public void OnCompletion()
        {
            throw new NotImplementedException();
        }

        public void OnPlaybackStatusChanged(int state)
        {
            switch (state)
            {
                case Android.Support.V4.Media.Session.PlaybackStateCompat.StatePlaying:
                    OnPlay();
                    break;
                case Android.Support.V4.Media.Session.PlaybackStateCompat.StateBuffering:
                case Android.Support.V4.Media.Session.PlaybackStateCompat.StateNone:
                    PlayToggle.SetImageResource(Resource.Drawable.play_loading);
                    PlayToggle.Tag = Resource.Drawable.play_loading;
                    break;
                case Android.Support.V4.Media.Session.PlaybackStateCompat.StatePaused:
                    PlayToggle.SetImageResource(Resource.Drawable.play_button);
                    PlayToggle.Tag = Resource.Drawable.play_button;
                    break;
            }
        }

        public void OnPlaybackMetaDataChanged(MediaMetadataCompat meta)
        {
            throw new NotImplementedException();
        }

        public void OnError(string error)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Music service connection

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            if (!IsBinded)
            {
                var binder = service as Music.MusicServiceBinder;
                binder.SetMusicServiceUpdateCallback(this);
                IsBinded = true;
                SendBroadcast(new Intent("com.spotypie.adnroid.musicservice.play"));
            }
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            ShowMessage("Service unbinded");
            //throw new NotImplementedException();
        }

        public void OnPlay()
        {
            Task.Run(() =>
            {
                Songs song = QueueHelper.GetPlayingSong();
                RunOnUiThread(() =>
                {
                    PlayerSongName.Text = song.Name;
                    PlayerArtistName.Text = song.ArtistName;

                    SongTimeSeekBar.Enabled = true;

                    PlayerSongName.Text = song.Name;

                    //if (GetActivity().MiniPlayer.Visibility == ViewStates.Gone)
                    //    GetActivity().MiniPlayer.Visibility = ViewStates.Visible;
                    //ViewLoadState = 2;

                    PlayToggle.SetImageResource(Resource.Drawable.pause);
                    PlayToggle.Tag = Resource.Drawable.pause;
                });
            });
        }

        public override dynamic GetInstance()
        {
            return this;
        }

        public override void SetScreen(LayoutScreenState screen)
        {
            switch (screen)
            {
                case LayoutScreenState.FullScreen:
                    break;
                case LayoutScreenState.Holder:
                    break;
            }
        }

        #endregion
    }
}