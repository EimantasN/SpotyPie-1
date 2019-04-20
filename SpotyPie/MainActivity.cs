﻿using Android.App;
using Android.OS;
using Android.Support.Constraints;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Mobile_Api;
using Mobile_Api.Models;
using Mobile_Api.Models.Enums;
using SpotyPie.Base;
using SpotyPie.Helpers;
using System;
using System.Windows.Input;
using SupportFragment = Android.Support.V4.App.Fragment;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;

namespace SpotyPie
{
    [Activity(Label = "SpotyPie", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Icon = "@drawable/logo_spotify", Theme = "@style/Theme.SpotyPie")]
    public class MainActivity : AppCompatActivity
    {
        private int LastViewLayer = 0;
        private int CurrentViewLayer = 1;
        //private BluetoothHelper _bluetoothHelper;

        private Current_state APPSTATE;

        FragmentBase CurrentFragment;

        FragmentBase Home;
        FragmentBase Browse;
        FragmentBase Search;
        FragmentBase Library;
        public AlbumFragment AlbumFragment;
        public SupportFragment ArtistFragment;

        BottomNavigationView bottomNavigation;
        public ImageButton PlayToggle;

        public TextView ArtistName;
        public TextView SongTitle;
        public ImageButton BackHeaderButton;
        public ImageButton OptionsHeaderButton;

        public int widthInDp = 0;
        public int HeightInDp = 0;
        public bool PlayerVisible = false;
        public ImageButton ShowPlayler;

        public TextView ActionName;
        public ConstraintLayout MiniPlayer;

        public FrameLayout Content;
        public FrameLayout FirstLayer;
        public FrameLayout SecondLayer;
        public FrameLayout PlayerContainer;

        public FragmentBase FirstLayerFragment;
        public FragmentBase SecondLayerFragment;
        public Player.Player Player;

        public int Add_to_playlist_id = 0;

        ConstraintLayout HeaderContainer;

        public SupportFragmentManager mSupportFragmentManager;

        public ICommand MyCommand { get; }

        private void MyCommandExecute()
        {
            //var deviceName = "MDR";
            //if (!_bluetoothHelper.IsConnected)
            //    _bluetoothHelper.Connect(deviceName);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Player = new Player.Player();
            APPSTATE = new Current_state((Player.Player)Player, this);

            //_bluetoothHelper = new BluetoothHelper(Application.ApplicationContext);
            //MyCommandExecute();

            HeaderContainer = FindViewById<ConstraintLayout>(Resource.Id.HeaderContainer);

            mSupportFragmentManager = SupportFragmentManager;

            PlayerContainer = FindViewById<FrameLayout>(Resource.Id.player_frame);
            FirstLayer = FindViewById<FrameLayout>(Resource.Id.first_layer);
            SecondLayer = FindViewById<FrameLayout>(Resource.Id.second_layer);
            Content = FindViewById<FrameLayout>(Resource.Id.content_frame);

            widthInDp = Resources.DisplayMetrics.WidthPixels;
            HeightInDp = Resources.DisplayMetrics.HeightPixels;
            PlayerContainer.Visibility = ViewStates.Gone;
            SecondLayer.Visibility = ViewStates.Gone;
            FirstLayer.Visibility = ViewStates.Gone;

            Home = new Browse();
            Browse = new MainFragment();
            Library = new LibraryFragment();

            SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.player_frame, Player)
                .Commit();
            bottomNavigation = FindViewById<BottomNavigationView>(Resource.Id.NavBot);
            ActionName = FindViewById<TextView>(Resource.Id.textView);

            #region MINI PLAYER

            MiniPlayer = FindViewById<ConstraintLayout>(Resource.Id.PlayerContainer);

            PlayToggle = FindViewById<ImageButton>(Resource.Id.play_stop);
            ShowPlayler = FindViewById<ImageButton>(Resource.Id.show_player);

