using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Paint
{
    /// <summary>
    /// Interaction logic for AngleSettingsWindow.xaml
    /// </summary>
    public partial class AngleSettingsWindow : Window
    {
        public float StartAngle { get; protected set; } = default;
        public float SweepAngle { get; protected set; } = default;
        public bool IsReady { get; protected set; } = false;

        public AngleSettingsWindow()
        {
            InitializeComponent();
        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            if(float.TryParse(startTextBox.Text, out float startAngle) && float.TryParse(sweepTextBox.Text, out float sweepAngle))
            {
                StartAngle = startAngle;
                SweepAngle = sweepAngle;
                IsReady = true;
                this.Hide();
            }
            else
            {
                MessageBox.Show("Incorrect data");
            }
        }
    }
}
