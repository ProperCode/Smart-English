using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;

namespace Smart_English
{
    /// <summary>
    /// Interaction logic for WindowLearningAuto.xaml
    /// </summary>
    public partial class WindowLearningAuto : Window
    {
        private readonly MainWindow _mainWindow;

        Thread THRlearning;
        int base_id = -1;
        string base_name = "";
        int current_word_nr;
        bool running = true;

        public WindowLearningAuto(MainWindow mainWindow, string Base_name)
        {
            InitializeComponent();

            _mainWindow = mainWindow;
            running = true;

            THRlearning = new Thread(new ThreadStart(learning));

            try
            {
                WindowState = WindowState.Maximized;

                Penglish.Inlines.Clear();
                Ppolish.Inlines.Clear();

                base_name = Base_name;

                for (int i = 0; i < Middle_Man.list_bases.Count; i++)
                {
                    if (Middle_Man.list_bases[i].name == base_name)
                    {
                        base_id = i;
                        current_word_nr = Middle_Man.list_bases[i].current_word_nr;
                        break;
                    }
                }

                THRlearning.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd WLA001", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void learning()
        {
            try
            {
                string sentence_en, sentence_pl;

                for (; current_word_nr < Middle_Man.list_sentences.Count + 1; current_word_nr++)
                {
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
                    if (running == true)
                        Thread.Sleep(sentence_en.Length * Middle_Man.time_per_char + Middle_Man.opoznienie_po_zdaniu * 1000);

                    this.Dispatcher.Invoke(DispatcherPriority.Normal,
                        new Action(() => { Ppolish.Inlines.Add(new Run(sentence_pl)); }));
                    if (Middle_Man.czytaj_po_pol == true && running == true)
                    {
                        Middle_Man.read(false, sentence_pl);
                    }
                    if (running == true)
                        Thread.Sleep(sentence_pl.Length * Middle_Man.time_per_char + Middle_Man.opoznienie_po_zdaniu * 1000);

                    if (running == true)
                        Thread.Sleep(1 + Middle_Man.opoznienie_pomiedzy * 1000);

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

                    _mainWindow.Dispatcher.Invoke(DispatcherPriority.Normal,
                        new Action(() => { this.Close(); }));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd WLA002", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Bzakoncz_nauke_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd WLA003", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                running = false;

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

                _mainWindow.calculate_remaining_time();

                _mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd WLA004", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}