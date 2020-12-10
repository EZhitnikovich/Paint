using System;
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
using System.Windows.Ink;
using Color = System.Windows.Media.Color;
using System.Windows.Controls;

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

        private Bitmap workImage;
        private double canvasWidth = 600;
        private double canvasHeight = 600;

        private string newCanvasWidthStr = "600";
        private string newCanvasHeightStr = "600";
        private int brushWidth = 2;
        private int brushHeight = 2;

        private bool isFill = false;

        private Cursor cursorEraser;

        private RelayCommand closeCommand;
        private RelayCommand minimizeCommand;
        private RelayCommand maximizeCommand;
        private RelayCommand openFileCommand;
        private RelayCommand saveFileCommand;
        private RelayCommand changeCanvasSize;
        private RelayCommand changeBrushSize;
        private RelayCommand brushCommand;
        private RelayCommand eraserCommand;

        private Color eraserColor = Color.FromArgb(255, 255, 255, 255);

        private static byte redValue = 0;
        private static byte greenValue = 0;
        private static byte blueValue = 0;
        private Color resultColor = Color.FromArgb(255, redValue, greenValue, blueValue);

        private DrawingAttributes drawingAttributes;

        private MainWindow window;

        public MainViewModel(MainWindow window)
        {
            workImage = new Bitmap(Convert.ToInt32(canvasWidth), Convert.ToInt32(canvasHeight));
            Graphics g = Graphics.FromImage(workImage);
            g.Clear(System.Drawing.Color.White);

            cursorEraser = new Cursor(Application.GetResourceStream(new Uri("Cursors/eraser.cur", UriKind.Relative)).Stream);
            this.window = window;
            drawingAttributes = new DrawingAttributes();
        }

        #region figureProp

        public bool IsFill
        {
            get { return isFill; }
            set
            {
                isFill = value;
                OnPropertyChanged("IsFill");
            }
        }

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
                        WorkImage = new Bitmap(openFileDialog.FileName);
                        CanvasHeight = WorkImage.Height;
                        CanvasWidth = WorkImage.Width;
                        window.inkCanvas.Strokes.Clear();
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
                        var size = new System.Windows.Size(CanvasWidth, CanvasHeight);
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

                    if ((heightBool && newCanvasHeight > 0 && newCanvasHeight < 10000) &&
                        (widthBool && newCanvasWidth > 0 && newCanvasWidth < 10000))
                    {
                        CanvasHeight = newCanvasHeight;
                        CanvasWidth = newCanvasWidth;

                        window.inkCanvas.Strokes.Clear();
                        Graphics g = Graphics.FromImage(WorkImage);
                        g.Clear(System.Drawing.Color.White);
                    }
                    else
                    {
                        MessageBox.Show("Entered invalid values");
                    }
                });
            }
        }

        public RelayCommand ChangeBrushSize
        {
            get
            {
                return changeBrushSize ??= new RelayCommand(obj =>
                {
                    DAttributes.Width = BrushWidth;
                    DAttributes.Height = BrushHeight;
                });
            }
        }

        public RelayCommand BrushCommand
        {
            get
            {
                return brushCommand ??= new RelayCommand(obj =>
                {
                    window.inkCanvas.UseCustomCursor = false;
                    DAttributes.Color = ResultColor;
                    window.inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
                });
            }
        }

        public RelayCommand EraserCommand
        {
            get
            {
                return eraserCommand ??= new RelayCommand(obj =>
                {
                    window.inkCanvas.UseCustomCursor = true;
                    window.inkCanvas.Cursor = cursorEraser;
                    DAttributes.Color = eraserColor;
                    window.inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
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

        public DrawingAttributes DAttributes
        {
            get { return drawingAttributes; }
            set
            {
                drawingAttributes = value;
                OnPropertyChanged("DAttributes");
            }
        }

        public Bitmap WorkImage
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

        public int BrushWidth
        {
            get { return brushWidth; }
            set
            {
                brushWidth = value;
                OnPropertyChanged("BrushWidth");
            }
        }

        public int BrushHeight
        {
            get { return brushHeight; }
            set
            {
                brushHeight = value;
                OnPropertyChanged("BrushHeight");
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
                DAttributes.Color = ResultColor;
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
                DAttributes.Color = ResultColor;
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
                DAttributes.Color = ResultColor;
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
