using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace MTMCL.NewGui
{
    /// <summary>
    /// TaskGui.xaml 的互動邏輯
    /// </summary>
    public partial class TaskGui : Window
    {
        System.Windows.Forms.Timer timer;
        bool startCount = false;
        Thread _task;
        Process _subTask;
        int hr = 0,min = 0, sec = 0;
        public TaskGui()
        {
            InitializeComponent();
        }
        public TaskGui(Thread task) {
            InitializeComponent();
            _task = task;
            _task.Start();
        }
        public TaskGui setThread(Thread task) {
            _task = task;
            _task.Start();
            return this;
        }
        public TaskGui setSubProcess(Process task)
        {
            _subTask = task;
            return this;
        }
        public TaskGui setTask(string name)
        {
            lblTaskName.Content = name;
            return this;
        }
        public TaskGui setTaskStatus(string status)
        {
            lblTaskStatus.Content = status;
            return this;
        }
        public TaskGui countTime() {
            startCount = true;
            return this;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new System.Windows.Forms.Timer();
            timer.Enabled = true;
            timer.Interval = 1000;
            timer.Tick += new EventHandler(this.timer_Tick);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            lblTime.Content = DateTime.Now.ToLocalTime().ToShortTimeString();
            if (!_task.IsAlive && ((_subTask == null) | (_subTask != null && _subTask.HasExited))) {
                Close();
            }
            if (startCount) {
                ++sec;
                if (sec >= 60) {
                    ++min;
                    sec -= 60;
                    if (min >= 60) {
                        ++hr;
                        min -= 60;
                    }
                }
                setTaskStatus(hr + ":" + toGoodString(min) + ":" + toGoodString(sec));
            }
        }
        private string toGoodString(int i) {
            string s = i.ToString();
            if (s.Length == 1) { return "0" + s; } else return s;
        }
    }
}
