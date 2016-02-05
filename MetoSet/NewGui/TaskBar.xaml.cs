using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MetoCraft.NewGui
{
    /// <summary>
    /// TaskBar.xaml 的互動邏輯
    /// </summary>
    public partial class TaskBar : UserControl
    {
        System.Windows.Forms.Timer timer;
        bool startCount = false;
        bool finished = false;
        bool detectAlive = true;
        Thread _task;
        Process _subTask;
        int hr = 0, min = 0, sec = 0;
        public TaskBar()
        {
            InitializeComponent();
        }
        public TaskBar(Thread task)
        {
            InitializeComponent();
            _task = task;
            _task.Start();
        }
        public TaskBar setThread(Thread task)
        {
            _task = task;
            _task.Start();
            return this;
        }
        public TaskBar setSubProcess(Process task)
        {
            _subTask = task;
            return this;
        }
        public TaskBar setTask(string name)
        {
            lblTaskName.Content = name;
            return this;
        }
        public TaskBar setTaskStatus(string status)
        {
            lblTaskStatus.Content = status;
            return this;
        }
        public bool isFinished() {
            return finished;
        }
        public bool needDetectAlive() {
            return detectAlive;
        }
        public TaskBar setDetectAlive(bool flag) {
            detectAlive = flag;
            return this;
        }
        public void noticeFinished() {
            finished = true;
        }
        public void noticeNotFinish()
        {
            finished = false;
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new System.Windows.Forms.Timer();
            timer.Enabled = true;
            timer.Interval = 1000;
            timer.Tick += new EventHandler(this.timer_Tick);
        }

        public TaskBar countTime()
        {
            startCount = true;
            return this;
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            if (!_task.IsAlive && ((_subTask == null) | (_subTask != null && _subTask.HasExited)) && needDetectAlive())
            {
                setTaskStatus("Finished");
                noticeFinished();
            }
            if (startCount)
            {
                ++sec;
                if (sec >= 60)
                {
                    ++min;
                    sec -= 60;
                    if (min >= 60)
                    {
                        ++hr;
                        min -= 60;
                    }
                }
                setTaskStatus(hr + ":" + toGoodString(min) + ":" + toGoodString(sec));
            }
        }
        private string toGoodString(int i)
        {
            string s = i.ToString();
            if (s.Length == 1) { return "0" + s; } else return s;
        }
    }
}
