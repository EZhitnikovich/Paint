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
        private int borderWidth = 1;

        public Font ResultFont { get; protected set; }
        public float StartAngle { get; protected set; }
        public float SweepAngle { get; protected set; }

        public bool IsRectangle { get; protected set; } = false;
        public bool IsEllipse { get; protected set; } = false;
        public bool IsLine { get; protected set; } = false;
        public bool IsPen { get; protected set; } = true;
        public bool IsYeydropper { get; protected set; } = false;
        public string ResultText { get; protected set; }
        public bool IsText { get; protected set; } = false;
        public bool IsArc { get; protected set; } = false;
        public bool IsPie { get; protected set; } = false;

        private bool isFill = false;
        private bool isExperiment = false;


        private Cursor cursorEraser;
        private Cursor cursorEyedropper;

        private RelayCommand closeCommand;
        private RelayCommand minimizeCommand;
        private RelayCommand maximizeCommand;
        private RelayCommand openFileCommand;
        private RelayCommand saveFileCommand;
        private RelayCommand changeCanvasSize;
        private RelayCommand changeBrushSize;
        private RelayCommand brushCommand;
        private RelayCommand eraserCommand;
        private RelayCommand eyedropperCommand;
        private RelayCommand clearCommand;
        private RelayCommand rectagleCommand;
        private RelayCommand ellipseCommand;
        private RelayCommand lineCommand;
        private RelayCommand textCommand;
        private RelayCommand arcCommand;
        private RelayCommand pieCommand;

        private Color eraserColor = Color.FromArgb(255, 255, 255, 255);

        private static byte redValue = 0;
        private static byte greenValue = 0;
        private static byte blueValue = 0;
        private Color resultColor = Color.FromArgb(255, redValue, greenValue, blueValue);

        private DrawingAttributes drawingAttributes;

        private MainWindow window;
        private TextSettingsWindow textWindow;
        private AngleSettingsWindow angleWindow;

        public MainViewModel(MainWindow window)
        {
            workImage = new Bitmap(Convert.ToInt32(canvasWidth), Convert.ToInt32(canvasHeight));
            Graphics g = Graphics.FromImage(workImage);
            g.Clear(System.Drawing.Color.White);

            cursorEraser = new Cursor(Application.GetResourceStream(new Uri("Cursors/eraser.cur", UriKind.Relative)).Stream);
            cursorEyedropper = new Cursor(Application.GetResourceStream(new Uri("Cursors/eyedropper.cur", UriKind.Relative)).Stream);
            this.window = window;
            drawingAttributes = new DrawingAttributes();
            textWindow = new TextSettingsWindow();
            angleWindow = new AngleSettingsWindow();
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

        public bool IsExperiment
        {
            get { return isExperiment; }
            set
            {
                isExperiment = value;
                OnPropertyChanged("IsExperiment");
            }
        }

        public RelayCommand RectangleCommand
        {
            get
            {
                return rectagleCommand ??= new RelayCommand(obj =>
                {
                    window.inkCanvas.UseCustomCursor = false;
                    SwitchCondition("rectangle");
                });
            }
        }

        public RelayCommand EllipseCommand
        {
            get
            {
                return ellipseCommand ??= new RelayCommand(obj =>
                {
                    window.inkCanvas.UseCustomCursor = false;
                    SwitchCondition("ellipse");
                });
            }
        }

        public RelayCommand LineCommand
        {
            get
            {
                return lineCommand ??= new RelayCommand(obj =>
                {
                    window.inkCanvas.UseCustomCursor = false;
                    SwitchCondition("line");
                });
            }
        }

        public RelayCommand TextCommand
        {
            get
            {
                return textCommand ??= new RelayCommand(obj =>
                {
                    window.inkCanvas.UseCustomCursor = false;
                    textWindow.ShowDialog();
                    if (textWindow.IsReady)
                    {
                        SwitchCondition("text");
                        ResultFont = textWindow.ResultFont;
                        ResultText = textWindow.ResultText;
                    }
                    SwitchCondition("TextCommand");
                });
            }
        }

        public RelayCommand ArcCommand
        {
            get
            {
                return arcCommand ??= new RelayCommand(obj =>
                {
                    window.inkCanvas.UseCustomCursor = false;
                    angleWindow.ShowDialog();
                    if (angleWindow.IsReady)
                    {
                        SwitchCondition("arc");
                        StartAngle = angleWindow.StartAngle;
                        SweepAngle = angleWindow.SweepAngle;
                    }
                    SwitchCondition("ArcCommand");
                });
            }
        }

        public RelayCommand PieCommand
        {
            get
            {
                return pieCommand ??= new RelayCommand(obj =>
                {
                    window.inkCanvas.UseCustomCursor = false;
                    angleWindow.ShowDialog();
                    if (angleWindow.IsReady)
                    {
                        SwitchCondition("pie");
                        StartAngle = angleWindow.StartAngle;
                        SweepAngle = angleWindow.SweepAngle;
                    }
                    SwitchCondition("PieCommand");
                });
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
                        WorkImage = new Bitmap((int)CanvasWidth, (int)CanvasHeight);
                        Graphics g = Graphics.FromImage(WorkImage);
                        g.Clear(System.Drawing.Color.White);
                        WorkImage = WorkImage;
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
                    SwitchCondition("pen");
                    DAttributes.Width = BrushWidth;
                    DAttributes.Height = BrushHeight;
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
                    SwitchCondition("pen");
                    DAttributes.Width = BrushWidth;
                    DAttributes.Height = BrushHeight;
                });
            }
        }

        public RelayCommand EyedropperCommand
        {
            get
            {
                return eyedropperCommand ??= new RelayCommand(obj =>
                {
                    window.inkCanvas.UseCustomCursor = true;
                    window.inkCanvas.Cursor = cursorEyedropper;
                    SwitchCondition("eyedropper");
                    window.inkCanvas.EditingMode = InkCanvasEditingMode.GestureOnly;
                    DAttributes.Width = 1;
                    DAttributes.Height = 1;
                });
            }
        }

        public RelayCommand ClearCommand
        {
            get
            {
                return clearCommand ??= new RelayCommand(obj =>
                {
                    window.inkCanvas.Strokes.Clear();
                    Graphics g = Graphics.FromImage(WorkImage);
                    g.Clear(System.Drawing.Color.White);
                    WorkImage = WorkImage;
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
                NewCanvasWidthStr = canvasWidth.ToString();
                OnPropertyChanged("CanvasWidth");
            }
        }

        public double CanvasHeight
        {
            get { return canvasHeight; }
            set
            {
                canvasHeight = value;
                NewCanvasHeightStr = canvasHeight.ToString();
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

        public int BorderWidth
        {
            get { return borderWidth; }
            set
            {
                borderWidth = value;
                OnPropertyChanged("BorderWidth");
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

        private void SwitchCondition(string condition)
        {
            switch (condition)
            {
                case "pen":
                    IsRectangle = false;
                    IsEllipse = false;
                    IsLine = false;
                    IsPen = true;
                    IsYeydropper = false;
                    IsText = false;
                    IsArc = false;
                    IsPie = false;
                    break;
                case "eyedropper":
                    IsRectangle = false;
                    IsEllipse = false;
                    IsLine = false;
                    IsPen = false;
                    IsYeydropper = true;
                    IsText = false;
                    IsArc = false;
                    IsPie = false;
                    break;
                case "line":
                    IsRectangle = false;
                    IsEllipse = false;
                    IsLine = true;
                    IsPen = false;
                    IsYeydropper = false;
                    IsText = false;
                    IsArc = false;
                    IsPie = false;
                    window.inkCanvas.EditingMode = InkCanvasEditingMode.None;
                    break;
                case "rectangle":
                    IsRectangle = true;
                    IsEllipse = false;
                    IsLine = false;
                    IsPen = false;
                    IsYeydropper = false;
                    IsText = false;
                    IsArc = false;
                    IsPie = false;
                    window.inkCanvas.EditingMode = InkCanvasEditingMode.None;
                    break;
                case "ellipse":
                    IsRectangle = false;
                    IsEllipse = true;
                    IsLine = false;
                    IsPen = false;
                    IsYeydropper = false;
                    IsText = false;
                    IsArc = false;
                    IsPie = false;
                    window.inkCanvas.EditingMode = InkCanvasEditingMode.None;
                    break;
                case "text":
                    IsRectangle = false;
                    IsEllipse = false;
                    IsLine = false;
                    IsPen = false;
                    IsYeydropper = false;
                    IsText = true;
                    IsArc = false;
                    IsPie = false;
                    window.inkCanvas.EditingMode = InkCanvasEditingMode.None;
                    break;
                case "arc":
                    IsRectangle = false;
                    IsEllipse = false;
                    IsLine = false;
                    IsPen = false;
                    IsYeydropper = false;
                    IsText = false;
                    IsArc = true;
                    IsPie = false;
                    window.inkCanvas.EditingMode = InkCanvasEditingMode.None;
                    break;
                case "pie":
                    IsRectangle = false;
                    IsEllipse = false;
                    IsLine = false;
                    IsPen = false;
                    IsYeydropper = false;
                    IsText = false;
                    IsArc = false;
                    IsPie = true;
                    window.inkCanvas.EditingMode = InkCanvasEditingMode.None;
                    break;
            }
        }
    }
}
