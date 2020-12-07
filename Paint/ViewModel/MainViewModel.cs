﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Paint.Model;
using System.Drawing;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.IO;
using Color = System.Windows.Media.Color;

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
        
        private BitmapImage workImage;
        private double canvasWidth = 600;
        private double canvasHeight = 600;
        private int penWidth = 1;
        private int penHeight = 1;

        private string newCanvasWidthStr;
        private string newCanvasHeightStr;

        private MainWindow window;

        public MainViewModel(MainWindow window)
        {
            this.window = window;
        }

        #region colors

        private static byte redValue = 0;
        private static byte greenValue = 0;
        private static byte blueValue = 0;
        private Color resultColor = Color.FromArgb(255, redValue, greenValue, blueValue);

        #endregion

        #region commands

        private RelayCommand closeCommand;
        private RelayCommand minimizeCommand;
        private RelayCommand maximizeCommand;
        private RelayCommand openFileCommand;
        private RelayCommand saveFileCommand;
        private RelayCommand changeCanvasSize;

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

        public RelayCommand OpenFileCommand
        {
            get
            {
                return openFileCommand ??= new RelayCommand(obj =>
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Filter = "PNG (*.png)|*.png|JPG (*.jpg)|*.jpg";
                    if (openFileDialog.ShowDialog() == true)
                    {
                        WorkImage = new BitmapImage(new Uri(openFileDialog.FileName));
                        CanvasHeight = WorkImage.Height;
                        CanvasWidth = WorkImage.Width;
                    }
                });
            }
        }

        public RelayCommand SaveFileDialog
        {
            get
            {
                return saveFileCommand ??= new RelayCommand(obj =>
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "PNG (*.png)|*.png|JPG (*.jpg)|*.jpg";
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        var size = new System.Windows.Size(window.inkCanvas.ActualWidth, window.inkCanvas.ActualHeight);
                        JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                        string path = saveFileDialog.FileName;
                        var bitmapTarget = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96, 96, PixelFormats.Default);
                        bitmapTarget.Render(window.inkCanvas);
                        encoder.Frames.Add(BitmapFrame.Create(bitmapTarget));

                        using(var filestream = new FileStream(path, FileMode.Create))
                        {
                            encoder.Save(filestream);
                        }
                    }
                });
            }
        }

        public RelayCommand ChangeCanvasSize
        {
            get
            {
                return changeCanvasSize ??= new RelayCommand(obj =>
                {
                    bool heightBool = double.TryParse(NewCanvasHeightStr, out double newCanvasHeight);
                    bool widthBool = double.TryParse(NewCanvasWidthStr, out double newCanvasWidth);

                    if ((heightBool && newCanvasHeight > 0) &&
                        (widthBool && newCanvasWidth > 0))
                    {
                        CanvasHeight = newCanvasHeight;
                        CanvasWidth = newCanvasWidth;

                        window.inkCanvas.Strokes.Clear();
                        WorkImage = new BitmapImage();
                    }
                    else
                    {
                        MessageBox.Show("Entered invalid valuest");
                    }
                });
            }
        }

        #endregion

        #region textProperty

        public string NewCanvasWidthStr
        {
            get { return newCanvasWidthStr; }
            set
            {
                newCanvasWidthStr = value;
                OnPropertyChanged("NewCanvasWidthStr");
            }
        }

        public string NewCanvasHeightStr
        {
            get { return newCanvasHeightStr; }
            set
            {
                newCanvasHeightStr = value;
                OnPropertyChanged("NewCanvasHeightStr");
            }
        }

        #endregion

        #region imageProperty

        public BitmapImage WorkImage
        {
            get { return workImage; }
            set
            {
                workImage = value;
                OnPropertyChanged("WorkImage");
            }
        }

        public double CanvasWidth
        {
            get { return canvasWidth; }
            set
            {
                canvasWidth = value;
                OnPropertyChanged("CanvasWidth");
            }
        }

        public double CanvasHeight
        {
            get { return canvasHeight; }
            set
            {
                canvasHeight = value;
                OnPropertyChanged("CanvasHeight");
            }
        }

        public int PenWidth
        {
            get { return penWidth; }
            set
            {
                penWidth = value;
                OnPropertyChanged("PenWidth");
            }
        }

        public int PenHeight
        {
            get { return penHeight; }
            set
            {
                penHeight = value;
                OnPropertyChanged("PenHeight");
            }
        }

        #endregion

        #region colorsProperty

        public byte RedValue
        {
            get { return redValue; }
            set
            {
                ResultColor = Color.FromArgb(255, value, greenValue, blueValue);
                window.inkCanvas.DefaultDrawingAttributes.Color = ResultColor;
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
                window.inkCanvas.DefaultDrawingAttributes.Color = ResultColor;
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
                window.inkCanvas.DefaultDrawingAttributes.Color = ResultColor;
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
    }
}
