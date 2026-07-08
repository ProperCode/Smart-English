using System.Diagnostics;
using System.IO;
using System.Media;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using System.Speech.Synthesis;

namespace Smart_English
{
    static class Middle_Man
    {
        public static string prog_name = "Smart English";
        public static string prog_version = "1.1";
        public static string latest_version = "";
        public static string copyright_text = "Copyright © 2026 Mikołaj Magowski. Wszystkie prawa zastrzeżone.";

        public static string bases_filename = "Bazy.xml";
        public static string stats_filename = "Statystyki.xml";
        public static string settings_filename = "Ustawienia.xml";
        public static string profiles_filename = "Profile.xml";
        public static string last_profile_filename = "Ostatni profil.xml";

        public static string users_directory_path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\" + prog_name + "\\";
        public static string profiles_folder_path = users_directory_path + "Profile" + "\\";
        public static string app_folder_path = AppDomain.CurrentDomain.BaseDirectory;
        public static string bases_folder_path = app_folder_path + "Bazy";
        public static string bases_folder_name = "Bazy";
        public static string profiles_file_path = users_directory_path + profiles_filename;
        public static string last_profile_path = users_directory_path + last_profile_filename;

        public static List<Profile> list_profiles = new List<Profile>();
        public static List<Sentence> list_sentences = new List<Sentence>();
        public static List<Sentences_base> list_bases = new List<Sentences_base>();

        public static int base_additional_time = 1000; //ms
        public static int time_per_char = 100; //120; //ms
        public static int time_per_char_presentation = 80;

        public static string url_homepage = "github.com/ProperCode/Smart-English";
        public static string url_homepage_full = "https://github.com/ProperCode/Smart-English";
        public static string url_latest_version = "https://raw.githubusercontent.com/ProperCode/Smart-English/refs/heads/main/inne/ostatnia%20wersja.txt";
        public static string url_download = "https://github.com/ProperCode/Smart-English";

        public static string default_en_voice = "Zira";
        public static string default_pl_voice = "Paulina";

        public static string profile_name = "";
        public static bool male = true;

        public static WindowProfiles WP;
        public static WindowLearningSemi WLS;
        public static WindowLearningManual WLM;
        public static WindowLearningAuto WLA;

        public static int opoznienie_po_zdaniu = 4;
        public static int opoznienie_pomiedzy = 0;

        public static bool czytaj_po_ang = true;
        public static bool czytaj_po_pol = true;
        public static string glos_ang = default_en_voice;
        public static string glos_pol = default_pl_voice;
        public static int glosnosc = 100;

        public static bool sprawdzaj_aktualizacje = true;

        public static int total_remembered = 0;
        public static int total_completions = 0;
        public static string last_base_name = "";

        public static Tryb tryb;

        public static void open_url(string url)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }

        public static async void read(bool english, string text)
        {
            try
            {
                using var synth = new SpeechSynthesizer();

                synth.Volume = Middle_Man.glosnosc; // 0-100
                synth.Rate = 0;     // -10 to 10
                synth.SetOutputToDefaultAudioDevice();

                string voice_name = "";

                IReadOnlyCollection<InstalledVoice> list = synth.GetInstalledVoices();

                if (english)
                {
                    voice_name = Middle_Man.glos_ang;
                }
                else
                {
                    voice_name = Middle_Man.glos_pol;
                }

                synth.SelectVoice(voice_name);

                synth.Speak(text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MM001", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void save_profiles()
        {
            XmlRootAttribute root = new XmlRootAttribute();
            root.ElementName = "Profile";
            root.IsNullable = true;

            XmlSerializer serial = new XmlSerializer(typeof(List<Profile>), root);

            StreamWriter sw = null;

            try
            {
                sw = new StreamWriter(profiles_file_path);
                serial.Serialize(sw, list_profiles);
                sw.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd MM002", MessageBoxButton.OK, MessageBoxImage.Error);

                try
                {
                    if (sw != null)
                        sw.Close();
                }
                catch (Exception ex2) { }
            }
        }

        public static void load_profiles()
        {
            if (File.Exists(profiles_file_path))
            {
                XmlDocument doc = new XmlDocument();
                XmlReader rdr = null;
                XmlNodeReader noderdr = null;

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ConformanceLevel = ConformanceLevel.Auto;
                settings.IgnoreComments = true;
                settings.IgnoreWhitespace = true;

                list_profiles = new List<Profile>();

                try
                {
                    Profile p;
                    string s = null;
                    int x = 0;

                    doc.Load(profiles_file_path);
                    noderdr = new XmlNodeReader(doc);
                    rdr = XmlReader.Create(noderdr, settings);

                    while (rdr.Read())
                    {
                        if (rdr.NodeType == XmlNodeType.Element)
                        {
                            if (rdr.Name == "name")
                            {
                                rdr.Read();
                                s = rdr.Value;
                            }
                            if (rdr.Name == "male")
                            {
                                rdr.Read();
                                p = new Profile(s, bool.Parse(rdr.Value));
                                list_profiles.Add(p);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Błąd MM003", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}