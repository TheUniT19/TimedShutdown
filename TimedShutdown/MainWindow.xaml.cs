using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

namespace TimedShutdown
{
    public partial class MainWindow : Window
    {
        public class Cancellation
        {            
            public bool IsCancelled { get; set; }
        }
        public List<Cancellation> Cancellations { get; set; } = new List<Cancellation>();
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Cancel(object sender, RoutedEventArgs e)
        {
            CancelAll();
        }

        private void CancelAll()
        {
            Process.Start("cmd.exe", "/C shutdown -a");
            Cancellations.ForEach(x => x.IsCancelled = true);
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

        private void Shutdown(object sender, RoutedEventArgs e)
        {
            if (float.TryParse(Input.Text, out var hours))
            {
                CancelAll();
                var timespan = TimeSpan.FromHours(hours);

                DelayAction(500, new Action(() =>
                {
                    Process.Start("cmd.exe", $"/C shutdown -s -t {timespan.TotalSeconds}");
                }));
            }

            else
            {
                if (DateTime.TryParse(Input.Text, out var time))
                {
                    CancelAll();
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

        private void Hibernate(object sender, RoutedEventArgs e)
        {
            if (float.TryParse(Input.Text, out var hours))
            {
                CancelAll();
                var timespan = TimeSpan.FromHours(hours);
                var cancellation = new Cancellation();
                Cancellations.Add(cancellation);

                DelayAction((int)timespan.TotalMilliseconds, new Action(() =>
                {
                    if (!cancellation.IsCancelled)
                    {
                        Process.Start("cmd.exe", $"/C shutdown -h");
                    }
                    Cancellations.Remove(cancellation);
                }));
            }

            else
            {
                if (DateTime.TryParse(Input.Text, out var time))
                {
                    CancelAll();
                    if (DateTime.Now > time)
                    {
                        time = time.AddDays(1);
                    }
                    var totalMs = (int)(time - DateTime.Now).TotalMilliseconds;
                    var cancellation = new Cancellation();
                    Cancellations.Add(cancellation);

                    DelayAction(totalMs, new Action(() =>
                    {
                        if (!cancellation.IsCancelled)
                        {
                            Process.Start("cmd.exe", $"/C shutdown -h");
                        }
                        Cancellations.Remove(cancellation);
                    }));
                }
            }
        }
    }
}
