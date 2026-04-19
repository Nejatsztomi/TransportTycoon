using System.Diagnostics;
using System.Windows.Controls;

namespace TransportTycoon.WPF.View.UserControls
{
    /// <summary>
    /// Interaction logic for LeftPanel.xaml
    /// </summary>
    public partial class LeftPanel : UserControl
    {
        #region Constructors
        public LeftPanel()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
                Debug.WriteLine(ex.InnerException);
            }
        }
        #endregion
    }
}
