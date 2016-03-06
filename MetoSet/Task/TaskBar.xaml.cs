using MTMCL.Threads;
using System;
using System.Diagnostics;
using System.Security.Permissions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MTMCL.Task
{
    /// <summary>
    /// TaskListBar.xaml 的互動邏輯
    /// </summary>
    public partial class TaskListBar : Button
    {
        System.Windows.Forms.Timer timer;
        bool startCount = false;
        bool finished = false;
        bool detectAlive = true;
        Thread _task;
        Process _subTask;
        public ImageSource ImgSrc { get; set; }
        int hr = 0, min = 0, sec = 0;
        public string TaskName { get; set; }
        public string TaskStatus { get; set; }
        public string Identifier { get; set; }
        public TaskListBar()
        {
            InitializeComponent();
            timer = new System.Windows.Forms.Timer();
            timer.Enabled = true;
            timer.Interval = 1000;
            timer.Tick += new EventHandler(this.timer_Tick);
        }
        public TaskListBar(Thread task)
        {
            InitializeComponent();
            _task = task;
            _task.Start();
        }
        public TaskListBar setThread(Thread task)
        {
            _task = task;
            _task.Start();
            return this;
        }
        public TaskListBar setThread(MTMCLThread task)
        {
            _task = task.Start();
            return this;
        }
        public TaskListBar setSubProcess(Process task)
        {
            _subTask = task;
            return this;
        }
        public TaskListBar setTask(string name)
        {
            TaskName = name;
            ((AccessText)lblTaskName.Content).Text = TaskName;
            return this;
        }
        public TaskListBar setTaskStatus(string status)
        {
            TaskStatus = status;
            lblTaskStatus.Content = TaskStatus;
            return this;
        }
        public bool isFinished() {
            return finished;
        }
        public bool needDetectAlive() {
            return detectAlive;
        }
        public TaskListBar setDetectAlive(bool flag) {
            detectAlive = flag;
            return this;
        }
        public void noticeFinished()
        {
            Dispatcher.Invoke(new Action(() => setTaskStatus(Lang.LangManager.GetLangFromResource("TaskFinish"))));
            finished = true;
        }
        public void noticeFailed()
        {
            Dispatcher.Invoke(new Action(() => setTaskStatus(Lang.LangManager.GetLangFromResource("TaskFail"))));
            finished = true;
        }
        [SecurityPermission(SecurityAction.Demand, ControlThread = true)]
        public void noticeExisted()
        {
            _task.Abort();
            Dispatcher.Invoke(new Action(() => setTaskStatus(Lang.LangManager.GetLangFromResource("TaskExist"))));
            finished = true;
        }
        public void noticeNotFinish()
        {
            setTaskStatus("");
            finished = false;
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public TaskListBar countTime()
        {
            startCount = true;
            return this;
        }
        public TaskListBar stopCountTime()
        {
            startCount = false;
            return this;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (_task != null)
            {
                if (!_task.IsAlive && ((_subTask == null) | (_subTask != null && _subTask.HasExited)) && needDetectAlive())
                {
                    stopCountTime().noticeFinished();
                }
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
