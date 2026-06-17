using System.IO;
using System.Windows;

namespace Smart_English
{
    /// <summary>
    /// Interaction logic for WindowAddProfile.xaml
    /// </summary>
    public partial class WindowAddProfile : Window
    {
        string old_profile_name = "";
        bool Create = true;

        public WindowAddProfile(bool create = true, string name = null)
        {
            try
            {
                InitializeComponent();

                if(create == false)
                {
                    Butworz.Content = "Edytuj";
                    this.Title = "Edytuj Profil";
                    old_profile_name = name;
                    Create = false;
                    TBnazwa.Text = name;

                    foreach (Profile p in Middle_Man.list_profiles)
                    {
                        if (p.name == name)
                        {
                            if (p.male)
                                RBmezczyzna.IsChecked = true;
                            else
                                RBkobieta.IsChecked = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd WAP001", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RBkobieta_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                RBmezczyzna.IsChecked = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd WAP002", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RBmezczyzna_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                RBkobieta.IsChecked = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd WAP003", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Butworz_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TBnazwa.Text = TBnazwa.Text.Trim();

                if (TBnazwa.Text.Length == 0)
                    throw new Exception("Pole nazwa nie może być puste.");
                else if (TBnazwa.Text.Length > 40)
                    throw new Exception("Nazwa nie może być dłuższa niż 40 znaków.");
                else if (TBnazwa.Text[TBnazwa.Text.Length - 1] == '.')
                    throw new Exception("Nazwa nie może kończyć się kropką.");

                if(TBnazwa.Text.Contains("<"))
                    throw new Exception("Nazwa nie może zawierać znaku <.");
                else if (TBnazwa.Text.Contains(">"))
                    throw new Exception("Nazwa nie może zawierać znaku >.");
                else if (TBnazwa.Text.Contains(":"))
                    throw new Exception("Nazwa nie może zawierać znaku :.");
                else if (TBnazwa.Text.Contains("\""))
                    throw new Exception("Nazwa nie może zawierać znaku \".");
                else if (TBnazwa.Text.Contains("/"))
                    throw new Exception("Nazwa nie może zawierać znaku /.");
                else if (TBnazwa.Text.Contains("\\"))
                    throw new Exception("Nazwa nie może zawierać znaku \\.");
                else if (TBnazwa.Text.Contains("|"))
                    throw new Exception("Nazwa nie może zawierać znaku |.");
                else if (TBnazwa.Text.Contains("?"))
                    throw new Exception("Nazwa nie może zawierać znaku ?.");
                else if (TBnazwa.Text.Contains("*"))
                    throw new Exception("Nazwa nie może zawierać znaku *.");
                else if (TBnazwa.Text.Contains("CON"))
                    throw new Exception("Nazwa nie może zawierać słowa CON.");
                else if (TBnazwa.Text.Contains("PRN"))
                    throw new Exception("Nazwa nie może zawierać słowa PRN.");
                else if (TBnazwa.Text.Contains("AUX"))
                    throw new Exception("Nazwa nie może zawierać słowa AUX.");
                else if (TBnazwa.Text.Contains("NUL"))
                    throw new Exception("Nazwa nie może zawierać słowa NUL.");
                else if (TBnazwa.Text.Contains("COM"))
                    throw new Exception("Nazwa nie może zawierać słowa COM.");
                else if (TBnazwa.Text.Contains("LPT"))
                    throw new Exception("Nazwa nie może zawierać słowa LPT.");

                if (RBkobieta.IsChecked == false && RBmezczyzna.IsChecked == false)
                    throw new Exception("Nie wybrano płci.");

                string nazwa_profilu = TBnazwa.Text;
                bool mezczyzna = false;

                if (RBmezczyzna.IsChecked == true)
                    mezczyzna = true;

                foreach (Profile p in Middle_Man.list_profiles)
                {
                    if (p.name == nazwa_profilu && old_profile_name != nazwa_profilu)
                        throw new Exception("Profil o tej nazwie już istnieje.");
                }

                string profile_directory_path = Middle_Man.profiles_folder_path + nazwa_profilu;
                string old_profile_directory_path = Middle_Man.profiles_folder_path + old_profile_name;
                string profile_bases_directory = profile_directory_path + "\\" + "Bazy";

                if (Create == true)
                {
                    Directory.CreateDirectory(profile_directory_path);

                    if (Directory.Exists(profile_directory_path))
                    {
                        Directory.CreateDirectory(profile_bases_directory);

                        if (Directory.Exists(profile_bases_directory))
                        {
                            string[] allfiles = Directory.GetFiles(Middle_Man.bases_folder_name, "*.*", SearchOption.TopDirectoryOnly);

                            foreach (string file in allfiles)
                            {
                                string[] tab = file.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                                string file_name = tab[tab.Length - 1];

                                File.Copy(file, profile_directory_path + "\\" + Middle_Man.bases_folder_name + "\\" + file_name);
                            }
                        }
                    }

                    Middle_Man.list_profiles.Add(new Profile(nazwa_profilu, mezczyzna));
                    Middle_Man.save_profiles();
                }
                else
                {
                    if (old_profile_name != nazwa_profilu && Directory.Exists(old_profile_directory_path))
                    {
                        Directory.Move(old_profile_directory_path, profile_directory_path);
                    }

                    for (int i = 0; i < Middle_Man.list_profiles.Count; i++)
                    {
                        if (Middle_Man.list_profiles[i].name == old_profile_name)
                        {
                            Middle_Man.list_profiles[i].name = nazwa_profilu;
                            Middle_Man.list_profiles[i].male = mezczyzna;
                        }
                    }
                    Middle_Man.save_profiles();
                }

                Middle_Man.WP.load_profiles();
                int id_profilu = Middle_Man.WP.get_id_by_name(nazwa_profilu);
                Middle_Man.WP.LBprofile.SelectedIndex = id_profilu;

                this.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd WAP004", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}