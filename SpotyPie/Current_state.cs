﻿using Android.App;
using Android.Content;
using Android.Views;
using Mobile_Api.Models;
using Mobile_Api.Models.Realm;
using Realms;
using SpotyPie.Enums.Activitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotyPie
{
    public class Current_state
    {
        public bool IsPlaying = false;

        public static int PrevId { get; set; }

        public static int Id { get; set; }

        public bool Start_music { get; set; } = false;

        public bool IsPlayerLoaded { get; set; } = false;

        public bool PlayerIsVisible { get; set; } = false;

        public float Progress { get; set; }

        public MainActivity Activity { get; set; }

        public string Current_Player_Image { get; set; }

        public Album Current_Album { get; set; } = null;

        public Artist Current_Artist { get; set; } = null;

        public Playlist Current_Playlist { get; set; } = null;

        public Songs Current_Song { get; set; } = null;

        public int Position { get; set; }

        public List<Songs> CurrentSongList { get; set; } = new List<Songs>();

        private Player.Player Player { get; set; }

        public Current_state(MainActivity activity)
        {
            this.Activity = activity;
            //Player = new Player.Player();
            //activity.mSupportFragmentManager.BeginTransaction()
            //        .Replace(Resource.Id.player_frame, Player)
            //        .Commit();
        }



        public Player.Player GetPlayer()
        {
            return Activity.GetPlayer();
        }

        public void SetSong(List<Songs> songs, int position = 0, bool refresh = false)
        {
            UpdateRealSongList(songs, position);
            Id = songs[position].Id;
            CurrentSongList = songs;
            Position = position;
            Activity.LoadFragmentInner(HomePage.Player, screen: Enums.LayoutScreenState.FullScreen);
        }

        private void UpdateRealSongList(List<Songs> songs, int position = 0)
        {
            Task.Run(() =>
            {
                Realm realm = Realm.GetInstance();
                ApplicationSongList songList = realm.All<ApplicationSongList>().FirstOrDefault(x => x.Id == 1);
                if (songList == null)
                    return;

                songList.Rewrite(realm, songs, position);
                realm.Dispose();

                Activity.RunOnUiThread(() =>
                {
                    Activity.SendBroadcast(new Intent("com.spotypie.adnroid.musicservice.play"));
                });
            });
        }

        public void SetCurrentSongList(List<Songs> songs)
        {
            if (songs != null && songs.Count > 0)
            {
                songs.First().SetIsPlaying(true);
                CurrentSongList.AddRange(songs);
            }
        }

        public void UpdateSongList(Songs song)
        {
            CurrentSongList.First(x => x.Id == Current_Song.Id).SetIsPlaying(true);
        }

        public void SetArtist(Artist art)
        {
        }

        public void SetAlbum(Album album)
        {
            Current_Album = album;
            Activity.RunOnUiThread(() =>
            {
                GetPlayer().PlayerPlaylistName.Text = Current_Album.Name;
            });
        }

        internal void Dispose()
        {
        }

        public void ShowHeaderNavigationButtons()
        {
            Activity.BackHeaderButton.Visibility = ViewStates.Visible;
            Activity.OptionsHeaderButton.Visibility = ViewStates.Visible;
        }

        public void HideHeaderNavigationButtons()
        {
            Activity.BackHeaderButton.Visibility = ViewStates.Gone;
            Activity.OptionsHeaderButton.Visibility = ViewStates.Gone;
        }

        internal void SetSongDuration(int Duration)
        {
            try
            {
                if (Current_Song.DurationMs != Duration)
                {
                    Task.Run(() => Activity.GetAPIService().SetSongDurationAsync(Current_Song.Id, Duration));
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void ToggleBotNav(bool show)
        {
            if (show)
            {
                this.Activity.BottomNavigation.Visibility = ViewStates.Visible;
            }
            else
            {
                this.Activity.BottomNavigation.Visibility = ViewStates.Gone;
            }
        }

        public void ToggleMiniPlayer(bool show)
        {
            if (show)
            {
                this.Activity.MiniPlayer.Visibility = ViewStates.Visible;
            }
            else
            {
                this.Activity.MiniPlayer.Visibility = ViewStates.Gone;
            }
        }
    }
}