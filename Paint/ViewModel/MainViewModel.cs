using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Paint.Model;

namespace Paint.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        #region colors

        private static byte redValue = 0;
        private static byte greenValue = 0;
        private static byte blueValue = 0;
        private Color resultColor = Color.FromArgb(255, redValue, greenValue, blueValue);

        #endregion

        #region colorsProperty

        public byte RedValue
        {
            get { return redValue; }
            set
            {
                ResultColor = Color.FromArgb(255, value, greenValue, blueValue);
                redValue = value;
                OnPropertyChanged("RedValue");
            }
        }

        public byte GreenValue
        {
            get { return greenValue; }
            set
            {
                ResultColor = Color.FromArgb(255, redValue, value, blueValue);
                greenValue = value;
                OnPropertyChanged("GreenValue");
            }
        }

        public byte BlueValue
        {
            get { return blueValue; }
            set
            {
                ResultColor = Color.FromArgb(255, redValue, greenValue, value);
                blueValue = value;
                OnPropertyChanged("BlueValue");
            }
        }

        public Color ResultColor
        {
            get { return resultColor; }
            set
            {
                resultColor = value;
                OnPropertyChanged("ResultColor");
            }
        }

        #endregion

        #region commands

        private RelayCommand closeCommand;
        private RelayCommand minimizeCommand;
        private RelayCommand maximizeCommand;

        #endregion
        
        #region commandsProperty

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
