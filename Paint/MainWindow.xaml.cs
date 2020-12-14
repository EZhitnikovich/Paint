using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Paint.ViewModel;
using Point = System.Windows.Point;
using Pen = System.Drawing.Pen;

namespace Paint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel mainView;
        public Bitmap tempBitmap;
        private Point startPoint;
        private bool drawing = false;

        public MainWindow()
        {
            InitializeComponent();
            mainView = new MainViewModel(this);
            DataContext = mainView;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void inkCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mainView.IsYeydropper)
            {
                var p = e.GetPosition(this.inkCanvas);
                var a = mainView.WorkImage.GetPixel(Convert.ToInt32(p.X), Convert.ToInt32(p.Y));
                mainView.ResultColor = System.Windows.Media.Color.FromArgb(255, a.R, a.G, a.B);
                mainView.RedValue = a.R;
                mainView.GreenValue = a.G;
                mainView.BlueValue = a.B;
            }

            //if(drawing)
            //mainView.WorkImage = tempBitmap;

            mainView.WorkImage = mainView.WorkImage;
            drawing = false;
        }

        private void inkCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (drawing)
            {
                Pen pen = new Pen(System.Drawing.Color.FromArgb(255,
                                                                mainView.ResultColor.R,
                                                                mainView.ResultColor.G,
                                                                mainView.ResultColor.B
                                                                ));
                pen.Width = Convert.ToSingle(mainView.BorderWidth);

                if (!mainView.IsExperiment)
                {
                    Image img = Image.FromHbitmap(tempBitmap.GetHbitmap());
                    mainView.WorkImage = new Bitmap(img);
                }

                Graphics g = Graphics.FromImage(mainView.WorkImage);
                SolidBrush brush = new SolidBrush(pen.Color);
                Point point = e.GetPosition(inkCanvas);

                if (mainView.IsRectangle)
                {
                    RectangleF rect = new RectangleF(Math.Min((int)startPoint.X, (int)point.X),
                                                 Math.Min((int)startPoint.Y, (int)point.Y),
                                                 Math.Abs((int)point.X - (int)startPoint.X),
                                                 Math.Abs((int)point.Y - (int)startPoint.Y));
                    if (mainView.IsFill)
                    {
                        g.FillRectangle(brush, rect);
                    }
                    else
                    {
                        g.DrawRectangle(pen, System.Drawing.Rectangle.Round(rect));
                    }
                }
                else if (mainView.IsEllipse)
                {
                    RectangleF rect = new RectangleF((int)startPoint.X,
                                                 (int)startPoint.Y,
                                                 (int)point.X - (int)startPoint.X,
                                                 (int)point.Y - (int)startPoint.Y);
                    if (mainView.IsFill)
                    {
                        g.FillEllipse(brush, rect);
                    }
                    else
                    {
                        g.DrawEllipse(pen, System.Drawing.Rectangle.Round(rect));
                    }
                }
                else if (mainView.IsLine)
                {
                    g.DrawLine(pen, (int)startPoint.X,
                                    (int)startPoint.Y,
                                    (int)point.X,
                                    (int)point.Y);
                }

                mainView.WorkImage = mainView.WorkImage;
            }
        }

        private void inkCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(mainView.IsEllipse ||
                mainView.IsLine ||
                mainView.IsRectangle)
                drawing = true;
            Image img = Image.FromHbitmap(mainView.WorkImage.GetHbitmap());
            tempBitmap = new Bitmap(img);
            startPoint = e.GetPosition(inkCanvas);
        }
    }
}
