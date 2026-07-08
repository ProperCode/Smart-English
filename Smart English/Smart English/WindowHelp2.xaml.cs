using System.Windows;
using System.Windows.Documents;

namespace Smart_English
{
    /// <summary>
    /// Interaction logic for WindowHelp2.xaml
    /// </summary>
    public partial class WindowHelp2 : Window
    {
        public WindowHelp2()
        {
            try
            {
                InitializeComponent();
                RTB.IsReadOnly = true;

                Paragraph paragraph = new Paragraph();

                paragraph.Inlines.Add(new Bold(new Run("1. Rozpoczęcie nauki:")));
                paragraph.Inlines.Add(new Run("\n- Aby rozpocząć naukę wybierz bazę i kliknij przycisk \"Rozpocznij naukę\"."));
                paragraph.Inlines.Add(new Bold(new Run("\n\n2. System powtórek:")));
                paragraph.Inlines.Add(new Run("\n- System powtórek działa tylko w trybach: ręcznym i półautomatycznym. Kliknięcie przycisku \"Wiedziałem\" podczas nauki"));
                paragraph.Inlines.Add(new Run(" oznacza dane zdanie jako zapamiętane i nie będzie ono więcej pojawiać się podczas nauki z danej bazy. Liczba zapamiętanych zdań dla"));
                paragraph.Inlines.Add(new Run(" danej bazy jest wyświetlana w głównym oknie programu w polu grupowym \"Informacje i statystyki\". Wszystkie zapamiętane zdania można"));
                paragraph.Inlines.Add(new Run(" oznaczyć jako niezapamiętane klikając w przycisk \"Zresetuj postępy w nauce\"."));
                paragraph.Inlines.Add(new Bold(new Run("\n\n3. Tryby nauki:")));
                paragraph.Inlines.Add(new Bold(new Run("\n- Automatyczny")));
                paragraph.Inlines.Add(new Run(" - w tym trybie zdania są prezentowane automatycznie po zadanym czasie. Wartości ustawień \"Dodatkowego opóźnienia po pojawieniu się"));
                paragraph.Inlines.Add(new Run(" zdania\" i \"Opóźnienia pomiędzy nauką kolejnych zdań\" mają wpływ na szybkość nauki w tym trybie. System powtórek nie działa w tym trybie."));
                paragraph.Inlines.Add(new Run(" Pozostały czas nauki dla wybranej bazy w tym trybie jest wyświetlany w głównym oknie programu w polu grupowym \"Informacje i statystyki\"."));
                paragraph.Inlines.Add(new Bold(new Run("\n- Półautomatyczny")));
                paragraph.Inlines.Add(new Run(" - w tym trybie tłumaczenie zdania pojawia się po zadanym czasie. Wartość ustawienia \"Dodatkowego opóźnienia po pojawieniu się"));
                paragraph.Inlines.Add(new Run(" zdania\" ma wpływ na szybkość nauki w tym trybie. Kliknięcie przycisku \"Wiedziałem\" skutkuje oznaczeniem danego zdania jako zapamiętanego i to"));
                paragraph.Inlines.Add(new Run(" zdanie nie będzie pojawiać się przy następnych naukach z danej bazy. Kliknięcie przycisku \"Nie wiedziałem\" oznacza, że dane zdanie"));
                paragraph.Inlines.Add(new Run(" pojawi się przy następnej nauce z danej bazy."));
                paragraph.Inlines.Add(new Bold(new Run("\n- Ręczny")));
                paragraph.Inlines.Add(new Run(" - w tym trybie tłumaczenie zdania pojawia się po kliknięciu przycisku \"Pokaż tłumaczenie\". Przyciski \"Wiedziałem\""));
                paragraph.Inlines.Add(new Run(" i \"Nie wiedziałem\" funkcjonują w tym trybie tak samo jak w trybie półautomatycznym."));
                paragraph.Inlines.Add(new Bold(new Run("\n\n4. Resetowanie postępów w nauce:")));
                paragraph.Inlines.Add(new Run("\n- Kilka lat po nauczeniu się wszystkich baz zalecane jest zresetowanie zapamiętanych zdań poprzez przycisk \"Zresetuj postępy w nauce\""));
                paragraph.Inlines.Add(new Run(" i rozpoczęcie nauki na nowo. Bazy, które użytkownik opanował są oznaczone kolorem zielonym."));

                RTB.Document.Blocks.Add(paragraph);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd WH001", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
