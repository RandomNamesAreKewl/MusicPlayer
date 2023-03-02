using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace MusicPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        enum MusicState
        {
            NONE,
            PLAYING,
            PAUSED,
        }

        MediaPlayer player;
        MusicState state = MusicState.NONE;
        public MainWindow()
        {
            player = new MediaPlayer();
            player.MediaOpened += Player_MediaOpened;
            player.MediaEnded += Player_MediaEnded;
            InitializeComponent();
            RefreshSongs();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += timer1_Tick;
            timer.Interval = TimeSpan.FromSeconds(0.1);
            timer.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            TimeSlider.Value = player.Position.TotalMilliseconds;
        }

        private void SeekPosition(object? sender, EventArgs e)
        {
            player.Position = TimeSpan.FromMilliseconds((int)TimeSlider.Value);
        }

        private void Player_MediaOpened(object? sender, EventArgs e)
        {
            TimeSlider.Maximum = player.NaturalDuration.TimeSpan.TotalMilliseconds;
        }

        private void Player_MediaEnded(object? sender, EventArgs e)
        {
            state = MusicState.NONE;
            player.Stop();
            SongList.SelectedIndex += 1;
            if (SongList.SelectedIndex >= SongList.Items.Count)
                SongList.SelectedIndex = 0;
            PlayPause();

        }

        private void AddSong(string name, string filename)
        {
            SongList.Items.Add(new CheckBox { Content = name, Tag = filename, IsChecked = true });
        }

        public void RefreshSongs(object sender, RoutedEventArgs e)
        {
            RefreshSongs();
        }

        public void RefreshSongs()
        {
            state = MusicState.NONE;
            player.Stop();
            SongList.Items.Clear();
            foreach (var entry in Directory.GetFiles("music"))
            {
                if(entry.EndsWith(".part")) continue;
                string songname = Path.GetFileNameWithoutExtension(entry);
                AddSong(songname, entry);
            }
            if(SongList.Items.Count > 0)
            {
                SongList.SelectedIndex = 0;
            }
        }

        public void PlayPause()
        {
            if (SongList.SelectedIndex < 0) return;
            switch (state)
            {
                case MusicState.NONE:
                    state = MusicState.PLAYING;
                    player.Open(new Uri((SongList.SelectedItem as CheckBox).Tag as string, UriKind.RelativeOrAbsolute));
                    player.Play();
                    break;
                case MusicState.PLAYING:
                    state = MusicState.PAUSED;
                    player.Pause();
                    break;
                case MusicState.PAUSED:
                    state = MusicState.PLAYING;
                    player.Play();
                    break;
            }
        }

        private void PlayPause(object sender, RoutedEventArgs e)
        {
            PlayPause();
        }

        private void ChangeVolume(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.Volume = e.NewValue / 100;
        }

        private void SongList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(state != MusicState.NONE)
            {
                state = MusicState.NONE;
                player.Stop();
                player.Open(new Uri((SongList.SelectedItem as CheckBox).Tag as string, UriKind.RelativeOrAbsolute));
                player.Play();
                state = MusicState.PLAYING;
            }
        }

        private void DownloadSong(object sender, RoutedEventArgs e)
        {
            DownloadSongDialog dialog = new DownloadSongDialog();
            dialog.onFinished += SongFinishedDownloading;
            dialog.ShowDialog();
        }

        private void SongFinishedDownloading(object? sender, EventArgs e)
        {
            RefreshSongs();
        }
    }
}
