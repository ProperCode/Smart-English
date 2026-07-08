using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Smart_English
{
    /// <summary>
    /// Interaction logic for WindowLearningManual.xaml
    /// </summary>
    public partial class WindowLearningManual : Window
    {
        private readonly MainWindow _mainWindow;

        Thread THRlearning;
        int base_id = -1;
        string base_name = "";
        int current_word_nr;
        int remembered_at_start;
        bool running = true;
        bool answered = false;
        bool show_translation = false;
        bool buttons_disabled = true; //chodzi o przyciski Wiedziałem i Nie wiedziałem

        public WindowLearningManual(MainWindow mainWindow, string Base_name)
        {
            InitializeComponent();

            _mainWindow = mainWindow;
            running = true;

            THRlearning = new Thread(new ThreadStart(learning));

            try
            {
                WindowState = WindowState.Maximized;

                if (Middle_Man.male == false)
                {
                    Bwiedzialem.Content = "Wiedziałam";
                    Bnie_wiedzialem.Content = "Nie wiedziałam";
                }

                Penglish.Inlines.Clear();
                Ppolish.Inlines.Clear();

                base_name = Base_name;

                if(base_name == "Czasowniki nieregularne")
                {
                    Bpokaz_tlumaczenie.Content = "Pokaż formy przeszłe";
                }

                for (int i = 0; i < Middle_Man.list_bases.Count; i++)
                {
                    if (Middle_Man.list_bases[i].name == base_name)
                    {
                        base_id = i;
                        current_word_nr = Middle_Man.list_bases[i].current_word_nr;
                        break;
                    }
                }

                int remembered = 0;
                for (int i = 0; i < Middle_Man.list_sentences.Count; i++)
                {
                    if (Middle_Man.list_sentences[i].remembered)
                        remembered++;
                }
                remembered_at_start = remembered;

                THRlearning.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd WLM001", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void learning()
        {
            try
            {
                string sentence_en, sentence_pl;

                for (; current_word_nr < Middle_Man.list_sentences.Count + 1; current_word_nr++)
                {
                    if (Middle_Man.list_sentences[current_word_nr - 1].remembered == true)
                        continue;

                    answered = false;
                    show_translation = false;
                    buttons_disabled = true;
                    sentence_en = Middle_Man.list_sentences[current_word_nr - 1].sentence_en;
                    sentence_pl = Middle_Man.list_sentences[current_word_nr - 1].sentence_pl;

                    this.Dispatcher.Invoke(DispatcherPriority.Normal,
                        new Action(() => { Penglish.Inlines.Clear(); }));
                    this.Dispatcher.Invoke(DispatcherPriority.Normal,
                        new Action(() => { Ppolish.Inlines.Clear(); }));

                    this.Dispatcher.Invoke(DispatcherPriority.Normal,
                        new Action(() => { Penglish.Inlines.Add(new Run(sentence_en)); }));
                    if (Middle_Man.czytaj_po_ang == true && running == true)
                    {
                        Middle_Man.read(true, sentence_en);
                    }

                    while (show_translation == false && running == true)
                    {
                        Thread.Sleep(100);
                    }

                    this.Dispatcher.Invoke(DispatcherPriority.Normal,
                        new Action(() => { Ppolish.Inlines.Add(new Run(sentence_pl)); }));
                    if (base_name == "Czasowniki nieregularne")
                    {
                        if (Middle_Man.czytaj_po_ang == true && running == true)
                        {
                            Middle_Man.read(true, sentence_pl);
                        }
                    }
                    else
                    {
                        if (Middle_Man.czytaj_po_pol == true && running == true)
                        {
                            Middle_Man.read(false, sentence_pl);
                        }
                    }
                    buttons_disabled = false;

                    while (answered == false && running == true)
                    {
                        Thread.Sleep(100);
                    }

                    if (running == false)
                        break;
                }

                if (running == true)
                {
                    current_word_nr = 1;

                    for (int i = 0; i < Middle_Man.list_bases.Count; i++)
                    {
                        if (Middle_Man.list_bases[i].name == base_name)
                        {
                            Middle_Man.list_bases[i].completions++;
                        }
                    }

                    _mainWindow.Dispatcher.Invoke(DispatcherPriority.Normal,
                        new Action(() => { _mainWindow.TBliczba_ukonczen.Text = (int.Parse(_mainWindow.TBliczba_ukonczen.Text) + 1).ToString(); }));
                    _mainWindow.Dispatcher.Invoke(DispatcherPriority.Normal,
                        new Action(() => { _mainWindow.TBliczba_ukonczen_all.Text = (int.Parse(_mainWindow.TBliczba_ukonczen_all.Text) + 1).ToString(); }));
                    Middle_Man.total_completions++;

                    this.Dispatcher.Invoke(DispatcherPriority.Normal,
                        new Action(() => { this.Close(); }));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd WLM002", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Bpokaz_tlumaczenie_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                show_translation = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd WLM003", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Bnie_wiedzialem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (buttons_disabled == false)
                {
                    answered = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd WLM004", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Bwiedzialem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (buttons_disabled == false)
                {
                    answered = true;
                    Middle_Man.list_sentences[current_word_nr - 1].remembered = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd WLM005", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Bnie_wiedzialem_MouseEnter(object sender, MouseEventArgs e)
        {
            Bnie_wiedzialem.Foreground = Brushes.Black;
        }

        private void Bnie_wiedzialem_MouseLeave(object sender, MouseEventArgs e)
        {
            Bnie_wiedzialem.Foreground = Brushes.White;
        }

        private void Bwiedzialem_MouseEnter(object sender, MouseEventArgs e)
        {
            Bwiedzialem.Foreground = Brushes.Black;
        }

        private void Bwiedzialem_MouseLeave(object sender, MouseEventArgs e)
        {
            Bwiedzialem.Foreground = Brushes.White;
        }

        private void Bzakoncz_nauke_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd WLM006", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                running = false;

                int remembered = 0;
                for (int i = 0; i < Middle_Man.list_sentences.Count; i++)
                {
                    if (Middle_Man.list_sentences[i].remembered)
                        remembered++;
                }

                int difference = remembered - remembered_at_start;

                Middle_Man.total_remembered = Middle_Man.total_remembered + difference;
                Middle_Man.last_base_name = base_name;

                _mainWindow.save_base(base_name);

                for (int i = 0; i < Middle_Man.list_bases.Count; i++)
                {
                    if (Middle_Man.list_bases[i].name == base_name)
                    {
                        Middle_Man.list_bases[i].current_word_nr = current_word_nr;
                    }
                }
                _mainWindow.save_bases();

                _mainWindow.save_stats();

                _mainWindow.TBnr_zdania.Text = current_word_nr.ToString();
                _mainWindow.TBliczba_zapamietanych.Text = remembered.ToString();

                int total_sentences = 0;
                for (int i = 0; i < Middle_Man.list_bases.Count; i++)
                {
                    total_sentences += Middle_Man.list_bases[i].sentences_nr;
                }

                double learning_percentage = Math.Floor((double)Middle_Man.total_remembered / (double)total_sentences * 100);

                _mainWindow.TBliczba_zapamietanych_all.Text = Middle_Man.total_remembered.ToString() + " (" + learning_percentage + "%)";

                _mainWindow.calculate_remaining_time();

                if (remembered == Middle_Man.list_sentences.Count)
                {
                    ListBoxItem item = (ListBoxItem)_mainWindow.LBbazy.ItemContainerGenerator.ContainerFromIndex(base_id);
                    item.Foreground = Brushes.Green;
                }

                _mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd WLM007", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}