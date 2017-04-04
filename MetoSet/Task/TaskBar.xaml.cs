using MTMCL.Threads;
using System;
using System.Collections.Generic;
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
    public partial class TaskListBar : Button, IDisposable
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
        public List<string> Log { get; set; }
        public delegate void LogUpdate(string log);
        public event LogUpdate OnLogUpdate;
        public delegate void StateUpdate(string state);
        public event StateUpdate OnStateUpdate;
        public TaskListBar()
        {
            InitializeComponent();
            timer = new System.Windows.Forms.Timer();
            timer.Enabled = true;
            timer.Interval = 1000;
            timer.Tick += new EventHandler(this.timer_Tick);
            Log = new List<string>();
        }
        public TaskListBar(Thread task)
        {
            InitializeComponent();
            _task = task;
            if(_task.ThreadState == System.Threading.ThreadState.Unstarted)
                _task.Start();
        }
        public TaskListBar setThread(Thread task)
        {
            _task = task;
            if (_task.ThreadState == System.Threading.ThreadState.Unstarted)
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
            OnStateUpdate?.Invoke(status);
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
            Dispatcher.Invoke(new Action(() => setTaskStatus(Lang.LangManager.GetLocalized("TaskFinish"))));
            finished = true;
        }
        public void noticeFailed()
        {
            Dispatcher.Invoke(new Action(() => setTaskStatus(Lang.LangManager.GetLocalized("TaskFail"))));
            finished = true;
        }
        [SecurityPermission(SecurityAction.Demand, ControlThread = true)]
        public void noticeExisted()
        {
            _task.Abort();
            Dispatcher.Invoke(new Action(() => setTaskStatus(Lang.LangManager.GetLocalized("TaskExist"))));
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
        public TaskListBar log(string log)
        {
            Log.Add(log);
            OnLogUpdate?.Invoke(log);
            return this;
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

        #region IDisposable Support
        private bool disposedValue = false; // 偵測多餘的呼叫

        protected virtual void Dispose (bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 處置 Managed 狀態 (Managed 物件)。
                    timer.Dispose();
                    if(_subTask != null)
                        _subTask.Dispose();
                    ImgSrc = null;
                    Log = null;
                }

                // TODO: 釋放 Unmanaged 資源 (Unmanaged 物件) 並覆寫下方的完成項。
                // TODO: 將大型欄位設為 null。

                disposedValue = true;
            }
        }

        // TODO: 僅當上方的 Dispose(bool disposing) 具有會釋放 Unmanaged 資源的程式碼時，才覆寫完成項。
        // ~TaskListBar() {
        //   // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 加入這個程式碼的目的在正確實作可處置的模式。
        public void Dispose ()
        {
            // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果上方的完成項已被覆寫，即取消下行的註解狀態。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
