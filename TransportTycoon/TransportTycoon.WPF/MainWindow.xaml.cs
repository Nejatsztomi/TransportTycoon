using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TransportTycoon.WPF.ViewModel;

namespace TransportTycoon.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constructors
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Private event methods
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (DataContext is MainViewModel mainViewModel)
            {
                if (!mainViewModel.CanClose())
                {
                    e.Cancel = true;
                    return;
                }
                Application.Current.Shutdown();
            }
        }
        #endregion
    }
}
