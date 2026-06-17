using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;

namespace Smart_English
{
    /// <summary>
    /// Interaction logic for WindowProfiles.xaml
    /// </summary>
    public partial class WindowProfiles : Window
    {
        private readonly MainWindow _mainWindow;

        public WindowProfiles(MainWindow mainWindow)
        {
            try
            {
                InitializeComponent();

                _mainWindow = mainWindow;

                Middle_Man.load_profiles();

                foreach (Profile p in Middle_Man.list_profiles)
                {
                    LBprofile.Items.Add(p.name);
                }

                load_last_profile();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd WP001", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LBprofile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if(LBprofile.SelectedIndex == -1)
                {
                    Bwczytaj.IsEnabled = false;
                    Bedytuj.IsEnabled = false;
                    Busun.IsEnabled = false;
                }
                else
                {
                    Bwczytaj.IsEnabled = true;
                    Bedytuj.IsEnabled = true;
                    Busun.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd WP002", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void load_profiles()
        {
            try
            {
                Middle_Man.load_profiles();

                LBprofile.Items.Clear();

                foreach (Profile p in Middle_Man.list_profiles)
                {
                    LBprofile.Items.Add(p.name);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd WP003", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public int get_id_by_name(string name)
        {
            try
            {
                for (int i = 0; i < LBprofile.Items.Count; i++)
                {
                    if (LBprofile.Items[i].ToString() == name)
                    {
                        return i;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd WP004", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return -1;
        }

        void save_last_profile(string name)
        {
            try
            {
                XmlDocument xml_doc = new XmlDocument();

                XmlNode root_node = xml_doc.CreateElement("last_profile");

                XmlAttribute attribute = xml_doc.CreateAttribute("version");
                attribute.Value = "1";
                root_node.Attributes.Append(attribute);

                xml_doc.AppendChild(root_node);

                XmlNode stats_node;

                stats_node = xml_doc.CreateElement("name");
                stats_node.InnerText = name;
                root_node.AppendChild(stats_node);

                xml_doc.Save(Middle_Man.last_profile_path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd WP005", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void load_last_profile()
        {
            try
            {
                if (File.Exists(Middle_Man.last_profile_path))
                {
                    XmlDocument xml_doc = new XmlDocument();
                    xml_doc.Load(Middle_Man.last_profile_path);

                    XmlNodeList settings = xml_doc.SelectNodes("//last_profile");

                    int version = -1;
                    bool parsing_v = false;

                    if (settings[0].Attributes["version"] != null)
                        parsing_v = int.TryParse(settings[0].Attributes["version"].Value, out version);

                    if (parsing_v && version == 1)
                    {
                        XmlNodeList nodes = settings[0].ChildNodes;

                        foreach (XmlNode node in nodes)
                        {
                            if (node.Name == "name")
                            {
                                for (int i = 0; i < LBprofile.Items.Count; i++)
                                {
                                    if (LBprofile.Items[i].ToString() == node.InnerText)
                                    {
                                        LBprofile.SelectedIndex = i;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd WP006", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Bwczytaj_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LBprofile.SelectedIndex != -1)
                {
                    int id = LBprofile.SelectedIndex;
                    string profile_name = LBprofile.Items[id].ToString();
                    Middle_Man.profile_name = profile_name;

                    foreach (Profile p in Middle_Man.list_profiles)
                    {
                        if (p.name == profile_name)
                        {
                            if (p.male)
                                Middle_Man.male = true;
                            else
                                Middle_Man.male = false;
                            break;
                        }
                    }

                    save_last_profile(profile_name);

                    _mainWindow.Show();

                    _mainWindow.loaded();

                    this.Hide();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd WP007", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Butworz_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowAddProfile WAP = new WindowAddProfile();
                WAP.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd WP008", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Bedytuj_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LBprofile.SelectedIndex != -1)
                {
                    int id = LBprofile.SelectedIndex;
                    string profile_name = LBprofile.Items[id].ToString();

                    WindowAddProfile WAP = new WindowAddProfile(false, profile_name);
                    WAP.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd WP009", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Busun_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LBprofile.SelectedIndex != -1)
                {
                    int id = LBprofile.SelectedIndex;
                    string profile_name = LBprofile.Items[id].ToString();

                    MessageBoxResult result = MessageBox.Show("Czy na pewno chcesz usunąć profil " + profile_name + "?", "Potwierdzenie",
                        MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        string profile_directory_path = Middle_Man.profiles_folder_path + profile_name;

                        Directory.Delete(profile_directory_path, true);

                        for (int i = 0; i < Middle_Man.list_profiles.Count; i++)
                        {
                            if (Middle_Man.list_profiles[i].name == profile_name)
                            {
                                Middle_Man.list_profiles.RemoveAt(i);
                                break;
                            }
                        }

                        Middle_Man.save_profiles();
                        Middle_Man.WP.load_profiles();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd WP010", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                Process.GetCurrentProcess().Kill();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd WP011", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LBprofile_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Bwczytaj_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd WP012", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}