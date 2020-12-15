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
using System.Windows.Forms;
using System.Drawing;

namespace Paint
{
    /// <summary>
    /// Interaction logic for TextSettingsWindow.xaml
    /// </summary>
    public partial class TextSettingsWindow : Window
    {
        public Font ResultFont { get; protected set; } = default;
        public bool IsReady { get; protected set; } = false;
        public string ResultText { get; protected set; }

        public TextSettingsWindow()
        {
            InitializeComponent();
        }

        private void fontSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            FontDialog fontDialog = new FontDialog();
            fontDialog.Font = ResultFont;

            if (fontDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                return;

            ResultFont = fontDialog.Font;
            IsReady = true;
        }

        private void acceptSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            ResultText = inputTextBox.Text;
            this.Hide();
        }
    }
}
