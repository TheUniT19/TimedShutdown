using System;
using System.Diagnostics;
using System.Windows;

namespace TimedShutdown
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Time(object sender, RoutedEventArgs e)
        {
            var success = DateTime.TryParse(Input.Text, out var time);
            if (success)
            {
                Process.Start("cmd.exe", "/C shutdown -a");
                if (DateTime.Now > time)
                {
                    time = time.AddDays(1);
                }
                var totalSeconds = (float)(time - DateTime.Now).TotalSeconds;
                var roundedSeconds = totalSeconds.ToString("#");
                Process.Start("cmd.exe", $"/C shutdown -s -t {roundedSeconds}");
            }
            else
            {
                success = float.TryParse(Input.Text, out var hours);
                if (success)
                {
                    Process.Start("cmd.exe", "/C shutdown -a");
                    var timespan = TimeSpan.FromHours(hours);
                    Process.Start("cmd.exe", $"/C shutdown -s -t {timespan.TotalSeconds}");
                }
            }
        }
        private void Cancel(object sender, RoutedEventArgs e)
        {
            Process.Start("cmd.exe", "/C shutdown -a");
        }
    }
}
