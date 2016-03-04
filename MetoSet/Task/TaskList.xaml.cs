﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace MTMCL.Task
{
    /// <summary>
    /// TaskList.xaml 的互動邏輯
    /// </summary>
    public partial class TaskList : Grid
    {
        public TaskList()
        {
            InitializeComponent();
        }

        private void butBack_Click(object sender, RoutedEventArgs e)
        {
            Back();
        }
        private void Back()
        {
            MeCore.MainWindow.gridMain.Visibility = Visibility.Visible;
            MeCore.MainWindow.gridOthers.Visibility = Visibility.Collapsed;
            var ani = new DoubleAnimationUsingKeyFrames();
            ani.KeyFrames.Add(new LinearDoubleKeyFrame(0, TimeSpan.FromSeconds(0)));
            ani.KeyFrames.Add(new LinearDoubleKeyFrame(1, TimeSpan.FromSeconds(0.2)));
            MeCore.MainWindow.gridMain.BeginAnimation(OpacityProperty, ani);
        }
        
        private void Grid_Initialized(object sender, EventArgs e)
        {
            foreach (var item in MeCore.MainWindow.taskdict.Values)
            {
                item.Click += TaskBar_Click;
                panelTask.Children.Add(item);
            }
        }
        private void TaskBar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is TaskListBar)
            {
                MessageBox.Show("");
            }
        }

        private void Grid_Unloaded(object sender, RoutedEventArgs e)
        {
            panelTask.Children.Clear();
        }
    }
}