            SongTitle = FindViewById<TextView>(Resource.Id.song_name);
            SongTitle.Selected = true;
            SongTitle.Text = "Labai ilgas tekstas kuris keičiasi jeigu tekstas netelpa";
            ArtistName = FindViewById<TextView>(Resource.Id.artist_name);
            ArtistName.Selected = true;

            if (GetState().IsPlaying)
                PlayToggle.SetImageResource(Resource.Drawable.pause);
            else
                PlayToggle.SetImageResource(Resource.Drawable.play_button);

            //MiniPlayer.Visibility = ViewStates.Gone;

            PlayToggle.Click += PlayToggle_Click;
            ShowPlayler.Click += MiniPlayer_Click;
            MiniPlayer.Click += MiniPlayer_Click;

            #endregion

            BackHeaderButton = FindViewById<ImageButton>(Resource.Id.back);
            OptionsHeaderButton = FindViewById<ImageButton>(Resource.Id.options);

            BackHeaderButton.Click += BackHeaderButton_Click;

            bottomNavigation.NavigationItemSelected += BottomNavigation_NavigationItemSelected;
            LoadFragment(Resource.Id.home);
        }

        protected override void OnDestroy()
        {
            APPSTATE.Dispose();
            base.OnDestroy();
        }

        public Current_state GetState()
        {
            return APPSTATE;
        }

        public override void OnBackPressed()
        {
            if (Player.CheckChildFragments())
            {

                switch (CurrentViewLayer)
                {
                    case 1:
                        base.OnBackPressed();
                        break;
                    case 2:
                        {
                            ToogleSecondLayer(false);
                            break;
                        }
                    case 3:
                        {
                            ToogleThirdLayer(false);
                            if (LastViewLayer != 1)
                            {
                                ToogleSecondLayer(true);
                            }
                            break;
                        }
                    case 4:
                        {
                            TogglePlayer(false);
                            switch (LastViewLayer)
                            {
                                case 1:
                                    break;
                                case 2:
                                    {
                                        ToogleSecondLayer(true);
                                        break;
                                    }
                                case 3:
                                    {
                                        ToogleThirdLayer(true);
                                        break;
                                    }
                            }
                            break;
                        }
                }
            }
        }

        public static void LoadOptionsMeniu()
        {
        }

