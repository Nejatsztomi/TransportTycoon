using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Windows;
using TransportTycoon.Model;
using TransportTycoon.WPF.ViewModel;

namespace TransportTycoon.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Fields
        private GameModel model = null!;
        private MainViewModel mainViewModel = null!;
        private MainWindow view = null!;
        #endregion
        #region Properties
        #endregion
        #region Constructor
        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
        }
        #endregion
        #region Public Methods
        #endregion
        #region Private Methods
        private void App_Startup(object sender, StartupEventArgs e)
        {
            
        }
        #endregion
        #region Private event Methods

        

        private void ViewModel_Exit(object? sender, EventArgs e)
        {
            view.Close();
        }
        #endregion

    }

}
