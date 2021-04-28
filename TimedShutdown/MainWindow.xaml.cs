using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

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
            var success = float.TryParse(Input.Text, out var hours);
            if (success)
            {
                Process.Start("cmd.exe", "/C shutdown -a");
                var timespan = TimeSpan.FromHours(hours);

                DelayAction(500, new Action(() => 
                {
                    Process.Start("cmd.exe", $"/C shutdown -s -t {timespan.TotalSeconds}");
                }));
            }

            else
            {
                success = DateTime.TryParse(Input.Text, out var time);
                if (success)
                {
                    Process.Start("cmd.exe", "/C shutdown -a");
                    if (DateTime.Now > time)
                    {
                        time = time.AddDays(1);
                    }
                    var totalSeconds = (float)(time - DateTime.Now).TotalSeconds;
                    var roundedSeconds = totalSeconds.ToString("#");
                    DelayAction(500, new Action(() =>
                    {
                        Process.Start("cmd.exe", $"/C shutdown -s -t {roundedSeconds}");
                    }));
                }
            }
        }
        private void Cancel(object sender, RoutedEventArgs e)
        {
            Process.Start("cmd.exe", "/C shutdown -a");
        }

        private static void DelayAction(int millisecond, Action action)
        {
            var timer = new DispatcherTimer();
            timer.Tick += delegate

            {
                action.Invoke();
                timer.Stop();
            };

            timer.Interval = TimeSpan.FromMilliseconds(millisecond);
            timer.Start();
        }
    }
}