        private void BackHeaderButton_Click(object sender, EventArgs e)
        {
            try
            {
                OnBackPressed();
            }
            catch (System.Exception)
            {
                Home = new MainFragment();
                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.content_frame, Home)
                    .Commit();
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        private void PlayToggle_Click(object sender, EventArgs e)
        {
            GetState().Music_play_toggle();
        }

        private void MiniPlayer_Click(object sender, EventArgs e)
        {
            GetState().Player_visiblibity_toggle();
        }

        private void BottomNavigation_NavigationItemSelected(object sender, BottomNavigationView.NavigationItemSelectedEventArgs e)
        {
            LoadFragment(e.Item.ItemId);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            return base.OnOptionsItemSelected(item);
        }

        private void SetUpViewPager(ViewPager viewPager)
        {
            TabAdapter adapter = new TabAdapter(SupportFragmentManager);
            //adapter.AddFragment(new Home(), "Home");
            //adapter.AddFragment(new Browse(), "Browse");
            //adapter.AddFragment(new Search(), "Search");
            //adapter.AddFragment(new LibraryFragment(), "Library");

            viewPager.Adapter = adapter;
        }

        private SongService SongAPI { get; set; }
        private AlbumService AlbumAPI { get; set; }

        private ArtistService ArtistAPI { get; set; }

        private Object ServiceLock { get; set; } = new Object();

        public dynamic GetService(ApiServices service)
        {
            lock (ServiceLock)
            {
                switch (service)
                {
                    case ApiServices.Songs:
                        {
                            if (SongAPI == null)
                                return SongAPI = new SongService();
                            return SongAPI;
                        }
                    case ApiServices.Albums:
                        {
                            if (AlbumAPI == null)
                                return AlbumAPI = new AlbumService();
                            return AlbumAPI;
                        }
                    case ApiServices.Artist:
                        {
                            if (ArtistAPI == null)
                                return ArtistAPI = new ArtistService();
                            return ArtistAPI;
                        }
                    default:
                        return null;
                }
            }
        }

        void LoadFragment(int id)
        {
            if (HeaderContainer.Visibility == ViewStates.Gone)
                HeaderContainer.Visibility = ViewStates.Visible;

            if (CurrentFragment != null)
            {
                CurrentFragment.Hide();
            }

            FragmentBase fragment = null;
            switch (id)
            {
                case Resource.Id.home:
                    fragment = Browse;
                    ActionName.Text = "Home";
                    break;
                case Resource.Id.browse:
                    fragment = Home;
                    ActionName.Text = "MUSE";
                    break;
                case Resource.Id.search:
                    fragment = Search;
                    HeaderContainer.Visibility = ViewStates.Gone;
                    break;
                case Resource.Id.library:
                    fragment = Library;
                    ActionName.Text = "Library";
                    break;
            }

            if (fragment == null)
                return;

            CurrentFragment = fragment;
            if (!fragment.IsAdded)
            {
                SupportFragmentManager.BeginTransaction()
                    .Add(Resource.Id.content_frame, CurrentFragment)
                    .Commit();
            }
            else
            {
                CurrentFragment.Show();
            }
        }

        public void RemoveCurrentFragment()
        {
            if (FirstLayerFragment != null)
            {
                var transaction = mSupportFragmentManager.BeginTransaction();
                transaction.Remove(FirstLayerFragment);
                transaction.Commit();
                transaction.SetTransition(Android.Support.V4.App.FragmentTransaction.TransitFragmentClose);
                FirstLayerFragment = null;
                FirstLayer.TranslationX = widthInDp;
            }
        }

        public void RemoveSplash(FragmentBase fr)
        {
            SupportFragmentManager.BeginTransaction().Remove(fr).CommitAllowingStateLoss();
        }

        public void LoadAlbum(Album album)
        {
            if (AlbumFragment == null)
            {
                AlbumFragment = new AlbumFragment();
            }
            //Current_state.SetAlbum(Dataset[position]);
            SupportFragmentManager.BeginTransaction().Replace(Resource.Id.first_layer, AlbumFragment).Commit();
            AlbumFragment.SetAlbum(album);
            ToogleSecondLayer(true);
        }

        public void ToogleSecondLayer(bool show)
        {
            //SHOW
            if (show)
            {
                LastViewLayer = CurrentViewLayer;
                CurrentViewLayer = 2;
                HideOthers();
                AlbumFragment.ForceUpdate();
                FirstLayer.Visibility = ViewStates.Visible;
                FirstLayer.BringToFront();
            }
            else
            {
                FirstLayer.Visibility = ViewStates.Gone;
            }
        }

        public void TogglePlayer(bool show)
        {
            //SHOW
            if (show)
            {
                LastViewLayer = CurrentViewLayer;
                CurrentViewLayer = 4;
                HideOthers();
                PlayerContainer.Visibility = ViewStates.Visible;
                PlayerContainer.BringToFront();
            }
            else
            {
                PlayerContainer.Visibility = ViewStates.Gone;
            }
        }

        public void ToogleThirdLayer(bool show)
        {
            //SHOW
            if (show)
            {
                LastViewLayer = CurrentViewLayer;
                CurrentViewLayer = 3;
                HideOthers();
                SecondLayer.Visibility = ViewStates.Visible;
                SecondLayer.BringToFront();
            }
            else
            {
                SecondLayer.Visibility = ViewStates.Gone;
            }
        }

        public void HideOthers()
        {
            if (CurrentViewLayer != 2)
                ToogleSecondLayer(false);

            if (CurrentViewLayer != 3)
                ToogleThirdLayer(false);

            if (CurrentViewLayer != 4)
                TogglePlayer(false);

            if (CurrentViewLayer == 2)
            {
                bottomNavigation.Visibility = ViewStates.Visible;
            }
            else
            {
                bottomNavigation.Visibility = ViewStates.Gone;
            }
        }
    }
}

