using System.Windows;

namespace Smart_English
{
    /// <summary>
    /// Interaction logic for WindowChangelog.xaml
    /// </summary>
    public partial class WindowChangelog : Window
    {
        public WindowChangelog()
        {
            try
            {
                InitializeComponent();

                TB.IsReadOnly = true;

                TB.Text = "[1.1] - , 2026:"
                + "\n- ";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd WC001", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
