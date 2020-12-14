using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Paint.ViewModel;

namespace Paint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel mainView;

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
        }
    }
}
