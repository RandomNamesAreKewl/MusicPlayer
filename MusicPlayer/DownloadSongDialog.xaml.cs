using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MusicPlayer
{
    /// <summary>
    /// Interaction logic for DownloadSongDialog.xaml
    /// </summary>
    public partial class DownloadSongDialog : Window
    {
        public event EventHandler onFinished;
        public DownloadSongDialog()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            Process p = new Process
            {
                StartInfo = {
                    FileName = "yt-dlp.exe",
                    WorkingDirectory = Directory.GetCurrentDirectory(),
                    Arguments = "--extract-audio --audio-format \"wav\" -o \"music/%(title)s.%(ext)s\" " + UrlBox.Text
                }
            };
            p.Exited += P_Exited;
            p.Start();
            Close();
        }

        private void P_Exited(object? sender, EventArgs e)
        {
            onFinished?.Invoke(this, e);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
