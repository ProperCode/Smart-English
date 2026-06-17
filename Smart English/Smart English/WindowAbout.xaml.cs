using System.Windows;
using System.Windows.Input;

namespace Smart_English
{
    /// <summary>
    /// Interaction logic for WindowAbout.xaml
    /// </summary>
    public partial class WindowAbout : Window
    {
        public WindowAbout()
        {
            try
            {
                InitializeComponent();

                Lprogram_name.Content = Middle_Man.prog_name;
                Llatest_version.Content = "Ostatnia wersja: " + Middle_Man.latest_version;
                Linstalled_version.Content = "Zainstalowana wersja: " + Middle_Man.prog_version;
                Lhomepage.Content = Middle_Man.url_homepage;
                Lcopyright.Content = Middle_Man.copyright_text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd WA001", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Beula_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowEULA w = new WindowEULA();
                w.Owner = Application.Current.MainWindow;
                w.ShowInTaskbar = false;
                w.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd WA002", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Lhomepage_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Middle_Man.open_url("https://" + Lhomepage.Content.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd WA003", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Lhomepage_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void Lhomepage_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = null;
        }

        private void Bchangelog_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //WindowChangelog wc = new WindowChangelog();
                //wc.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd WA004", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}