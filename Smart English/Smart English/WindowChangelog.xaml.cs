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

                TB.Text = "[1.1] - 8 lipca 2026:"
                + "\n- Bazy, które użytkownik opanował są od teraz oznaczone kolorem zielonym."
                + "\n- Dodano idiomy i czasowniki nieregularne.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd WC001", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
