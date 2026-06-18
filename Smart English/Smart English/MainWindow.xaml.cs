//Błąd MW044

using System.Diagnostics;
using System.IO;
using System.Media;
using System.Net.Http;
using System.Speech.Synthesis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using System.Xml.Serialization;
using System.Speech.Synthesis;

namespace Smart_English
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool saving_enabled = false;
        bool loading_error = false;

        public MainWindow()
        {
            try
            {
                is_program_already_running();

                InitializeComponent();

                this.Hide();

                Middle_Man.WP = new WindowProfiles(this);
                Middle_Man.WP.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MW001", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void loaded()
        {
            try
            {
                update_app_if_necessary();

                saving_enabled = false;

                this.Title = Middle_Man.prog_name + " " + Middle_Man.prog_version;

                Lnazwa_profilu.Content = "Nazwa profilu: " + Middle_Man.profile_name;

                Middle_Man.list_bases = new List<Sentences_base>();

                TBnr_zdania.Text = "";
                TBliczba_zdan.Text = "";
                TBliczba_zapamietanych.Text = "";
                TBpozostaly_czas.Text = "";
                TBliczba_ukonczen.Text = "";

                TBliczba_zapamietanych_all.Text = "0 (0%)";
                TBliczba_ukonczen_all.Text = "0";

                restore_default_settings();

                string bases_xml_file_path = Middle_Man.profiles_folder_path + Middle_Man.profile_name + "\\" + Middle_Man.bases_filename;

                if (File.Exists(bases_xml_file_path) == false)
                {
                    create_bases_file();
                }

                Middle_Man.total_remembered = 0;
                Middle_Man.total_completions = 0;

                load_bases();
                load_stats();

                load_settings();
                calculate_remaining_time();

                fix_wrong_values();

                saving_enabled = true;

                if (loading_error)
                {
                    MessageBox.Show("Wykryto błąd wczytywania ustawień. Wszystkie ustawienia zostaną przywrócone do domyślnych i zapisane.",
                        "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    restore_default_settings();
                    save_settings(); //save settings so loading error won't happen again (default values
                                     //will take place of unread values)
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MW002", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void save_bases()
        {
            XmlRootAttribute root = new XmlRootAttribute();
            root.ElementName = "Sentences_base";
            root.IsNullable = true;

            XmlSerializer serial = new XmlSerializer(typeof(List<Sentences_base>), root);

            StreamWriter sw = null;

            string bases_xml_file_path = Middle_Man.profiles_folder_path + Middle_Man.profile_name + "\\" + Middle_Man.bases_filename;

            try
            {
                sw = new StreamWriter(bases_xml_file_path);
                serial.Serialize(sw, Middle_Man.list_bases);
                sw.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MW003", MessageBoxButton.OK, MessageBoxImage.Error);

                try
                {
                    if (sw != null)
                        sw.Close();
                }
                catch (Exception ex2) { }
            }
        }

        void load_bases()
        {
            try
            {
                LBbazy.Items.Clear();

                string profile_directory_path = Middle_Man.profiles_folder_path + Middle_Man.profile_name;
                string profile_bases_directory = profile_directory_path + "\\" + "Bazy";

                if (Directory.Exists(profile_bases_directory))
                {
                    string[] allfiles = Directory.GetFiles(profile_bases_directory, "*.*", SearchOption.TopDirectoryOnly);

                    foreach (string file in allfiles)
                    {
                        string[] tab = file.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                        string file_name = tab[tab.Length - 1];

                        LBbazy.Items.Add(file_name.Replace(".xml", ""));
                    }
                }

                string bases_xml_file_path = Middle_Man.profiles_folder_path + Middle_Man.profile_name + "\\" + Middle_Man.bases_filename;

                if (File.Exists(bases_xml_file_path))
                {
                    XmlDocument doc = new XmlDocument();
                    XmlReader rdr = null;
                    XmlNodeReader noderdr = null;

                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.ConformanceLevel = ConformanceLevel.Auto;
                    settings.IgnoreComments = true;
                    settings.IgnoreWhitespace = true;

                    Sentences_base sb;
                    string s = null;
                    int x = 0, y = 0;

                    doc.Load(bases_xml_file_path);
                    noderdr = new XmlNodeReader(doc);
                    rdr = XmlReader.Create(noderdr, settings);

                    while (rdr.Read())
                    {
                        if (rdr.NodeType == XmlNodeType.Element)
                        {
                            if (rdr.Name == "name")
                            {
                                rdr.Read();
                                s = rdr.Value.Replace(".xml", "");
                            }
                            if (rdr.Name == "sentences_nr")
                            {
                                rdr.Read();
                                x = int.Parse(rdr.Value);
                            }
                            if (rdr.Name == "current_word_nr")
                            {
                                rdr.Read();
                                y = int.Parse(rdr.Value);
                            }
                            if (rdr.Name == "completions")
                            {
                                rdr.Read();
                                sb = new Sentences_base(s, x, y, int.Parse(rdr.Value));
                                Middle_Man.list_bases.Add(sb);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MW004", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void create_bases_file()
        {
            try
            {
                string profile_directory_path = Middle_Man.profiles_folder_path + Middle_Man.profile_name;
                string profile_bases_directory = profile_directory_path + "\\" + "Bazy";

                if (Directory.Exists(profile_bases_directory))
                {
                    string[] allfiles = Directory.GetFiles(profile_bases_directory, "*.*", SearchOption.TopDirectoryOnly);

                    foreach (string file in allfiles)
                    {
                        string[] tab = file.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                        string file_name = tab[tab.Length - 1];

                        string base_name = file_name.Replace(".xml", "");

                        file_name = profile_bases_directory + @"\" + base_name + ".xml";

                        List<Sentence> temp_list_sentences = new List<Sentence>();

                        XmlDocument doc = new XmlDocument();
                        XmlReader rdr = null;
                        XmlNodeReader noderdr = null;

                        XmlReaderSettings settings = new XmlReaderSettings();
                        settings.ConformanceLevel = ConformanceLevel.Auto;
                        settings.IgnoreComments = true;
                        settings.IgnoreWhitespace = true;

                        Sentence S;
                        string s1, s2;
                        bool s3 = false;
                        s1 = s2 = null;

                        doc.Load(file_name);
                        noderdr = new XmlNodeReader(doc);
                        rdr = XmlReader.Create(noderdr, settings);

                        while (rdr.Read())
                        {
                            if (rdr.NodeType == XmlNodeType.Element)
                            {
                                if (rdr.Name == "sentence_en")
                                {
                                    rdr.Read();
                                    s1 = rdr.Value;
                                }
                                if (rdr.Name == "sentence_pl")
                                {
                                    rdr.Read();
                                    s2 = rdr.Value;
                                }
                                if (rdr.Name == "remembered")
                                {
                                    rdr.Read();
                                    s3 = Convert.ToBoolean(rdr.Value);
                                    S = new Sentence(s1, s2, s3);

                                    temp_list_sentences.Add(S);
                                }
                            }
                        }

                        int sentences_nr = temp_list_sentences.Count;

                        int remembered = 0;
                        foreach (Sentence s in temp_list_sentences)
                        {
                            if (s.remembered)
                                remembered++;
                        }

                        Sentences_base sb = new Sentences_base(base_name, temp_list_sentences.Count, 1, 0);
                        Middle_Man.list_bases.Add(sb);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MW005", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void save_stats()
        {
            try
            {
                string stats_file_path = Middle_Man.profiles_folder_path + Middle_Man.profile_name + "\\" + Middle_Man.stats_filename;

                XmlDocument xml_doc = new XmlDocument();

                XmlNode root_node = xml_doc.CreateElement("stats");

                XmlAttribute attribute = xml_doc.CreateAttribute("version");
                attribute.Value = "1";
                root_node.Attributes.Append(attribute);

                xml_doc.AppendChild(root_node);

                XmlNode stats_node;

                stats_node = xml_doc.CreateElement("total_remembered");
                stats_node.InnerText = Middle_Man.total_remembered.ToString();
                root_node.AppendChild(stats_node);

                stats_node = xml_doc.CreateElement("total_completions");
                stats_node.InnerText = Middle_Man.total_completions.ToString();
                root_node.AppendChild(stats_node);

                stats_node = xml_doc.CreateElement("last_base_name");
                stats_node.InnerText = Middle_Man.last_base_name.ToString();
                root_node.AppendChild(stats_node);

                xml_doc.Save(stats_file_path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MW006", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void load_stats()
        {
            try
            {
                string stats_file_path = Middle_Man.profiles_folder_path + Middle_Man.profile_name + "\\" + Middle_Man.stats_filename;

                if (File.Exists(stats_file_path))
                {
                    XmlDocument xml_doc = new XmlDocument();
                    xml_doc.Load(stats_file_path);

                    XmlNodeList settings = xml_doc.SelectNodes("//stats");

                    int version = -1;
                    bool parsing_v = false;

                    if (settings[0].Attributes["version"] != null)
                        parsing_v = int.TryParse(settings[0].Attributes["version"].Value, out version);

                    if (parsing_v && version == 1)
                    {
                        XmlNodeList nodes = settings[0].ChildNodes;

                        foreach (XmlNode node in nodes)
                        {
                            if (node.Name == "total_remembered")
                            {
                                Middle_Man.total_remembered = int.Parse(node.InnerText);

                                int total_sentences = 0;
                                for (int i = 0; i < Middle_Man.list_bases.Count; i++)
                                {
                                    total_sentences += Middle_Man.list_bases[i].sentences_nr;
                                }

                                double learning_percentage = Math.Floor((double)Middle_Man.total_remembered / (double)total_sentences * 100);

                                TBliczba_zapamietanych_all.Text = Middle_Man.total_remembered.ToString() + " (" + learning_percentage + "%)";
                            }
                            else if (node.Name == "total_completions")
                            {
                                Middle_Man.total_completions = int.Parse(node.InnerText);
                                TBliczba_ukonczen_all.Text = node.InnerText;
                            }
                            else if (node.Name == "last_base_name")
                            {
                                Middle_Man.last_base_name = node.InnerText;

                                for (int i = 0; i < LBbazy.Items.Count; i++)
                                {
                                    if (LBbazy.Items[i].ToString() == Middle_Man.last_base_name)
                                    {
                                        LBbazy.SelectedIndex = i;
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
                MessageBox.Show(ex.Message, "Błąd MW007", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void restore_default_settings()
        {
            try
            {
                RBautomatyczny.IsChecked = false;
                RBpolautomatyczny.IsChecked = false;
                RBreczny.IsChecked = true;

                TBopoznienie_po_pojawieniu.Text = "4";
                TBopoznienie_pomiedzy.Text = "0";

                CHBczytaj_po_ang.IsChecked = true;
                CHBczytaj_po_pol.IsChecked = true;

                using var synth = new SpeechSynthesizer();

                List<string> voices_EN = new List<string>();
                List<string> voices_PL = new List<string>();

                IReadOnlyCollection<InstalledVoice> list = synth.GetInstalledVoices();

                CBglos_ang.Items.Clear();
                CBglos_pol.Items.Clear();
                CBglosnosc.Items.Clear();

                foreach (InstalledVoice voice in list)
                {
                    if (voice.VoiceInfo.Name.Contains("David"))
                    {
                        voices_EN.Add(voice.VoiceInfo.Name);
                        CBglos_ang.Items.Add(voice.VoiceInfo.Name);
                    }
                    else if (voice.VoiceInfo.Name.Contains("Emma"))
                    {
                        voices_EN.Add(voice.VoiceInfo.Name);
                        CBglos_ang.Items.Add(voice.VoiceInfo.Name);
                    }
                    else if (voice.VoiceInfo.Name.Contains("Salli"))
                    {
                        voices_EN.Add(voice.VoiceInfo.Name);
                        CBglos_ang.Items.Add(voice.VoiceInfo.Name);
                    }
                    else if (voice.VoiceInfo.Name.Contains("Hazel"))
                    {
                        voices_EN.Add(voice.VoiceInfo.Name);
                        CBglos_ang.Items.Add(voice.VoiceInfo.Name);
                    }
                    else if (voice.VoiceInfo.Name.Contains("Zira"))
                    {
                        voices_EN.Add(voice.VoiceInfo.Name);
                        CBglos_ang.Items.Add(voice.VoiceInfo.Name);
                    }
                    else if (voice.VoiceInfo.Name.Contains("James"))
                    {
                        voices_EN.Add(voice.VoiceInfo.Name);
                        CBglos_ang.Items.Add(voice.VoiceInfo.Name);
                    }
                    else if (voice.VoiceInfo.Name.Contains("Linda"))
                    {
                        voices_EN.Add(voice.VoiceInfo.Name);
                        CBglos_ang.Items.Add(voice.VoiceInfo.Name);
                    }
                    else if (voice.VoiceInfo.Name.Contains("Richard"))
                    {
                        voices_EN.Add(voice.VoiceInfo.Name);
                        CBglos_ang.Items.Add(voice.VoiceInfo.Name);
                    }
                    else if (voice.VoiceInfo.Name.Contains("George"))
                    {
                        voices_EN.Add(voice.VoiceInfo.Name);
                        CBglos_ang.Items.Add(voice.VoiceInfo.Name);
                    }
                    else if (voice.VoiceInfo.Name.Contains("Susan"))
                    {
                        voices_EN.Add(voice.VoiceInfo.Name);
                        CBglos_ang.Items.Add(voice.VoiceInfo.Name);
                    }
                    else if (voice.VoiceInfo.Name.Contains("Sean"))
                    {
                        voices_EN.Add(voice.VoiceInfo.Name);
                        CBglos_ang.Items.Add(voice.VoiceInfo.Name);
                    }
                    else if (voice.VoiceInfo.Name.Contains("Catherine"))
                    {
                        voices_EN.Add(voice.VoiceInfo.Name);
                        CBglos_ang.Items.Add(voice.VoiceInfo.Name);
                    }
                    else if (voice.VoiceInfo.Name.Contains("Mark"))
                    {
                        voices_EN.Add(voice.VoiceInfo.Name);
                        CBglos_ang.Items.Add(voice.VoiceInfo.Name);
                    }
                    else if (voice.VoiceInfo.Name.Contains("Adam"))
                    {
                        voices_PL.Add(voice.VoiceInfo.Name);
                        CBglos_pol.Items.Add(voice.VoiceInfo.Name);
                    }
                    else if (voice.VoiceInfo.Name.Contains("Paulina"))
                    {
                        voices_PL.Add(voice.VoiceInfo.Name);
                        CBglos_pol.Items.Add(voice.VoiceInfo.Name);
                    }
                }

                if (CBglos_ang.Items.Count > 0)
                    CBglos_ang.SelectedIndex = 0;

                for (int i = 0; i < CBglos_ang.Items.Count; i++)
                {
                    if (CBglos_ang.Items[i].ToString() != null && CBglos_ang.Items[i].ToString().Contains(Middle_Man.default_en_voice))
                    {
                        CBglos_ang.SelectedIndex = i;
                        break;
                    }
                }

                if (CBglos_pol.Items.Count > 0)
                    CBglos_pol.SelectedIndex = 0;

                for (int i = 0; i < CBglos_pol.Items.Count; i++)
                {
                    if (CBglos_pol.Items[i].ToString() != null && CBglos_pol.Items[i].ToString().Contains(Middle_Man.default_pl_voice))
                    {
                        CBglos_pol.SelectedIndex = i;
                        break;
                    }
                }

                for (int i = 0; i <= 100; i += 5)
                {
                    CBglosnosc.Items.Add(i.ToString());
                }

                CBglosnosc.SelectedIndex = CBglosnosc.Items.Count - 1;

                CHBsprawdzaj_aktualizacje.IsChecked = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MW008", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void fix_wrong_values()
        {
            try
            {
                if (RBautomatyczny.IsChecked == false && RBpolautomatyczny.IsChecked == false
                    && RBreczny.IsChecked == false)
                {
                    RBpolautomatyczny.IsChecked = true;
                }

                bool parsing = int.TryParse(TBopoznienie_po_pojawieniu.Text, out int result);

                if (parsing == false || result < 0 || result > 1000)
                    TBopoznienie_po_pojawieniu.Text = "3";

                parsing = int.TryParse(TBopoznienie_pomiedzy.Text, out result);

                if (parsing == false || result < 0 || result > 1000)
                    TBopoznienie_pomiedzy.Text = "0";

                if (CBglos_ang.SelectedIndex == -1)
                {
                    if (CBglos_ang.Items.Count > 0)
                        CBglos_ang.SelectedIndex = 0;

                    for (int i = 0; i < CBglos_ang.Items.Count; i++)
                    {
                        if (CBglos_ang.Items[i].ToString() != null && CBglos_ang.Items[i].ToString().Contains(Middle_Man.default_en_voice))
                        {
                            CBglos_ang.SelectedIndex = i;
                            break;
                        }
                    }
                }

                if (CBglos_pol.SelectedIndex == -1)
                {
                    if (CBglos_pol.Items.Count > 0)
                        CBglos_pol.SelectedIndex = 0;

                    for (int i = 0; i < CBglos_pol.Items.Count; i++)
                    {
                        if (CBglos_pol.Items[i].ToString() != null && CBglos_pol.Items[i].ToString().Contains(Middle_Man.default_pl_voice))
                        {
                            CBglos_pol.SelectedIndex = i;
                            break;
                        }
                    }
                }

                if (CBglosnosc.SelectedIndex == -1)
                    CBglosnosc.SelectedIndex = CBglosnosc.Items.Count - 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MW009", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void save_settings()
        {
            try
            {
                string settings_file_path = Middle_Man.profiles_folder_path + Middle_Man.profile_name + "\\" + Middle_Man.settings_filename;

                XmlDocument xml_doc = new XmlDocument();

                XmlNode root_node = xml_doc.CreateElement("settings");

                XmlAttribute attribute = xml_doc.CreateAttribute("version");
                attribute.Value = "1";
                root_node.Attributes.Append(attribute);

                xml_doc.AppendChild(root_node);

                XmlNode settings_node;

                settings_node = xml_doc.CreateElement("tryb");
                settings_node.InnerText = Middle_Man.tryb.ToString();
                root_node.AppendChild(settings_node);

                settings_node = xml_doc.CreateElement("opoznienie_po_zdaniu");
                settings_node.InnerText = Middle_Man.opoznienie_po_zdaniu.ToString();
                root_node.AppendChild(settings_node);

                settings_node = xml_doc.CreateElement("opoznienie_pomiedzy");
                settings_node.InnerText = Middle_Man.opoznienie_pomiedzy.ToString();
                root_node.AppendChild(settings_node);

                settings_node = xml_doc.CreateElement("czytaj_po_ang");
                settings_node.InnerText = Middle_Man.czytaj_po_ang.ToString();
                root_node.AppendChild(settings_node);

                settings_node = xml_doc.CreateElement("czytaj_po_pol");
                settings_node.InnerText = Middle_Man.czytaj_po_pol.ToString();
                root_node.AppendChild(settings_node);

                settings_node = xml_doc.CreateElement("glos_ang");
                settings_node.InnerText = Middle_Man.glos_ang.ToString();
                root_node.AppendChild(settings_node);

                settings_node = xml_doc.CreateElement("glos_pol");
                settings_node.InnerText = Middle_Man.glos_pol.ToString();
                root_node.AppendChild(settings_node);

                settings_node = xml_doc.CreateElement("glosnosc");
                settings_node.InnerText = Middle_Man.glosnosc.ToString();
                root_node.AppendChild(settings_node);

                settings_node = xml_doc.CreateElement("sprawdzaj_aktualizacje");
                settings_node.InnerText = Middle_Man.sprawdzaj_aktualizacje.ToString();
                root_node.AppendChild(settings_node);

                xml_doc.Save(settings_file_path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MW010", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void load_settings()
        {
            try
            {
                string settings_file_path = Middle_Man.profiles_folder_path + Middle_Man.profile_name + "\\" + Middle_Man.settings_filename;

                if (File.Exists(settings_file_path))
                {
                    XmlDocument xml_doc = new XmlDocument();
                    xml_doc.Load(settings_file_path);

                    XmlNodeList settings = xml_doc.SelectNodes("//settings");

                    int version = -1;
                    bool parsing_v = false;

                    if (settings[0].Attributes["version"] != null)
                        parsing_v = int.TryParse(settings[0].Attributes["version"].Value, out version);

                    if (parsing_v && version == 1)
                    {
                        XmlNodeList nodes = settings[0].ChildNodes;

                        foreach (XmlNode node in nodes)
                        {
                            if (node.Name == "tryb")
                            {
                                string str = node.InnerText;
                                if (str == "automatyczny")
                                {
                                    Middle_Man.tryb = Tryb.automatyczny;
                                    RBautomatyczny.IsChecked = true;
                                }
                                else if (str == "polautomatyczny")
                                {
                                    Middle_Man.tryb = Tryb.polautomatyczny;
                                    RBpolautomatyczny.IsChecked = true;
                                }
                                else if (str == "reczny")
                                {
                                    Middle_Man.tryb = Tryb.reczny;
                                    RBreczny.IsChecked = true;
                                }
                            }
                            else if (node.Name == "opoznienie_po_zdaniu")
                            {
                                Middle_Man.opoznienie_po_zdaniu = int.Parse(node.InnerText);
                                TBopoznienie_po_pojawieniu.Text = node.InnerText;
                            }
                            else if (node.Name == "opoznienie_pomiedzy")
                            {
                                Middle_Man.opoznienie_pomiedzy = int.Parse(node.InnerText);
                                TBopoznienie_pomiedzy.Text = node.InnerText;
                            }
                            else if (node.Name == "czytaj_po_ang")
                            {
                                Middle_Man.czytaj_po_ang = bool.Parse(node.InnerText);
                                CHBczytaj_po_ang.IsChecked = Middle_Man.czytaj_po_ang;
                            }
                            else if (node.Name == "czytaj_po_pol")
                            {
                                Middle_Man.czytaj_po_pol = bool.Parse(node.InnerText);
                                CHBczytaj_po_pol.IsChecked = Middle_Man.czytaj_po_pol;
                            }
                            else if (node.Name == "glos_ang")
                            {
                                Middle_Man.glos_ang = node.InnerText;
                                CBglos_ang.SelectedItem = node.InnerText;
                            }
                            else if (node.Name == "glos_pol")
                            {
                                Middle_Man.glos_pol = node.InnerText;
                                CBglos_pol.SelectedItem = node.InnerText;
                            }
                            else if (node.Name == "glosnosc")
                            {
                                Middle_Man.glosnosc = int.Parse(node.InnerText);
                                CBglosnosc.Text = node.InnerText;
                            }
                            else if (node.Name == "sprawdzaj_aktualizacje")
                            {
                                Middle_Man.sprawdzaj_aktualizacje = bool.Parse(node.InnerText);
                                CHBsprawdzaj_aktualizacje.IsChecked = Middle_Man.sprawdzaj_aktualizacje;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                loading_error = true;
                MessageBox.Show(ex.Message, "Błąd MW011", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void save_base(string base_name)
        {
            string profile_directory_path = Middle_Man.profiles_folder_path + Middle_Man.profile_name;
            string profile_bases_directory = profile_directory_path + "\\" + "Bazy";

            string file_name = profile_bases_directory + @"\" + base_name + ".xml";

            XmlRootAttribute root = new XmlRootAttribute();
            root.ElementName = "Sentence";
            root.IsNullable = true;

            XmlSerializer serial = new XmlSerializer(typeof(List<Sentence>), root);

            StreamWriter sw = null;

            try
            {
                sw = new StreamWriter(file_name);
                serial.Serialize(sw, Middle_Man.list_sentences);
                sw.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MW013", MessageBoxButton.OK, MessageBoxImage.Error);

                try
                {
                    if (sw != null)
                        sw.Close();
                }
                catch (Exception ex2) { }
            }
        }

        void load_base(string base_name)
        {
            string profile_directory_path = Middle_Man.profiles_folder_path + Middle_Man.profile_name;
            string profile_bases_directory = profile_directory_path + "\\" + "Bazy";

            string file_name = profile_bases_directory + @"\" + base_name + ".xml";

            Middle_Man.list_sentences = new List<Sentence>();

            XmlDocument doc = new XmlDocument();
            XmlReader rdr = null;
            XmlNodeReader noderdr = null;

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Auto;
            settings.IgnoreComments = true;
            settings.IgnoreWhitespace = true;

            try
            {
                Sentence S;
                string s1, s2;
                bool s3 = false;
                s1 = s2 = null;

                doc.Load(file_name);
                noderdr = new XmlNodeReader(doc);
                rdr = XmlReader.Create(noderdr, settings);

                while (rdr.Read())
                {
                    if (rdr.NodeType == XmlNodeType.Element)
                    {
                        if (rdr.Name == "sentence_en")
                        {
                            rdr.Read();
                            s1 = rdr.Value;
                        }
                        if (rdr.Name == "sentence_pl")
                        {
                            rdr.Read();
                            s2 = rdr.Value;
                        }
                        if (rdr.Name == "remembered")
                        {
                            rdr.Read();
                            s3 = Convert.ToBoolean(rdr.Value);
                            S = new Sentence(s1, s2, s3);

                            Middle_Man.list_sentences.Add(S);
                        }
                    }
                }

                TBliczba_zdan.Text = Middle_Man.list_sentences.Count.ToString();

                int remembered = 0;
                foreach(Sentence s in Middle_Man.list_sentences)
                {
                    if (s.remembered)
                        remembered++;
                }
                TBliczba_zapamietanych.Text = remembered.ToString();

                bool found = false;

                for(int i = 0; i < Middle_Man.list_bases.Count && found == false; i++)
                {
                    if (Middle_Man.list_bases[i].name == base_name)
                    {
                        TBnr_zdania.Text = Middle_Man.list_bases[i].current_word_nr.ToString();
                        TBliczba_ukonczen.Text = Middle_Man.list_bases[i].completions.ToString();
                        found = true;
                    }
                }

                if(found == false)
                {
                    Sentences_base sb = new Sentences_base(base_name, Middle_Man.list_sentences.Count, 1, 0);
                    Middle_Man.list_bases.Add(sb);

                    TBnr_zdania.Text = "1";
                    TBliczba_ukonczen.Text = "0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MW014", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void calculate_remaining_time()
        {
            try
            {
                if (LBbazy.SelectedIndex != -1)
                {
                    if (Middle_Man.tryb == Tryb.automatyczny)
                    {
                        int id = LBbazy.SelectedIndex;
                        string base_name = LBbazy.Items[id].ToString();

                        int learning_time = 0, time_ms = 0;
                        int current_word_nr = int.Parse(TBnr_zdania.Text);

                        for (int i = current_word_nr - 1; i < Middle_Man.list_sentences.Count; i++)
                        {
                            time_ms = 0;
                            time_ms += Middle_Man.opoznienie_po_zdaniu * 1000 + Middle_Man.list_sentences[i].sentence_en.Length * Middle_Man.time_per_char;
                            time_ms += Middle_Man.opoznienie_po_zdaniu * 1000 + Middle_Man.list_sentences[i].sentence_pl.Length * Middle_Man.time_per_char;

                            time_ms += Middle_Man.opoznienie_pomiedzy * 1000;
                            learning_time += (int)(time_ms / 1000);
                        }

                        if (learning_time > 60)
                        {
                            learning_time = learning_time / 60; //w minutach
                            TBpozostaly_czas.Text = learning_time + " min";
                        }
                        else TBpozostaly_czas.Text = learning_time + " sek";

                        int minutes = learning_time;
                        if (learning_time > 60)
                        {
                            learning_time = learning_time / 60; //w godzinach
                            minutes -= learning_time * 60;
                            if (minutes > 0)
                                TBpozostaly_czas.Text = learning_time + "h i " + minutes + " min";
                            else
                                TBpozostaly_czas.Text = learning_time + "h";
                        }
                    }
                    else
                    {
                        TBpozostaly_czas.Text = "Nieznany";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MW015", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        void is_program_already_running()
        {
            try
            {
                Process[] arr = Process.GetProcesses();
                string[] a;
                int i = 0;

                foreach (Process p in arr)
                {
                    if (p.ProcessName == Middle_Man.prog_name)
                    {
                        i++;
                    }
                }

                if (i > 1)
                {
                    MessageBox.Show(Middle_Man.prog_name + " jest już włączony.", "Błąd MW016", MessageBoxButton.OK, MessageBoxImage.Error);
                    Process.GetCurrentProcess().Kill();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MW017", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Wmain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                MIzamknij_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MW018", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MIzamknij_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.GetCurrentProcess().Kill();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MW019", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MIpomoc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowHelp2 w = new WindowHelp2();
                w.Owner = System.Windows.Application.Current.MainWindow;
                w.ShowInTaskbar = false;
                w.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MW020", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MIzmien_profil_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Hide();
                Middle_Man.WP.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MW021", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void MIabout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowAbout w = new WindowAbout();
                w.Owner = System.Windows.Application.Current.MainWindow;
                w.ShowInTaskbar = false;
                w.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MW022", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LBbazy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if(LBbazy.SelectedIndex != -1)
                {
                    int id = LBbazy.SelectedIndex;
                    string base_name = LBbazy.Items[id].ToString();

                    load_base(base_name);
                    calculate_remaining_time();
                }

                if (saving_enabled)
                {
                    save_settings();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MW024", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RBautomatyczny_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                Middle_Man.tryb = Tryb.automatyczny;
            
                if (saving_enabled)
                {
                    save_settings();
                    calculate_remaining_time();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MW025", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RBpolautomatyczny_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                Middle_Man.tryb = Tryb.polautomatyczny;

                if (saving_enabled)
                {
                    save_settings();
                    calculate_remaining_time();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MW026", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RBreczny_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                Middle_Man.tryb = Tryb.reczny;

                if (saving_enabled)
                {
                    save_settings();
                    calculate_remaining_time();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MW027", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TBopoznienie_po_pojawieniu_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string str = TBopoznienie_po_pojawieniu.Text.Trim();

                if(str.Length > 0)
                {
                    bool parse = int.TryParse(str, out int opoznienie);

                    if(parse)
                    {
                        Middle_Man.opoznienie_po_zdaniu = opoznienie;

                        if (saving_enabled)
                        {
                            save_settings();
                            calculate_remaining_time();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MW028", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TBopoznienie_pomiedzy_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string str = TBopoznienie_pomiedzy.Text.Trim();

                if (str.Length > 0)
                {
                    bool parse = int.TryParse(str, out int opoznienie);

                    if (parse)
                    {
                        Middle_Man.opoznienie_pomiedzy = opoznienie;

                        if (saving_enabled)
                        {
                            save_settings();
                            calculate_remaining_time();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MW029", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CHBczytaj_po_ang_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                Middle_Man.czytaj_po_ang = true;

                if (saving_enabled)
                {
                    save_settings();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MW030", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CHBczytaj_po_ang_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                Middle_Man.czytaj_po_ang = false;

                if (saving_enabled)
                {
                    save_settings();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MW031", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CHBczytaj_po_pol_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                Middle_Man.czytaj_po_pol = true;

                if (saving_enabled)
                {
                    save_settings();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MW032", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CHBczytaj_po_pol_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                Middle_Man.czytaj_po_pol = false;

                if (saving_enabled)
                {
                    save_settings();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MW033", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CBglos_ang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if(CBglos_ang.SelectedIndex != -1)
                {
                    Middle_Man.glos_ang = CBglos_ang.SelectedItem.ToString();

                    if (saving_enabled)
                    {
                        save_settings();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MW034", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CBglos_pol_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (CBglos_pol.SelectedIndex != -1)
                {
                    Middle_Man.glos_pol = CBglos_pol.SelectedItem.ToString();

                    if (saving_enabled)
                    {
                        save_settings();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MW035", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CBglosnosc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (CBglosnosc.SelectedIndex != -1)
                {
                    Middle_Man.glosnosc = int.Parse(CBglosnosc.SelectedItem.ToString());

                    if (saving_enabled)
                    {
                        save_settings();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MW036", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CHBsprawdzaj_aktualizacje_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                Middle_Man.sprawdzaj_aktualizacje = true;

                if (saving_enabled)
                {
                    save_settings();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MW037", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CHBsprawdzaj_aktualizacje_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                Middle_Man.sprawdzaj_aktualizacje = false;

                if (saving_enabled)
                {
                    save_settings();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MW038", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Bzresetuj_nr_slowka_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LBbazy.SelectedIndex != -1)
                {
                    int id = LBbazy.SelectedIndex;
                    string base_name = LBbazy.Items[id].ToString();

                    for (int i = 0; i < Middle_Man.list_bases.Count; i++)
                    {
                        if (Middle_Man.list_bases[i].name == base_name)
                        {
                            Middle_Man.list_bases[i].current_word_nr = 1;
                            break;
                        }
                    }
                    TBnr_zdania.Text = "1";

                    save_bases();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MW039", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Brozpocznij_nauke_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LBbazy.SelectedIndex != -1)
                {
                    int liczba_zdan = int.Parse(TBliczba_zdan.Text);
                    int liczba_zapamietanych = int.Parse(TBliczba_zapamietanych.Text);

                    if (liczba_zapamietanych >= liczba_zdan
                        && (Middle_Man.tryb == Tryb.polautomatyczny || Middle_Man.tryb == Tryb.reczny))
                    {
                        if(Middle_Man.male == true)
                            MessageBox.Show("Nauczyłeś się już tej bazy. Jeśli chcesz się z niej uczyć ponownie zresetuj postępy w nauce.", "Informacja",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        else if (Middle_Man.male == false)
                            MessageBox.Show("Nauczyłaś się już tej bazy. Jeśli chcesz się z niej uczyć ponownie zresetuj postępy w nauce.", "Informacja",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        int id = LBbazy.SelectedIndex;
                        string base_name = LBbazy.Items[id].ToString();

                        if (Middle_Man.tryb == Tryb.polautomatyczny)
                        {
                            Middle_Man.WLS = new WindowLearningSemi(this, base_name);
                            Middle_Man.WLS.Show();

                            this.Hide();
                        }
                        else if (Middle_Man.tryb == Tryb.reczny)
                        {
                            Middle_Man.WLM = new WindowLearningManual(this, base_name);
                            Middle_Man.WLM.Show();

                            this.Hide();
                        }
                        else if (Middle_Man.tryb == Tryb.automatyczny)
                        {
                            Middle_Man.WLA = new WindowLearningAuto(this, base_name);
                            Middle_Man.WLA.Show();

                            this.Hide();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Nie wybrano żadnej bazy.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MW040", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Bprzywroc_ustawienia_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Czy na pewno chcesz przywrócić ustawienia domyślne?", "Potwierdzenie",
                        MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    restore_default_settings();
                    save_settings();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MW041", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Bzresetuj_postepy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Czy na pewno chcesz zresetować postępy w nauce?", "Potwierdzenie",
                        MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    string profile_directory_path = Middle_Man.profiles_folder_path + Middle_Man.profile_name;
                    string profile_bases_directory = profile_directory_path + "\\" + "Bazy";

                    if (Directory.Exists(profile_bases_directory))
                    {
                        string[] allfiles = Directory.GetFiles(Middle_Man.bases_folder_name, "*.*", SearchOption.TopDirectoryOnly);

                        foreach (string file in allfiles)
                        {
                            string[] tab = file.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                            string file_name = tab[tab.Length - 1];

                            File.Copy(file, profile_directory_path + "\\" + Middle_Man.bases_folder_name + "\\" + file_name, true);
                        }
                    }

                    TBnr_zdania.Text = "";
                    TBliczba_zdan.Text = "";
                    TBliczba_zapamietanych.Text = "";
                    TBpozostaly_czas.Text = "";
                    TBliczba_ukonczen.Text = "";

                    Middle_Man.total_remembered = 0;
                    Middle_Man.total_completions = 0;

                    TBliczba_zapamietanych_all.Text = "0 (0%)";
                    TBliczba_ukonczen_all.Text = "0";

                    string stats_file_path = Middle_Man.profiles_folder_path + Middle_Man.profile_name + "\\" + Middle_Man.stats_filename;
                    string bases_xml_file_path = Middle_Man.profiles_folder_path + Middle_Man.profile_name + "\\" + Middle_Man.bases_filename;

                    File.Delete(stats_file_path);
                    File.Delete(bases_xml_file_path);

                    Middle_Man.list_bases.Clear();

                    create_bases_file();

                    if (LBbazy.SelectedIndex != -1)
                    {
                        int id = LBbazy.SelectedIndex;
                        string base_name = LBbazy.Items[id].ToString();

                        load_base(base_name);
                        calculate_remaining_time();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MW042", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LBbazy_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Brozpocznij_nauke_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MW043", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        async void update_app_if_necessary()
        {
            try
            {
                string content = await Client.GetStringAsync(Middle_Man.url_latest_version);
                Middle_Man.latest_version = content.Replace("\r\n", "").Trim();
            }
            catch (TaskCanceledException)
            {
                // Timeout po 3 sekundach
                Middle_Man.latest_version = "nieznana";
            }
            catch (HttpRequestException ex)
            {
                // Błąd połączenia
                Middle_Man.latest_version = "nieznana";
            }

            bool update_available = false;

            try
            {
                if (Middle_Man.latest_version != "nieznana" &&
                    int.Parse(Middle_Man.latest_version.Replace(".", "")) > int.Parse(Middle_Man.prog_version.Replace(".", "")))
                {
                    update_available = true;
                }

                if ((bool)CHBsprawdzaj_aktualizacje.IsChecked && update_available)
                {
                    MessageBoxResult result = MessageBox.Show("Jest dostępna nowa wersja programu. Czy chcesz ją teraz pobrać?", "Nowa Wersja Dostępna",
                        MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        //Open download page
                        Middle_Man.open_url(Middle_Man.url_homepage_full);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MW044", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private readonly HttpClient Client = new()
        {
            Timeout = TimeSpan.FromSeconds(3)
        };

        public async Task<string> DownloadStringAsync(string url)
        {
            return await Client.GetStringAsync(url);
        }
    }
}