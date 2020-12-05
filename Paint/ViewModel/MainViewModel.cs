using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Paint.Model;

namespace Paint.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private RelayCommand closeCommand;
        private RelayCommand minimizeCommand;
        private RelayCommand maximizeCommand;

        #region commands

        public RelayCommand CloseCommand
        {
            get
            {
                return closeCommand ??= new RelayCommand(obj =>
                    {
                        App.Current.Shutdown();
                    });
            }
        }

        public RelayCommand MinimizeCommand
        {
            get
            {
                return minimizeCommand ??= new RelayCommand(obj =>
                {
                    App.Current.MainWindow.WindowState = WindowState.Minimized;
                });
            }
        }

        public RelayCommand MaximizeCommand
        {
            get
            {
                return maximizeCommand ??= new RelayCommand(obj =>
                {
                    if (App.Current.MainWindow.WindowState == WindowState.Maximized)
                        App.Current.MainWindow.WindowState = WindowState.Normal;
                    else if (App.Current.MainWindow.WindowState == WindowState.Normal)
                        App.Current.MainWindow.WindowState = WindowState.Maximized;
                });
            }
        }

        #endregion
    }
}
